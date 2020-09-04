using System;
using System.Threading;

namespace Gateway
{
    internal class SocketLocker : SocketSynchronizer
    {
        #region Field's
        private bool socketSendSoftLocked,
                     socketSendHardLocked,
                     socketReceiveLocked;
        private int sendingLockersCount,
                    sendingHardLockersCount,
                    receivingLockersCount;

        #endregion
        #region Method's
        internal override SocketAccessToken GetSendAccess()
        {
            SpinWait spin = new SpinWait();
            while (sendState == 0 || socketSendSoftLocked)
            {
                WaitForSendAccess(spin);
            }
            return new SocketAccessToken(this, AccessType.Send, sendCTS.Token);
        }
        internal SocketAccessToken GetHighPrioritySendAccess()
        {
            SpinWait spin = new SpinWait();
            while (sendState == 0 || socketSendHardLocked)
            {
                WaitForSendAccess(spin);
            }
            Interlocked.Exchange(ref sendState, 0);
            return new SocketAccessToken(this, AccessType.Send, sendCTS.Token);
        }
        internal override SocketAccessToken GetReceiveAccess()
        {
            SpinWait spin = new SpinWait();
            while (receiveState == 0 || socketReceiveLocked)
            {
                WaitForReceiveAccess(spin); 
            }
            Interlocked.Exchange(ref receiveState, 0);
            return new SocketAccessToken(this, AccessType.Receive, receiveCTS.Token);
        }
        internal SocketSendLockToken GetSendingLock()
        {
            if(Interlocked.Increment(ref sendingLockersCount) == 1)
            {
                socketSendSoftLocked = true;
            }
            return new SocketSendLockToken(this);
        }
        internal void HardLockSocket()
        {
            if(Interlocked.Increment(ref sendingHardLockersCount) == 1)
            {
                socketSendHardLocked = true;
            }
        }
        internal SocketLockToken GetReceivingLock()
        {
            if(Interlocked.Increment(ref receivingLockersCount) == 1)
            {
                socketReceiveLocked = true;
            }
            return new SocketLockToken(this, LockType.Receive);
        }
        internal SocketLockToken GetSuspendingLock(bool abortCurrentOperations)
        {
            SocketLockToken result = SuspendLock();
            if (abortCurrentOperations)
            {
                AbortReceiving();
                AbortSending();
            }
            return result;
        }
        internal SocketLockToken GetSuspendingLock(Action onReceivingAborted, Action onSendingAborted)
        {
            SocketLockToken result = SuspendLock();
            AbortReceiving(onReceivingAborted);
            AbortSending(onSendingAborted);
            return result;
        }
        private SocketLockToken SuspendLock()
        {
            if (Interlocked.Increment(ref receivingLockersCount) == 1)
            {
                socketReceiveLocked = true;
            }
            if (Interlocked.Increment(ref sendingLockersCount) == 1)
            {
                socketSendSoftLocked = true;
            }
            if (Interlocked.Increment(ref sendingHardLockersCount) == 1)
            {
                socketSendHardLocked = true;
            }
            return new SocketLockToken(this, LockType.Suspend);
        }
        internal void UnlockSocket(SocketLockToken token) // TODO : доделать
        {
            switch (token.type)
            {
                case LockType.Receive:
                    if (Interlocked.Decrement(ref receivingLockersCount) == 0)
                    {
                        socketReceiveLocked = false;
                        if (receiveKernelWaitingsCount != 0)
                        {
                            receiveARE.Set();
                        }
                    }
                    break;
                case LockType.Send:
                    SocketSendLockToken sendToken = token as SocketSendLockToken;
                    if (sendToken.IsHardLocked)
                    {
                        if (Interlocked.Decrement(ref sendingHardLockersCount) == 0)
                        {
                            socketSendHardLocked = false;
                        }
                    }
                    if (Interlocked.Decrement(ref sendingLockersCount) == 0)
                    {
                        socketSendSoftLocked = false;
                        if (sendKernelWaitingsCount != 0)
                        {
                            sendARE.Set();
                        }
                    }
                    break;
                case LockType.Suspend:
                    if (Interlocked.Decrement(ref receivingLockersCount) == 0)
                    {
                        socketReceiveLocked = false;
                        if (receiveKernelWaitingsCount != 0)
                        {
                            receiveARE.Set();
                        }
                    }
                    if (Interlocked.Decrement(ref sendingHardLockersCount) == 0)
                    {
                        socketSendHardLocked = false;
                    }
                    if (Interlocked.Decrement(ref sendingLockersCount) == 0)
                    {
                        socketSendSoftLocked = false;
                        if (sendKernelWaitingsCount != 0)
                        {
                            sendARE.Set();
                        }
                    }
                    break;
            }
        }
        #endregion
        #region Ctor's
        internal SocketLocker()
        {
            socketSendSoftLocked = false;
            socketSendHardLocked = false;
        }
        #endregion
    }
    internal class SocketLockToken : IDisposable
    {
        private protected readonly SocketLocker locker;
        internal LockType type;
        public void Dispose()
        {
            locker.UnlockSocket(this);
        }
        internal SocketLockToken(SocketLocker locker, LockType type)
        {
            this.type = type;
            this.locker = locker;
        }
    }
    internal class SocketSendLockToken : SocketLockToken
    {
        internal bool IsHardLocked { get; private set; }
        internal void HardLockSocket()
        {
            IsHardLocked = true;
            locker.HardLockSocket();
        }
        internal SocketSendLockToken(SocketLocker locker) : base(locker, LockType.Send) { }
    }
    internal enum LockType : byte
    {
        Receive,
        Send,
        Suspend
    }



