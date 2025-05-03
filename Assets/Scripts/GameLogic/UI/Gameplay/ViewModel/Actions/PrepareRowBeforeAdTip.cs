// using System;
// using System.Collections.Generic;
// using Cysharp.Threading.Tasks;
// using GameLogic.Audio;
// using GameLogic.Bootstrapper;
// using GameLogic.Factories;
// using GameLogic.Model.DataProviders;
// using Infrastructure.Extensions;
// using UnityEngine;
// using Zenject;
//
// namespace GameLogic.UI.Gameplay
// {
//     public class PrepareRowBeforeAdTip : BaseGameplayViewModelAction 
//     {
//         [Inject] private UserContextDataProvider _userContext;
//         [Inject] private GameDefsDataProvider _gameDefs;
//         [Inject] private ViewManager _viewManager;
//         [Inject] private ViewModelFactory _viewModelFactory;
//         [Inject] private ColorsSettings _colorsSettings;
//         [Inject] private GameplaySettings _gameplaySettings;
//         [Inject] private Cluster.Factory _clusterFactory;
//         [Inject] private AudioPlayer _audioPlayer;
//         [Inject] private SoundsSettings _soundsSettings;
//
//         public override async UniTask ExecuteAsync(GameplayViewModelContext context)
//         {
//             var levelDef = _gameDefs.Levels[context.LevelProgress.LevelDefId];
//
//             foreach (var pair in levelDef.Words)
//             {
//                 var word = pair.Key;
//                 foreach (var wordRow in context.WordRowsClusters)
//                 {
//                     var playerWord = context.WordRowsClusters.GetWord(wordRow.Key);
//                     if (playerWord.Equals(word, StringComparison.CurrentCultureIgnoreCase) == false) 
//                         continue;
//
//                     context.AdTip.FormedWords.Add(word);
//                     context.AdTip.ImmutableWordRows.Add(wordRow.Key);
//                 }
//                 if (context.AdTip.FormedWords.Contains(word))
//                     continue;
//
//                 context.AdTip.NotFormedWords.Add(word);
//             }
//             if (context.AdTip.FormedWords.Count == levelDef.Words.Count)
//             {
//                 Debug.LogWarning("All words are guessed");
//                 return;
//             }
//
//             var availableClusters = new List<Cluster>();
//             availableClusters.AddRange(context.DistributedClusters);
//             availableClusters.AddRange(context.UndistributedClusters);
//             foreach (var immutableWordRow in immutableWordRows)
//             {
//                 var immutableClusters = context.WordRowsClusters[immutableWordRow];
//                 availableClusters.RemoveAll(immutableClusters.Contains);
//             }
//
//             
//             var needWordRow = context.WordRows.Find(wr => immutableWordRows.Contains(wr));
//             var clusters = context.WordRowsClusters[needWordRow];
//             for (var i = clusters.Count - 1; i >= 0; i--)
//             {
//                 var cluster = clusters[i];
//                 context.WordRowsClusters.RemoveCluster(cluster);
//                 context.DistributedClusters.Remove(cluster);
//                 context.UndistributedClusters.Add(cluster);
//                 cluster.SetParent(context.UndistributedClustersHolder);
//
//                 await UniTask.Delay(250);
//                 if (context.IsDisposed) return;
//             }
//
//             var needWord = unguessedWords[0];
//             var needWordClusters = levelDef.Words.GetWordClusters(needWord);
//             for (int i = needWordClusters.Count - 1; i >= 0; i--)
//             {
//                 var clusterText = needWordClusters[i];
//                 var cluster = rowsFreeClusters.Find(c => c.GetText() == clusterText);
//                 if (cluster != null)
//                 {
//                     rowsFreeClusters.Remove(cluster);
//                     needWordClusters.Remove(clusterText);
//                     context.WordRowsClusters.RemoveCluster(cluster);
//                     context.DistributedClusters.Remove(cluster);
//                     context.UndistributedClusters.Add(cluster);
//                     cluster.SetParent(context.UndistributedClustersHolder);
//
//                     await UniTask.Delay(250);
//                     if (context.IsDisposed) return;
//                 }
//             }
//
//             needWordClusters = levelDef.Words.GetWordClusters(needWord);
//             for (int i = 0; i < needWordClusters.Count; i++)
//             {
//                 var clusterText = needWordClusters[i];
//                 var cluster = context.UndistributedClusters.Find(c => c.GetText() == clusterText);
//
//                 cluster.SetBackgroundColor(_colorsSettings.GhostClusterBackColor);
//                 cluster.SetTextColor(_colorsSettings.GhostClusterTextColor);
//
//                 _viewManager.TryGetView<GameplayView>(out var gameplayView);
//                 var originalClusterText = cluster.GetText();
//                 var clickedCluster = _clusterFactory.Create();
//                 clickedCluster.SetParent(gameplayView.transform);
//                 clickedCluster.SetText(originalClusterText);
//                 clickedCluster.SetBackgroundColor(_colorsSettings.SelectedClusterBackColor);
//                 clickedCluster.SetTextColor(_colorsSettings.SelectedClusterTextColor);
//                 clickedCluster.SetRotation(_gameplaySettings.DraggedClusterRotation);
//                 var position = cluster.GetPosition();
//                 var offset = _gameplaySettings.DraggedClusterOffsetPosition.AddZ();
//                 clickedCluster.SetPosition(position + offset);
//                 var hintCluster = AddHintClusterToWordRow(context, needWordRow, cluster);
//
//                 await UniTask.Delay(125);
//                 if (context.IsDisposed) return;
//
//                 _audioPlayer.PlaySound(_soundsSettings.DropClusterSound);
//
//                 context.AllClusters.Remove(hintCluster);
//                 hintCluster.Dispose();
//
//                 context.WordRowsClusters[needWordRow].Add(cluster);
//                 context.UndistributedClusters.Remove(cluster);
//                 context.DistributedClusters.Add(cluster);
//
//                 cluster.SetParent(needWordRow.ClustersHolder);
//                 cluster.SetBackgroundColor(_colorsSettings.DefaultClusterBackColor);
//                 cluster.SetTextColor(_colorsSettings.DefaultClusterTextColor);
//
//                 clickedCluster.Dispose();
//
//                 await UniTask.Delay(125);
//                 if (context.IsDisposed) return;
//             }
//         }
//
//         private Cluster AddHintClusterToWordRow(GameplayViewModelContext context, WordRow wordRow, Cluster cluster)
//         {
//             var hintCluster = CreateHintCluster(context, cluster);
//             hintCluster.SetParent(wordRow.ClustersHolder);
//             context.AllClusters.Add(hintCluster);
//             return hintCluster;
//         }
//
//         private Cluster CreateHintCluster(GameplayViewModelContext context, Cluster cluster)
//         {
//             var hintCluster = _clusterFactory.Create();
//             hintCluster.SetBackgroundColor(_colorsSettings.GhostClusterBackColor);
//             hintCluster.SetTextColor(_colorsSettings.GhostClusterTextColor);
//             hintCluster.SetText(cluster.GetText());
//             return hintCluster;
//         }
//     }
// }
