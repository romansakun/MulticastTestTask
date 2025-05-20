using Cysharp.Threading.Tasks;
using GameLogic.Audio;
using GameLogic.Bootstrapper;
using GameLogic.Model.DataProviders;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;
using Zenject;

namespace GameLogic.UI.HowToPlayHint
{
    public class HowToPlayHintView : View, IPointerClickHandler
    {
        [Inject] private UserContextDataProvider _userContext;
        [Inject] private AudioPlayer _audioPlayer;
        [Inject] private SoundsSettings _soundsSettings;
        
        [SerializeField] private VideoPlayer _videoPlayer;
        [SerializeField] private VideoClip[] _videoClips;
        [SerializeField] private TextMeshProUGUI _titleText;

        private int _currentVideoIndex = 0;
        private HowToPlayHintViewModel _viewModel;

        public override UniTask Initialize(ViewModel viewModel)
        {
            UpdateViewModel(ref _viewModel, viewModel);

            PlayHelpVideo();

            return UniTask.CompletedTask;
        }

        protected override void Subscribes()
        {
        }

        protected override void Unsubscribes()
        {
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnOkButtonClicked();
        }

        private void OnOkButtonClicked()
        {
            _audioPlayer.PlaySound(_soundsSettings.TapSound);

            if (_currentVideoIndex < _videoClips.Length - 1)
            {
                _currentVideoIndex++;
                _videoPlayer.Stop();
                PlayHelpVideo();
            }
            else
            {
                _viewModel.OkButtonClicked();
            }
        }

        private void PlayHelpVideo()
        {
            _videoPlayer.clip = _videoClips[_currentVideoIndex];
            _titleText.text = _userContext.GetLocalizedText($"VIDEO_TITLE_{_currentVideoIndex + 1}");
            _videoPlayer.Play();
        }
    }
}