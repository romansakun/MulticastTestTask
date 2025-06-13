using System;
using GameLogic.Model.DataProviders;
using GameLogic.Model.Definitions;
using GameLogic.UI;
using MessagePack;
using UnityEngine;
using Zenject;

namespace GameLogic.Bootstrapper
{
    [CreateAssetMenu(fileName = "ScriptableSettingsInstaller", menuName = "Installers/ScriptableSettingsInstaller")]
    public class ScriptableSettingsInstaller : ScriptableObjectInstaller 
    {
        [SerializeField] private TextAsset _localGameDefsRawTextAsset;
        [SerializeField] private ColorsSettings _colorsSettings;
        [SerializeField] private GameplaySettings _gameplaySettings;

        public override void InstallBindings()
        {
            //var localGameDefs = Newtonsoft.Json.JsonConvert.DeserializeObject<GameDefs>(_localGameDefsTextAsset.text);
            var localGameDefs = MessagePackSerializer.Deserialize<GameDefs>(_localGameDefsRawTextAsset.bytes);
            var gameDefsProxy = new GameDefsDataProvider().SetGameDefs(localGameDefs);
            Container.Bind<GameDefsDataProvider>().FromInstance(gameDefsProxy).AsSingle();

            Container.Bind<ColorsSettings>().FromInstance(_colorsSettings).AsSingle();
            Container.Bind<GameplaySettings>().FromInstance(_gameplaySettings).AsSingle();

            SignalsInstaller.Install(Container);
        }
    }

    [Serializable]
    public class ColorsSettings
    {
        public float GhostClusterAlpha = 0.5f;
        public ElementColorDictionary ElementColors = new ElementColorDictionary();
        public LeaderboardRankColorDictionary LeaderboardRankColors = new LeaderboardRankColorDictionary();
    }

    [Serializable]
    public class GameplaySettings
    {
        public Vector3 DraggedClusterRotation = new Vector3(0, 0, 10);
        public Vector2 DraggedClusterOffsetPosition = new Vector2(0, 15);

        public Vector2 ClusterOffsetPosition()
        {
            var aspectRatio = (Screen.height * 1f) / Screen.width;
            return new Vector2(DraggedClusterOffsetPosition.x * aspectRatio, DraggedClusterOffsetPosition.y * aspectRatio);
        }
    }

    [Serializable] public class ElementColorDictionary : SerializableDictionary<ElementColor, Color> { }
    [Serializable] public class LeaderboardRankColorDictionary : SerializableDictionary<int, Color> { }
}