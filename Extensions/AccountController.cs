using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace TimeManager.Extensions
{
  [Route("[controller]/[action]")]
  public class AccountController : Controller
  {
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
      await HttpContext.SignOutAsync();
      return RedirectToPage("/Index");
    }
  }
}