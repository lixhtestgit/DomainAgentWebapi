using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DomainAgentWebapi.Common
{
    public static class HttpClientSingleston
    {
        public async static Task<string> MyGet(this HttpClient httpClient, string url)
        {
            string result = null;

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (var response = await httpClient.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        using (Stream stream = await response.Content.ReadAsStreamAsync())
                        {
                            using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
                            {
                                result = await streamReader.ReadToEndAsync();
                            }
                        }

                    }
                }
            }
            return result ?? "";
        }

        public async static Task<byte[]> MyGetFile(this HttpClient httpClient, string url)
        {
            byte[] result = null;
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (var response = await httpClient.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadAsByteArrayAsync();
                    }
                }
            }
            return result ?? new byte[0];
        }

    }
}
