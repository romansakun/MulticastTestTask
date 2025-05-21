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
        [SerializeField] private GameObject _loadingCircle;

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


        private const string VideoUrl = "https://chess4chess.somee.com/Videos/";

        private void PlayHelpVideo()
        {
#if UNITY_WEBGL
            _videoPlayer.url = VideoUrl + _videoClips[_currentVideoIndex].name + ".mp4";
#else
            _videoPlayer.clip = _videoClips[_currentVideoIndex];
#endif
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