using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TimeManager.Extensions;

namespace TimeManager.Model
{
    public class OvertimeValidation
    {
        [Required(ErrorMessage = "Datum darf nicht leer sein")]
        [RegularExpression(@"^(\d{2})\.(\d{2})\.(\d{4})$", ErrorMessage = "Die Eingabe muss in folgendem Format sein: dd.MM.yyyy")]
        public string Date { get; set; }

        [Required(ErrorMessage = "Stunden darf nicht leer sein")]
        public decimal Hours { get; set; }

        [Required(ErrorMessage = "Kunde darf nicht leer sein")]
        public string Customer { get; set; }

        public Guid IdOvertimeDetail { get; set; }
    }
}