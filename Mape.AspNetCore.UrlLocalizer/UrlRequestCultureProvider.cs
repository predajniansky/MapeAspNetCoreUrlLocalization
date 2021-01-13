using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Mape.AspNetCore.UrlLocalizer
{
  /// <summary>
  /// extract current culture from URL
  /// </summary>
  public class UrlRequestCultureProvider : RequestCultureProvider
  {
    private List<CultureInfo> _cultures;

    public UrlRequestCultureProvider(List<CultureInfo> cultures) : base()
    {
      _cultures = cultures;
    }


    public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {
      await Task.Yield();
      var xx = httpContext.Request.Path.Value.Split('/');

      //set default culture if not defined in URL
      string culture = _cultures[0].TwoLetterISOLanguageName;

      //set cluture from URL
      if (xx.Length > 1)
      {
        foreach (var pom in _cultures)
          if (pom.TwoLetterISOLanguageName.Equals(xx[1], System.StringComparison.InvariantCultureIgnoreCase))
          {
            culture = pom.TwoLetterISOLanguageName;
            break;
          }
      }

  
      return new ProviderCultureResult(culture);
    }
  }
}
