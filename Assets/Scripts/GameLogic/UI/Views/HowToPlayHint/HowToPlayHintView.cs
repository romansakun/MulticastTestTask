using System.Collections.Generic;
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
        [Inject] private GameDefsDataProvider _gameDefs;
        [Inject] private AudioPlayer _audioPlayer;
        [Inject] private SoundsSettings _soundsSettings;

        [SerializeField] private VideoPlayer _videoPlayer;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private GameObject _loadingCircle;

        private List<string> _videoUrls;
        private int _currentVideoIndex = 0;
        private HowToPlayHintViewModel _viewModel;

        public override UniTask Initialize(ViewModel viewModel)
        {
            UpdateViewModel(ref _viewModel, viewModel);

            _videoUrls = _gameDefs.Localizations[_userContext.LocalizationDefId.Value].TutorialVideos;
            PlayHelpVideo();

            return UniTask.CompletedTask;
        }

        protected override void Subscribes()
        {
            _videoPlayer.waitForFirstFrame = true;
            _videoPlayer.prepareCompleted += OnVideoPlayerPrepareCompleted;
        }

        protected override void Unsubscribes()
        {
            _videoPlayer.prepareCompleted -= OnVideoPlayerPrepareCompleted;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnOkButtonClicked();
        }

        private void OnOkButtonClicked()
        {
            _audioPlayer.PlaySound(_soundsSettings.TapSound);

            if (_currentVideoIndex < _videoUrls.Count - 1)
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


        private const string VideoUrl = "https://chess4chess.somee.com/Videos/";

        private void PlayHelpVideo()
        {
            _videoPlayer.url = VideoUrl + _videoUrls[_currentVideoIndex] + ".mp4";
            _titleText.text = _userContext.GetLocalizedText($"VIDEO_TITLE_{_currentVideoIndex + 1}");

            _loadingCircle.SetActive(true);
            _videoPlayer.Prepare();
        }

        private void OnVideoPlayerPrepareCompleted(VideoPlayer source)
        {
            _loadingCircle.SetActive(false);
            _videoPlayer.Play();
        }

    }
}