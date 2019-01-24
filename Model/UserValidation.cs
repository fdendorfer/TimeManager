using System.ComponentModel.DataAnnotations;

namespace TimeManager.Model
{
  public class UserValidation
  {
    // Set, when a user is opened for editing
    public string ID { get; set; }

    public string IdPermission { get; set; }

    [Required(ErrorMessage = "Benutzername darf nicht leer sein")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Vorname darf nicht leer sein")]
    public string Firstname { get; set; }

    [Required(ErrorMessage = "Nachname darf nicht leer sein")]
    public string Lastname { get; set; }

    public string Password { get; set; }

    public string Department { get; set; }

    [Required(ErrorMessage = "Ferientage darf nicht leer sein")]
    public string Holidays { get; set; }

    public bool Deactivated { get; set; }
  }
}
