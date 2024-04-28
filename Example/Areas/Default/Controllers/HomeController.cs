using Example.Models;
using Mape.AspNetCore.UrlLocalizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Example.Areas.DefaultArea.Controllers
{
  [Area("Default")]
  [LocalizedName(Language = "sk", Name = "uvod")]
  [LocalizedName(Language = "es", Name = "introduction")]
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
      _logger = logger;
    }

    public IActionResult Index()
    {
      return View();
    }

    [LocalizedName(Language = "sk", Name = "sukromie")]
    [LocalizedName(Language = "es", Name = "intimidad")]
    public IActionResult Privacy()
    {
      return View();
    }

    [LocalizedName(Language = "en", Name = "contact", IsFullName = true)]
    [LocalizedName(Language = "sk", Name = "kontakt", IsFullName = true)]
    [LocalizedName(Language = "es", Name = "contacto", IsFullName = true)] 
    public IActionResult Contact()
    {
      
      return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
