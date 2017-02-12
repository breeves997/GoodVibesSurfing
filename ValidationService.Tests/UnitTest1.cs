using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using ServiceFabric.Mocks;
using Microsoft.ServiceFabric.Data.Collections;

namespace ValidationService.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestServiceState_Dictionary()
        {
            var context = MockStatefulServiceContextFactory.Default;
            var stateManager = new MockReliableStateManager();
            var service = new ValidationService(context, stateManager);

            //const string stateName = "test";
            //var payload = new Payload(StatePayload);

            ////create state
            //await service.InsertAsync(stateName, payload);

            ////get state
            //var dictionary = await stateManager.TryGetAsync<IReliableDictionary<string, Payload>>(MyStatefulService.StateManagerDictionaryKey);
            //var actual = (await dictionary.Value.TryGetValueAsync(null, stateName)).Value;
            //Assert.AreEqual(StatePayload, actual.Content);
        }
    }
}
