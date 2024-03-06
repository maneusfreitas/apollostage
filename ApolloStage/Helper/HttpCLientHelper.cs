using ApolloStage.Factories.IFactories;


namespace ApolloStage
{
    public class HttpCLientHelper : IHttpClientHelper
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IFactories factories;
        private readonly string tokenType = "Bearer";

        public HttpCLientHelper(IHttpClientFactory httpClientFactory, IFactories factories)
        {
            this.httpClientFactory = httpClientFactory;
            this.factories = factories;
        }

        public async Task<string> PostAsync(string uri, IEnumerable<KeyValuePair<string, string>> content)
        {
            using (var client = httpClientFactory.CreateClient())
            {
                using (var formContent = factories.GetFormUrlEncodedContent(content))
                {
                    formContent.Headers.Clear();
                    formContent.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    var response = client.PostAsync(uri, formContent).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(result))
                        {
                         return result;
                        }
                    }
                    return null;

                }
            }
        }

        public async Task<string> SendAysnc(string uri, string token)
        {
            using (var client = httpClientFactory.CreateClient())
            {
                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Get;
                    request.RequestUri = new Uri(uri);
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(tokenType, token);

                    var response = client.SendAsync(request).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(result))
                        {
                            return result;
                        }
                    }
                    return null;
                }
            }
        }
    }
}