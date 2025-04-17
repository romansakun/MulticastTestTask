using System;
using System.Collections.Generic;

namespace Infrastructure.LogicUtility
{
    public class LogicBuilder<TContext> where TContext : class, IContext
    {
        private readonly List<IBuildNode> _builderNodes = new();
        private IBuildNode _rootBuildNode = null;
        private TContext _context = null;
        private bool _safeMode = false;

        public LogicBuilder<TContext> AddContext(TContext context)
        {
            _context = context;
            return this;
        }

        public LogicBuilder<TContext> SetSafeMode(bool safeMode)
        {
            _safeMode = safeMode;
            return this;
        }

        public BuilderSelector<TContext> AddSelector<T>() where T : ISelector<TContext>
        {
            var selector = new BuilderSelector<TContext>(this, typeof(T));
            _builderNodes.Add(selector);
            return selector;
        }

        public BuildAction<TContext> AddAction<T>() where T : IAction<TContext>
        {
            var action = new BuildAction<TContext>(this, typeof(T));
            _builderNodes.Add(action);
            return action;
        }        

        public BuildActionWithOptions<TContext> AddActionWithOptions<T>() where T : IAction<TContext>
        {
            var action = new BuildActionWithOptions<TContext>(this, typeof(T));
            _builderNodes.Add(action);
            return action;
        }

        public void SetAsRootNode(IBuildNode rootNode)
        {
            if (_rootBuildNode != null)
                throw new Exception($"Root node already was set: {_rootBuildNode.NodeType.Name}");

            _rootBuildNode = rootNode;
        }

        public LogicAgent<TContext> Build()
        {
            if (_rootBuildNode == null)
                throw new Exception($"Root node not was set!");

            var nodes = new Dictionary<int, INode<TContext>>(_builderNodes.Capacity);
            var mapNodeIds = new Dictionary<IBuildNode, int>(_builderNodes.Capacity);
            var nodeId = 1;

            //create nodes:
            foreach(var builderNode in _builderNodes)
            {
                var node = TryCreateByType<INode<TContext>>(builderNode.NodeType);
                if (_rootBuildNode == builderNode)
                {
                    nodes.Add(0, node);
                    mapNodeIds.Add(builderNode, 0);
                }
                else
                {
                    nodes.Add(nodeId, node);
                    mapNodeIds.Add(builderNode, nodeId);
                    nodeId++;
                }

                if (builderNode is BuilderSelector<TContext> builderSelector)
                {
                    if (node is not ISelector<TContext> selector)
                        throw new Exception($"Builder selector must contains selector node type!\nCheck your builder selector node type:\n{builderNode.NodeType.FullName}");

                    if (builderSelector.Qualifiers.Count == 0)
                        throw new Exception($"Builder selector must have at least one qualifier!\nCheck your builder selector:\n{builderNode.NodeType.FullName}");

                    foreach(var builderQualifier in builderSelector.Qualifiers)
                    {
                        var qualifier = TryCreateByType<IQualifier<TContext>>(builderQualifier.NodeType);
                        if (qualifier == null)
                            throw new Exception($"Builder qualifier must contains qualifier node type!\nCheck your builder node type:\n{builderQualifier.NodeType.FullName}");

                        selector.Qualifiers.Add(qualifier);
                        nodes.Add(nodeId, qualifier);
                        mapNodeIds.Add(builderQualifier, nodeId);
                        nodeId++;
                    }
                }

                if (builderNode is BuildActionWithOptions<TContext> builderActionWithOptions)
                {
                    if (node is not ActionWithOptions<TContext> actionWithOptions)
                        throw new Exception($"Builder action with options must contains `ActionWithOptions` node type!\nCheck your builder action with options node type:\n{builderNode.NodeType.FullName}");
                    
                    if (builderActionWithOptions.OptionScorersTypes.Count == 0)
                        throw new Exception($"Builder action with options must have at least one option scorer!\nCheck your builder action with options:\n{builderNode.NodeType.FullName}");

                    var optionScorers = new List<IOptionScorer<TContext>>();
                    foreach(var builderOptionScorerType in builderActionWithOptions.OptionScorersTypes)
                    {
                        var optionScorer = TryCreateByType<IOptionScorer<TContext>>(builderOptionScorerType);
                        if (optionScorer == null)
                            throw new Exception($"Builder qualifier must contains qualifier node type!\nCheck your builder node type:\n{builderOptionScorerType.FullName}");

                        optionScorers.Add(optionScorer);
                    }
                    actionWithOptions.SetOptions(optionScorers);
                }
            }

            // connect nodes:
            foreach (var pair in mapNodeIds)
            {
                var mapNode = pair.Key;
                if (mapNode.Next == null)
                    continue;

                nodeId = pair.Value;
                var nodeNextId = mapNodeIds[mapNode.Next];

                if (!nodes.ContainsKey(nodeId) || !nodes.ContainsKey(nodeNextId))
                    throw new Exception($"Nodes in your map dont contains {nodeId} or {nodeNextId}");

                nodes[nodeId].Next = nodes[nodeNextId];
            }

            var rootNode = nodes[0];
            var context = _context ?? TryCreateByType<TContext>(typeof(TContext));
            var logicAgent = new LogicAgent<TContext>(context , rootNode, _safeMode);
            return logicAgent;
        }

        private T TryCreateByType<T>(Type type)
        {
            T obj;
            try
            {
                obj = CreateByType<T>(type);
            }
            catch (Exception error)
            {
                throw new Exception($"Cant create: {error.Message}\n{error.StackTrace}");
            }
            if (obj == null)
            {
                throw new Exception($"Cant create object for type: '{type.Name}'");
            }

            return obj;
        }

        protected virtual T CreateByType<T>(Type type)
        {
            var creatingObject = Activator.CreateInstance(type) ?? throw new Exception($"Cant create object by type: {type.Name}");
            return (T)creatingObject;
        }
    }
}