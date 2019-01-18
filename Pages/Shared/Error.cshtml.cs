using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TimeManager.Pages
{
  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public class ErrorPageModel : PageModel
  {
    public string RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public IActionResult OnGet()
    {
      RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
      return Page();
    }
  }
}
