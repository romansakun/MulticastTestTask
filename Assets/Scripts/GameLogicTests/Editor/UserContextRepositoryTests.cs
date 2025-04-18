using GameLogic.Model.Repositories;
using Infrastructure.Services;
using NUnit.Framework;
using Zenject;

[TestFixture]
public class UserContextRepositoryTests
{
    private DiContainer _container;

    [SetUp]
    public void Setup()
    {
        _container = new DiContainer();

        _container.Bind<IFileService>().FromSubstitute();
        _container.BindInterfacesAndSelfTo<UserContextRepository>().AsSingle();
    }

    [Test]
    public void WhenUpdateLocalization()
    {
        var userContextRepository = _container.Resolve<UserContextRepository>();
        userContextRepository.Initialize();

        userContextRepository.UpdateLocalization("RU");
        userContextRepository.UpdateLocalizationLevelId("RU", 1);

        Assert.AreEqual("RU", userContextRepository.CurrentLocalizationDefId.Value);
        Assert.AreEqual(("RU", 1), userContextRepository.CurrentLocalizationLevelId.Value);
    }

}

