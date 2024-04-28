using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Mape.AspNetCore.UrlLocalizer
{
  /// <summary>
  /// Dictionary for URL translations
  /// </summary>
  public class TranslationDatabase : ITranslationDatabase
  {
    public TranslationDatabase(List<CultureInfo> cultures)
    {
      Cultures = cultures;
    }

    /// <summary>
    /// Dictionary of translated strings
    /// </summary>
    private static List<TranslationDatabaseItem> translationDict;

    /// <summary>
    /// Extract translated controller and action names
    /// </summary>
    /// <returns></returns>
    private static List<TranslationDatabaseItem> FillTranstationDictionary()
    {
      if (translationDict == null)
      {
        translationDict = new List<TranslationDatabaseItem>();

        Assembly asm = Assembly.GetEntryAssembly();

        var controllers = asm.GetTypes()
          .Where(type => typeof(Microsoft.AspNetCore.Mvc.Controller).IsAssignableFrom(type)).Select(p => p);

        foreach (var controller in controllers)
        {
          //add controller translated names
          var cName = controller.Name.ToLowerInvariant().Substring(0, controller.Name.Length - 10);

          var attrs = controller.GetCustomAttributes<LocalizedNameAttribute>();
          if (attrs != null)
          {
            foreach (var attr in attrs)
            {
              translationDict.Add(new TranslationDatabaseItem()
              {
                Language = attr.Language.ToLowerInvariant(),
                Name = attr.Name.ToLowerInvariant(),
                OriginalName = cName
              });
            }
          }

          //add action translated name
          foreach (var method in controller.GetMembers())
          {
            attrs = method.GetCustomAttributes<LocalizedNameAttribute>();
            if (attrs != null)
            {
              foreach (var attr in attrs)
              {
                //action item
                var mItem = new TranslationDatabaseItem()
                {
                  Language = attr.Language.ToLowerInvariant(),
                  Name = string.IsNullOrEmpty(attr.Name) ? string.Empty : attr.Name.ToLowerInvariant(),
                  IsFullName = attr.IsFullName,
                  OriginalName = method.Name.ToLowerInvariant()
                };

                //controller item
                var cItem = translationDict.FirstOrDefault(p => p.Language.Equals(mItem.Language) && p.OriginalName.Equals(cName));

                //add not translated if not exist
                if (cItem == null)
                {
                  cItem = new TranslationDatabaseItem()
                  {
                    Language = mItem.Language,
                    Name = cName,
                    OriginalName = cName
                  };
                  translationDict.Add(cItem);
                }

                if (cItem.Items == null)
                  cItem.Items = new List<TranslationDatabaseItem>();

                cItem.Items.Add(mItem);
              }
            }
          }
        }
      }

      return translationDict;
    }

    /// <summary>
    /// List of available cultures
    /// </summary>
    public List<CultureInfo> Cultures { get; private set; }

    /// <summary>
    /// Default culture
    /// </summary>
    public string DefaultCulture
    {
      get
      {
        return Cultures[0].TwoLetterISOLanguageName;
      }
    }

    /// <summary>
    /// Get translated strings
    /// </summary>
    /// <param name="lang">culture</param>
    /// <param name="area">original area</param>
    /// <param name="controller">original controller</param>
    /// <param name="action">original action</param>
    /// <returns>array of translated strings [area, controller, action]</returns>
    public async Task<string[]> Resolve(string lang, string area, string controller, string action, ICollection<string> queryPrms)
    {
      string locController = controller;
      string locAction = action;
      string locArea = area;

      var normalizedLang = Cultures[0].TwoLetterISOLanguageName;

      if (!string.IsNullOrEmpty(lang))
        normalizedLang = lang.ToLowerInvariant();

      if (!string.IsNullOrEmpty(controller))
      {
        var trans = FillTranstationDictionary();

        var normalizedController = controller.ToLowerInvariant();

        var cTrans = trans.FirstOrDefault(p => p.Language.Equals(normalizedLang) && p.Name.Equals(normalizedController));
        if (cTrans != null)
        {
          locController = cTrans.OriginalName;

          if (!string.IsNullOrEmpty(action) && cTrans.Items != null)
          {
            var normalizedAction = action.ToLowerInvariant();

            var aItem = cTrans.Items.FirstOrDefault(p => !p.IsFullName && p.Name.Equals(normalizedAction));
            if (aItem != null)
            {
              locAction = aItem.OriginalName;
            }

          }

        }
        else if (string.IsNullOrEmpty(action) || action.Equals("Index", StringComparison.InvariantCultureIgnoreCase))
        {
          foreach (var cItem in trans.Where(p => p.Language.Equals(normalizedLang) && p.Items != null).ToList())
          {
            var aItem = cItem.Items.FirstOrDefault(p => p.IsFullName && p.Name.Equals(normalizedController));

            if (aItem != null)
            {
              locController = cItem.OriginalName;
              locAction = aItem.OriginalName;

              break;
            }
          }
        }

      }

      return await Task.FromResult(new string[] { locArea, locController, locAction });
    }

    /// <summary>
    /// Resolv original strings from translated strings
    /// </summary>
    /// <param name="lang">culture</param>
    /// <param name="area">translated area</param>
    /// <param name="controller">translated culture</param>
    /// <param name="action">translated action</param>
    /// <returns>arry of original strings [area, controller, action]</returns>
    public async Task<string[]> GetTranslation(string lang, string area, string controller, string action)
    {
      string locArea = area;
      string locController = controller;
      string locAction = action;

      if (!string.IsNullOrEmpty(lang) && !string.IsNullOrEmpty(controller))
      {
        var trans = FillTranstationDictionary();

        var normalizedLang = lang.ToLowerInvariant();
        var normalizedController = controller.ToLowerInvariant();

        var cTrans = trans.FirstOrDefault(p => p.Language.Equals(normalizedLang) && p.OriginalName.Equals(normalizedController));

        if (cTrans != null)
        {
          locController = cTrans.Name;

          if (cTrans.Items != null && !string.IsNullOrEmpty(action))
          {
            var normalizedAction = action.ToLowerInvariant();

            cTrans = cTrans.Items.FirstOrDefault(p => p.OriginalName.Equals(normalizedAction));
            if (cTrans != null)
            {
              if (cTrans.IsFullName)
              {
                locController = cTrans.Name;
                locAction = "Index";
              }
              else
              {
                locAction = cTrans.Name;
              }
            }
          }
        }
      }
      return await Task.FromResult(new string[] { locArea, locController, locAction });
    }
  }
}
