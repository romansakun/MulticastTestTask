using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameLogic.Model.Contexts;
using GameLogic.Model.DataProviders;
using Infrastructure.LogicUtility;

namespace GameLogic.Helpers
{
    public class UserContextMigrationHelper
    {
        public const int ACTUAL_VERSION = 1;

        LogicAgent<UserContextMigrationContext> _agent;
        
        public UniTask Migrate(UserContext userContext)
        {
            // var logicBuilder = new LogicBuilder<UserContextMigrationContext>();
            // _agent = logicBuilder.Build();
            //
            if (userContext.Version == 0)
            {
                // userContext.LevelsProgress = new Dictionary<string, LevelProgressContext>();
                // userContext.Consumables = new ConsumablesContext();
                userContext.Version = ACTUAL_VERSION;
            }
            return UniTask.CompletedTask;
        }

    }

    internal class UserContextMigrationContext : IContext
    {
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}