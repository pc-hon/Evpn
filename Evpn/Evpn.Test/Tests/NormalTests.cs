using Evpn.Test.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using Telerik.JustMock;

namespace Evpn.Test.Tests
{
    [TestClass]
    public class NormalTests : BaseTest
    {
        [TestMethod]
        public void TestNormal()
        {
            // Setup
            var model = CreateMockedModelWithMockedRequest(MockedWebRequestCreate.CreateWithResponse(HttpStatusCode.OK, TestDataReader.ReadLocations()));

            // Finish the test when BeginCalculateBestLocationAsync is called
            SetupWaitForComplete(() => model.BeginCalculateBestLocationAsync());

            // Act
            model.RefreshLocations();
            WaitForComplete(3000);

            // Asserts
            Assert.AreEqual(24, model.Locations.Count);
            Assert.AreEqual("Los Angeles", model.Locations[0].Name);
            Assert.AreEqual(80, model.Locations[0].SortOrder);

            Assert.AreEqual("Refresh Test Button", model.RefreshText);

            Mock.Assert(model);
        }

        [TestMethod]
        public void TestChangeUrl()
        {
            // Setup
            var model = CreateMockedModelWithMockedRequest(MockedWebRequestCreate.CreateWithResponse(HttpStatusCode.OK, TestDataReader.ReadLocations()));

            var newUrl = GetNewUrl();
            WebRequest.RegisterPrefix(newUrl, MockedWebRequestCreate.CreateWithResponse(HttpStatusCode.OK, TestDataReader.ReadLocations2()));

            // Set a wait timer when BeginCalculateBestLocationAsync is called
            SetupWaitForComplete(() => model.BeginCalculateBestLocationAsync()).Occurs(2);

            // Act
            model.RefreshLocations(); // Refresh with first Url
            WaitForComplete(3000);
            ResetWait();

            // Asserts
            Assert.AreEqual(24, model.Locations.Count);
            Assert.AreEqual("Los Angeles", model.Locations[0].Name);

            // Act 2
            model.ServerUrl = newUrl;
            model.RefreshLocations(); // Refresh with new Url
            WaitForComplete(3000);

            // Asserts
            Assert.AreEqual(2, model.Locations.Count);
            Assert.AreEqual("LA 2 - Best for North China", model.Locations[0].Name);
            Assert.AreEqual(90, model.Locations[0].SortOrder);

            Assert.AreEqual("Refresh Test Button", model.RefreshText);

            Mock.Assert(model);
        }
    }
}
