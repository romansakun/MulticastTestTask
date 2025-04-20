using System;
using GameLogic.Model.DataProviders;
using GameLogic.Model.Definitions;
using UnityEngine;
using Zenject;

namespace GameLogic.Bootstrapper
{
    [CreateAssetMenu(fileName = "ScriptableSettingsInstaller", menuName = "Installers/ScriptableSettingsInstaller")]
    public class ScriptableSettingsInstaller : ScriptableObjectInstaller 
    {
        [SerializeField] private TextAsset _localGameDefsTextAsset;
        [SerializeField] private SoundsSettings _soundsSettings;
        [SerializeField] private ColorsSettings _colorsSettings;

        public override void InstallBindings()
        {
            var localGameDefs = Newtonsoft.Json.JsonConvert.DeserializeObject<GameDefs>(_localGameDefsTextAsset.text);
            var gameDefsProxy = new GameDefsDataProvider().SetGameDefs(localGameDefs);
            Container.Bind<GameDefsDataProvider>().FromInstance(gameDefsProxy).AsSingle();
            Container.Bind<SoundsSettings>().FromInstance(_soundsSettings).AsSingle();
            Container.Bind<ColorsSettings>().FromInstance(_colorsSettings).AsSingle();
        }
    }

    [Serializable]
    public class SoundsSettings
    {
        public AudioClip DropClusterSound;
        public AudioClip SelectClusterSound;
        public AudioClip WrongAnswerSound;
        public AudioClip SuccessSound;
        public AudioClip BackgroundMusic;
    }

    [Serializable]
    public class ColorsSettings
    {
        public Color DefaultClusterTextColor;
        public Color DefaultClusterBackColor;

        public Color SelectedClusterTextColor;
        public Color SelectedClusterBackColor;

        public Color GhostClusterTextColor;
        public Color GhostClusterBackColor;
    }
}