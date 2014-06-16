using Evpn.Models;

namespace Evpn.Services
{
    public interface IRestClient
    {
        RestResult<ExpressVpnResponse> GetLocations(string url);
    }
}
