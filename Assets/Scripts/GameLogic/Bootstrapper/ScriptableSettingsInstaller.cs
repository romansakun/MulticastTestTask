using System;
using GameLogic.Model.DataProviders;
using GameLogic.Model.Definitions;
using GameLogic.UI;
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
        [SerializeField] private GameplaySettings _gameplaySettings;

        public override void InstallBindings()
        {
            var localGameDefs = Newtonsoft.Json.JsonConvert.DeserializeObject<GameDefs>(_localGameDefsTextAsset.text);
            var gameDefsProxy = new GameDefsDataProvider().SetGameDefs(localGameDefs);
            Container.Bind<GameDefsDataProvider>().FromInstance(gameDefsProxy).AsSingle();

            Container.Bind<SoundsSettings>().FromInstance(_soundsSettings).AsSingle();
            Container.Bind<ColorsSettings>().FromInstance(_colorsSettings).AsSingle();
            Container.Bind<GameplaySettings>().FromInstance(_gameplaySettings).AsSingle();

            SignalsInstaller.Install(Container);
        }
    }

    [Serializable]
    public class SoundsSettings
    {
        public AudioClip DropClusterSound;
        public AudioClip TapSound;
        public AudioClip WrongAnswerSound;
        public AudioClip SuccessSound;
        public AudioClip BackgroundMusic;
    }

    [Serializable]
    public class ColorsSettings
    {
        public float GhostClusterAlpha = 0.5f;
        public ElementColorDictionary ElementColors = new ElementColorDictionary();
    }

    [Serializable]
    public class GameplaySettings
    {
        public Vector3 DraggedClusterRotation = new Vector3(0, 0, 10);
        public Vector2 DraggedClusterOffsetPosition = new Vector2(0, 15);
    }

    [Serializable]
    public class ElementColorDictionary : SerializableDictionary<ElementColor, Color> { }
}