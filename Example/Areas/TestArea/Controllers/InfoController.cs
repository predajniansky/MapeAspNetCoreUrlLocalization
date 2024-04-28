using Mape.AspNetCore.UrlLocalizer;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.Areas.TestArea.Controllers
{
  [Area("TestArea")]
  [LocalizedName(Language = "sk", Name = "areaInfoSk")]
  [LocalizedName(Language = "es", Name = "areaInfoEn")]
  public class InfoController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}