    internal class SocketSynchronizer
    {
        // State: 1 - available, 0 - non
        private protected int sendState,
                              receiveState,
                              sendKernelWaitingsCount,
                              receiveKernelWaitingsCount;
        private protected readonly AutoResetEvent sendARE,
                                                  receiveARE;
        private protected CancellationTokenSource receiveCTS,
                                                  sendCTS;

        internal void AbortReceiving()
        {
            receiveCTS.Token.Register(ResetReceiveCTS);
            receiveCTS.Cancel();
        }
        internal void AbortReceiving(Action onAbortationCompleted)
        {
            receiveCTS.Token.Register(onAbortationCompleted);
            AbortReceiving();
        }
        internal void AbortSending()
        {
            sendCTS.Token.Register(ResetSendCTS);
            //sendCTS.Cancel(); // TODO: кладёт поток на лопатки при вызове, не создавая никаких исключений.
        }
        internal void AbortSending(Action onAbortationCompleted)
        {
            sendCTS.Token.Register(onAbortationCompleted);
            AbortSending();
        }
        private void ResetReceiveCTS()
        {
            receiveCTS = new CancellationTokenSource();
        }
        private void ResetSendCTS()
        {
            sendCTS = new CancellationTokenSource();
        }
        internal virtual SocketAccessToken GetSendAccess()
        {
            SpinWait spin = new SpinWait();
            while (sendState == 0)
            {
                WaitForSendAccess(spin);
            }
            Interlocked.Exchange(ref sendState, 0);
            return new SocketAccessToken(this, AccessType.Send, sendCTS.Token);
        }
        internal virtual SocketAccessToken GetReceiveAccess()
        {
            SpinWait spin = new SpinWait();
            while (receiveState == 0)
            {
                WaitForReceiveAccess(spin);
            }
            Interlocked.Exchange(ref receiveState, 0);
            return new SocketAccessToken(this, AccessType.Receive, receiveCTS.Token);
        }
        private protected void WaitForSendAccess(SpinWait spin)
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
        private protected void WaitForReceiveAccess(SpinWait spin)
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
        internal virtual void Release(SocketAccessToken token)
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
            receiveCTS = new CancellationTokenSource();
            sendCTS = new CancellationTokenSource();
            sendState = 1;
            receiveState = 1;
        }
    }
    internal class SocketAccessToken : IDisposable
    {
        private readonly SocketSynchronizer synchronizer;
        internal CancellationToken CancellationToken { get; }
        internal readonly AccessType type;
        internal void OnCancellation(Action<object> action, object state)
        {
            CancellationToken.Register(action, state);
        }
        public void Dispose()
        {
            synchronizer.Release(this);
        }
        internal SocketAccessToken(SocketSynchronizer synchronizer, AccessType type, CancellationToken cancellationToken)
        {
            this.type = type;
            this.synchronizer = synchronizer;
            CancellationToken = cancellationToken;
        }
    }
    internal enum AccessType : byte
    {
        Send,
        Receive
    }
}