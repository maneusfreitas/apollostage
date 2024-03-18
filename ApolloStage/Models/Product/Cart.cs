using System;
namespace ApolloStage.Models.Product
{
	public class Cart
	{
		public Cart()
		{
		}
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string Image { get; set; }
        public string Pname { get; set; }
        public decimal Total { get; set; }
        public int Pointssend { get; set; }
    }
}

