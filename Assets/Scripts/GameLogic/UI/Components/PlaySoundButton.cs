using GameLogic.Audio;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameLogic.UI.Components
{
    [RequireComponent(typeof(Button))]
    public class PlaySoundButton : MonoBehaviour
    {
        [Inject] private AudioPlayer _audioPlayer;

        [SerializeField] private Button _button;

        private void OnValidate()
        {
            _button ??= GetComponent<Button>();
        }

        private void Awake()
        {
            _button ??= GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            _audioPlayer.PlaySound("TapSound");
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }
    }
}