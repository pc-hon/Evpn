using Evpn.Models;
using Evpn.Services;
using Evpn.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Telerik.JustMock;

namespace Evpn.Test.Tests
{
    [TestClass]
    public class BestLocationTests : BaseTest
    {
        [TestMethod]
        public void TestBestLocation()
        {
            // -- Setup -- 
            var model = CreateMockedModel();
            var restClient = CreateMockedIRestClientFor(model);
            Mock.Arrange(() => model.BeginCalculateBestLocationAsync()).CallOriginal();

            // Setup Mocked Response
            var response = CreateMockedResponseFor(restClient);
            var locA = CreateLocationFor(response.Value, "A");
            var locB = CreateLocationFor(response.Value, "B");

            // Setup Mocked IP with fake pings
            CreateMockedIpFor(locA, 200);
            CreateMockedIpFor(locA, 500);
            CreateMockedIpFor(locB, 1000);
            CreateMockedIpFor(locB, 10, 3000); // introduce a wait time of 3 sec for this ping

            // Setup Mocked BestLocation ViewModel
            var bestLoc = CreateMockedBestLocationFor(model);
            // Set wait timer to wait for SetCompleted to be called
            SetupWaitForComplete(() => bestLoc.SetCompleted())
                .OccursOnce();


            // -- Act -- 
            model.RefreshLocations();
            WaitForComplete(5000); // Wait for the SetCompleted to be called.
            

            // -- Assert --
            Mock.Assert(bestLoc);
            Assert.AreEqual(locB.Name, bestLoc.Name); // locB takes longer to ping but has a faster ping result
        }

        private IRestClient CreateMockedIRestClientFor(ApplicationViewModel model)
        {
            var restClient = Mock.Create<IRestClient>(Constructor.NotMocked, Behavior.CallOriginal);
            Mock.Arrange(() => model.RestClient).Returns(restClient);

            return restClient;
        }

        private RestResult<ExpressVpnResponse> CreateMockedResponseFor(IRestClient restClient)
        {
            var response = Mock.Create<ExpressVpnResponse>(Constructor.NotMocked, Behavior.CallOriginal);
            response.Icons = new List<Icon>();
            response.Locations = new List<Location>();
            response.RefreshText = "test";

            var result = new RestResult<ExpressVpnResponse>();
            result.Value = response;
            result.HttpStatusCode = HttpStatusCode.OK;

            Mock.Arrange(() => restClient.GetLocations(Arg.AnyString)).Returns(result);

            return result;
        }

        private Location CreateLocationFor(ExpressVpnResponse response, string locName)
        {
            var loc = new Location() { Name = locName, ServerIps = new List<Ip>() };
            response.Locations.Add(loc);
            return loc;
        }

        private BestLocationViewModel CreateMockedBestLocationFor(ApplicationViewModel model)
        {
            model.BestLocation = Mock.Create<BestLocationViewModel>(Constructor.NotMocked, Behavior.CallOriginal);
            return model.BestLocation;
        }

        private void CreateMockedIpFor(Location loc, int pingReturnTime, int pingExecutionTime = 0)
        {
            var ip = Mock.Create<Ip>(Constructor.NotMocked, Behavior.CallOriginal);

            var mockedArrange = Mock.Arrange(() => ip.Ping());
            if (pingExecutionTime > 0)
                mockedArrange.DoInstead(() => Task.Delay(pingExecutionTime).Wait()).Returns(pingReturnTime);
            else
                mockedArrange.Returns(pingReturnTime);

            loc.ServerIps.Add(ip);
        }

    }
}
