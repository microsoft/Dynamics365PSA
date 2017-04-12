using System;

namespace Common.Utilities
{
    public abstract class Timer
    {
        private static Type proxyType;

        public abstract event EventHandler<object> Tick;

        public abstract TimeSpan Interval { get; set; }
        public abstract bool IsEnabled { get; }

        public static void Initialize<TTimer>() where TTimer : Timer
        {
            proxyType = typeof(TTimer);
        }

        public static Timer Create()
        {
            if (proxyType == null)
            {
                throw new Exception("Please call Initialize() to initialize Timer");
            }

            return Activator.CreateInstance(proxyType) as Timer;
        }

        public abstract void Start();

        public abstract void Stop();
    }
}
