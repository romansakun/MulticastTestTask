using GameLogic.Bootstrapper;
using GameLogic.Model.DataProviders;
using GameLogic.Model.Operators;
using GameLogic.UI;
using GameLogic.UI.MainMenu;
using UnityEngine;
using Zenject;

namespace Stores
{
    public class StoreBehaviour : MonoBehaviour 
    {
        [Inject] protected SignalBus _signalBus;
        [Inject] protected DiContainer _diContainer;
        [Inject] protected ViewManager _viewManager;

        protected UserContextOperator _userContextOperator;
        protected UserContextDataProvider _userContext;

        protected virtual void Awake()
        {
            _signalBus.Subscribe<UserContextInitializedSignal>(OnUserContextInitializedInternal);
        }

        protected virtual void OnDestroy()
        {
            _signalBus.Unsubscribe<UserContextInitializedSignal>(OnUserContextInitializedInternal);
        }

        private void OnUserContextInitializedInternal(UserContextInitializedSignal signal)
        {
            _userContextOperator = _diContainer.Resolve<UserContextOperator>();
            _userContext = _diContainer.Resolve<UserContextDataProvider>();
            OnUserContextInitialized();
        }

        protected virtual void OnUserContextInitialized()
        {

        }

        protected string GetLocalizationDefId(string lang)
        {
            switch (lang)
            {
                case "ru": return SystemLanguage.Russian.ToString();
                default: return SystemLanguage.English.ToString();
            }
        }

        protected void TrySetActiveLeaderboardsButton()
        {
            if (_userContext == null) return;
            if (_viewManager.TryGetView<MainMenuView>(out var mainMenuView) == false) return;
            var state = _userContext.TryGetLastCompletedLevelProgress(out _);
            mainMenuView.SetActiveLeaderboardsButton(state);
        }

    }
}