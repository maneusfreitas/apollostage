using System;
namespace ApolloStage.Models
{
	public class CheckoutOrderResponse
	{
		public CheckoutOrderResponse()
		{
		}
        public string? SessionId {get; set; }
	    public string? PubKey { get;set; }
	}
}

