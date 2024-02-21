using System;
using System.ComponentModel.DataAnnotations;

namespace ApolloStage.Models.Extra
{
	public class Classification
	{
        [Key]
        public int Id { get; set; }
        public string userEmail { get; set; }
        public string albumId { get; set; }
        public int starRating { get; set; }
    }
}

