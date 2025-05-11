using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameLogic.Ads;
using GameLogic.Audio;
using GameLogic.Bootstrapper;
using GameLogic.Model.DataProviders;
using GameLogic.Model.Operators;
using Infrastructure.Extensions;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class TryApplyAdTip : BaseGameplayViewModelAction
    {
        [Inject] private UserContextDataProvider _userContext;
        [Inject] private UserContextOperator _userContextOperator;
        [Inject] private GameDefsDataProvider _gameDefs;
        [Inject] private ViewManager _viewManager;
        [Inject] private ColorsSettings _colorsSettings;
        [Inject] private GameplaySettings _gameplaySettings;
        [Inject] private Cluster.Factory _clusterFactory;
        [Inject] private AudioPlayer _audioPlayer;
        [Inject] private SoundsSettings _soundsSettings;
        [Inject] private IAdsShower _adsShower;

        public override async UniTask ExecuteAsync(GameplayViewModelContext context)
        {
            if (_userContext.AdsTipsCount.Value <= 0)
            {
                var hasReward = await _adsShower.Show();
                if (context.IsDisposed) return;
                if (hasReward)
                {
                    _userContextOperator.AddAdsTip();
                    context.IsTipByAdsActive.Value = false;
                }
                return;
            }


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

                context.AdTip.SuitableWordRow.SetClusterAsChild(cluster);
                cluster.SetColorAlpha(1);

                clickedCluster.Dispose();

                await UniTask.Delay(125);
                if (context.IsDisposed) return;
            }

            context.AdTip.Reset();

            _userContextOperator.UseAdsTip();
            context.IsTipByAdsActive.Value = _userContext.AdsTipsCount.Value <= 0;
        }

        private Cluster AddHintClusterToWordRow(GameplayViewModelContext context, WordRow wordRow, Cluster cluster)
        {
            var hintCluster = CreateHintCluster(context, cluster);
            wordRow.SetClusterAsChild(hintCluster);
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