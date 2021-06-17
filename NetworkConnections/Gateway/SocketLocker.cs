using System;
using System.Net.WebSockets;
using System.Threading;

namespace Gateway
{
    internal class WebSocket
	{
        ClientWebSocket socket;
        SocketSynchronizer synchronizer;
        Locker locker;
        int sendState;

        internal void GetSendAccess()
        {
            var lockState = locker.LockState;
			while (IsSocketLocked())
			{
                WaitForSendAccess();
			}
            // Grand access
            
            bool IsSocketLocked()
			{
                return sendState == 0 || lockState != Locker.LockType.None;
			}
        }
        internal void GetHighPrioritySendAccess()
		{
            var lockState = locker.LockState;
            while (IsSocketLocked())
			{
                WaitForSendAccess();
			}
            // Grand access

            bool IsSocketLocked()
			{
                return sendState == 0 || lockState == Locker.LockType.Overall;
			}
		}
        void WaitForSendAccess()
		{
            if (!spin.NextSpinWillYield)
            {
                spin.SpinOnce();
            }
            else
            {
                sender.WaitForAccess();
            }
        }
        internal void LockSending() { }
        internal void Unlock() { }

        class Locker
		{
            int lockersCount;
            int overallLockersCount;

            internal LockType State { get; private set; }

            bool IsSocketLocked(LockType lockType)
			{
                return State.HasFlag(lockType);
			}
            internal LockToken LockCommon()
			{
				if (Interlocked.Increment(ref lockersCount) == 1) // TODO: в локальный метод
				{
					if (IsFirstLocker())
					{
                        LockSocket(LockType.Common);
					}
				}
                var token = new LockToken(LockType.Common, this);
                return token;

                bool IsFirstLocker()
				{
                    return lockersCount == 0; // TODO: без интерлокеда будет рабоать?
                }
			}
            internal LockToken LockOverall() // TODO: два метода очень похожи их можно объеденить ^^^
			{
                if (Interlocked.Increment(ref overallLockersCount) == 1) // TODO: в локальный метод
                {
                    if (IsFirstLocker())
                    {
                        LockSocket(LockType.Overall);
                    }
                }
                var token = new LockToken(LockType.Overall, this);
                return token;

                bool IsFirstLocker()
                {
                    return overallLockersCount == 0;
                }
            }
            void LockSocket(LockType lockType)
            {
                State |= lockType;
            }
            void DisposeToken(LockToken token)
			{
                HandleToken();
                UpdateSocketState();
                token = null;

                void HandleToken()
				{
                    LockType tokenType = token.lockType;
                    switch (tokenType)
                    {
                        case LockType.Common:
                            Interlocked.Decrement(ref lockersCount);
                            break;
                        case LockType.Overall:
                            Interlocked.Decrement(ref overallLockersCount);
                            break;
                        default:
                            throw new Exception(); // TODO: исключение
                    }
                }
            }
            void UpdateSocketState()
            {
				if (CommonLockersExist())
				{
                    State &= ~LockType.Common;
				}
				if (OverallLockersExist())
				{
                    State &= ~LockType.Overall;
                }

                bool CommonLockersExist()
				{
                    return lockersCount != 0; // TODO: будет работать без интерлокеда?
				}
                bool OverallLockersExist()
				{
                    return overallLockersCount != 0;
				}
            }
            void Unlock()
			{
                State = LockType.None;
			}

            [Flags]
            internal enum LockType : byte
			{
                None,
                Common,
                Overall
			}

            internal class LockToken : IDisposable
            {
                internal readonly LockType lockType;
				readonly Locker locker;

                internal LockToken(LockType lockType, Locker locker)
                {
                    this.lockType = lockType;
                    this.locker = locker;
                }
                LockToken() { }

                public void Dispose()
                {
                    locker.DisposeToken(this);
                }
            }
        }
        class Accesser
		{

		}
	}


    internal class SocketLocker : SocketSynchronizer
    {
        #region Field's
        bool socketSendSoftLocked,
             socketSendHardLocked,
             socketReceiveLocked;
        int sendingLockersCount,
            sendingHardLockersCount,
            receivingLockersCount;

