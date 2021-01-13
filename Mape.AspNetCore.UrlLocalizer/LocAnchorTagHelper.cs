using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace Mape.AspNetCore.UrlLocalizer
{
  /// <summary>
  /// overrides default tag helper for a tag
  /// automatically translate controller and action to current languge
  /// </summary>
  [HtmlTargetElement("a")]
  [HtmlTargetElement("a", Attributes = ActionCultureName)]

  public class LocAnchorTagHelper : AnchorTagHelper
  {
    private const string ActionCultureName = "asp-culture";

    private ITranslationDatabase _translationDatabase;

    public LocAnchorTagHelper(IHtmlGenerator generator, ITranslationDatabase translationDatabase) : base(generator)
    {
      _translationDatabase = translationDatabase;
    }

    [HtmlAttributeName(ActionCultureName)]
    public string Culture { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
      output.TagName = "a";

      if (!string.IsNullOrEmpty(Controller) || !string.IsNullOrEmpty(Action))
      {
        var culture = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

        //check if default root Home/Index
        bool isDefaultRoute = "Home".Equals(Controller, StringComparison.InvariantCultureIgnoreCase)
          && "Index".Equals(Action, StringComparison.InvariantCultureIgnoreCase);

        if (!isDefaultRoute)
        {
          var trans = _translationDatabase.GetTranslation(culture, Area, Controller, Action).Result;

          Area = trans[0];
          Controller = trans[1];
          Action = trans[2];
        }

        var href = output.Attributes["href"];
        if (href != null)
          output.Attributes.Remove(href);

        if (isDefaultRoute && !string.IsNullOrEmpty(Culture))
        {
          base.Process(context, output);

          href = output.Attributes["href"];
          output.Attributes.Remove(href);
          if (Culture.Equals(_translationDatabase.DefaultCulture, StringComparison.InvariantCultureIgnoreCase))
            output.Attributes.Add(new TagHelperAttribute("href", ViewContext.HttpContext.Request.PathBase + "/"));
          else
            output.Attributes.Add(new TagHelperAttribute("href", ViewContext.HttpContext.Request.PathBase + "/" + Culture));
        }
        else
        {
          base.Process(context, output);
        }
      }
      else
      {
        base.Process(context, output);
      }
    }
  }
}
