using System.ComponentModel.DataAnnotations;

namespace ApolloStage.Models
{
    public class Dashboard
    {
        [Key]
        public int Id { get; set; }
        public int TotalEmail { get; set; }
        public int TotalProducts { get; set; }
        public int TotalUsers { get; set; }

        public int TotalListenList { get; set; }
        public int TotalReviews { get; set; }
        public int TotalRatings { get; set; }
        public int TotalProductSales { get; set; }

        public decimal balance { get; set; }
        public List<dynamic> ordercoutstatus { get; set; }
        public List<ReviewReports> Reports { get; set; }

        public List<dynamic> orders { get; set; }

        public List<dynamic> averageRatings { get; set; }
        public Dictionary<string, int> GenderChart { get; set; }

    }
}
