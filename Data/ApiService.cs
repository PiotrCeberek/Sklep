namespace Projekt.Data
{
    using global::Projekt.Models;
    using Newtonsoft.Json;
    using System.Xml.Serialization;

    namespace Projekt.Services
    {
        public class ApiService
        {
            private readonly HttpClient _httpClient;

            public ApiService(HttpClient httpClient)
            {
                _httpClient = httpClient;
            }

            public async Task<WalutyAPI> GetWalutyAsync(string apiUrl)
            {
                var response = await _httpClient.GetStringAsync(apiUrl);

                var json = JsonConvert.DeserializeObject<dynamic>(response);

                var waluty = new WalutyAPI
                {
                    Nazwa = json.currency,
                    Cena = json.rates[0].mid
                };
                    
                return waluty;
            }
        }
    }

}
