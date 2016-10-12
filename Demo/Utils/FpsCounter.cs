using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace graf3d.Demo.Utils
{
    /// <summary>
    ///     Licznik klatek na sekundę.
    /// </summary>
    internal class FpsCounter
    {
        private readonly Queue<double> _times = new Queue<double>();
        private Stopwatch _watch;
        public int Average { get; private set; }
        public int High { get; private set; } = -1;
        public int Low { get; private set; } = -1;

        public event Action StatsChanged;

        public void Tick()
        {
            if (_watch == null)
            {
                _watch = Stopwatch.StartNew();
                return;
            }

            _watch.Stop();

            _times.Enqueue(_watch.Elapsed.TotalSeconds);
            while (_times.Count > 60)
            {
                _times.Dequeue();
            }

            var fps = (int)Math.Round(_times.Count / _times.Sum());

            if (fps != Average)
            {
                Average = fps;
                StatsChanged?.Invoke();
            }

            if (fps < Low || Low == -1)
            {
                Low = fps;
                StatsChanged?.Invoke();
            }

            if (fps > High || High == -1)
            {
                High = fps;
                StatsChanged?.Invoke();
            }

            _watch = Stopwatch.StartNew();
        }
    }
}