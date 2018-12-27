using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TimeManager.Model;
using TimeManager.Database;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace TimeManager.Pages
{
  public class OwnTimesModel : PageModel
  {
    // Local variables
    public string[] reasons;
    public List<SelectListItem> rates;

    // Fields used for validation
    [BindProperty]
    public AbsenceValidation Absence { get; set; } = new AbsenceValidation();
    [BindProperty]
    public OvertimeValidation Overtime { get; set; } = new OvertimeValidation();

    public void OnGet()
    {
    }

    public IActionResult OnPostAbsence(AbsenceValidation model)
    {
      if (ModelState.IsValid)
      { 
        return Page();
      }
      foreach (var item in ModelState.Values.Where(v => v.Errors != null))
      {
        foreach (var item2 in item.Errors)
        {
          ModelState.AddModelError(string.Empty, item2.ErrorMessage);
        }
      }
      return Page();
    }

    public IActionResult OnPostOvertime(OvertimeValidation model)
    {
      if (ModelState.IsValid)
      {
        return Page();
      }
      foreach (var item in ModelState.Values.Where(v => v.Errors != null))
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
