using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TourManager.Pages
{
  public class IndexModel : PageModel
  {

    [Required]
    [StringLength(100, MinimumLength = 3)]
    [Display(Name = "Benutzername")]
    public string Username { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
      if (!ModelState.IsValid)
        return Page();

      return RedirectToPage("/OwnTimes");
    }
  }
}