        #endregion
        #region Method's
        internal override SocketAccessToken GetSendAccess()
        {
            while (sendState == 0 || socketSendSoftLocked)
            {
                WaitForSendAccess();
            }
            return new SocketAccessToken(this, AccessType.Send, sendCTS.Token);
        }
        internal SocketAccessToken GetHighPrioritySendAccess()
        {
            while (sendState == 0 || socketSendHardLocked)
            {
                WaitForSendAccess();
            }
            Interlocked.Exchange(ref sendState, 0);
            return new SocketAccessToken(this, AccessType.Send, sendCTS.Token);
        }
        internal override SocketAccessToken GetReceiveAccess()
        {
            while (receiveState == 0 || socketReceiveLocked)
            {
                WaitForReceiveAccess(); 
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
        Synchonizer sender,
                    reciever;
        SpinWait spin;

        internal void AbortReceiving()
        {
            reciever.Abort();
        }
        internal void AbortReceiving(Action onAbortationCompleted)
        {
            reciever.Abort(onAbortationCompleted);
        }
        internal void AbortSending()
        {
            sender.Abort(); // TODO: кладёт поток на лопатки при вызове, не создавая никаких исключений.
        }
        internal void AbortSending(Action onAbortationCompleted)
        {
            sender.Abort(onAbortationCompleted);
        }
        internal virtual SocketAccessToken GetSendAccess()
        {
            while (IsSocketLocked(AccessType.Send))
            {
                WaitForSendAccess();
            }
            sender.SetState(0);
            return new SocketAccessToken(this, AccessType.Send, sender.Token);
        }
        internal virtual SocketAccessToken GetReceiveAccess()
        {
            while (IsSocketLocked(AccessType.Receive))
            {
                WaitForReceiveAccess();
            }
            reciever.SetState(0);
            return new SocketAccessToken(this, AccessType.Receive, reciever.Token);
        }
        protected virtual bool IsSocketLocked(AccessType type)
		{
            return type == AccessType.Receive 
                        ? reciever.State == 0 
                        : sender.State == 0;
		}
        private protected void WaitForSendAccess()
        {
            if (!spin.NextSpinWillYield)
            {
                spin.SpinOnce();
            }
            else
            {
                sender.WaitForAccess();
            }
        }
        private protected void WaitForReceiveAccess()
        {
            if (!spin.NextSpinWillYield)
            {
                spin.SpinOnce();
            }
            else
            {
                reciever.WaitForAccess();
            }
        }
        internal virtual void Release(SocketAccessToken token)
        {
            if (token.type == AccessType.Send)
            {
                sender.Release();
            }
            else
            {
                reciever.Release();
            }
        }

        internal SocketSynchronizer()
        {
            reciever = new Synchonizer();
            sender = new Synchonizer();
            spin = new SpinWait();
        }

        private class Synchonizer
		{
            internal int State; // 1 - available, 0 - non

            readonly AutoResetEvent ARE;
            int waitingThreadsCount;
            CancellationTokenSource CTS;

			internal Synchonizer()
			{
                State = 1;
                waitingThreadsCount = 0;
                ARE = new AutoResetEvent(false);
                CTS = new CancellationTokenSource();
			}

			internal CancellationToken Token => CTS.Token;
            bool ThreadsWaiting => waitingThreadsCount != 0; // TODO: именование

            internal void Abort()
			{
                CTS.Token.Register(ResetCTS);
                CTS.Cancel();
			}
            void ResetCTS()
            {
                CTS = new CancellationTokenSource();
            }
            internal void Abort(Action onAbortationCompleted)
			{
                CTS.Token.Register(onAbortationCompleted);
                Abort();
			}
            internal void SetState(int state)
			{
                bool validState = IsValidState(state);
				if (validState)
				{
                    Interlocked.Exchange(ref State, state);
                }
				else
				{
                    string msg = "state should only be 1 for avaliable and 0 for non";
                    throw new ArgumentOutOfRangeException("state", state, msg);
				}
			}
            bool IsValidState(int state)
			{
                return state == 0 || state == 1;
			}
            internal void Release()
			{
                if(ThreadsWaiting)
				{
                    ARE.Set();
				}
                SetState(1);
			}
            internal void WaitForAccess()
			{
                Interlocked.Increment(ref waitingThreadsCount);
                ARE.WaitOne();
                Interlocked.Decrement(ref waitingThreadsCount);
            }
        }
    }
    internal class SocketAccessToken : IDisposable
    {
        internal readonly AccessType type;
        readonly SocketSynchronizer synchronizer;

        internal SocketAccessToken(SocketSynchronizer synchronizer, 
                                    AccessType type, 
                                    CancellationToken cancellationToken)
        {
            this.type = type;
            this.synchronizer = synchronizer;
            CancellationToken = cancellationToken;
        }

        internal CancellationToken CancellationToken { get; private set; }

        internal void OnCancellation(Action<object> action, object state)
        {
            CancellationToken.Register(action, state);
        }
        public void Dispose()
        {
            synchronizer.Release(this);
        }
    }
    internal enum AccessType : byte
    {
        Send,
        Receive
    }
}