using System.ComponentModel.DataAnnotations;

namespace ApolloStage.Models
{
    public class ProfileViewModel
    {
        public ApplicationUser User { get; set; }
        public List<Album> Albums { get; set; }
    }
}
