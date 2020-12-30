using System;

namespace AdaptivePredicate
{
    internal class FrequencyLimitPredicate
    {
        private readonly int maxExecutionsPerSecond;
        private readonly object locker;
        private int currentExecutionsPerSecond;
        private int currentBaseMilliseconds;

        public FrequencyLimitPredicate(int maxExecutionsPerSecond)
        {
            this.maxExecutionsPerSecond = maxExecutionsPerSecond;
            this.locker = new object();
            this.currentExecutionsPerSecond = 0;
            this.currentBaseMilliseconds = Environment.TickCount;
        }

        public bool Evaluate()
        {
            if (this.maxExecutionsPerSecond < 1)
            {
                return false;
            }

            var nowMilliseconds = Environment.TickCount;
            lock (this.locker)
            {
                var deltaMilliseconds = nowMilliseconds - this.currentBaseMilliseconds;
                if (deltaMilliseconds >= 0 &&
                    deltaMilliseconds < 1000)
                {
                    if (this.currentExecutionsPerSecond < this.maxExecutionsPerSecond)
                    {
                        this.currentExecutionsPerSecond++;
                        return true;
                    }

                    return false;
                }

                this.currentBaseMilliseconds = nowMilliseconds;
                this.currentExecutionsPerSecond = 1;

                return true;
            }
        }
    }
}