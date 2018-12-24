using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TimeManager.Model;

namespace TimeManager.Pages
{
  public class OwnTimesModel : PageModel
  {
    [BindProperty]
    public AbsenceValidation Absence { get; set; } = new AbsenceValidation();
    [BindProperty]
    public OvertimeModel Overtime { get; set; } = new OvertimeModel();

    public IActionResult OnPostAbsence()
    {
      if (ModelState.IsValid)
      {
        return Page();
      }
      foreach (var item in ModelState.Values.Reverse().Where(v => v.Errors != null))
      {
        foreach (var item2 in item.Errors)
        {
          ModelState.AddModelError(string.Empty, item2.ErrorMessage);
        }
      }
      return Page();
    }
    public IActionResult OnPostOvertime()
    {
      if (ModelState.IsValid)
      {
        return Page();
      }
      foreach (var item in ModelState.Values.Reverse().Where(v => v.Errors != null))
      {
        foreach (var item2 in item.Errors)
        {
          ModelState.AddModelError(string.Empty, item2.ErrorMessage);
        }
      }
      return Page();
    }
  }
}
