using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class CryptoService
{
    private readonly HttpClient _httpClient;


    public class Cryptocurrency
    {
        public string Name { get; set; }
        public double Price { get; set; }
    }


    public CryptoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Cryptocurrency>> GetCryptocurrenciesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Cryptocurrency>>("http://localhost:8080/api/cryptocurrencies");
    }
}