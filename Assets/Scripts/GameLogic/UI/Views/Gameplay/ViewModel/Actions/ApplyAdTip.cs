using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameLogic.Audio;
using GameLogic.Bootstrapper;
using GameLogic.Factories;
using GameLogic.Model.DataProviders;
using Infrastructure.Extensions;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class ApplyAdTip : BaseGameplayViewModelAction
    {
        [Inject] private UserContextDataProvider _userContext;
        [Inject] private GameDefsDataProvider _gameDefs;
        [Inject] private ViewManager _viewManager;
        [Inject] private ViewModelFactory _viewModelFactory;
        [Inject] private ColorsSettings _colorsSettings;
        [Inject] private GameplaySettings _gameplaySettings;
        [Inject] private Cluster.Factory _clusterFactory;
        [Inject] private AudioPlayer _audioPlayer;
        [Inject] private SoundsSettings _soundsSettings;

        public override async UniTask ExecuteAsync(GameplayViewModelContext context)
        {
            // cleaning row
            var replacingClusters = context.WordRowsClusters[context.AdTip.SuitableWordRow];
            for (var i = replacingClusters.Count - 1; i >= 0; i--)
            {
                context.UndistributedClustersScrollRectNormalizedPosition.Value = 0;

                var cluster = replacingClusters[i];
                context.WordRowsClusters.RemoveCluster(cluster);
                context.DistributedClusters.Remove(cluster);
                context.UndistributedClusters.Add(cluster);
                cluster.SetParent(context.UndistributedClustersHolder);
                cluster.SetSiblingIndex(0);
                _audioPlayer.PlaySound(_soundsSettings.DropClusterSound);

                await UniTask.Delay(250);
                if (context.IsDisposed) return;
            }
            //

            // resolve available clusters
            var availableRowsClusters = new List<Cluster>();
            availableRowsClusters.AddRange(context.DistributedClusters);
            foreach (var immutableWordRow in context.AdTip.ImmutableWordRows)
            {
                var immutableClusters = context.WordRowsClusters[immutableWordRow];
                availableRowsClusters.RemoveAll(immutableClusters.Contains);
            }
            //


            var levelDef = _gameDefs.Levels[context.LevelProgress.LevelDefId];
            var needWord = context.AdTip.NotFormedWords[0];
            var needWordClusters = levelDef.Words.GetWordClusters(needWord);
            for (int i = 0; i < needWordClusters.Count; i++)
            {
                var clusterText = needWordClusters[i];
                var cluster = context.UndistributedClusters.Find(c => c.IsValueEqual(clusterText));
                if (cluster == null)
                {
                    cluster = availableRowsClusters.Find(c => c.IsValueEqual(clusterText));
                    context.WordRowsClusters.RemoveCluster(cluster);
                    context.DistributedClusters.Remove(cluster);
                    context.UndistributedClusters.Add(cluster);
                    cluster.SetParent(context.UndistributedClustersHolder);
                    cluster.SetSiblingIndex(0);
                    _audioPlayer.PlaySound(_soundsSettings.DropClusterSound);
                    context.UndistributedClustersScrollRectNormalizedPosition.Value = 0;

                    await UniTask.Delay(125);
                    if (context.IsDisposed) return;
                }
                cluster.SetColorAlpha(_colorsSettings.GhostClusterAlpha);

                var clusterIndex = cluster.GetSiblingIndex();
                var scrollValue = clusterIndex / (float) context.UndistributedClusters.Count;
                context.UndistributedClustersScrollRectNormalizedPosition.Value = scrollValue;

                _viewManager.TryGetView<GameplayView>(out var gameplayView);
                var originalClusterText = cluster.GetText();
                var clickedCluster = _clusterFactory.Create();
                clickedCluster.SetParent(gameplayView.transform);
                clickedCluster.SetText(originalClusterText);
                cluster.SetColorAlpha(1);
                clickedCluster.SetRotation(_gameplaySettings.DraggedClusterRotation);
                var position = cluster.GetPosition();
                var offset = _gameplaySettings.DraggedClusterOffsetPosition.AddZ();
                clickedCluster.SetPosition(position + offset);
                var hintCluster = AddHintClusterToWordRow(context, context.AdTip.SuitableWordRow, cluster);

                await UniTask.Delay(125);
                if (context.IsDisposed) return;

                _audioPlayer.PlaySound(_soundsSettings.DropClusterSound);

                context.AllClusters.Remove(hintCluster);
                hintCluster.Dispose();

                context.WordRowsClusters[context.AdTip.SuitableWordRow].Add(cluster);
                context.UndistributedClusters.Remove(cluster);
                context.DistributedClusters.Add(cluster);

                cluster.SetParent(context.AdTip.SuitableWordRow.ClustersHolder);
                cluster.SetColorAlpha(1);

                clickedCluster.Dispose();

                await UniTask.Delay(125);
                if (context.IsDisposed) return;
            }
            
            context.AdTip.Reset();
        }

        private Cluster AddHintClusterToWordRow(GameplayViewModelContext context, WordRow wordRow, Cluster cluster)
        {
            var hintCluster = CreateHintCluster(context, cluster);
            hintCluster.SetParent(wordRow.ClustersHolder);
            context.AllClusters.Add(hintCluster);
            return hintCluster;
        }

        private Cluster CreateHintCluster(GameplayViewModelContext context, Cluster cluster)
        {
            var hintCluster = _clusterFactory.Create();
            cluster.SetColorAlpha(_colorsSettings.GhostClusterAlpha);
            hintCluster.SetText(cluster.GetText());
            return hintCluster;
        }
    }
}