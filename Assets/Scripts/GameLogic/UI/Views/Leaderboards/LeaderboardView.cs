using Cysharp.Threading.Tasks;
using GameLogic.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic.UI.Leaderboards
{
    public class LeaderboardView : View
    {
        [SerializeField] private ViewContentAnimations _animations;
        [SerializeField] private RectTransform _playersContainer;
        [SerializeField] private RectTransform _myPlayerContainer;
        //[SerializeField] private ScrollRect  _scrollRect;
        [SerializeField] private GameObject _thomb;
        [SerializeField] private Button _closeButton;

        private LeaderboardViewModel _viewModel;

        public override UniTask Initialize(ViewModel viewModel)
        {
            UpdateViewModel(ref _viewModel, viewModel);

            _viewModel.CreatePlayerLines(_playersContainer, _myPlayerContainer);

            return UniTask.CompletedTask;
        }

        protected override void Subscribes()
        {
            _closeButton.onClick.AddListener(_viewModel.OnCloseButtonClicked);
            _viewModel.IsLeaderboardLoaded.Subscribe(OnLeaderboardLoaded);
        }

        protected override void Unsubscribes()
        {
            _closeButton.onClick.RemoveAllListeners();
            _viewModel.IsLeaderboardLoaded.Unsubscribe(OnLeaderboardLoaded);
        }

        private void OnLeaderboardLoaded(bool state)
        {
            _thomb.SetActive(state == false);
            
        }

        public override UniTask AnimateShowing()
        {
            return _animations.ShowByScaleAndAlpha();
        }

        public override UniTask AnimateClosing()
        {
            return _animations.HideByScaleAndAlpha();
        }
    }
}