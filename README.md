# MapeAspNetCoreUrlLocalization

The package allows you to locate URLs. The language is automatically detected from the URL.
</br>
</br>
For example yourweb/home/contact is translated to
* yourweb/contact 
* yourweb/sk/kontakt 
* yourweb/es/contacto 
</br> 
Example contains complete project with url and view localization


## Quick Start ASP.NET Core 3.0

Modify Startup.cs
```
    public void ConfigureServices(IServiceCollection services)
    {
      List<CultureInfo> cultures = new List<CultureInfo> {
        new CultureInfo("en"), //default language
        new CultureInfo("sk"),
        new CultureInfo("es"),
      };

      services.AddSingleton<ITranslationDatabase>(p => new TranslationDatabase(cultures));
      services.AddSingleton<TranslationTransformer>();

      //directory with resource files
      services.AddLocalization(options => options.ResourcesPath = "Resources");

      //configure language
      services.Configure<RequestLocalizationOptions>(options =>
      {
        //default language
        options.DefaultRequestCulture = new RequestCulture(cultures[0].TwoLetterISOLanguageName);
        options.SupportedCultures = cultures;
        options.SupportedUICultures = cultures;
        //nastavi provdier na zistenie jazyka z URL
        options.RequestCultureProviders.Clear();
        options.RequestCultureProviders = new[] { new UrlRequestCultureProvider(cultures) { } };
      });

      //check language v URL adrese
      services.Configure<RouteOptions>(options =>
      {
        options.ConstraintMap.Add("culture", typeof(CultureRouteConstraint));
      });

      services.AddControllersWithViews()
         .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }
      app.UseHttpsRedirection();
      app.UseStaticFiles();

      //setup localization
      var localizeOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
      app.UseRequestLocalization(localizeOptions.Value);

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        //setup localized route
        //with culture in URL
        endpoints.MapDynamicControllerRoute<TranslationTransformer>("{culture:culture}/{controller=Home}/{action=Index}/{id?}");

        //without culture in URL - defult culture is used
        endpoints.MapDynamicControllerRoute<TranslationTransformer>("{controller=Home}/{action=Index}/{id?}");

        //setup standard routes
        //with culture in URL
        endpoints.MapControllerRoute(
          name: "defaultc",
          pattern: "{culture:culture}/{controller=Home}/{action=Index}/{id?}");

        //without culture in URL - defult culture is used
        endpoints.MapControllerRoute(
          name: "default",
          pattern: "{controller=Home}/{action=Index}/{id?}");
      });
    }

```
Add translation to Controller - add LocalizedName atribure
```
  [LocalizedName(Language = "sk", Name = "uvod")]
  [LocalizedName(Language = "es", Name = "introduction")]
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
      _logger = logger;
    }

    public IActionResult Index()
    {
      return View();
    }

    [LocalizedName(Language = "sk", Name = "sukromie")]
    [LocalizedName(Language = "es", Name = "intimidad")]
    public IActionResult Privacy()
    {
      return View();
    }

    [LocalizedName(Language = "en", Name = "contact", IsFullName = true)]
    [LocalizedName(Language = "sk", Name = "kontakt", IsFullName = true)]
    [LocalizedName(Language = "es", Name = "contacto", IsFullName = true)] 
    public IActionResult Contact()
    {
      
      return View();
    }
```
Add tag helper to _ViewImports.cshtml
```
@addTagHelper *, Mape.AspNetCore.UrlLocalizer
