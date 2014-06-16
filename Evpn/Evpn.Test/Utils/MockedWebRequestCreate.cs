using System;
using System.IO;
using System.Net;
using Telerik.JustMock;

namespace Evpn.Test.Utils
{

    public class MockedWebRequestCreate : IWebRequestCreate
    {
        #region Factory methods

        public static MockedWebRequestCreate CreateWithResponse(HttpStatusCode httpStatusCode, string content)
        {
            // Setup Response
            var response = Mock.Create<HttpWebResponse>();
            Mock.Arrange(() => response.StatusCode).Returns(httpStatusCode);
            Mock.Arrange(() => response.GetResponseStream()).Returns(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content)));

            // Setup Request with defined response
            var request = Mock.Create<HttpWebRequest>();
            Mock.Arrange(() => request.GetResponse())
                .Returns(response);

            return new MockedWebRequestCreate() { m_request = request };
        }

        public static MockedWebRequestCreate CreateWithTimeoutException()
        {
            return CreateWithException(new WebException("The operation has timed out", WebExceptionStatus.Timeout));
        }

        public static MockedWebRequestCreate CreateWithException(Exception ex)
        {
            // Setup Request with defined exception
            var request = Mock.Create<HttpWebRequest>();
            Mock.Arrange(() => request.GetResponse())
                .Throws(ex);

            return new MockedWebRequestCreate() { m_request = request };
        }

        #endregion


        #region IWebRequestCreate methods

        private HttpWebRequest m_request;

        public WebRequest Create(Uri uri)
        {
            return m_request;
        }

        #endregion
    }
}
