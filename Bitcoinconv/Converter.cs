using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;

namespace Bitcoinconv
{
    public class Converter
    {
        private HttpClient client;
        public Converter() { }
        public Converter(HttpClient client)
        {
            this.client = client;
        }
        private const string API_URL = "https://api.coindesk.com/v1/bpi/currentprice.json";
        public async Task<decimal> GetExchangeRate(string currency)
        {
            var response = await this.client.GetStringAsync(API_URL);
            var jsonDoc = JsonDocument.Parse(Encoding.ASCII.GetBytes(response));

            var curr_obj = jsonDoc.RootElement.GetProperty("bpi").GetProperty(currency);
            if (curr_obj.TryGetProperty("rate", out JsonElement rate))
                return Decimal.Parse(rate.GetString());
            else
                return 0;
        }
    }
}
