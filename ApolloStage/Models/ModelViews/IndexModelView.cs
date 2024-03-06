using System;


namespace ApolloStage.Models
{
	public class IndexModelView
	{
		
        public List<Artist> Artists { get; set; }
        public List<Playlist> Playlists { get; set; }
        public List<Album> Albums { get; set; }
        public List<Categories.Categories> Categories { get; set; }
    }
}

