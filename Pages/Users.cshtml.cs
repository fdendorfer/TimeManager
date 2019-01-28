using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TimeManager.Database;
using TimeManager.Model;

namespace TimeManager.Pages
{
  public class UsersPageModel : PageModel
  {
    // Local variables
    public readonly DatabaseContext _db;
    public List<SelectListItem> departments = new List<SelectListItem>();
    public List<SelectListItem> permissions;
    public List<UserModel> Users;
    // Field used for validation
    [BindProperty]
    public UserValidation ValUser { get; set; } = new UserValidation();

    public UsersPageModel(DatabaseContext db)
    {
      _db = db;
    }

    // Set id for single user and onlyActive json of all users
    public IActionResult OnGet([FromQuery] string id, [FromQuery] string onlyActive)
    {
      if (id == null && onlyActive == null)
        return Page();

      if (onlyActive != null)
      {
        Users = (from u in _db.User
                 select u).ToList();
        if (onlyActive == "true")
          Users = Users.Where(u => u.Deactivated == false).ToList();

        return new JsonResult(Users);
      }

      Thread.CurrentThread.CurrentCulture = new CultureInfo("de-CH");
      var user = (from u in _db.User
                  where u.ID == new Guid(id)
                  select u).Single();

      // Fillin the userPartial with existing values
      ValUser.ID = user.ID.ToString();
      ValUser.IdPermission = user.IdPermission.ToString();
      ValUser.Username = user.Username;
      ValUser.Firstname = user.Firstname;
      ValUser.Lastname = user.Lastname;
      ValUser.Department = user.Department;
      ValUser.Holidays = user.Holidays.ToString();
      ValUser.Deactivated = user.Deactivated;

      return new JsonResult(JsonConvert.SerializeObject(ValUser));
    }

    // New or update user
    public IActionResult OnPost()
    {
      var isValid = Validator.TryValidateObject(ValUser, new ValidationContext(ValUser, serviceProvider: null, items: null), new List<ValidationResult>(), true);

      if (isValid)
      {
        var user = new UserModel()
        {
          ID = ValUser.ID == null ? Guid.NewGuid() : new Guid(ValUser.ID),
          IdPermission = new Guid(ValUser.IdPermission),
          Firstname = ValUser.Firstname,
          Lastname = ValUser.Lastname,
          Username = ValUser.Username,
          Department = ValUser.Department,
          Holidays = Convert.ToDecimal(ValUser.Holidays),
          Deactivated = ValUser.Deactivated
        };
        var testUser = _db.User.AsNoTracking().SingleOrDefault(u => u.ID == user.ID);
        if (ValUser.Password == null && testUser != null)
        {
          user.Password = testUser.Password;
        } else
        {
          user.PasswordValue = ValUser.Password;
        }

        var record = _db.User.AsNoTracking().SingleOrDefault(u => u.ID == user.ID);
        if (record == null)
        {
          _db.User.Add(user);
        } else
        {
          record = user;
          _db.Update(record);
        }
        _db.SaveChanges();

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

    // Deactivate User
    public void OnDelete([FromQuery] string id)
    {
      _db.User.Find(new Guid(id)).Deactivated = true;
      _db.SaveChanges();
    }
  }
}
