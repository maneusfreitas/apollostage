using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using ApolloStage.DTO;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

using ApolloStage.Factories.IFactories;

namespace ApolloStage.Services
{
    public class SpotifyService : IMusicService
    {
        private readonly IConfiguration configuration;
        private readonly IHttpClientHelper httpClientHelper;
        private readonly IFactories factories;
        private readonly ISingleton singleton;
        private readonly string BASEURL = "https://api.spotify.com/v1/artists/";
        private readonly string tokenURL = "https://accounts.spotify.com/api/token";
        private  String token;
        public SpotifyService()
        {
        }

        public SpotifyService(IConfiguration configuration, IHttpClientHelper httpClientHelper, IFactories factories, ISingleton singleton )
        {
            this.configuration = configuration;
            this.httpClientHelper = httpClientHelper;
            this.factories = factories;
            this.singleton = singleton;
        }
        private static string _accessToken; 

        public void GettToken(String toke)
        {
            _accessToken = toke;
        }
        public static string AccessToken
        {
            get { return _accessToken; }
        }
        public string getTheToken()
        {
            
            return token;
        }
        public async Task<Artist> GetArtist(string id)
        {
            await GetToken();
            var result = await httpClientHelper.SendAysnc($"{BASEURL}{id}", singleton.GetToken()); ;
            if (!string.IsNullOrEmpty(result))
            {
                var artist = JsonConvert.DeserializeObject<Artist>(result);
                if (artist != null)
                {
                    var albums = await GetArtistAlbums(id);
                    
                    if (albums is null)
                    {
                        return null;
                    }
                    artist.Albums = albums;
                    return artist;
                }
            }
            return null;

        }

        public async Task<Albums> GetArtistAlbums(string id)
        {
            var result = await httpClientHelper.SendAysnc(BuilAlbumsURI(id), singleton.GetToken()); ;
            if (!string.IsNullOrEmpty(result))
            {
                var albums = JsonConvert.DeserializeObject<Albums>(result);
                if (albums != null)
                {
                    Console.WriteLine(result);
                    return albums;
                }
            }
            return null;
        }

        private string BuilAlbumsURI(string id)
        {
            var query = new Dictionary<string, string>()
            {
                ["market"] = "US"
            };

            string baseURL = $"{BASEURL}{id}/albums";
            var uri = QueryHelpers.AddQueryString(baseURL, query);
            return uri;
        }

        private async Task GetToken()
        {
            string clientId = "c9d1ad81fd094e4d82630455b6009b8f";
            string clientSecret = "6a5c1cea22364044b07e85d25b061691";


            using (HttpClient httpClient = new HttpClient())
            {
                // Configurar as credenciais de autorização no formato Base64
                string base64Credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

                // Configurar a solicitação POST
                var request = new HttpRequestMessage(HttpMethod.Post, tokenURL);
                request.Headers.Add("Authorization", $"Basic {base64Credentials}");
                request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

                // Enviar a solicitação
                var response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    // Ler a resposta JSON
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);
                    var authResponse = JsonConvert.DeserializeObject<AuthenticationResponse>(responseBody);
                    if (authResponse != null)
                    {
                        GettToken(authResponse.Access_Token);
                        singleton.SetToken(authResponse.Access_Token);
                    }
                    // Analisar a resposta JSON para obter o token de acesso
                    // (neste exemplo, supomos que a resposta é um objeto JSON com um campo "access_token")
                    // Você pode usar bibliotecas como Newtonsoft.Json para analisar a resposta JSON.
                    // Exemplo: var authResponse = JsonConvert.DeserializeObject<AuthenticationResponse>(responseBody);
                    // var accessToken = authResponse.access_token;
                }
                else
                {
                    Console.WriteLine($"Erro na autenticação: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
                }
            }
        }
    }
}
