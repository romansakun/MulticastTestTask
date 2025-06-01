using GameLogic.Factories;
using GameLogic.UI;
using GameLogic.UI.Gameplay;
using GameLogic.UI.Leaderboards;
using Infrastructure.GameActions;
using Infrastructure.Pools;
using Infrastructure.Services;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class MainInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind(typeof(ITimerService), typeof(ITickable)).To<TimerService>().AsSingle();
            Container.Bind<IAssetsLoader>().To<AddressableAssetsLoader>().AsSingle();

            Container.Bind<GameAppReloader>().AsSingle();

            Container.Bind<LogicBuilderFactory>().AsSingle();
            Container.Bind<GameActionFactory>().AsSingle();
            Container.Bind<ViewModelFactory>().AsSingle();

            Container.BindInterfacesAndSelfTo<DeferredMonoPool<Cluster>>().AsSingle();
            Container.BindInterfacesAndSelfTo<DeferredMonoPool<WordRow>>().AsSingle();
            Container.BindInterfacesAndSelfTo<DeferredMonoPool<PlayerLine>>().AsSingle();
            Container.BindFactory<Cluster, Cluster.Factory>().AsSingle();
            Container.BindFactory<WordRow, WordRow.Factory>().AsSingle();
            Container.BindFactory<PlayerLine, PlayerLine.Factory>().AsSingle();

            //Container.BindInterfacesAndSelfTo<TutorialService>().AsSingle();
            Container.BindInterfacesAndSelfTo<ViewManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameActionExecutor>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameAppLoader>().AsSingle();
        }
    }
}