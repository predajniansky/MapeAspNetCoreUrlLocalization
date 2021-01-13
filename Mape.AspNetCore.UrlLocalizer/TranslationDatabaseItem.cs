using System.Collections.Generic;

namespace Mape.AspNetCore.UrlLocalizer
{
  /// <summary>
  /// Dictionary item for translation
  /// </summary>
  public class TranslationDatabaseItem
  {
    /// <summary>
    /// Cultrue
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// Translated name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Original name
    /// </summary>
    public string OriginalName { get; set; }

    /// <summary>
    /// determine if translated name is without controoler in URL
    /// for example 
    ///   IsFullName=false http://www.exaple.com/home/contact 
    ///   IsFullName=true http://www.exaple.com/contact 
    /// </summary>
    public bool IsFullName { get; set; }

    /// <summary>
    /// Child items (actions for controller)
    /// </summary>
    public List<TranslationDatabaseItem> Items { get; set; }
  }
}
