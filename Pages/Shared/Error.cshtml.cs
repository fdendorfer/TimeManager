using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TimeManager.Pages
{
  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public class ErrorPageModel : PageModel
  {
    public IActionResult OnGet()
    {
      Console.Error.WriteLine("Error occured on traceidentifier: " + Activity.Current?.Id ?? HttpContext.TraceIdentifier);
      return Page();
    }
  }
}
