using System;

namespace AdaptivePredicate
{
    internal class FrequencyLimitPredicate
    {
        private readonly int maxExecutionsPerSecond;
        private readonly object locker;
        private int currentExecutionsPerSecond;
        private int currentBaseMs;

        public FrequencyLimitPredicate(int maxExecutionsPerSecond)
        {
            this.maxExecutionsPerSecond = maxExecutionsPerSecond;
            this.locker = new object();
            this.currentExecutionsPerSecond = 0;
            this.currentBaseMs = Environment.TickCount;
        }

        public bool Evaluate()
        {
            if (this.maxExecutionsPerSecond < 1)
            {
                return false;
            }

            var nowMs = Environment.TickCount;
            lock (this.locker)
            {
                var deltaMilliseconds = nowMs - this.currentBaseMs;
                if (deltaMilliseconds < 0 ||
                    deltaMilliseconds >= 1000)
                {
                    this.currentBaseMs = nowMs;
                    this.currentExecutionsPerSecond = 1;
                    return true;
                }

                if (this.currentExecutionsPerSecond < this.maxExecutionsPerSecond)
                {
                    this.currentExecutionsPerSecond++;
                    return true;
                }

                return false;                
            }
        }
    }
}