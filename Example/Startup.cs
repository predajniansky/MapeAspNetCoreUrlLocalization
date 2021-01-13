using Mape.AspNetCore.UrlLocalizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Example
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
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
  }
}
