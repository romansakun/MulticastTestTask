using System;

namespace Infrastructure.LogicUtility
{
    public interface ILogicAgent : IDisposable
    {
        IContext LogicContext { get; }
    }
}