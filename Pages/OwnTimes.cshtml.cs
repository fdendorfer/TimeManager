using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TourManager.Pages
{
  public class OwnTimesModel : PageModel
  {
    public static string[] OvertimeDetail = new string[] { "Werktag", "Samstag", "Sonntag" };
    
    public void OnGet()
    {

    }
  }
}
