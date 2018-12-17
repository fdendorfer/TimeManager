using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TimeManager.Model {
  public class LoginModel : IdentityUser {
    [PersonalData]
    [Required(ErrorMessage = "Benutzername muss ausgefüllt werden")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Der Benutzername muss zwischen 2 und 100 Zeichen lang sein")]
    [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Der Benutzername darf nur Buchstaben und Zahlen enthalten")]
    public string Username { get; set; }

    [PersonalData]
    [Required(ErrorMessage = "Passwort muss ausgefüllt werden")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Das Passwort muss zwischen 3 und 100 Zeichen lang sein")]
    [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Das Passwort darf nur Buchstaben und Zahlen enthalten")]
    public string Password { get; set; }
  }
}