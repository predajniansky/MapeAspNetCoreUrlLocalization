using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Mape.AspNetCore.UrlLocalizer
{
  /// <summary>
  /// Dictionary for URL translations
  /// </summary>
  public interface ITranslationDatabase
  {
    /// <summary>
    /// List of available cultures
    /// </summary>
    List<CultureInfo> Cultures { get; }

    /// <summary>
    /// Default culture
    /// </summary>
    string DefaultCulture { get; }

    /// <summary>
    /// Get translated strings
    /// </summary>
    /// <param name="lang">culture</param>
    /// <param name="area">original area</param>
    /// <param name="controller">original controller</param>
    /// <param name="action">original action</param>
    /// <returns>array of translated strings [area, controller, action]</returns>
    Task<string[]> GetTranslation(string lang, string area, string controller, string action);

    /// <summary>
    /// Resolv original strings from translated strings
    /// </summary>
    /// <param name="lang">culture</param>
    /// <param name="area">translated area</param>
    /// <param name="controller">translated culture</param>
    /// <param name="action">translated action</param>
    /// <returns>arry of original strings [area, controller, action]</returns>
    Task<string[]> Resolve(string lang, string area, string controller, string action, ICollection<string> queryPrms);
  }
}