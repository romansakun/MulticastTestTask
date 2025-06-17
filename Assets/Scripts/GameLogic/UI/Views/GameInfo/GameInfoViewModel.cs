using System;
using GameLogic.Model.DataProviders;
using GameLogic.UI.MainMenu;
using Infrastructure;
using Infrastructure.Extensions;
using Infrastructure.Services;
using Zenject;

namespace GameLogic.UI.GameInfo
{
    public class GameInfoViewModel : ViewModel
    {
        [Inject] private GameDefsDataProvider _gameDefs;
        [Inject] private UserContextDataProvider _userContext;
        [Inject] private ViewManager _viewManager;
        [Inject] private ITimerService _timerService;

        public IReactiveProperty<string> AdHintInfoText => _adHintInfoText;
        private readonly ReactiveProperty<string> _adHintInfoText = new();

        private IDisposable _timer;

        public override void Initialize()
        {
            if (_userContext.AdsTipsCount.Value != _gameDefs.DefaultSettings.ConsumablesFreeCount)
            {
                SetTimerToHintUpdates();
            }
            else
            {
                _adHintInfoText.Value = _userContext.GetLocalizedText("INFO_AD_HINT");
            }
        }

        private void SetTimerToHintUpdates()
        {
            var duration = _userContext.GetConsumablesUpdateDurationSeconds();
            _timer = _timerService.SetTimer(duration, ts =>
            {
                var infoText = _userContext.GetLocalizedText("INFO_AD_HINT");
                var leftText = _userContext.GetLocalizedText("LEFT");
                _adHintInfoText.Value = ts.HmsD2($"{infoText}\n({leftText}: ", ")");
            }, null, 1000);
        }

        public async void OnBackButtonClicked()
        {
            _viewManager.TryGetView<MainMenuView>(out var view);
            view.gameObject.SetActive(true);
            await _viewManager.Close<GameInfoView>();
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            _adHintInfoText.Dispose();
        }
    }
}