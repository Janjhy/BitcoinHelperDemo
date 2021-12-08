using System.Text.Json;

namespace BitcoinHelperDemo
{
    // For bitcoin, the earliest datapoint returned by Crypto Api is 2013-4-28.
    // If both dates for a range are before this, no datapoints are returned.
    // Same if to date is before the for date.
    // If only from date is before the earliest datapoint, it will return data starting at 2013-4-28.
    public class CryptoApiService
    {
        private  HttpClient client = new HttpClient();

        public async Task<CryptoApiDataClass> GetCryptoRangeData(string path)
        {
            HttpResponseMessage res = await client.GetAsync(path);
            CryptoApiDataClass data = new CryptoApiDataClass();

            if(res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                data = JsonSerializer.Deserialize<CryptoApiDataClass>(content) ?? throw new ArgumentException();
            }
            return data;
        }
    }
}
