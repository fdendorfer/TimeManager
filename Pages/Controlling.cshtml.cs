using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TimeManager.Database;
using TimeManager.Model;

namespace TimeManager.Pages
{
  public class ControllingPageModel : PageModel
  {
    public void OnPost() {
      Console.WriteLine("");
    }
  }
}