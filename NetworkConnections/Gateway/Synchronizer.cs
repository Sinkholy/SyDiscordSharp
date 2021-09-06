using System;
using System.Threading;

namespace Gateway
{
    internal class DoubleStateSynchronizer<T> // TODO: не хотел бы я делать его интернал, 
                                              // но по другому пока не смог
    {
        readonly Locker regularLocker;
        readonly Locker overallLocker;
        readonly DoubleStateQueue queue;
        readonly T target;

        public DoubleStateSynchronizer(T target)
        {
            this.target = target;
            regularLocker = new Locker();
            overallLocker = new Locker();
            queue = new DoubleStateQueue();
        }

        internal Access GetAccess()
        {
            queue.GetAccess();
            regularLocker.EnterLock();
            return new Access(this, target, false);
        }
        internal Access GetPriorityAccess()
        {
            queue.GetPriorityAccess();
            overallLocker.EnterLock();
            return new Access(this, target, true);
        }
        internal Lock EnterRegularLock()
        {
            regularLocker.EnterLock();
            return new Lock(this, false);
        }
        internal Lock EnterOverallLock()
        {
            overallLocker.EnterLock();
            return new Lock(this, true);
        }
        void TryToPassControlToNextThread()
        {
            if (!overallLocker.IsLocked)
            {
                if (queue.PriorityThreadsInQueue)
                {
                    queue.PassControlToNextPriorityThread();
                }
                else if (!regularLocker.IsLocked)
                {
                    queue.PassControlToNextThread();
                }
            }
        }
        internal void DisposeAccess(Access access)
        {
            if (access.IsPriority)
            {
                regularLocker.ExitLock();
            }
            else
            {
                overallLocker.ExitLock();
            }
            TryToPassControlToNextThread();
        }
        internal void DisposeLock(Lock @lock)
        {
            if (@lock.IsPriority)
            {
                regularLocker.ExitLock();
            }
            else
            {
                overallLocker.ExitLock();
            }
            TryToPassControlToNextThread();
        }

        internal class Access : IDisposable
        {
            readonly DoubleStateSynchronizer<T> synchronizer;
            readonly T target;
            readonly bool isPriority;

            internal Access(DoubleStateSynchronizer<T> synchronizer, T target, bool isPriority)
            {
                this.synchronizer = synchronizer;
                this.target = target;
                this.isPriority = isPriority;
            }

            internal T Target => target;
            internal bool IsPriority => isPriority;

            public void Dispose()
            {
                synchronizer.DisposeAccess(this);
            }
        }
        internal class Lock : IDisposable
        {
            readonly DoubleStateSynchronizer<T> synchronizer;
            readonly bool isPriority;

            internal bool IsPriority => isPriority;

            internal Lock(DoubleStateSynchronizer<T> synchronizer, bool isPriority)
            {
                this.synchronizer = synchronizer;
                this.isPriority = isPriority;
            }

            public void Dispose()
            {
                synchronizer.DisposeLock(this);
            }
        }
    }
    internal class Synchronizer<T> // TODO: читай у DoubleStateSync
    {
        readonly Locker locker;
        readonly Queue queue;
        readonly T target;

        internal Synchronizer(T target)
        {
            this.target = target;
            locker = new Locker();
            queue = new Queue();
        }

        internal bool IsLocked => locker.IsLocked;
        internal bool ThreadsAreInQueue => queue.ThreadsInQueue;
        internal int ThreadsInQueueCount => queue.ThreadsInQueueCount;

        internal Access GetAccess()
        {
            queue.GetAccess();
            locker.EnterLock();
            return new Access(this, target);
        }
        internal Lock EnterLock()
        {
            locker.EnterLock();
            return new Lock(this);
        }

        internal void DisposeAccess()
        {
            ExitLock();
        }
        void ExitLock()
        {
            locker.ExitLock();
            TryToPassControlToNextThread();

            void TryToPassControlToNextThread()
            {
                if (!locker.IsLocked)
                {
                    if (queue.ThreadsInQueue)
                    {
                        queue.PassControlToNextThread();
                    }
                }
            }
        }
        internal void DisposeLock()
        {
            ExitLock();
        }

        internal class Access : IDisposable
        {
            readonly Synchronizer<T> synchronizer;
            readonly T target;

            internal Access(Synchronizer<T> synchronizer, T target)
            {
                this.synchronizer = synchronizer;
                this.target = target;
            }

            internal T Target => target;

            public void Dispose()
            {
                synchronizer.DisposeAccess();
            }
        }
        internal class Lock : IDisposable
        {
            readonly Synchronizer<T> synchronizer;

            internal Lock(Synchronizer<T> synchronizer)
            {
                this.synchronizer = synchronizer;
            }

