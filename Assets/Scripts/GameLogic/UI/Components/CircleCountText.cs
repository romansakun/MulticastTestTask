using TMPro;
using UnityEngine;

namespace GameLogic.UI.Components
{
    public class CircleCountText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _countText;

        public void SetCount(int count, bool withPlus = false)
        {
            _countText.text = withPlus 
                ? $"+{count}" 
                : count.ToString();
        }
    }
}