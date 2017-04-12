using Common.Utilities;
using System;

namespace Common.Android.Utilities
{
    public class AndroidTimer : Timer
    {
        private System.Timers.Timer timer;

        public override TimeSpan Interval { get { return TimeSpan.FromMilliseconds(timer.Interval); } set { timer.Interval = value.TotalMilliseconds; } }
        public override bool IsEnabled { get { return timer.Enabled; } }

        public override event EventHandler<object> Tick;

        public AndroidTimer()
        {
            timer = new System.Timers.Timer();
            timer.Elapsed += (o, s) =>
            {
                if (this.Tick != null)
                    this.Tick(o, s);
            };
        }

        public override void Start() { timer.Start(); }

        public override void Stop() { timer.Stop(); }
    }
}