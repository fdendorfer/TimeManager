using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TimeManager.Database;
using TimeManager.Extensions;
using TimeManager.Model;

namespace TimeManager.Pages {
  public class IndexModel : PageModel {
    [BindProperty]
    public LoginValidation Login { get; set; } = new LoginValidation();
    
    public async void OnGetAsync()
    {
      await Test.Do();
    }

    public async Task<IActionResult> OnPostAsync() {
      // Is model validation successful
      if (ModelState.IsValid) {
        // using db for error fallback
        using (var db = new DatabaseContext()) {
          // Trying to get a user from db where username and password match
          var user = db.User.FirstOrDefault(u => u.Username == Login.Username && u.MatchesPassword(Login.Password) && u.Deactivated == false);
          // If a matching user is found
          if (user != null) {
            //Login successful

            // Getting the db users permission as string
            var permission = db.Permission.FirstOrDefault(p => p.ID == user.IdPermission);

            // List of properties, the authentication cookie will store
            var claims = new List<Claim> {
              new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
              new Claim(ClaimTypes.Name, user.Username),
              new Claim(ClaimTypes.Role, permission.Description)
            };

            // ClaimsIdentity assigns a scheme to claims
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            // Cookie specific properties
            var authProperties = new AuthenticationProperties {};
            // Creates cookie (and redirects to OwnTimes)
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties); // To log out await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Redirects to OwnTimes, if SignInAsync doesn't do it
            return RedirectToPage("/OwnTimes");
          } else {
            // Show error, if no matching db user was found
            ModelState.AddModelError(string.Empty, "Benutzername oder Passwort ist falsch");
            return Page();
          }
        }
      }
      // If there are data annotation errors in the model, they will be added to ModelErrors to show in ValidationSummary
      foreach (var item in ModelState.Values.Reverse().Where(v => v.Errors != null)) {
        foreach (var item2 in item.Errors) 
        {
          ModelState.AddModelError(string.Empty, item2.ErrorMessage);
        }
      }
      return Page();
    }
  }
}
