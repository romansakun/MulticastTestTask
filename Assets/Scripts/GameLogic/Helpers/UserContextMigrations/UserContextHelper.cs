using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameLogic.Model;
using GameLogic.Model.Contexts;
using GameLogic.Model.DataProviders;
using Infrastructure.LogicUtility;
using Infrastructure.Services;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

namespace GameLogic.Helpers
{
    public class UserContextHelper
    {
        private const int ACTUAL_VERSION = 1;

        [Inject] private IFileService _fileService;
        [Inject] private GameDefsDataProvider _gameDefs;

        private UserContext _userContext;
        private LogicAgent<UserContextMigrationContext> _agent;

        public UniTask<bool> TryMigrate(UserContext userContext)
        {
            // var logicBuilder = new LogicBuilder<UserContextMigrationContext>();
            // _agent = logicBuilder.Build();

            var result = false;
            if (userContext.Version == 0)
            {
                userContext.LevelsProgress = new Dictionary<string, LevelProgressContext>();
                userContext.Consumables = new ConsumablesContext();
                userContext.Version = ACTUAL_VERSION;
                result = true;
            }
            if (string.IsNullOrEmpty(userContext.LocalizationDefId))
            {
                userContext.LocalizationDefId = GetLocalizationDefId();
                result = true;
            }

            if (result)
            {
                SavePlayerContext();
            }
            return UniTask.FromResult(result);
        }

        public UserContext GetOrCreateUserContext()
        {
            if (TryLoadingPlayerContext() == false)
            {
                CreateNewPlayerContext();
            }
            return _userContext;
        }

        private bool TryLoadingPlayerContext()
        {
            if (_fileService.TryReadAllText(GamePaths.PlayerContext, out var json) == false)
                return false;

            try
            {
                _userContext = JsonConvert.DeserializeObject<UserContext>(json);
            }
            catch (JsonReaderException ex)
            {
                Debug.LogError("JSON parsing error: " + ex.Message);
                Debug.LogError("Raw JSON: " + json);
                return false;
            }

            return _userContext != null;
        }

        private void SavePlayerContext()
        {
            var json = JsonConvert.SerializeObject(_userContext, Formatting.None);
            _fileService.WriteAllText(GamePaths.PlayerContext, json);
        }

        private void CreateNewPlayerContext()
        {
            _userContext = new UserContext
            {
                Version = ACTUAL_VERSION,
                LocalizationDefId = GetLocalizationDefId()
            };

            var json = JsonConvert.SerializeObject(_userContext, Formatting.None);
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