using geo_server.Models;

namespace geo_server.Services
{
    public interface IGeoService
    {
        Task<List<Address>> GetAddressesByCoordinates(double latitude, double longitude);
        Task<Tuple<double, double>> GetGeoDataByAddress(Address address);

    }
}
