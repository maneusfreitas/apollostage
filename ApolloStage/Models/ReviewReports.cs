using System.ComponentModel.DataAnnotations;

namespace ApolloStage.Models
{
    public class ReviewReports
    {
        [Key]
        public int Id { get; set; }
        public string IdReview { get; set; }
        public string IdUserMail { get; set; }
        public string causa { get; set; }
        public string descricao { get; set; }
        public int count { get; set; }
    }
}
