using Common.Utilities;
using System;
using Windows.UI.Xaml;

namespace Common.Windows.Utilities
{
    public class WindowsTimer : Timer
    {
        private DispatcherTimer timer;

        public override TimeSpan Interval { get { return timer.Interval; } set { timer.Interval = value; } }
        public override bool IsEnabled { get { return timer.IsEnabled; } }

        public override event EventHandler<object> Tick;

        public WindowsTimer()
        {
            timer = new DispatcherTimer();
            timer.Tick += (o, s) =>
            {
                if (this.Tick != null)
                    this.Tick(o, s);
            };
        }

        public override void Start() { timer.Start(); }

        public override void Stop() { timer.Stop(); }
    }
}
