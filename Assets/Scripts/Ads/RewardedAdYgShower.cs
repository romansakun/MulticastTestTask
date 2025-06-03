// using Cysharp.Threading.Tasks;
// using GameLogic.Ads;
// using UnityEngine;
// using YG;
//
// namespace Ads
// {
//     public class RewardedAdYgShower : MonoBehaviour, IAdsShower
//     {
//         private bool _isReward = false;
//         private bool _isError = false;
//         private bool _isClose = false;
//
//         private void Awake()
//         {
//             YG2.onErrorRewardedAdv += HandleError; 
//             YG2.onErrorAnyAdv += HandleError; 
//             YG2.onCloseAnyAdv += HandleClose; 
//         }
//
//         private void OnDestroy()
//         {
//             YG2.onErrorRewardedAdv -= HandleError; 
//             YG2.onErrorAnyAdv -= HandleError; 
//             YG2.onCloseAnyAdv -= HandleClose; 
//         }
//
//         private void HandleClose()
//         {
//             _isClose = true;
//         }
//
//         private void HandleError()
//         {
//             _isError = true;
//         }
//
//         public async UniTask<bool> Show()
//         {
//             _isReward = false;
//             _isError = false;
//             _isClose = false;
//
//             YG2.RewardedAdvShow(RewardedAds.YG_REWARDED_AD_ID, () =>
//             {
//                 _isReward = true;
//             });
//
//             while (_isReward == false && _isError == false && _isClose == false)
//             {
//                 await UniTask.Yield();
//             }
//             await UniTask.Yield();
//
//             return _isReward;
//         }
//     }
// }