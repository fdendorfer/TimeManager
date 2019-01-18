using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TimeManager.Database;
using TimeManager.Model;

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

    public async Task<JsonResult> OnGetAbsence([FromQuery] string id)
    {
      using (var db = new DatabaseContext())
      {
        return await Task.Run(() =>
         {
           Thread.CurrentThread.CurrentCulture = new CultureInfo("de-CH");
           var absence = (from a in db.Absence
                          where a.ID == new Guid(id)
                          select a).Single();

           // Filling the absencePartial with existing values
           Absence.ID = absence.ID.ToString();
           Absence.AbsenceDateFrom = absence.AbsentFrom.ToShortDateString();
           Absence.AbsenceDateTo = absence.AbsentTo.ToShortDateString();
           if (absence.AbsentFrom.ToShortTimeString() == DateTime.MinValue.ToShortTimeString())
           {
             Absence.FullDay = true;
           } else
           {
             Absence.AbsenceTimeFrom = absence.AbsentFrom.ToShortTimeString();
             Absence.AbsenceTimeTo = absence.AbsentTo.ToShortTimeString();
           }
           Absence.Negative = absence.Negative;
           Absence.OtherReason = absence.Reason;
           Absence.Approved = absence.Approved;

           return new JsonResult(JsonConvert.SerializeObject(Absence));
         });
      }
    }

    public async Task<JsonResult> OnGetOvertime([FromQuery] string id)
    {
      using (var db = new DatabaseContext())
      {
        return await Task.Run(() => {
          Thread.CurrentThread.CurrentCulture = new CultureInfo("de-CH");
          var overtime = (from o in db.Overtime
                         where o.ID == new Guid(id)
                         select o).Single();

          // Filling the overtimePartial with existing values
          Overtime.ID = overtime.ID.ToString();
          Overtime.IdOvertimeDetail = overtime.IdOvertimeDetail.ToString();
          Overtime.Date = overtime.Date.ToShortDateString();
          Overtime.Hours = overtime.Hours;
          Overtime.Customer = overtime.Customer;

          return new JsonResult(JsonConvert.SerializeObject(Overtime));
        });
      }
    }

    public void OnDeleteAbsence([FromQuery] string id)
    {
      using (var db = new DatabaseContext())
      {
        db.Absence.Remove(db.Absence.Find(new Guid(id)));
        db.SaveChanges();
      }
    }

    public void OnDeleteOvertime([FromQuery] string id)
    {
      using (var db = new DatabaseContext())
      {
        db.Overtime.Remove(db.Overtime.Find(new Guid(id)));
        db.SaveChanges();
      }
    }

    public IActionResult OnPostAbsence()
    {
      var isValid = Validator.TryValidateObject(Absence, new ValidationContext(Absence, serviceProvider: null, items: null), new List<ValidationResult>(), true);

      if (isValid)
      {
        var dateTimeFrom = DateTime.Parse(Absence.AbsenceDateFrom + " " + Absence.AbsenceTimeFrom, new CultureInfo("de-CH"));
        var dateTimeTo = DateTime.Parse(Absence.AbsenceDateTo + " " + Absence.AbsenceTimeTo, new CultureInfo("de-CH"));
        var userID = new Guid(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        using (var db = new DatabaseContext())
        {
          var absence = new AbsenceModel()
          {
            ID = Absence.ID == null ? Guid.NewGuid() : new Guid(Absence.ID),
            IdUser = userID,
            IdAbsenceDetail = db.AbsenceDetail.Where(a => a.Reason == Absence.Reason).Select(a => a.ID).FirstOrDefault(),
            AbsentFrom = dateTimeFrom,
            AbsentTo = dateTimeTo,
            Negative = Absence.Negative,
            Reason = Absence.OtherReason ?? Absence.Reason,
            Approved = Absence.Approved,
            CreatedOn = DateTime.Now
          };

          var record = db.Absence.AsNoTracking().SingleOrDefault(a => a.ID == absence.ID);
          if (record == null)
          {
            db.Absence.Add(absence);
          } else
          {
            record = absence;
            db.Update(record);
          }
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
      var isValid = Validator.TryValidateObject(Overtime, new ValidationContext(Overtime, serviceProvider: null, items: null), new List<ValidationResult>());

      if (isValid)
      {
        var userID = new Guid(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        using (var db = new DatabaseContext())
        {
          var overtime = new OvertimeModel()
          {
            ID = Overtime.ID == null ? Guid.NewGuid() : new Guid(Overtime.ID),
            IdOvertimeDetail = new Guid(Overtime.IdOvertimeDetail),
            IdUser = new Guid(User.FindFirst(ClaimTypes.NameIdentifier).Value),
            Customer = Overtime.Customer,
            Date = DateTime.Parse(Overtime.Date, new CultureInfo("de-CH")),
            Hours = Overtime.Hours,
            CreatedOn = DateTime.Now
          };

          var record = db.Overtime.AsNoTracking().SingleOrDefault(o => o.ID == overtime.ID);
          if (record == null)
          {
            db.Overtime.Add(overtime);
          } else
          {
            record = overtime;
            db.Update(record);
          }
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
