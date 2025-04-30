using System;
using GameLogic.Ads;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

namespace Ads
{
    public class RewardedAdYandexShower : MonoBehaviour, IAdsShower
    {
        private const string REWARDED_AD_ID = "R-M-15325632-1";

        private RewardedAdLoader _rewardedAdLoader;
        private RewardedAd _rewardedAd;
        private string _message = "";

        public void Awake()
        {
            _rewardedAdLoader = new RewardedAdLoader();
            _rewardedAdLoader.OnAdLoaded += HandleAdLoaded;
            _rewardedAdLoader.OnAdFailedToLoad += HandleAdFailedToLoad;

            RequestRewardedAd();
        }

        public void Show(uint slotId)
        {
            TryShowRewardedAd();
        }

        public bool TryShowRewardedAd()
        {
            if (_rewardedAd == null)
            {
                DisplayMessage("RewardedAd is not ready yet");

                return false;
            }

            _rewardedAd.OnAdClicked += HandleAdClicked;
            _rewardedAd.OnAdShown += HandleAdShown;
            _rewardedAd.OnAdFailedToShow += HandleAdFailedToShow;
            _rewardedAd.OnAdImpression += HandleImpression;
            _rewardedAd.OnAdDismissed += HandleAdDismissed;
            _rewardedAd.OnRewarded += HandleRewarded;

            _rewardedAd.Show();
            return true;
        }

        private void RequestRewardedAd()
        {
            DisplayMessage("RewardedAd is not ready yet");
            //Sets COPPA restriction for user age under 13
            MobileAds.SetAgeRestrictedUser(true);

            if (_rewardedAd != null)
            {
                _rewardedAd.Destroy();
            }

            _rewardedAdLoader.LoadAd(CreateAdRequest(REWARDED_AD_ID));
            DisplayMessage("Rewarded Ad is requested");
        }

        private AdRequestConfiguration CreateAdRequest(string adUnitId)
        {
            return new AdRequestConfiguration.Builder(adUnitId).Build();
        }

        #region Rewarded Ad callback handlers

        private void HandleAdLoaded(object sender, RewardedAdLoadedEventArgs args)
        {
            DisplayMessage("HandleAdLoaded event received");
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
        }

        private void HandleImpression(object sender, ImpressionData impressionData)
        {
            var data = impressionData == null ? "null" : impressionData.rawData;
            DisplayMessage($"HandleImpression event received with data: {data}");
        }

        private void HandleRewarded(object sender, Reward args)
        {
            DisplayMessage($"HandleRewarded event received: amout = {args.amount}, type = {args.type}");
        }

        private void HandleAdFailedToShow(object sender, AdFailureEventArgs args)
        {
            DisplayMessage($"HandleAdFailedToShow event received with _message: {args.Message}");
        }

        private void DisplayMessage(String message)
        {
            _message = message + (_message.Length == 0 ? "" : "\n--------\n" + _message);
            print(message);
        }

        #endregion

    }
}