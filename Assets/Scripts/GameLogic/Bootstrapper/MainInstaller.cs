using Factories;
using GameLogic.UI;
using Infrastructure;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class MainInstaller : MonoInstaller  
    {
        public override void InstallBindings()
        {
            Container.Bind<AssetLoader>().AsSingle();
            Container.Bind<GameAppReloader>().AsSingle();
            Container.Bind<ViewModelFactory>().AsSingle();
            Container.Bind<LogicBuilderFactory>().AsSingle();

            Container.BindInterfacesAndSelfTo<ViewManager>().AsSingle();
        }
    }
}