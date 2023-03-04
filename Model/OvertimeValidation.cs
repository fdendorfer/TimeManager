using System.ComponentModel.DataAnnotations;

namespace TimeManager.Model
{
  public class OvertimeValidation
  {
    // Set, when an overtime is opened for editing
    public string ID { get; set; }

    public string IdOvertimeDetail { get; set; }

    [Required(ErrorMessage = "Datum darf nicht leer sein")]
    [RegularExpression(@"^(\d{2})\.(\d{2})\.(\d{4})$", ErrorMessage = "Die Eingabe muss in folgendem Format sein: dd.MM.yyyy")]
    public string Date { get; set; }

    [Required(ErrorMessage = "Stunden darf nicht leer sein")]
    public decimal Hours { get; set; }

    [Required(ErrorMessage = "Kunde darf nicht leer sein")]
    public string Customer { get; set; }

    public bool Approved { get; set; }

  }
}