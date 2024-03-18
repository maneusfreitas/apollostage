using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ApolloStage.Models
{
    public class ApplicationUser : IdentityUser
    {

        public ApplicationUser()
        {
            FavoriteAlbum = new List<FavoriteAlbum>(); 
        }

        [Key]
        public Guid Id { get; set; }

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
        public bool Admin { get; set; }

        [Required]
        public bool ConfirmedEmail { get; set; }

        public int points { get; set; }

        public ICollection<FavoriteAlbum> FavoriteAlbum { get; set; }


        public string Morada { get; set; }

        public string Cidade { get; set; }

        public string CodigoPostal { get; set; }

        public string Pais { get; set; }

        public string Numerotel { get; set; }



        public string NomeEntrega { get; set; }

        public string MoradaEntrega { get; set; }

        public string CidadeEntrega { get; set; }

        public string CodigoPostalEntrega { get; set; }

        public string PaisEntrega { get; set; }

        public string NumerotelEntrega { get; set; }
        public bool ConfirmoEnvio { get; set; }
    }
}
