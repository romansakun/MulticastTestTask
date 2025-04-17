using System;
using System.Collections.Generic;

namespace Infrastructure.LogicUtility
{
    public class BuildAction<TContext> : IBuildNode where TContext : class, IContext
    {
        public LogicBuilder<TContext> LogicBuilder { get; }
        public Type NodeType { get; }
        public IBuildNode Next { get; private set; }

        private readonly List<BuildAction<TContext>> _chainActions = new List<BuildAction<TContext>>();

        public BuildAction(LogicBuilder<TContext> builder, Type actionType)
        {
            LogicBuilder = builder;
            NodeType = actionType;
        }

        public BuildAction<TContext> JoinAction<T>() where T : IAction<TContext>
        {
            var lastAction = _chainActions.Count > 0 
                ? _chainActions[^1] 
                : this;
            var appendedAction = LogicBuilder.AddAction<T>();
            lastAction.DirectTo(appendedAction);
            _chainActions.Add(appendedAction);
            return this;
        }

        public void DirectTo (IBuildNode next)
        {   
            if (next == this)
                throw new Exception($"Direction node can't be owner!");

            if (_chainActions.Exists(ca => ca == next))
                throw new Exception($"Direction node can't be in chain!");

            if (_chainActions.Count > 0)
                _chainActions[^1].Next = next;
            else
                Next = next;
        }

        public BuildAction<TContext> SetAsRoot()
        {
            LogicBuilder.SetAsRootNode(this);
            return this;
        }
    }
}