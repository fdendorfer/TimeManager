using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TimeManager.Model;
using TimeManager.Database;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System;
using System.Security.Claims;
using System.Globalization;

namespace TimeManager.Pages
{
  public class OwnTimesPageModel : PageModel
  {
    // Local variables
    public string[] reasons;
    public List<SelectListItem> rates;

    // Fields used for validation
    [BindProperty]
    public AbsenceValidation Absence { get; set; } = new AbsenceValidation();
    [BindProperty]
    public OvertimeValidation Overtime { get; set; } = new OvertimeValidation();

    public IActionResult OnPostAbsence()
    {
      var oldModelState = ModelState;
      //ModelState.Clear();
      TryValidateModel(Absence);

      if (ModelState.IsValid)
      {
        var dateTimeFrom = DateTime.Parse(Absence.AbsenceDateFrom + " " + Absence.AbsenceTimeFrom, new CultureInfo("de-CH"));
        var dateTimeTo = DateTime.Parse(Absence.AbsenceDateTo + " " + Absence.AbsenceTimeTo, new CultureInfo("de-CH"));
        var userID = new Guid(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        using (var db = new DatabaseContext())
        {
          var absence = new AbsenceModel()
          {
            ID = Guid.NewGuid(),
            IdUser = userID,
            IdAbsenceDetail = db.AbsenceDetail.Where(a => a.Reason == Absence.Reason).Select(a => a.ID).FirstOrDefault(),
            AbsentFrom = dateTimeFrom,
            AbsentTo = dateTimeTo,
            Reason = Absence.Reason,
            Approved = Absence.Approved,
            CreatedOn = DateTime.Now
          };
          db.Absence.Add(absence);
          db.SaveChanges();
        }
        return StatusCode(202); // HTTP 202 ACCEPTED
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

    public IActionResult OnPostOvertime()
    {
      bool modelValid;
      {
        var pm = new OwnTimesPageModel();
        var ms = pm.ModelState;
        ms.Clear();
        //Validate onto ms
        modelValid = pm.TryValidateModel(Overtime);
      }
      
      //TryValidateModel(Overtime);
      

      if (modelValid)
      {
        var userID = new Guid(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        
        using (var db = new DatabaseContext())
        {
          var overtime = new OvertimeModel()
          {
            ID = Guid.NewGuid(),
            IdOvertimeDetail = Overtime.IdOvertimeDetail,
            IdUser = new Guid(User.FindFirst(ClaimTypes.NameIdentifier).Value),
            Customer = Overtime.Customer,
            Date = DateTime.Parse(Overtime.Date, new CultureInfo("de-CH")),
            Hours = Overtime.Hours,
            CreatedOn = DateTime.Now
          };
          db.Overtime.Add(overtime);
          db.SaveChanges();
        }
        return StatusCode(202); // HTTP 202 ACCEPTED
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
