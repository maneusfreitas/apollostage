using System;
namespace ApolloStage
{
	public class AlbumsResponseList
	{
		public AlbumsResponseList()
		{
		}
        public List<Album> Items { get; set; }
        public int Limit { get; set; }
    }
}

