using System;
namespace ApolloStage
{
	public class ArtistsRoot
	{
		public ArtistsRoot()
		{
		}
        public string href { get; set; }
        public List<Artist> items { get; set; }
        public int limit { get; set; }
        public string next { get; set; }
        public int offset { get; set; }
        public string previous { get; set; }
        public int total { get; set; }
    }
}

