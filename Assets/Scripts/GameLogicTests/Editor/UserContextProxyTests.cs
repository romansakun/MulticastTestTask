using GameLogic.Model.Proxy;
using Infrastructure.Services;
using NUnit.Framework;
using Zenject;

[TestFixture]
public class UserContextProxyTests
{
    private DiContainer _container;

    [SetUp]
    public void Setup()
    {
        _container = new DiContainer();

        _container.Bind<IFileService>().FromSubstitute();
        _container.BindInterfacesAndSelfTo<UserContextProxy>().AsSingle();
    }

    [Test]
    public void WhenUpdateLocalization()
    {
        var userContextRepository = _container.Resolve<UserContextProxy>();
        userContextRepository.Initialize();

        userContextRepository.UpdateLocalization("RU");
        userContextRepository.UpdateLocalizationLevelId("RU", 1);

        Assert.AreEqual("RU", userContextRepository.CurrentLocalizationDefId.Value);
        Assert.AreEqual(("RU", 1), userContextRepository.CurrentLocalizationLevelId.Value);
    }

}

