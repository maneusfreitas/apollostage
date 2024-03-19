using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ApolloStage.Models;
using ApolloStage.Models.Categories;
using ApolloStage.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using ApolloStage.Data;
using Microsoft.AspNetCore.Identity;
using ApolloStage.Models.Product;


namespace ApolloStage.Controllers;

public class HomeController : Controller
{
    private readonly IHttpClientHelper httpClientHelper;

    private readonly string baseUrl = "https://api.spotify.com/v1";
    private readonly string playlistx = "https://api.spotify.com/v1/browse/featured-playlists";
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ISingleton singleton;
    
    public HomeController(ApplicationDbContext context, IHttpClientHelper httpClientHelper, ISingleton singleton, UserManager<ApplicationUser> userManager)
    {
        this.httpClientHelper = httpClientHelper;
        this.singleton = singleton;
        _context = context;
        _userManager = userManager;
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public async Task<IActionResult> SearchArtist(string id,string value, string searchType)
    {

        try
        {
            if (searchType == "Products")
            {
                List<Tshirt> tshirts = _context.Tshirt.Where(p => p.Title.Contains(value)).ToList();
                List<Mug> mugs = _context.Mug.Where(p => p.title.Contains(value)).ToList();

                MarketProductsModel viewModel = new MarketProductsModel
                {
                    Tshirts = tshirts,
                    Mugs = mugs
                };

                return View("~/Views/Market/Index.cshtml", viewModel);
            }
            string searchUrl = $"{baseUrl}/search?q={id}&type=artist&offset=0&limit=11";

            var response = await httpClientHelper.SendAysnc(searchUrl, SpotifyService.AccessToken);
            var searchResult = JsonConvert.DeserializeObject<SearchResult>(response);

            List<Artist> artistList = searchResult.artists.items;
            if (searchType != "Artist")
            {
                if (artistList.Count > 0 && artistList[0].id != null)
                {
                    string artistId = artistList[0].id;

                    string albumsUrl = $"{baseUrl}/artists/{artistId}/albums?limit=10";
                    var albumsResponse = await httpClientHelper.SendAysnc(albumsUrl, SpotifyService.AccessToken);
                    var albumsResult = JsonConvert.DeserializeObject<Albums>(albumsResponse);

                    List<Album> albumList = albumsResult.items;
                    var userEmail = User.FindFirst(ClaimTypes.Email).Value;
                    foreach (var album in albumList)
                    {
                        bool albumExists = _context.FavoriteAlbum.Any(f => f.UserMail == userEmail && f.AlbumId == album.id);

                        if (albumExists)
                        {
                            album.uri = "true"; // Substitua "AdditionalAttribute" pelo nome do seu atributo adicional e "valor" pelo valor desejado

                        }
                        else if (!albumExists)
                        {
                            album.uri = "false"; // Substitua "AdditionalAttribute" pelo nome do seu atributo adicional e "valor" pelo valor desejado

                        }

                    }
                    foreach (var album in albumList)
                    {
                        var rating = _context.AlbumRatings
                                          .FirstOrDefault(f => f.userEmail == userEmail && f.albumId == album.id);

                        if (rating != null)
                        {
                            album.classificacaoEspecifica = rating.starRating;
                        }
                        else
                        {
                            album.classificacaoEspecifica = 0; // Ou defina um valor padrão, se necessário
                        }
                    }

                    string x = value.Trim();
                    TempData["artistName"] = "Albuns off: " + char.ToUpper(x[0]) + x.Substring(1);
                    var admin = await _userManager.FindByEmailAsync(userEmail);
                    TempData["meu"] = admin.Admin;
                    return View("Artist", albumList);
                }
            }
            else
            {
                var userEmail = User.FindFirst(ClaimTypes.Email).Value;
                var admin = await _userManager.FindByEmailAsync(userEmail);
                TempData["meu"] = admin.Admin;
                return View("ArtistList", artistList);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro na pesquisa do artista: " + ex.Message);
        }

        return View("NotFound");
    }




    [HttpPost]
        public async Task<IActionResult> SearchArtistAjax(string id)
        {
            try
            {
                string searchUrl = $"{baseUrl}/search?q={id}&type=artist&offset=0&limit=11";

                // Faça uma solicitação GET para a URL de pesquisa usando httpClientHelper
                var response = await httpClientHelper.SendAysnc(searchUrl, SpotifyService.AccessToken);
                var searchResult = JsonConvert.DeserializeObject<SearchResult>(response);
                List<Artist> artistList = searchResult.artists.items;

                // Crie uma lista de objetos que contêm nome e imagem do artista
                var artistDataList = artistList.Select(artist => new
                {
                    Name = artist.name,
                    ImageUrl = artist.images.Count > 0 ? artist.images[0].URL : null
                }).ToList();

                return Json(artistDataList);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro na pesquisa do artista: " + ex.Message);
                return Json(new List<object>()); // Retorna uma lista vazia em caso de erro
            }
        }


        public async Task<ActionResult> ArtistDetails(string id)
        {
            var artist = id;

            if (artist == null)
            {
                return RedirectToAction("NotFound");
            }

            return View(artist);
        }


        public async Task<ActionResult> Index(string id = "6fOMl44jA4Sp5b9PpYCkzz")
        {
        var userEmail = User.FindFirst(ClaimTypes.Email).Value;
        var admin = await _userManager.FindByEmailAsync(userEmail);

        List<Artist> artistList = new List<Artist>();
            List<Playlist> playlists = new List<Playlist>();

            Artist artist;

            using (var httpClient = new HttpClient())
            {
            //https://apollostage1.azurewebsites.net
            //https://apollostage20240303150613.azurewebsites.net
            using (var response = await httpClient.GetAsync($"https://localhost:7164/GetArtist/{id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        artist = JsonConvert.DeserializeObject<Artist>(result);

                        if (artist != null)
                        {
                            artistList.Add(artist);
                        }
                    }
                }
            }
           

            // albuns lançados recentemente
            string apiUrl = "https://api.spotify.com/v1/browse/new-releases";

            var responsex = await httpClientHelper.SendAysnc(apiUrl, SpotifyService.AccessToken);
            

            // top10 playlist albuns
           
            var responseplaylists = await httpClientHelper.SendAysnc(playlistx, SpotifyService.AccessToken);
            var json = responseplaylists;

            var jsonObject = JObject.Parse(json);
            var items = jsonObject["playlists"]["items"];

            var playlistsx = items
                .Select(item => new Playlist
                {
                    Collaborative = (bool)item["collaborative"],
                    Description = (string)item["description"],
                    ExternalUrls = new ExternalUrls { spotify = (string)item["external_urls"]["spotify"] },
                    Href = (string)item["href"],
                    Id = (string)item["id"],
                    Name = (string)item["name"],
                    Owner = new Owner { Id = (string)item["owner"]["id"], Type = (string)item["owner"]["type"] },
                    Public = (bool)item["collaborative"],
                    SnapshotId = (string)item["snapshot_id"],
                    Tracks = new Tracks { Href = (string)item["href"], Total = 0 },
                    Type = (string)item["type"],
                    Uri = (string)item["uri"]
                })
                .ToList();



        foreach (var favoriteAlbum in playlistsx)
        {
            var playlistUrl = $"https://api.spotify.com/v1/playlists/{favoriteAlbum.Id}";

            // Realize a solicitação para obter informações detalhadas da playlist
            var responseJsonr = await httpClientHelper.SendAysnc(playlistUrl, SpotifyService.AccessToken);

            // Verifique se a resposta não é nula ou vazia
            if (!string.IsNullOrEmpty(responseJsonr))
            {
                // Deserialize a resposta para uma instância da classe Playlist
                var detailedPlaylist = JsonConvert.DeserializeObject<Playlist>(responseJsonr);

                // Atualize as informações na lista original
                var originalPlaylist = playlistsx.FirstOrDefault(p => p.Id == favoriteAlbum.Id);
                if (originalPlaylist != null)
                {
                    originalPlaylist.Images = detailedPlaylist.Images;
                    // Adicione outras propriedades que deseja atualizar
                }
            }
            else
            {
                // Lide com o caso em que a resposta é nula ou vazia, se necessário
            }
        }
        

        

        // categorias 

        string spotifyCategories = "https://api.spotify.com/v1/browse/categories?locale=pt-PT";
            var responseJson = await httpClientHelper.SendAysnc(spotifyCategories, SpotifyService.AccessToken);
            var responseJObject = JObject.Parse(responseJson);
            Console.WriteLine(spotifyCategories);

            var categoriesList = responseJObject["categories"]["items"]
                .Select(categoryItem => new Categories
                {
                    href = (string)categoryItem["href"],
                    items = new List<Item>
                    {
            new Item
            {
                id = (string)categoryItem["id"],
                name = (string)categoryItem["name"],
                href = (string)categoryItem["icons"][0]["url"]
            }
                    }
                })
                .ToList();

        // top 10 genero musical
        string rAlbums = "4czdORdCWP9umpbhFXK2fW,1YZiR5FINFOlZPGKSVplIY,2cWBwpqMsDJC1ZUwz813lo,6QtnCAFmqOwR75jOOmU7k9,6zaisPwfcIAfdUGPj3mmGY,0u7sgzvlLmPLvujXxy9EeY,3elU9JzR0DtbdRv8EOzBa4,3VWrUk4vBznMYXGMPc7dRB,2acDkDTWdNFie1HjcFa4Ny,6i6folBtxKV28WX3msQ4FE";
        string urlnew = "https://api.spotify.com/v1/albums?ids=" + rAlbums;
        var responseJsonw = await httpClientHelper.SendAysnc(urlnew, SpotifyService.AccessToken);

        var responseJObjectw = JObject.Parse(responseJsonw);
        var albums = responseJObjectw["albums"].ToObject<List<Album>>();


        var albumList = albums.Select(albumItem => new Album
        {
            album_type = albumItem.album_type,
            artists = albumItem.artists,
            available_markets = albumItem.available_markets,
            external_urls = albumItem.external_urls,
            href = albumItem.href,
            id = albumItem.id,
            images = albumItem.images,
            name = albumItem.name,
            release_date = albumItem.release_date,
            release_date_precision = albumItem.release_date_precision,
            type = albumItem.type,
            uri = albumItem.uri,
            label = albumItem.label,
            popularity = albumItem.popularity,
        }).ToList();

        TempData["meu"] = admin.Admin;
        return View(albumList);

    
        }

        public ActionResult NotFound()
        {
            return View();
        }
}