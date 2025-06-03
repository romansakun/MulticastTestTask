using System;
using Cysharp.Threading.Tasks;
using GameLogic.Ads;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

namespace Ads
{
    public class RewardedAdYandexShower : MonoBehaviour, IAdsShower
    {
        private RewardedAdLoader _rewardedAdLoader;
        private RewardedAd _rewardedAd;
        private Reward _reward;
        private string _message = "";
        private bool _isRequesting = false;
        private bool _isAdDismissed = false;

        public void Awake()
        {
            //Sets COPPA restriction for user age under 13
            MobileAds.SetAgeRestrictedUser(true);

            _rewardedAdLoader = new RewardedAdLoader();
            _rewardedAdLoader.OnAdLoaded += HandleAdLoaded;
            _rewardedAdLoader.OnAdFailedToLoad += HandleAdFailedToLoad;

            RequestRewardedAd();
        }

        public UniTask<bool> Show()
        {
#if UNITY_EDITOR
            return UniTask.FromResult(true);
#else
            return ShowInternal();
#endif
        }

        private async UniTask<bool> ShowInternal()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return false;

            _reward = null;
            if (await TryShowRewardedAd() == false)
                return false;

            _isAdDismissed = false;
            var waitSecondsTask = UniTask.Delay(TimeSpan.FromSeconds(120));
            var waitAdDismissedTask = UniTask.WaitWhile(() => _isAdDismissed == false);
            var waitRewardTask = UniTask.WaitWhile(() => _reward == null);
            await UniTask.WhenAny(waitAdDismissedTask, waitSecondsTask, waitRewardTask);

            return _reward != null;
        }

        private void RequestRewardedAd()
        {
            _isRequesting = true;
            if (_rewardedAd != null)
            {
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }
            _rewardedAdLoader.LoadAd(CreateAdRequest(RewardedAds.RUSTORE_REWARDED_AD_ID));
            DisplayMessage("Rewarded Ad is requested");
        }

        private async UniTask<bool> TryShowRewardedAd()
        {
            if (_rewardedAd == null)
            {
                DisplayMessage("RewardedAd is not ready yet");
                if (_isRequesting == false)
                    RequestRewardedAd();
                
                await UniTask.WaitWhile(() => _rewardedAd == null);
            }
            if (_rewardedAd == null)
                return false;

            _rewardedAd.OnAdClicked += HandleAdClicked;
            _rewardedAd.OnAdShown += HandleAdShown;
            _rewardedAd.OnAdFailedToShow += HandleAdFailedToShow;
            _rewardedAd.OnAdImpression += HandleImpression;
            _rewardedAd.OnAdDismissed += HandleAdDismissed;
            _rewardedAd.OnRewarded += HandleRewarded;

            _rewardedAd.Show();
            return true;
        }

        private AdRequestConfiguration CreateAdRequest(string adUnitId)
        {
            return new AdRequestConfiguration.Builder(adUnitId).Build();
        }

        #region Rewarded Ad callback handlers

        private void HandleAdLoaded(object sender, RewardedAdLoadedEventArgs args)
        {
            DisplayMessage("HandleAdLoaded event received");
            _isRequesting = false;
            _rewardedAd = args.RewardedAd;
        }

        private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            DisplayMessage($"HandleAdFailedToLoad event received with _message: {args.Message}");
            RequestRewardedAd();
        }

        private void HandleAdClicked(object sender, EventArgs args)
        {
            DisplayMessage("HandleAdClicked event received");
        }

        private void HandleAdShown(object sender, EventArgs args)
        {
            DisplayMessage("HandleAdShown event received");
        }

        private void HandleAdDismissed(object sender, EventArgs args)
        {
            DisplayMessage("HandleAdDismissed event received");

            _rewardedAd?.Destroy();
            _rewardedAd = null;
            _isAdDismissed = true;
        }

        private void HandleImpression(object sender, ImpressionData impressionData)
        {
            var data = impressionData == null ? "null" : impressionData.rawData;
            DisplayMessage($"HandleImpression event received with data: {data}");
        }

        private void HandleRewarded(object sender, Reward args)
        {
            DisplayMessage($"HandleRewarded event received: amout = {args.amount}, type = {args.type}");
            RequestRewardedAd();

            //successful
            _reward = args;
        }

        private void HandleAdFailedToShow(object sender, AdFailureEventArgs args)
        {
            DisplayMessage($"HandleAdFailedToShow event received with _message: {args.Message}");
            RequestRewardedAd();
        }

        private void DisplayMessage(String message)
        {
            _message = message + (_message.Length == 0 ? "" : "\n--------\n" + _message);
            print(message);
        }

        #endregion

    }
}