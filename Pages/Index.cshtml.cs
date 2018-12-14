using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TimeManager.Model;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace TourManager.Pages {
  public class IndexModel : PageModel {
    [BindProperty]
    public LoginModel Login { get; set; } = new LoginModel();

    public async Task<IActionResult> OnPostAsync() {
      if (ModelState.IsValid) {

        using (var db = new DatabaseContext()) {
          var user = db.User.FirstOrDefault(u => u.Username == Login.Username && u.MatchesPassword(Login.Password));
          if (user != null) {
            //Login successful
            var permission = db.Permission.FirstOrDefault(p => p.ID == user.IdPermission);
            
            var claims = new List<Claim> {
              new Claim("Username", user.Username),
              new Claim("Permission", permission.Description)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties); // To log out await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToPage("/OwnTimes");
          }
        }
      }
      return Page();
    }
  }

  public class LoginModel : IdentityUser {
    [PersonalData]
    [Required(ErrorMessage = "Benutzername muss ausgefüllt werden")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Der Benutzername muss zwischen 2 und 100 Zeichen lang sein")]
    [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Der Benutzername darf nur Buchstaben und Zahlen enthalten")]
    public string Username { get; set; }

    [PersonalData]
    [Required(ErrorMessage = "Passwort muss ausgefüllt werden")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Das Passwort muss zwischen 3 und 100 Zeichen lang sein")]
    [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Das Passwort darf nur Buchstaben und Zahlen enthalten")]
    public string Password { get; set; }
  }
}
