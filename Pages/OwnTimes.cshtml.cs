using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TimeManager.Model;

namespace TourManager.Pages
{
  public class OwnTimesModel : PageModel
  {    
    [BindProperty]
    public AbsenceModel Absence { get; set; } = new AbsenceModel();
    public OvertimeModel Overtime { get; set; } = new OvertimeModel();
    
    public void OnPost() 
    {
      Console.WriteLine("Catch All");
    }

    public void OnPostAbsence()
    {
      Console.WriteLine("OnPostAbsence");
    }
    public void OnPostOvertime()
    {

    }
  }
}