            public void Dispose()
            {
                synchronizer.DisposeLock();
            }
        }
    }
    class DoubleStateQueue : Queue
    {
        const int PriorityReserved = 2;

        int priorityThreadsCount;

        internal new bool ThreadsInQueue => base.ThreadsInQueue || priorityThreadsCount > 0;
        internal new int ThreadsInQueueCount => base.ThreadsInQueueCount + priorityThreadsCount;
        internal bool PriorityThreadsInQueue => priorityThreadsCount > 0;
        internal int PriorityThreadsInQueueCount => priorityThreadsCount;
        internal bool RegularThreadsInQueue => base.ThreadsInQueue;
        internal int RegularThreadsInQueueCount => base.ThreadsInQueueCount;

        internal void GetPriorityAccess()
        {
            if (!TryToGetAccess())
            {
                EnqueueForAccess();
            }

            bool TryToGetAccess()
            {
                return TryToSetState(Unavailable, Available) == Available
                    || TryToSetState(Unavailable, PriorityReserved) == PriorityReserved;
            }
            void EnqueueForAccess()
            {
                Interlocked.Increment(ref priorityThreadsCount);
                while (!TryToGetAccess())
                {
                    WaitForAccess();
                }
                Interlocked.Decrement(ref priorityThreadsCount);
            }
        }
        internal new void PassControlToNextThread()
        {
            if (PriorityThreadsInQueue)
            {
                SetState(PriorityReserved);
            }
            else
            {
                SetState(Available);
            }
            SetARE();
        }
        internal void PassControlToNextPriorityThread()
        {
            SetState(PriorityReserved);
            SetARE();
        }
    }
    class Queue
    {
        static SpinWait spin;
        protected const int Available = 1;
        protected const int Unavailable = 0;

        int threadsInQueueCount;
        readonly AutoResetEvent ARE;
        int getState = Available;

        static Queue() // TODO: проверь как работает статичный конструктор
        {
            spin = new SpinWait();
        }
        internal Queue()
        {
            ARE = new AutoResetEvent(false); // TODO: проверь стартовое значние правильно ли стоит
        }

        internal bool ThreadsInQueue => threadsInQueueCount > 0;
        internal int ThreadsInQueueCount => threadsInQueueCount;

        internal void GetAccess()
        {
            if (!TryToGetAccess())
            {
                EnqueueForAccess();
            }

            bool TryToGetAccess()
            {
                return Interlocked.CompareExchange(ref getState, Unavailable, Available) == Available;
            }
            void EnqueueForAccess()
            {
                Interlocked.Increment(ref threadsInQueueCount);
                while (!TryToGetAccess())
                {
                    WaitForAccess();
                }
                Interlocked.Decrement(ref threadsInQueueCount);
            }
        }
        protected void WaitForAccess()
        {
            if (!spin.NextSpinWillYield)
            {
                spin.SpinOnce();
            }
            else
            {
                ARE.WaitOne();
            }
        }
        internal void PassControlToNextThread()
        {
            Interlocked.Exchange(ref getState, Available);
            ARE.Set();
        }
        protected void SetARE()
        {
            ARE.Set();
        }
        protected int TryToSetState(int newState, int comparand)
        {
            return Interlocked.CompareExchange(ref getState, newState, comparand);
        }
        protected int SetState(int state)
        {
            return Interlocked.Exchange(ref getState, state);
        }
    }
    class Locker
    {
        const int Locked = 1;
        const int Unlocked = 0;

        int state = Unlocked;
        int lockersCount;

        internal bool IsLocked => state == Locked;
        internal int LockersCount => lockersCount;

        internal void EnterLock()
        {
            if (IncrementAndReturnIsFirstLocker())
            {
                Lock();
            }

            bool IncrementAndReturnIsFirstLocker()
            {
                return Interlocked.Increment(ref lockersCount) == 1; // TODO: нужен ли здесь интерлокед?
            }
        }
        internal bool ExitLock()
        {
            bool result = false;
            if (DecrementAndReturnIsLastLocker())
            {
                Unlock();
                result = true;
            }
            return result;

            bool DecrementAndReturnIsLastLocker()
            {
                return Interlocked.Decrement(ref lockersCount) == 0;
            }
        }
        protected void Lock()
        {
            Interlocked.CompareExchange(ref state, Locked, Unlocked);
        }
        protected void Unlock()
        {
            Interlocked.CompareExchange(ref state, Unlocked, Locked);
        }
        protected void IncrementLockersCount()
        {
            Interlocked.Increment(ref lockersCount);
        }
        protected void DecrementLockersCount()
        {
            Interlocked.Decrement(ref lockersCount);
        }
    }
}