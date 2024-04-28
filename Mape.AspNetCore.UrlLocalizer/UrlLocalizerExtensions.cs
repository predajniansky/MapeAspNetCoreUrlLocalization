using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Globalization;

namespace Mape.AspNetCore.UrlLocalizer
{
    public static class UrlLocalizerExtensions
    {
        public static void MapLocalizedRoute(this WebApplication app)
        {
            //nastavenie lokalizovanej URL
            app.MapDynamicControllerRoute<TranslationTransformer>("{culture:culture}/{controller=Home}/{action=Index}/{id?}");
            app.MapDynamicControllerRoute<TranslationTransformer>("{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                  name: "defaultc",
                  pattern: "{culture:culture}/{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        }

        public static IServiceCollection AddRouteLocalization(this IServiceCollection services, List<CultureInfo> cultures)
        {
            services.AddSingleton<ITranslationDatabase>(p => new TranslationDatabase(cultures));
            services.AddSingleton<TranslationTransformer>();

            //nastavenie jazyka
            services.Configure<RequestLocalizationOptions>(options =>
            {
                //default jazyk
                options.DefaultRequestCulture = new RequestCulture(cultures[0].TwoLetterISOLanguageName);
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
                //nastavenie provdier na zistenie jazyka z URL
                options.RequestCultureProviders.Clear();
                options.RequestCultureProviders = new[] { new UrlRequestCultureProvider(cultures) { } };
            });

            //kontrola jazyka v URL
            services.Configure<RouteOptions>(options =>
            {
                options.ConstraintMap.Add("culture", typeof(CultureRouteConstraint));
            });

            return services;
        }
    }
}
