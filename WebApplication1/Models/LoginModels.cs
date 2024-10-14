using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class LoginModels
    {
        [Required(ErrorMessage = "Le numéro de dossier est requis")]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "Le numéro de dossier doit être un nombre de 6 chiffres.")]

        public int Numero { get; set; }
        public string nom { get; set; }
        public string mail { get; set; }
        public bool remember{ get; set; }
    }
}