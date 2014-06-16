using Evpn.Models;
using Evpn.Models.Serializer;
using System.Net;

namespace Evpn.Services
{
    public class RestClient : IRestClient
    {
        public const int DEFAULT_TIMEOUT = 10000; // 10 seconds

        public RestResult<ExpressVpnResponse> GetLocations(string url)
        {
            var result = new RestResult<ExpressVpnResponse>();
            
            var httpResponse = GetResponse(url, result);
            
            if (httpResponse != null)
            {
                using (var stream = httpResponse.GetResponseStream())
                {
                    if (stream != null)
                    {
                        var serializer = new CustomSerializer<ExpressVpnResponse>();
                        try
                        {
                            result.Value = serializer.Deserialize(stream);
                        }
                        catch { }
                    }
                }
            }

            return result;
        }

        private HttpWebResponse GetResponse<T>(string url, RestResult<T> result)
        {
            var req = (HttpWebRequest) WebRequest.Create(url);
            req.Timeout = DEFAULT_TIMEOUT;

            try
            {
                var response = (HttpWebResponse)req.GetResponse();
                if (response != null)
                {
                    result.HttpStatusCode = response.StatusCode;
                    return response;
                }
            }
            catch (WebException ex)
            {
                result.WebExceptionCode = ex.Status;
            }
            catch { }

            return null;
        }
    }
}
