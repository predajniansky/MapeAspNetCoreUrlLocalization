using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mape.AspNetCore.UrlLocalizer
{
  public static class HtmlHelperLinkExtensions
  {
    //
    // Summary:
    //     Returns an anchor (<a>) element that contains a URL path to the specified action.
    //
    // Parameters:
    //   helper:
    //     The Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper instance this method extends.
    //
    //   linkText:
    //     The inner text of the anchor element. Must not be null.
    //
    //   actionName:
    //     The name of the action.
    //
    //   controllerName:
    //     The name of the controller.
    //
    //   routeValues:
    //     An System.Object that contains the parameters for a route. The parameters are
    //     retrieved through reflection by examining the properties of the System.Object.
    //     This System.Object is typically created using System.Object initializer syntax.
    //     Alternatively, an System.Collections.Generic.IDictionary`2 instance containing
    //     the route parameters.
    //
    //   htmlAttributes:
    //     An System.Object that contains the HTML attributes for the element. Alternatively,
    //     an System.Collections.Generic.IDictionary`2 instance containing the HTML attributes.
    //
    // Returns:
    //     A new Microsoft.AspNetCore.Html.IHtmlContent containing the anchor element.
    public static IHtmlContent ActionLink(this IHtmlHelper helper, ITranslationDatabase translationDatabase, string linkText, string actionName, string controllerName, object routeValues = null, object htmlAttributes = null)
    {
      if (!string.IsNullOrEmpty(controllerName) && !string.IsNullOrEmpty(actionName))
      {
        //check if default root Home/Index
        bool isDefaultRoute = "Home".Equals(controllerName, StringComparison.InvariantCultureIgnoreCase)
          && "Index".Equals(actionName, StringComparison.InvariantCultureIgnoreCase);

        if (!isDefaultRoute)
        {
          var culture = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

          var trans = translationDatabase.GetTranslation(culture, null, controllerName, actionName).Result;

          controllerName = trans[1];
          actionName = trans[2];
        }
      }

      return helper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes);
    }
  }
}
