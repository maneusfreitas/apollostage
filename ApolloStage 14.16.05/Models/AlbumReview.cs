using System.ComponentModel.DataAnnotations;

namespace ApolloStage.Models
{
    public class AlbumReview
    {
        [Key]
        public int Id { get; set; }

        public string UserMail { get; set; }

        public string AlbumId { get; set; }

        public string reviewTitle { get; set; }

        public string reviewDescription { get; set; }

        public string reviewRecommendation { get; set; }

        public bool itsMine { get; set; }
    }
}
