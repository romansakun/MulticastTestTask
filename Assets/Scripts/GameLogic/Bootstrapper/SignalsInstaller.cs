using Zenject;

namespace GameLogic.Bootstrapper
{
    public class SignalsInstaller : Installer<SignalsInstaller>
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<UserContextInitializedSignal>();
            Container.DeclareSignal<GameAppLoadedSignal>();
            Container.DeclareSignal<StartShowingGameplayViewSignal>();
            Container.DeclareSignal<StartShowingLeaderboardViewSignal>();
        }
    }

    public struct UserContextInitializedSignal { }
    public struct GameAppLoadedSignal { }
    public struct StartShowingGameplayViewSignal { }
    public struct StartShowingLeaderboardViewSignal { }
}