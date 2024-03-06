using System;
namespace ApolloStage.Models.Categories
{
	public class Categories
	{
		public Categories()
		{
		}
        public string href { get; set; }
        public List<Item> items { get; set; }
        public int limit { get; set; }
        public int offset { get; set; }
        public int total { get; set; }
    }
}

