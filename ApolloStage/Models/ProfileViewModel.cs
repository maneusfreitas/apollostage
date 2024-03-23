using System.ComponentModel.DataAnnotations;

namespace ApolloStage.Models
{
    public class ProfileViewModel
    {
        public ApplicationUser User { get; set; }
        public List<Album> Album { get; set; }
        public List<Album> Listenslist { get; set; }

        public int NReviwesUser { get; set; }
        public int NReviwesTotal { get; set; }

        public int NRecommendedalbum { get; set; }
        public int NNotRecommendedalbum { get; set; }

        public int NUserRatings{ get; set; }
        public int NTotalRatings { get; set; }

        public Dictionary<string, int> GenderChart { get; set; }
    }
}
