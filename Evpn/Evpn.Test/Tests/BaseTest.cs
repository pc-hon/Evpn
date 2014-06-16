using Evpn.Test.Utils;
using Evpn.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Telerik.JustMock;
using Telerik.JustMock.Expectations;

namespace Evpn.Test.Tests
{
    [TestClass]
    public class BaseTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            m_testUrl = GetNewUrl();
        }

        private string m_testUrl;

        protected string GetNewUrl()
        {
            return @"https://xvjune2014trial.apiary.io/test" + Guid.NewGuid().ToString();
        }

        protected ApplicationViewModel CreateMockedModel()
        {
            var model = Mock.Create<ApplicationViewModel>(Constructor.NotMocked, Behavior.CallOriginal);
            model.ServerUrl = m_testUrl;
            return model;
        }

        protected ApplicationViewModel CreateMockedModelWithMockedRequest(MockedWebRequestCreate mockedRequest)
        {
            // Register mocked request
            WebRequest.RegisterPrefix(m_testUrl, mockedRequest);

            // Create mocked model
            var model = CreateMockedModel();

            return model;
        }

        #region Wait timer to wait for test to complete

        private object m_lockObject = new object();
        private bool m_isCompleted = false;

        protected ActionExpectation SetupWaitForComplete(Expression<Action> testCompletedSignal)
        {
            return Mock.Arrange(testCompletedSignal).DoInstead(() => { lock (m_lockObject) { m_isCompleted = true; } });
        }

        protected void WaitForComplete(int waitTimeMs)
        {
            if (m_isCompleted)
                return;

            for (; waitTimeMs > 0; waitTimeMs -= 1000)
            {
                Task.Delay(1000).Wait();

                if (m_isCompleted)
                    return;
            }
            throw new InternalTestFailureException("Test failed to complete.");
        }

        protected void ResetWait()
        {
            lock (m_lockObject) { m_isCompleted = false; }
        }
        #endregion
    }
}
