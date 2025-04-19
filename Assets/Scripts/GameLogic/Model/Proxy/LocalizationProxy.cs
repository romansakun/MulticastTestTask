using System;
using GameLogic.Model.Definitions;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace GameLogic.Model.Proxy
{
    public class LocalizationProxy : IDisposable
    {
        [Inject] private GameDefsProxy _gameDefs;
        [Inject] private UserContextProxy _userContext;

        private LocalizationDef _currentLocalization;

        public event Action OnLocalizationChanged;

        public void SetLocalization(SystemLanguage language)
        {
            SetLocalization(language.ToString());
        }

        public void SetLocalization(string language)
        {
            if (_gameDefs.Localizations.TryGetValue(language, out var localization))
            {
                _currentLocalization = localization;
                _userContext.UpdateLocalization(_currentLocalization.Id);

                OnLocalizationChanged?.Invoke();
            }
            throw new Exception($"Localization for {language} not found");
        }

        public string GetText(string key, Object invoker = null)
        {
            if (_currentLocalization.LocalizationText.TryGetValue(key, out var text))
            {
                return text;
            }
            Debug.LogWarning($"{key} not found in {_currentLocalization.Id} localization", invoker);
            return key;
        }

        // public bool TryGetNextLevel(out LevelDef levelDef)
        // {
        //     _userContext.CurrentLocalizationLevelId
        //     var levelDefId = _currentLocalization.Levels[levelIndex];
        //     return _gameDefs.Levels.TryGetValue(levelDefId, out levelDef);
        // }

        public void Dispose()
        {
            OnLocalizationChanged = null;
        }

    }
    
}