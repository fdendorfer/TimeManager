using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TimeManager.Database;
using TimeManager.Extensions;
using TimeManager.Model;

namespace TimeManager.Pages
{
  public class ControllingPageModel : PageModel
  {
    // Local variables
    private readonly DatabaseContext _db;
    public List<ControllingModel> Absences;

    public ControllingPageModel(DatabaseContext db)
    {
      _db = db;
    }

    public IActionResult OnGet([FromQuery] string uncheckedOnly)
    {
      if (uncheckedOnly == null)
        return Page();

      UserModel user = _db.User.Where(u => u.ID == new Guid(User.FindFirst(ClaimTypes.NameIdentifier).Value)).FirstOrDefault();
      Absences = (from a in _db.Absence
                  join u in _db.User on a.IdUser
                  equals u.ID
                  where u.Department == user.Department
                  where u.Deactivated == false
                  orderby a.AbsentFrom
                  select new ControllingModel()
                  {
                    IdUser = u.ID,
                    IdAbsence = a.ID,
                    Name = u.Username,
                    AbsentFrom = a.AbsentFrom,
                    AbsentTo = a.AbsentTo,
                    Reason = a.Reason,
                    Approved = a.Approved
                  }).ToList();
      if (uncheckedOnly == "true")
        Absences = Absences.Where(a => a.Approved == false).ToList();

      return new JsonResult(Absences);
    }

    public IActionResult OnPost([FromQuery]string idAbsence, [FromQuery]bool value)
    {
      // Gets user from db by id and changes the value of approved
      var absence = _db.Absence.SingleOrDefault(a => a.ID == new Guid(idAbsence));
      absence.Approved = value;
      _db.SaveChanges();
      // Returning page to update view
      return Page();
    }

    public async Task<IActionResult> OnGetFerienlisteAsync()
    {
      UserModel user = _db.User.Where(u => u.ID == new Guid(User.FindFirst(ClaimTypes.NameIdentifier).Value)).FirstOrDefault();

      var filename = @"Ferienliste_" + user.Department + ".xlsx";

      await ExcelHandler.CreateFerienliste(user.Department, _db);
      return File(System.IO.File.ReadAllBytes(@"C:\temp\" + filename), "application/vnd.ms-excel", filename);
    }

    public async Task<IActionResult> OnGet‹berzeitkontrolleAsync()
    {
      UserModel user = _db.User.Where(u => u.ID == new Guid(User.FindFirst(ClaimTypes.NameIdentifier).Value)).FirstOrDefault();

      var filename = @"‹berzeitkontrolle_" + user.Department + ".xlsx";

      await ExcelHandler.Create‹berzeitkontrolle(user.Department, _db);
      return File(System.IO.File.ReadAllBytes(@"C:\temp\" + filename), "application/vnd.ms-excel", filename);
    }
  }
}