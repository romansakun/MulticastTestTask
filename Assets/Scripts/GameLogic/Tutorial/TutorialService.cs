using System;
using GameLogic.Bootstrapper;
using GameLogic.Factories;
using GameLogic.Model.DataProviders;
using GameLogic.UI;
using Zenject;

namespace GameLogic.Tutorial
{
    public class TutorialService: IInitializable, IDisposable
    {
        [Inject] private UserContextDataProvider _userContext;
        [Inject] private GameDefsDataProvider _gameDefs;
        [Inject] private LogicBuilderFactory _logicBuilderFactory;
        [Inject] private GameAppReloader _gameAppReloader;
        [Inject] private ViewModelFactory _viewModelFactory;
        [Inject] private ViewManager _viewManager;


        
        public void Initialize()
        {
            
        }

        public void Dispose()
        {
            
        }
    }
}