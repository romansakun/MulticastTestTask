using System;
using Cysharp.Threading.Tasks;
using GameLogic.Helpers;
using GameLogic.Model.DataProviders;
using GameLogic.Model.Operators;
using GameLogic.Model.Repositories;
using Infrastructure;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class UserContextLoader : IAsyncOperation
    {
        [Inject] private DiContainer _diContainer;
        [Inject] private SignalBus _signalBus;

        public async UniTask ProcessAsync()
        {
            var userContextHelper = _diContainer.Instantiate<UserContextHelper>();
            var userContext = userContextHelper.GetOrCreateUserContext();
            await userContextHelper.TryMigrate(userContext);
            try
            {
                var repository = _diContainer.Instantiate<UserContextRepository>(new object[] { userContext });
                repository.TryUpdateFreeConsumablesCount();
                
                _diContainer.Bind<UserContextDataProvider>().AsSingle().WithArguments(repository);
                _diContainer.Bind<UserContextOperator>().AsSingle().WithArguments(repository);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load player context:\nCONTEXT: {JsonConvert.SerializeObject(userContext)}:\nERROR: {e.Message}");
                throw;
            }
            await UniTask.Yield();
            _signalBus.Fire<UserContextInitializedSignal>();
        }
    }
}