using ApolloStage;
using ApolloStage.DTO;
using ApolloStage.Models;

namespace ApolloStage
{
    public class AlbumDetails
    {
        public string AlbumId { get; set; }
        public List<Track> Tracks { get; set; }
        public List<Album> AlbumsInfo { get; set; }
        public List<AlbumReview> AlbumReviews { get; set; }
    }


}
