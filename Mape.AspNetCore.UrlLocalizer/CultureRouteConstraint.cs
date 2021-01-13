using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Mape.AspNetCore.UrlLocalizer
{
  /// <summary>
  /// Check language in URL address.
  /// </summary>
  public class CultureRouteConstraint : IRouteConstraint
  {
    private ITranslationDatabase _translationDatabase;

    public CultureRouteConstraint(ITranslationDatabase translationDatabase) : base()
    {
      _translationDatabase = translationDatabase;
    }

    public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
    {
      if (!values.ContainsKey("culture"))
      {
        return false;
      }

      var lang = values["culture"].ToString();

      //povolene jazyky
      foreach (var culture in _translationDatabase.Cultures)
        if (culture.TwoLetterISOLanguageName.Equals(lang, System.StringComparison.InvariantCultureIgnoreCase))
          return true;

      return false;
    }
  }
}
