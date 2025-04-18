using Factories;
using GameLogic.Model.Repositories;
using GameLogic.UI;
using Infrastructure;
using Infrastructure.GameActions;
using Infrastructure.Services;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class MainInstaller : MonoInstaller  
    {
        public override void InstallBindings()
        {
            Container.Bind<IFileService>().FromInstance(new FileService()).AsSingle();
            Container.Bind<GameAppReloader>().AsSingle();
            Container.Bind<GameActionExecutor>().AsSingle();
            Container.Bind<AssetLoader>().AsSingle();

            Container.Bind<LogicBuilderFactory>().AsSingle();
            Container.Bind<ViewModelFactory>().AsSingle();
            Container.Bind<GameActionFactory>().AsSingle();

            Container.BindInterfacesAndSelfTo<ViewManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<UserContextRepository>().AsSingle();
        }
    }
}