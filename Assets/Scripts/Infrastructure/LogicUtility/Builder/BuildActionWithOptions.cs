using System;
using System.Collections.Generic;

namespace Infrastructure.LogicUtility
{
    public class BuildActionWithOptions<TContext> : IBuildNode where TContext : class, IContext
    {
        public LogicBuilder<TContext> LogicBuilder { get; }
        public Type NodeType { get; }
        public IBuildNode Next { get; private set; }
        public List<Type> OptionScorersTypes { get; }


        public BuildActionWithOptions(LogicBuilder<TContext> builder, Type actionType)
        {
            LogicBuilder = builder;
            NodeType = actionType;
            OptionScorersTypes = new List<Type>();
        }

        public BuildActionWithOptions<TContext> AddOptionScorer<T>() where T : IOptionScorer<TContext>
        {
            OptionScorersTypes.Add(typeof(T));

            return this;
        }

        public void DirectTo(IBuildNode next)
        {
            if (next == this)
                throw new Exception($"Direction node can't be owner!");

            Next = next;
        }

        public BuildActionWithOptions<TContext> SetAsRoot()
        {
            LogicBuilder.SetAsRootNode(this);
            return this;
        }

    }
}