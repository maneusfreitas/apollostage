using System;
namespace ApolloStage
{
	public class Albums
	{
		public Albums()
		{
		}
        public string href { get; set; }
        public List<Album> items { get; set; }
        public int limit { get; set; }
        public string next { get; set; }
        public int offset { get; set; }
        public object previous { get; set; }
        public int total { get; set; }
    }
}

