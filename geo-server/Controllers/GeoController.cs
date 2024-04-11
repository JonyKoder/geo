using geo_server.Models;
using geo_server.Services;
using Microsoft.AspNetCore.Mvc;

namespace geo_server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GeoController : ControllerBase
    {
        private readonly IGeoService _geoService;
        private readonly ILogger<GeoController> _logger;

        public GeoController(ILogger<GeoController> logger, IGeoService geoService)
        {
            _logger = logger;
            _geoService = geoService;
        }

        [HttpPost("GetGeoDataByAddress")]
        public async Task<ActionResult<Tuple<double, double>>> GetGeoDataByAddress(Address address)
        {
            try
            {
                var geoData = await _geoService.GetGeoDataByAddress(address);
                return Ok(geoData);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return StatusCode(500, $"Failed to retrieve geodata for address: {address}");
            }
        }

        [HttpPost("GetNearestAddressesByGeoPosition")]
        public async Task<ActionResult<List<Address>>> GetNearestAddressesByGeoPosition(double latitude, double longitude)
        {
            try
            {
                var nearestAddresses = await _geoService.GetAddressesByCoordinates(latitude, longitude);
                return Ok(nearestAddresses);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return StatusCode(500, $"Failed to retrieve nearest addresses for latitude: {latitude} and longitude: {longitude}");
            }
        }
    }
}
