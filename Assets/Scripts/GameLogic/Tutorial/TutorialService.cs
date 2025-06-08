using System;
using System.Collections.Generic;
using GameLogic.Bootstrapper;
using GameLogic.Factories;
using GameLogic.Model.DataProviders;
using GameLogic.UI;
using Zenject;

namespace GameLogic.Tutorial
{
    public class TutorialService: IInitializable, IDisposable
    {
        [Inject] private DiContainer _diContainer;
        [Inject] private UserContextDataProvider _userContext;
        [Inject] private GameDefsDataProvider _gameDefs;
        [Inject] private LogicBuilderFactory _logicBuilderFactory;
        [Inject] private GameAppReloader _gameAppReloader;
        [Inject] private ViewModelFactory _viewModelFactory;
        [Inject] private ViewManager _viewManager;

        private readonly List<TutorialComponent> _tutorialComponents = new();
        
        public void Initialize()
        {
            
        }

        public void Dispose()
        {
            
        }

        public void RegisterComponent(TutorialComponent tutorialComponent)
        {
            _tutorialComponents.Add(tutorialComponent);
        }
    }
}