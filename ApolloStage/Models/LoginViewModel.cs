using System.ComponentModel.DataAnnotations;

namespace ApolloStage.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string? UserMail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}