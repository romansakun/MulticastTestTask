using System;
using Infrastructure.LogicUtility;
using Zenject;

namespace Factories
{
    public class LogicBuilderFactory
    {
        [Inject] private DiContainer _diContainer;

        public ZenjectLogicBuilder<TContext> Create<TContext>() where TContext : class, IContext
        {
            var builder = _diContainer.Instantiate<ZenjectLogicBuilder<TContext>>(Array.Empty<object>());
            return builder;
        }
    }

    public class ZenjectLogicBuilder<TContext> : LogicBuilder<TContext> where TContext : class, IContext
    {
        [Inject] private DiContainer _container;

        protected override T CreateByType<T>(Type type)
        {
            var result = (T) _container.Instantiate(type, Array.Empty<object>());
            return result;
        }
    }

}