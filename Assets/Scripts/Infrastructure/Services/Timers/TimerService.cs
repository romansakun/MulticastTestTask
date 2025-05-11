using System;
using Zenject;

namespace Infrastructure.Services
{
    public class TimerService : ITimerService, ITickable
    {
        public event Action OnTick = delegate { };

        public void Tick()
        {
            OnTick.Invoke();
        }

        public IDisposable SetTimer(int durationSeconds, Action<TimeSpan> onTick, Action onEnd, int intervalMs = 0)
        {
            return SetTimer(DateTime.Now.AddSeconds(durationSeconds), onTick, onEnd, intervalMs);
        }

        public IDisposable SetTimer(DateTime endTime, Action<TimeSpan> onTick, Action onEnd, int intervalMs = 0)
        {
            return new Timer(endTime, onTick, onEnd, intervalMs, this);
        }

        private class Timer : IDisposable
        {
            private readonly int _interval;
            private readonly DateTime _endTime;
            private readonly Action _onEnd;
            private readonly Action<TimeSpan> _onTick;
            private readonly ITimerService _timerService;

            private DateTime _intervalEnd;

            public Timer (DateTime endTime, Action<TimeSpan> onTick, Action onEnd, int intervalMS, ITimerService timerService)
            {
                _endTime = endTime;
                _interval = intervalMS;
                _intervalEnd = DateTime.Now;
                _onTick = onTick;
                _onEnd = onEnd;
                _timerService = timerService;
                _timerService.OnTick += Tick;
            }

            private void Tick()
            {
                var now = DateTime.Now;
                var leftTime = _endTime - now;

                if (_interval > 0)
                {
                    var leftInterval = _intervalEnd - now;
                    if (leftInterval.TotalMilliseconds <= 0)
                    {
                        _onTick?.Invoke(leftTime);
                        _intervalEnd = now.AddMilliseconds(_interval);
                    }
                }
                else
                {
                    _onTick.Invoke(leftTime);
                }

                if (leftTime.TotalMilliseconds <= 0)
                {
                    _onEnd?.Invoke();
                    Dispose();
                }
            }

            public void Dispose()
            {
                if (_timerService == null) 
                    return;

                _timerService.OnTick -= Tick;
            }
        }

    }
}