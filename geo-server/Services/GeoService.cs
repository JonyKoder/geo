using Dadata;
using Newtonsoft.Json;
using Address = geo_server.Models.Address;

namespace geo_server.Services
{
    public class GeoService : IGeoService
    {
        private readonly HttpClient _httpClient;
        private const string OpenStreetMapBaseUrl = "https://nominatim.openstreetmap.org";
        private const string DadataBaseUrl = "https://cleaner.dadata.ru";

        public GeoService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<Tuple<double, double>> GetGeoDataByAddress(Address address)
        {
            var query = $"{OpenStreetMapBaseUrl}/search?country={address.Region}&city={address.City}&street={address.Street}&format=json&limit=2";
            var request = new HttpRequestMessage(HttpMethod.Get, query);
            request.Headers.Add("User-Agent", ".NET Framework Test Client");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Не удалось получить геоданные для адреса: {address}");
            }

            var json = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(json);

            if (data != null && data.Count > 0)
            {
                double latitude = data[0].lat;
                double longitude = data[0].lon;

                return new Tuple<double, double>(latitude, longitude);
            }
            else
            {
                throw new Exception($"Данные геоданных не найдены для адреса: {address}");
            }
        }


        public async Task<List<Address>> GetAddressesByCoordinates(double latitude, double longitude)
        {
            var token = "23257f23f093082910b14e88f6c98764a7707cbf";
            var api = new SuggestClientAsync(token);

            try
            {
                var result = await api.Geolocate(lat: latitude, lon: longitude);

                var addresses = result.suggestions.Select(item => new Address
                {
                    Region = item.data.region_with_type,
                    City = item.data.city_with_type,
                    Street = item.data.street_with_type,
                    HouseNumber = item.data.house,
                    ApartmentNumber = item.data.flat
                }).Take(10).ToList(); // Устанавливаем лимит в 10 адресов

                return addresses;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении адресов по координатам: {ex.Message}");
            }
        }

    }
    public class OpenStreetMapAddress
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }

    public class DadataAddress
    {
        public string Region { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string ApartmentNumber { get; set; }
    }
}
