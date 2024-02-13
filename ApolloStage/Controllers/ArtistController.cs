using Microsoft.AspNetCore.Mvc;
using ApolloStage.Services;

namespace ApolloStage.Controllers
{
    [ApiController]
    [Route("api/artist")]
    public class ArtistController : ControllerBase
    {
        private readonly IMusicService spotifyService;

        public ArtistController(IMusicService spotifyService)
        {
            this.spotifyService = spotifyService;
        }

        [HttpGet("/GetArtist/{id}")]
        public async Task<ActionResult> Get(string id = "6fOMl44jA4Sp5b9PpYCkzz")
        {

            var artist = await spotifyService.GetArtist(id);
           if (artist is null)
            {
                return BadRequest();
            }
            return Ok(artist);
        }

        
    }
}