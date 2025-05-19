using GameLogic.Bootstrapper;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameLogic.UI.Components
{
    [RequireComponent(typeof(Graphic))]
    public class GraphicColor : MonoBehaviour
    {
        [Inject] private ColorsSettings _colorsSettings;

        [SerializeField] private ElementColor _elementColor;
        [SerializeField] private Graphic _graphic;

        private void OnValidate()
        {
            _graphic ??= GetComponent<Graphic>();
        }

        private void Awake()
        {
            _graphic ??= GetComponent<Graphic>();
            _graphic.color = _colorsSettings.ElementColors[_elementColor];
        }
    }
}