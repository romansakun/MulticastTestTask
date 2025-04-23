using GameLogic.Model.DataProviders;
using TMPro;
using UnityEngine;
using Zenject;

namespace GameLogic.UI.Components
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizationText : MonoBehaviour
    {
        [Inject] private UserContextDataProvider _userContext;

        [SerializeField] private string _localizationKey;
        [SerializeField] private TextMeshProUGUI _textMeshPro;

        public string LocalizationKey
        {
            get => _localizationKey;
            set
            {
                _localizationKey = value;
                SetLocalizationText();
            }
        }

        private void OnValidate()
        {
            _textMeshPro ??= GetComponent<TextMeshProUGUI>();
        }

        private void Awake()
        {
            _textMeshPro ??= GetComponent<TextMeshProUGUI>();
            LocalizationKey = _localizationKey;
        }

        private void Start()
        {
            _userContext.LocalizationDefId.Subscribe(OnChangeUserLocalization, false);
        }

        private void OnChangeUserLocalization(string localizationDefId)
        {
            SetLocalizationText();
        }

        private void SetLocalizationText()
        {
            if (string.IsNullOrEmpty(LocalizationKey) == false)
                _textMeshPro.text = _userContext.GetLocalizedText(LocalizationKey);
        }

        private void OnDestroy()
        {
            _userContext.LocalizationDefId.Unsubscribe(OnChangeUserLocalization);
        }
    }
}