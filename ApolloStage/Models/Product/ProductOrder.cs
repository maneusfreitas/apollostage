using System;
using System.ComponentModel.DataAnnotations;

namespace ApolloStage.Models.Product
{
	public class ProductOrder
	{
		public ProductOrder()
		{
		}
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        [EmailAddress]
        public string UserMail { get; set; }

        [Required]
        public string TshirtTitle { get; set; }

        [Required]
        public string TshirtSize { get; set; }
        [Required]
        public string TshirtColor { get; set; }

        [Required]
        public int TshirtCount { get; set; }

        [Required]
        public decimal TshirtPrice { get; set; }

        [Required]
        public DateTime data { get; set; }

        public string State { get; set; }

        public int Pointstoapply { get; set; }
    }
}

