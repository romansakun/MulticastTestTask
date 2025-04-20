using GameLogic.Model.DataProviders;
using Infrastructure.Services;
using NUnit.Framework;
using Zenject;

[TestFixture]
public class UserContextDataProviderTests
{
    private DiContainer _container;

    [SetUp]
    public void Setup()
    {
        _container = new DiContainer();

        _container.Bind<IFileService>().FromSubstitute();
        _container.BindInterfacesAndSelfTo<UserContextDataProvider>().AsSingle();
    }

    [Test]
    public void WhenUpdateLocalization()
    {
        // var userContextRepository = _container.Resolve<UserContextDataProvider>();
        // userContextRepository.Initialize();
        //
        // userContextRepository.UpdateLocalization("RU");
        // userContextRepository.StartLevel("RU", 1);
        //
        //
        // Assert.AreEqual(("RU", 1), userContextRepository.CurrentLocalizationLevelId.Value);
    }

}

