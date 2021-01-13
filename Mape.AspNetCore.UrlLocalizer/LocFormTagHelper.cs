using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace Mape.AspNetCore.UrlLocalizer
{
  /// <summary>
  /// overrides default tag helper for form tag
  /// automatically translate action to current language
  /// </summary>
  [HtmlTargetElement("form")]
  public class LocFormTagHelper : FormActionTagHelper
  {
    private const string ActionCultureName = "asp-culture";

    private ITranslationDatabase _translationDatabase;

    public LocFormTagHelper(IUrlHelperFactory urlHelperFactory, ITranslationDatabase translationDatabase) : base(urlHelperFactory)
    {
      _translationDatabase = translationDatabase;
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
      if (!string.IsNullOrEmpty(Controller) && !string.IsNullOrEmpty(Action))
      {
        var culture = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

        var trans = _translationDatabase.GetTranslation(culture, Area, Controller, Action).Result;

        Area = trans[0];
        Controller = trans[1];
        Action = trans[2];

        var href = output.Attributes["action"];
        if (href != null)
          output.Attributes.Remove(href);

        base.Process(context, output);
      }
      else
      {
        base.Process(context, output);
      }
    }
  }
}
