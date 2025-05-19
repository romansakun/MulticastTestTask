namespace GameLogic.UI.Gameplay
{
    public struct ConsumableButtonState
    {
        public int Count { get; set; }
        public bool IsShowCount { get; set; }
        public bool IsShowPlusCount { get; set; }
        public bool IsShowConsumable { get; set; }
        public bool IsShowAdsIcon { get; set; }

        public static ConsumableButtonState State(int count, int maxCount)
        {
            var isAdsNow = count <= 0;
            return new ConsumableButtonState()
            {
                Count = isAdsNow ? maxCount : count,
                IsShowConsumable = isAdsNow == false,
                IsShowCount = count < maxCount,
                IsShowPlusCount = isAdsNow,
                IsShowAdsIcon = isAdsNow
            };
        }
    }

}