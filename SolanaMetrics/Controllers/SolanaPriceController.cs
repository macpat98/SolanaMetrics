using Microsoft.AspNetCore.Mvc;

namespace SolanaMetrics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolanaPriceController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SolanaPriceController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("fetch-history")]
        public async Task<IActionResult> FetchHistory()
        {
            var apiUrl = "https://api.coingecko.com/api/v3/coins/solana/market_chart?vs_currency=usd&days=7&interval=daily";

            try
            {
                // download data
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(apiUrl);

                if(!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new
                    {
                        Message = "Error during downloading data from CoinGecko API.",
                        Details = await response.Content.ReadAsStringAsync()
                    });
                }

                // read data from response
                var data = await response.Content.ReadAsStringAsync();

                // save data to JSON file
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "solana_history.json");
                await System.IO.File.WriteAllTextAsync(filePath, data);

                return Ok(new
                {
                    Message = "Historical Solana price data was fetched and saved to file.",
                    FilePath = filePath
                });
            }
            catch(HttpRequestException ex)
            {
                return StatusCode(500, new
                {
                    Message = "Communication error with CoinGecko API",
                    Details = ex.Message
                });
            }
        }
    }
}
