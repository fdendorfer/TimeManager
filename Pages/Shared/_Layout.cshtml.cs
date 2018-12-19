using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TimeManager.Pages.Shared {
  public class _Layout : PageModel {
    private static HttpContext _httpContext;

    public static async void Logout() {
      await _Layout._httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
    public _Layout() {
      _httpContext = HttpContext;
    }
  }
}
