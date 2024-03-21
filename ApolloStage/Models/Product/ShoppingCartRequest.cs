using System;
namespace ApolloStage.Models.Product
{
	public class ShoppingCartRequest
	{
		public ShoppingCartRequest()
		{
		}
        public List<Cart> cartContent { get; set; }
        public int points { get; set; }
    }
}

