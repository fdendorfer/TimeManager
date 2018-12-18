using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TimeManager.Database;
using TimeManager.Model;

namespace TourManager.Pages {
  public class IndexModel : PageModel {
    [BindProperty]
    public LoginModel Login { get; set; } = new LoginModel();

    public async Task<IActionResult> OnPostAsync() {
      if (ModelState.IsValid) 
        { 
        using (var db = new DatabaseContext()) {
          var user = db.User.FirstOrDefault(u => u.Username == Login.Username && u.MatchesPassword(Login.Password));
          if (user != null) {
            //Login successful
            var permission = db.Permission.FirstOrDefault(p => p.ID == user.IdPermission);

            var claims = new List<Claim> {
              new Claim(ClaimTypes.Name, user.Username),
              new Claim(ClaimTypes.Role, permission.Description)
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
}
