using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ApolloStage.Models
{
    public class ApplicationUser : IdentityUser, IValidatableObject
    {
        [Required]
        [EmailAddress]
        public string UserMail { get; set; }

        [Required]
        public string Name { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string Gender { get; set; }

        public string Code { get; set; }

        [Required]
        public bool ConfirmedEmail { get; set; } 

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
        
            if (DateOfBirth > DateTime.Now.AddYears(100))
            {
                yield return new ValidationResult("A data de nascimento não pode ser superior a 100 anos", new[] { nameof(DateOfBirth) });
            }
        }
    }


}
