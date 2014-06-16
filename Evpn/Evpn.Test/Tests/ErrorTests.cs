using Evpn.Test.Utils;
using Evpn.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using Telerik.JustMock;

namespace Evpn.Test.Tests
{
    [TestClass]
    public class ErrorTests : BaseTest
    {
        [TestMethod]
        public void TestRestTimeout()
        {
            TestRestError(MockedWebRequestCreate.CreateWithTimeoutException(), 
                ApplicationViewModel.ERROR_TEXT_TIMED_OUT);
        }

        [TestMethod]
        public void TestRest403Error()
        {
            TestRestError(MockedWebRequestCreate.CreateWithResponse(HttpStatusCode.Forbidden, ""),
                ApplicationViewModel.ERROR_TEXT_403);
        }

        [TestMethod]
        public void TestRestRandomException()
        {
            TestRestError(MockedWebRequestCreate.CreateWithException(new Exception()), 
                ApplicationViewModel.ERROR_TEXT_UNKNOWN);
        }

        private void TestRestError(MockedWebRequestCreate mockedRequest, string errorMessage)
        {
            // Setup
            var model = CreateMockedModelWithMockedRequest(mockedRequest);

            // Ensure Calculate locaiton is never called
            Mock.Arrange(() => model.BeginCalculateBestLocationAsync())
                .DoNothing()
                .OccursNever();

            // Set a wait timer when the correct error message is called
            SetupWaitForComplete(() => model.ShowMessageBox(Arg.Is(errorMessage))).OccursOnce();

            // Act
            model.RefreshLocations();
            WaitForComplete(3000);

            // Assert
            Assert.AreEqual(0, model.Locations.Count);
            Mock.Assert(model);

        }
    }
}
