using System;
namespace ApolloStage.Models.Product
{
	public class Cart
	{
		public Cart()
		{
		}
        public string name { get; set; }
        public decimal price { get; set; }
        public decimal total { get; set; }
        public int count { get; set; }
        public string color { get; set; }
        public string size { get; set; }
        public string image { get; set; }
    }
}

