using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TimeManager.Models;

namespace TimeManager.Controllers
{
    public class OwnTimesController : Controller
    {
        [Route("OwnTimes")]
        [Route("OwnTimes/Index")]
        public IActionResult Index()
        {
            return View("OwnTimes");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
