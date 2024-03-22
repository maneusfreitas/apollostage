using System.ComponentModel.DataAnnotations;

namespace ApolloStage.Models
{
    public class ListenList
    {
        [Key]
        public int Id { get; set; }

        public string UserMail { get; set; }

        public string AlbumId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
