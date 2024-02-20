using ApolloStage;


namespace ApolloStage.Services
{
    public interface IMusicService
    {
        public string getTheToken();
        public Task<Artist> GetArtist(string id);
        public Task<Albums> GetArtistAlbums(string artistId);
    }
}
