using System;
using System.Collections.Generic;

namespace Infrastructure.LogicUtility
{
    public class BuilderSelector<TContext> : IBuildNode where TContext : class, IContext
    {
        public LogicBuilder<TContext> LogicBuilder { get; }
        public Type NodeType { get; }
        public List<BuilderQualifier> Qualifiers { get; }
        public IBuildNode Next { get; private set; }

        public BuilderSelector(LogicBuilder<TContext> builder, Type selectorType)
        {
            LogicBuilder = builder;
            NodeType = selectorType;
            Qualifiers = new List<BuilderQualifier>();
        }

        public BuilderSelector<TContext> AddQualifier<T>(IBuildNode next) where T : IQualifier<TContext>
        {
            if (next == this)
                throw new Exception($"Direction node for qualifier can't be his selector!");

            var qualifier = CreateBuilderQualifier(typeof(T));
            qualifier.DirectTo(next);
            return this;
        }

        private BuilderQualifier CreateBuilderQualifier(Type type)
        {
            if (Qualifiers.Exists(x => x.NodeType == type))
                throw new Exception($"{type.Name} qualifier already was added!");

            var builderQualifier = new BuilderQualifier(type);
            Qualifiers.Add(builderQualifier);
            return builderQualifier;
        }

        public void DirectTo (IBuildNode next)
        {
            if (next == this)
                throw new Exception($"Direction node can't be owner!");

            Next = next;
        }

        public BuilderSelector<TContext> SetAsRoot()
        {
            LogicBuilder.SetAsRootNode(this);
            return this;
        }
    }
}