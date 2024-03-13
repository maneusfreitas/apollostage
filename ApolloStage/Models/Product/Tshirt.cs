using System;
using System.ComponentModel.DataAnnotations;

namespace ApolloStage.Models.Product
{
   
    public class Tshirt
    {
		public Tshirt()
		{
		}
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Images1 { get; set; }
        public string Images2 { get; set; }
        public string Images3 { get; set; }
        public string Images4 { get; set; }
        public string Association { get; set; }
    }
}

