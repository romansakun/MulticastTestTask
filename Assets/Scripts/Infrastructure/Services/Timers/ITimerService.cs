using System;

namespace Infrastructure.Services
{
    public interface ITimerService 
    {
        event Action OnTick;
        IDisposable SetTimer(DateTime endTime, Action<TimeSpan> onTick, Action onEnd, int intervalMs = 0);
        IDisposable SetTimer(int durationSeconds, Action<TimeSpan> onTick, Action onEnd, int intervalMs = 0);
    }
}