using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace Mape.AspNetCore.UrlLocalizer
{
  /// <summary>
  /// Resolve original nema of controller and action from URL
  /// </summary>
  public class TranslationTransformer : DynamicRouteValueTransformer
  {
    private readonly ITranslationDatabase _translationDatabase;

    public TranslationTransformer(ITranslationDatabase translationDatabase)
    {
      _translationDatabase = translationDatabase;
    }

    public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
    {
      if (!values.ContainsKey("controller"))
        return values;      

      var controller = values.ContainsKey("controller") ? (string)values["controller"] : string.Empty;
      var action = values.ContainsKey("action") ? (string)values["action"] : string.Empty;
      var area = values.ContainsKey("area") ? (string)values["area"] : string.Empty;

      //get culture
      var language = _translationDatabase.DefaultCulture;
      if (values.ContainsKey("culture"))
        language = (string)values["culture"];

      //translate strigns
      var trans = await _translationDatabase.Resolve(language, area, controller, action);

      if (trans != null)
      {
        if (values.ContainsKey("area"))
          values["area"] = trans[0];

        if (values.ContainsKey("controller"))
          values["controller"] = trans[1];

        if (values.ContainsKey("action"))
          values["action"] = trans[2];
      }

      return values;
    }
  }
}