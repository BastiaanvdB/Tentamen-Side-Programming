using Newtonsoft.Json;
using Tentamen_Server_Side_Programming.Models.Weather;
using Tentamen_Server_Side_Programming.Services.Interface;

namespace Tentamen_Server_Side_Programming.Services
{
    public class WeatherForecastService : IWeatherForecastService
    {
        private static readonly HttpClient _httpClient;
        private readonly string _WeatherApiUrl = "https://data.buienradar.nl/2.0/feed/json";

        static WeatherForecastService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<List<StationMeasurement>> GetForecast()
        {
            try
            {
                using var responseMessage = await _httpClient.GetAsync(_WeatherApiUrl);

                if (responseMessage.IsSuccessStatusCode)
                {
                    string jsonString = await responseMessage.Content.ReadAsStringAsync();
                    List<StationMeasurement> weatherForecast = DeserializeForecast(jsonString);
                    return weatherForecast;
                }
                else
                {
                    throw new Exception("Fetching Weather Data Failed!");
                }

            }
            catch (HttpRequestException ex)
            {
                throw ex;
            }

        }

        private List<StationMeasurement> DeserializeForecast(string jsonString) 
        {
            var jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonString);
            var forecast = jsonObject?.actual?.stationmeasurements;

            if (forecast == null)
            {
                throw new Exception("Json Empty or stationmeasurements not found");
            }

            return JsonConvert.DeserializeObject<List<StationMeasurement>>(forecast.ToString());
        }
    }
}
