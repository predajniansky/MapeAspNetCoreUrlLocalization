using System;

namespace Mape.AspNetCore.UrlLocalizer
{
  /// <summary>
  /// localization forr controller or action name
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
  public class LocalizedNameAttribute : Attribute
  {
    /// <summary>
    /// cultrue
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// translated name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// determine if translated name is without controoler in URL
    /// for example 
    ///   IsFullName=false http://www.exaple.com/home/contact 
    ///   IsFullName=true http://www.exaple.com/contact 
    /// </summary>
    public bool IsFullName { get; set; }
  }
}
