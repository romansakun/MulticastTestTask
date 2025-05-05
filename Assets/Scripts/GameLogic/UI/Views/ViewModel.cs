using System;

namespace  GameLogic.UI
{
    public abstract class ViewModel : IDisposable
    {
        public abstract void Initialize();

        public virtual void Dispose()
        {

        }
    }

}