using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AppCenter
{
    public class ApiClient
    {
        public static async Task<T> GetDataAsync<T>(HttpClient client, string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);

            string jsonString = await response.Content.ReadAsStringAsync();

            T result = JsonConvert.DeserializeObject<T>(jsonString);

            return result;
        }

        public static async Task<List<T>> GetDataListAsync<T>(HttpClient client, string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);

            List<T> result = new List<T>();

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();

                result = JsonConvert.DeserializeObject<List<T>>(jsonString);
            }

            return result;
        }

        public static async Task<T> PostDataAsync<T>(HttpClient client, string url, string sourceVersion)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);

            FormUrlEncodedContent bodyParameters = PrepareBodyParameters(sourceVersion);

            request.Content = new StringContent(JsonConvert.SerializeObject(bodyParameters), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            Stream responseStream = await response.Content.ReadAsStreamAsync();
            using StreamReader streamReader = new StreamReader(responseStream);
            using JsonTextReader jsonTextReader = new JsonTextReader(streamReader);
            JsonSerializer serializer = new JsonSerializer();

            T result = serializer.Deserialize<T>(jsonTextReader);

            return result;
        }

        private static FormUrlEncodedContent PrepareBodyParameters(string sourceVersion)
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("sourceVersion", sourceVersion),
                new KeyValuePair<string, string>("debug", "true"),
            };

            FormUrlEncodedContent request = new FormUrlEncodedContent(parameters);

            return request;
        }
    }
}
