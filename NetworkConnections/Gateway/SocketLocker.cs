using System;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Threading;

namespace Gateway
{
    internal class SocketLocker : SocketSynchronizer
    {
        #region Field's
        private bool socketSendSoftLocked,
                     socketSendHardLocked,
                     socketReceiveLocked;
        private readonly ManualResetEventSlim sendMRES,
                                              receiveMRES;
        private int sendersKernelWaitingsCount,
                    receiverKernelWaitings;
        #endregion
        #region Method's
        internal SocketAccessToken GetSendAccess(bool highPriority)
        {
            if (socketSendSoftLocked)
            {
                if (highPriority)
                {
                    if (!socketSendHardLocked)
                    {
                        return base.GetSend();
                    }
                    else
                    {
                        Wait();
                    }
                }
                else
                {
                    Wait();
                }
            }
            return base.GetSend();

            void Wait()
            {
                SpinWait spin = new SpinWait();
                while (socketSendSoftLocked)
                {
                    if (!spin.NextSpinWillYield)
                    {
                        spin.SpinOnce();
                    }
                    else
                    {
                        Interlocked.Increment(ref sendersKernelWaitingsCount);
                        sendMRES.Wait();
                        if(Interlocked.Decrement(ref sendersKernelWaitingsCount) == 0)
                        {
                            sendMRES.Reset();
                        }
                    }
                }
            }
        }
        internal SocketAccessToken GetReceiveAccess()
        {
            if (socketReceiveLocked)
            {
                SpinWait spin = new SpinWait();
                while (socketReceiveLocked)
                {
                    if (!spin.NextSpinWillYield)
                    {
                        spin.SpinOnce();
                    }
                    else
                    {
                        Interlocked.Increment(ref receiverKernelWaitings);
                        receiveMRES.Wait();
                        Interlocked.Decrement(ref receiverKernelWaitings);
                        receiveMRES.Reset();
                    }
                }
            }
            return base.GetReceive();
        }
        internal SocketLockToken SendingSoftLock()
        {
            socketSendSoftLocked = true;
            return new SocketLockToken(this, AccessType.Send);
        }
        internal SocketLockToken SendingHardLock()
        {
            socketSendSoftLocked = true;
            socketSendHardLocked = true;
            return new SocketLockToken(this, AccessType.Send);
        }
        internal SocketLockToken ReceivingLock()
        {
            socketReceiveLocked = true;
            return new SocketLockToken(this, AccessType.Receive);
        }
        private void UnlockSocket(SocketLockToken token)
        {
            if(token.type == AccessType.Send)
            {
                socketSendSoftLocked = false;
                socketSendHardLocked = false;
                if(sendersKernelWaitingsCount != 0)
                {
                    sendMRES.Set();
                }
            }
            else
            {
                socketReceiveLocked = false;
                if(receiverKernelWaitings != 0)
                {
                    receiveMRES.Set();
                }
            }
        }
        #endregion
        #region Ctor's
        internal SocketLocker()
        {
            socketSendSoftLocked = false;
            socketSendHardLocked = false;
            receiveMRES = new ManualResetEventSlim(false, 0);
            sendMRES = new ManualResetEventSlim(false, 0);
            receiverKernelWaitings = 0;
            sendersKernelWaitingsCount = 0;
        }
        #endregion



        internal class SocketLockToken : IDisposable
        {
            private readonly SocketLocker locker;
            internal AccessType type;
            public void Dispose()
            {
                locker.UnlockSocket(this);
            }
            internal SocketLockToken(SocketLocker locker, AccessType type)
            {
                this.type = type;
                this.locker = locker;
            }
        }
    }

    internal class SocketSynchronizer
    {
        private int sendState,
                    receiveState,
                    sendKernelWaitingsCount,
                    receiveKernelWaitingsCount;
        private readonly AutoResetEvent sendARE,
                                        receiveARE;

        internal SocketAccessToken GetSend()
        {
            SpinWait spin = new SpinWait();
            while (sendState == 0)
            {
                if (!spin.NextSpinWillYield)
                {
                    spin.SpinOnce();
                }
                else
                {
                    Interlocked.Increment(ref sendKernelWaitingsCount);
                    sendARE.WaitOne();
                    Interlocked.Decrement(ref sendKernelWaitingsCount);
                }
            }
            Interlocked.Exchange(ref sendState, 0);
            return new SocketAccessToken(this, AccessType.Send);
        }
        internal SocketAccessToken GetReceive()
        {
            SpinWait spin = new SpinWait();
            while (receiveState == 0)
            {
                if (!spin.NextSpinWillYield)
                {
                    spin.SpinOnce();
                }
                else
                {
                    Interlocked.Increment(ref receiveKernelWaitingsCount);
                    receiveARE.WaitOne();
                    Interlocked.Decrement(ref receiveKernelWaitingsCount);
                }
            }
            Interlocked.Exchange(ref receiveState, 0);
            return new SocketAccessToken(this, AccessType.Receive);
        }
        internal void Release(SocketAccessToken token)
        {
            if (token.type == AccessType.Send)
            {
                if (sendKernelWaitingsCount != 0)
                {
                    sendARE.Set();
                }
                Interlocked.Exchange(ref sendState, 1);
            }
            else
            {
                if (receiveKernelWaitingsCount != 0)
                {
                    receiveARE.Set();
                }
                Interlocked.Exchange(ref receiveState, 1);
            }
        }

        internal SocketSynchronizer()
        {
            sendARE = new AutoResetEvent(false);
            receiveARE = new AutoResetEvent(false);
            sendState = 1;
            receiveState = 1;
        }



        internal class SocketAccessToken : IDisposable
        {
            private readonly SocketSynchronizer synchronizer;
            internal readonly AccessType type;
            public void Dispose()
            {
                synchronizer.Release(this);
            }
            internal SocketAccessToken(SocketSynchronizer synchronizer, AccessType type)
            {
                this.type = type;
                this.synchronizer = synchronizer;
            }
        }
    }



    internal enum AccessType : byte
    {
        Send,
        Receive
    }
}