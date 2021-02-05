using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mape.AspNetCore.UrlLocalizer
{
  public static class UrlHelperExtensions
  {
    //
    // Summary:
    //     Generates an absolute URL for an action method, which contains the specified
    //     action name, controller name, route values, protocol to use, host name, and fragment.
    //     Generates an absolute URL if the protocol and host are non-null. See the remarks
    //     section for important security information.
    //
    // Parameters:
    //   helper:
    //     The Microsoft.AspNetCore.Mvc.IUrlHelper.
    //
    //   translationDatabase:
    //     Dictionary for URL translations
    //
    //   action:
    //     The name of the action method. When null, defaults to the current executing action.
    //
    //   controller:
    //     The name of the controller. When null, defaults to the current executing controller.
    //
    //   values:
    //     An object that contains route values.
    //
    //   protocol:
    //     The protocol for the URL, such as "http" or "https".
    //
    //   host:
    //     The host name for the URL.
    //
    //   fragment:
    //     The fragment for the URL.
    //
    // Returns:
    //     The generated URL.
    //
    // Remarks:
    //     The value of host should be a trusted value. Relying on the value of the current
    //     request can allow untrusted input to influence the resulting URI unless the Host
    //     header has been validated. See the deployment documentation for instructions
    //     on how to properly validate the Host header in your deployment environment.
    public static string ActionLink(this IUrlHelper helper, ITranslationDatabase translationDatabase, string action = null,
      string controller = null, object values = null, 
      string protocol = null, string host = null, string fragment = null)
    {
      if (!string.IsNullOrEmpty(controller) && !string.IsNullOrEmpty(action))
      {
        //check if default root Home/Index
        bool isDefaultRoute = "Home".Equals(controller, StringComparison.InvariantCultureIgnoreCase)
          && "Index".Equals(action, StringComparison.InvariantCultureIgnoreCase);
        
        if (!isDefaultRoute)
        {
          var culture = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

          var trans = translationDatabase.GetTranslation(culture, null, controller, action).Result;

          controller = trans[1];
          action = trans[2];
        }
      }

      return helper.ActionLink( action ,
       controller ,  values ,
       protocol ,  host ,  fragment );
    }
  }
}
