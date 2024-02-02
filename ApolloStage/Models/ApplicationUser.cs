﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ApolloStage.Models
{
    public class ApplicationUser : IdentityUser
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
        public bool ConfirmedEmail { get; set; } // Corrigido para bool em vez de bool?
    }
}