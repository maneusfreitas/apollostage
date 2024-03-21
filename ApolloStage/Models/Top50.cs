using System.ComponentModel.DataAnnotations;

namespace ApolloStage.Models
{
    public class Top50
    {
        [Key]
        public int Id { get; set; }
        public string IdAlbum { get; set; }
        public int count { get; set; }
    }
}
