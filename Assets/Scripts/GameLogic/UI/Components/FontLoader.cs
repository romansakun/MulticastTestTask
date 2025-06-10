using Infrastructure.Services;
using TMPro;
using UnityEngine;
using Zenject;

namespace GameLogic.UI.Components
{
    public class FontLoader : MonoBehaviour
    {
        [Inject] IAssetsLoader _assetsLoader;

        [SerializeField] private TextMeshProUGUI _textMeshPro;

        private void OnValidate()
        {
            _textMeshPro ??= GetComponent<TextMeshProUGUI>();
        }

        private async void Awake()
        {
            _textMeshPro ??= GetComponent<TextMeshProUGUI>();
            var font = await _assetsLoader.LoadAsync<TMP_FontAsset>("FontSDF");
            font.material.shader = await _assetsLoader.LoadAsync<Shader>("FontShader");
            _textMeshPro.font = font;
        }
    }
}