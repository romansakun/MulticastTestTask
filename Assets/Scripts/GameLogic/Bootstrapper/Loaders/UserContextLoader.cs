using Cysharp.Threading.Tasks;
using GameLogic.Model;
using GameLogic.Model.Contexts;
using GameLogic.Model.DataProviders;
using GameLogic.Model.Operators;
using GameLogic.Model.Repositories;
using Infrastructure;
using Infrastructure.Services;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class UserContextLoader : IAsyncOperation
    {
        [Inject] private DiContainer _diContainer;
        [Inject] private IFileService _fileService;
        [Inject] private GameDefsDataProvider _gameDefs;
        [Inject] private SignalBus _signalBus;

        private UserContext _userContext;

        public async UniTask ProcessAsync()
        {
            if (TryLoadingPlayerContext() == false)
            {
                CreateNewPlayerContext();
            }

            var repository = _diContainer.Instantiate<UserContextRepository>(new object[] { _userContext });
            repository.TryUpdateFreeConsumablesCount();
            Debug.Log($"Last update consumables: {_userContext.Consumables.LastFreeUpdateTime}");

            _diContainer.Bind<UserContextDataProvider>().AsSingle().WithArguments(repository);
            _diContainer.Bind<UserContextOperator>().AsSingle().WithArguments(repository);

            await UniTask.Yield();
            _signalBus.Fire<UserContextInitializedSignal>();
        }

        private bool TryLoadingPlayerContext()
        {
            if (_fileService.TryReadAllText(GamePaths.PlayerContext, out var json) == false)
                return false;

            _userContext = JsonConvert.DeserializeObject<UserContext>(json);
            return true;
        }

        private void CreateNewPlayerContext()
        {
            _userContext = new UserContext
            {
                LocalizationDefId = GetLocalizationDefId()
            };

            var json = JsonConvert.SerializeObject(_userContext, Formatting.Indented);
            _fileService.WriteAllText(GamePaths.PlayerContext, json);
        }

        private string GetLocalizationDefId()
        {
            var systemLanguage = Application.systemLanguage.ToString();
            if (_gameDefs.Localizations.TryGetValue(systemLanguage, out var localizationDef) == false)
            {
                var defaultLocalization = _gameDefs.DefaultSettings.LocalizationDefId;
                localizationDef = _gameDefs.Localizations[defaultLocalization];
            }

            return localizationDef.Id;
        }

    }
}