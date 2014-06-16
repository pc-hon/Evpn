using System.Net;

namespace Evpn.Services
{
    public class RestResult<T>
    {
        public T Value { get; set;}

        public HttpStatusCode HttpStatusCode { get; set; }

        public WebExceptionStatus WebExceptionCode { get; set; }

        public bool IsSuccess { get { return HttpStatusCode == HttpStatusCode.OK && WebExceptionCode == WebExceptionStatus.Success; } }

        public bool IsTimeout { get { return HttpStatusCode == HttpStatusCode.RequestTimeout || WebExceptionCode == WebExceptionStatus.Timeout; } }

        public bool IsForbidden { get { return HttpStatusCode == HttpStatusCode.Forbidden; } }
    }
}
