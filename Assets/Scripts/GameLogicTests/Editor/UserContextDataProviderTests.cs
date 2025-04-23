using System.Collections.Generic;
using GameLogic.Model.Contexts;
using GameLogic.Model.DataProviders;
using GameLogic.Model.Definitions;
using GameLogic.Model.Operators;
using GameLogic.Model.Repositories;
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
        _container.Bind<GameDefsDataProvider>().AsSingle();

        var userContext = GetTestUserContext();
        var repository = _container.Instantiate<UserContextRepository>(new []{ userContext });
        _container.Bind<UserContextDataProvider>().AsSingle().WithArguments(repository);
        _container.Bind<UserContextOperator>().AsSingle().WithArguments(repository);
    }

    [Test]
    public void WhenUpdateSoundsMuted()
    {
        var gameDefs = GetTestGameDefs();
        var provider = _container.Resolve<GameDefsDataProvider>();
        provider.SetGameDefs(gameDefs);

        var userContextOperator = _container.Resolve<UserContextOperator>();
        var userContext = _container.Resolve<UserContextDataProvider>();

        Assert.IsFalse(userContext.IsSoundsMuted.Value);

        userContextOperator.SetSoundsMuted(true);

        Assert.IsTrue(userContext.IsSoundsMuted.Value);
    }

    private UserContext GetTestUserContext()
    {
        return new UserContext()
        {
            LocalizationDefId = "ru",
        };
    }

    private GameDefs GetTestGameDefs()
    {
        return new GameDefs()
        {
            Localizations = new Dictionary<string, LocalizationDef>()
            {
                {
                    "ru", new LocalizationDef() 
                    {
                        Id = "ru", Levels = 
                            new Dictionary<int, string>()
                            {
                                {1, "ru_1"}
                            }
                    }
                }
            },
            Levels = new Dictionary<string, LevelDef>()
            {
                {
                    "ru_1", new LevelDef()
                    {
                        Id = "ru_1",
                        Words = new Dictionary<string, List<int>>()
                        {
                            {"QWERTY", new List<int>() {1, 2, 2}}
                        }
                    }
                }
            }
        };
    }
}

