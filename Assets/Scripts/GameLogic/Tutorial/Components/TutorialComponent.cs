using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameLogic.Tutorial
{
    public class TutorialComponent : MonoBehaviour
    {
        [Inject] private TutorialService _tutorialService;

        [SerializeField] private Graphic _graphic;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private TutorialComponentType _tutorialComponentType;

        public Graphic Graphic => _graphic;
        public RectTransform RectTransform => _rectTransform;
        public TutorialComponentType ComponentType => _tutorialComponentType;

        private void Awake()
        {
            _tutorialService.RegisterComponent(this);
        }
    }
}