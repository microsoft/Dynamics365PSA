using Common.Utilities;
using System;

namespace Common.iOS.Utilities
{
    public class IOSTimer: Timer
    {
        private System.Timers.Timer timer;
        public override TimeSpan Interval
        {
            get
            {
                return TimeSpan.FromMilliseconds(timer.Interval);
            }
            set
            {
                timer.Interval = value.TotalMilliseconds;
            }
        }

        public override bool IsEnabled
        {
            get { return timer.Enabled; }
        }

        public override event EventHandler<object> Tick;

        public IOSTimer()
        {
            timer = new System.Timers.Timer();
            timer.Elapsed += (o, s) =>
            {
                if (this.Tick != null)
                    this.Tick(o, s);
            };
        }

        public override void Start()
        {
            timer.Start();
        }

        public override void Stop()
        {
            timer.Stop();
        }
    }
}