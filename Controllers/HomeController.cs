using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using TimeManager.Models;
using TimeManager.Database;

namespace TimeManager.Controllers
{
  public class HomeController : Controller
  {
    [Route("")]
    [Route("Home")]
    [Route("Home/Index")]
    public IActionResult Index()
    {
      return View();
    }

    [Route("")]
    [Route("Home")]
    [Route("Home/Index")]
    [HttpPost]
    public IActionResult Index(UserModel user)
    {
      if (DBConnection.Login(user.Username, user.Password))
      {
        return new RedirectResult("/OwnTimes");
      }
      return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
