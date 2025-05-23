using Zenject;

namespace GameLogic.Bootstrapper
{
    public class SignalsInstaller : Installer<SignalsInstaller>
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<UserContextInitializedSignal>();
        }
    }

    public struct UserContextInitializedSignal
    {

    }
}