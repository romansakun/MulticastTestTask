using Infrastructure.Extensions;
using TMPro;
using UnityEngine;

namespace GameLogic.UI.MainMenu
{
    public class FormedWordNumber : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private int _number;

        public RectTransform RectTransform => _rectTransform;
        public int Number => _number;

        public void SetNumber(int number)
        {
            _number = number;
            _text.text = number.ToString();
        }

        public void SetTextColorAlpha(float alpha)
        {
            _text.color = _text.color.WithAlpha(alpha);
        }

    }
}