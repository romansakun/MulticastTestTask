using Factories;
using GameLogic.UI;
using Infrastructure.GameActions;
using Infrastructure.Services;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class MainInstaller : MonoInstaller  
    {
        public override void InstallBindings()
        {
            Container.Bind<IFileService>().To<FileService>().AsSingle();
            Container.Bind<GameAppReloader>().AsSingle();

            Container.Bind<LogicBuilderFactory>().AsSingle();
            Container.Bind<GameActionFactory>().AsSingle();
            Container.Bind<ViewModelFactory>().AsSingle();

            Container.BindInterfacesAndSelfTo<AssetLoader>().AsSingle();
            Container.BindInterfacesAndSelfTo<ViewManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameActionExecutor>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameAppLoader>().AsSingle();
        }
    }
}