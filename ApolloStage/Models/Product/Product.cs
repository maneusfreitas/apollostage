using System;
using System.ComponentModel.DataAnnotations;

namespace ApolloStage.Models.Product
{
	public class Product
	{
		public Product()
		{
		}
        [Key]
        public int Id { get; set; }
        public string title { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}

