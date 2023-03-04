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
    public readonly DatabaseContext _db;
    public string[] reasons;
    public List<SelectListItem> userList;
    public List<SelectListItem> rates;
    public List<AbsenceModel> absences;
    public List<dynamic> overtimes;
    // Fields used for validation
    [BindProperty]
    public AbsenceValidation Absence { get; set; } = new AbsenceValidation();
    [BindProperty]
    public OvertimeValidation Overtime { get; set; } = new OvertimeValidation();

    public OwnTimesPageModel(DatabaseContext db)
    {
      _db = db;
      userList = (from u in _db.User select new SelectListItem(u.Username, u.ID.ToString())).ToList();
    }

    // This returns eighter the page normally or a json array of lists containing the absences and overtimes for a specific user eighter created in the last 30 days or all
    public IActionResult OnGet([FromQuery] string last30days, [FromQuery] string selectedUser)
    {
      if (last30days == null && selectedUser == null)
        return Page();

      // This array has always 4 values, 0 with filtered absences, 1 with filtered overtimes, 2 with remaining and total Holidays and 3 with total overtimes from this year
      dynamic[] result = new dynamic[4];
      // Absences
      absences = (from a in _db.Absence
                  where (a.IdUser == new Guid(selectedUser))
                  orderby a.AbsentFrom
                  select a).ToList();
      if (last30days == "true")
        absences = absences.Where(a => (DateTime.Now - a.LastChanged).TotalDays < 30).ToList();

      result[0] = absences;

      // Overtimes
      overtimes = (from o in _db.Overtime
                   join od in _db.OvertimeDetail on o.IdOvertimeDetail equals od.ID
                   where (o.IdUser == new Guid(selectedUser))
                   orderby o.Date
                   select new { o, od }).ToList<dynamic>();
      if (last30days == "true")
        overtimes = overtimes.Where(d => (DateTime.Now - d.o.CreatedOn).TotalDays < 30).ToList();

      result[1] = overtimes;

      // Remaining Holidays
      var absenceTotal = (from a in _db.Absence
                          where (a.AbsentFrom.Year == DateTime.Now.Year) || (a.AbsentTo.Year == DateTime.Now.Year)
                          where a.IdUser == new Guid(selectedUser)
                          select a.TotalDays).ToArray().Sum();

      result[2] = new dynamic[] { absenceTotal, _db.User.Where(u => u.ID.ToString() == selectedUser).SingleOrDefault().Holidays };

      // Done overtimes
      result[3] = (from o in _db.Overtime
                   join od in _db.OvertimeDetail on o.IdOvertimeDetail equals od.ID
                   where o.Date.Year == DateTime.Now.Year
                   where o.IdUser == new Guid(selectedUser)
                   let r = o.Hours * od.Rate
                   select Decimal.Round(r, 2)).ToArray().Sum();

      return new JsonResult(result);
    }

    public async Task<JsonResult> OnGetAbsence([FromQuery] string id)
    {
      return await Task.Run(() =>
       {
         Thread.CurrentThread.CurrentCulture = new CultureInfo("de-CH");
         var absence = (from a in _db.Absence
                        where a.ID == new Guid(id)
                        select a).Single();

         // Filling the absencePartial with existing values
         Absence.ID = absence.ID.ToString();
         Absence.AbsenceDateFrom = absence.AbsentFrom.ToShortDateString();
         Absence.AbsenceDateTo = absence.AbsentTo.ToShortDateString();
         Absence.FromAfternoon = absence.FromAfternoon;
         Absence.ToAfternoon = absence.ToAfternoon;
         Absence.Negative = absence.Negative;
         Absence.OtherReason = absence.Reason;
         Absence.Approved = absence.Approved;

         return new JsonResult(JsonConvert.SerializeObject(Absence));
       });
    }

    public async Task<JsonResult> OnGetOvertime([FromQuery] string id)
    {
      return await Task.Run(() =>
      {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("de-CH");
        var overtime = (from o in _db.Overtime
                        where o.ID == new Guid(id)
                        select o).Single();

        // Filling the overtimePartial with existing values
        Overtime.ID = overtime.ID.ToString();
        Overtime.IdOvertimeDetail = overtime.IdOvertimeDetail.ToString();
        Overtime.Date = overtime.Date.ToShortDateString();
        Overtime.Hours = overtime.Hours;
        Overtime.Approved = overtime.Approved;
        Overtime.Customer = overtime.Customer;

        return new JsonResult(JsonConvert.SerializeObject(Overtime));
      });
    }

    public void OnDeleteAbsence([FromQuery] string id)
    {
      _db.Absence.Remove(_db.Absence.Find(new Guid(id)));
      _db.SaveChanges();
    }

    public void OnDeleteOvertime([FromQuery] string id)
    {
      _db.Overtime.Remove(_db.Overtime.Find(new Guid(id)));
      _db.SaveChanges();
    }

    public IActionResult OnPostAbsence()
    {
      var isValid = Validator.TryValidateObject(Absence, new ValidationContext(Absence, serviceProvider: null, items: null), new List<ValidationResult>(), true);

      if (isValid)
      {
        var userID = new Guid(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var dateTimeFrom = DateTime.Parse(Absence.AbsenceDateFrom, new CultureInfo("de-CH"));
        var dateTimeTo = DateTime.Parse(Absence.AbsenceDateTo, new CultureInfo("de-CH"));
        var absenceTotal = (Decimal)0;
        // https://stackoverflow.com/questions/13440595/get-list-of-dates-from-startdate-to-enddate
        // Creates a List with each number from 0 to the count of days between AbsentFrom and AbsentTo
        List<double> daysToAdd = Enumerable.Range(0, (dateTimeTo - dateTimeFrom).Days + 1).ToList().ConvertAll(d => (double)d);
        // Creates a List with every date between and including AbsentFrom and AbsentTo
        List<DateTime> ListOfDates = daysToAdd.Select(dateTimeFrom.AddDays).ToList();
        foreach (var date in ListOfDates)
          if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
            if (Absence.Negative)
              absenceTotal -= 1;
            else
              absenceTotal += 1;

        // Subtract or add, depending on a.Negative, half a day from AbsentFrom, if FromAfternoon is true and AbsentFrom is not on a weekend
        if (Absence.FromAfternoon && dateTimeFrom.DayOfWeek != DayOfWeek.Saturday && dateTimeFrom.DayOfWeek != DayOfWeek.Sunday)
          absenceTotal = Absence.Negative ? absenceTotal + (Decimal)0.5 : absenceTotal - (Decimal)0.5;
        // Subtract or add, depending on a.Negative, half a day from AbsentTo, if ToAfternoon is true and AbsentTo is not on a weekend
        if (!Absence.ToAfternoon && dateTimeTo.DayOfWeek != DayOfWeek.Saturday && dateTimeTo.DayOfWeek != DayOfWeek.Sunday)
          absenceTotal = Absence.Negative ? absenceTotal + (Decimal)0.5 : absenceTotal - (Decimal)0.5;

        var absence = new AbsenceModel()
        {
          ID = Absence.ID == null ? Guid.NewGuid() : new Guid(Absence.ID),
          IdUser = userID,
          IdAbsenceDetail = _db.AbsenceDetail.Where(a => a.Reason == Absence.Reason).Select(a => a.ID).FirstOrDefault(),
          AbsentFrom = dateTimeFrom,
          AbsentTo = dateTimeTo,
          FromAfternoon = Absence.FromAfternoon,
          ToAfternoon = Absence.ToAfternoon,
          TotalDays = absenceTotal,
          Negative = Absence.Negative,
          Reason = Absence.OtherReason ?? Absence.Reason,
          Approved = Absence.Approved,
          LastChanged = DateTime.Now
        };

        var record = _db.Absence.AsNoTracking().SingleOrDefault(a => a.ID == absence.ID);
        if (record == null)
        {
          _db.Absence.Add(absence);
        } else
        {
          record = absence;
          _db.Update(record);
        }
        _db.SaveChanges();

        return StatusCode(202); // HTTP 202 ACCEPTED
      }
      return Page();
    }

    public IActionResult OnPostOvertime()
    {
      var isValid = Validator.TryValidateObject(Overtime, new ValidationContext(Overtime, serviceProvider: null, items: null), new List<ValidationResult>());

      if (isValid)
      {
        var userID = new Guid(User.FindFirst(ClaimTypes.NameIdentifier).Value);


        var overtime = new OvertimeModel()
        {
          ID = Overtime.ID == null ? Guid.NewGuid() : new Guid(Overtime.ID),
          IdOvertimeDetail = new Guid(Overtime.IdOvertimeDetail),
          IdUser = new Guid(User.FindFirst(ClaimTypes.NameIdentifier).Value),
          Customer = Overtime.Customer,
          Date = DateTime.Parse(Overtime.Date, new CultureInfo("de-CH")),
          Hours = Overtime.Hours,
          Approved = Overtime.Approved,
          LastChanged = DateTime.Now
        };

        var record = _db.Overtime.AsNoTracking().SingleOrDefault(o => o.ID == overtime.ID);
        if (record == null)
        {
          _db.Overtime.Add(overtime);
        } else
        {
          record = overtime;
          _db.Update(record);
        }
        _db.SaveChanges();

        return StatusCode(202); // HTTP 202 ACCEPTED
      }
      return Page();
    }
  }
}
