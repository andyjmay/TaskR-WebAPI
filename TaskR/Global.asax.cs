using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace TaskR {
  public class MvcApplication : System.Web.HttpApplication {
    public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
      filters.Add(new HandleErrorAttribute());
    }

    public static void RegisterRoutes(RouteCollection routes) {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      routes.MapHttpRoute(
          name: "DefaultApi",
          routeTemplate: "api/{controller}/{id}",
          defaults: new { id = RouteParameter.Optional }
      );

      routes.MapRoute(
          name: "Default",
          url: "{controller}/{action}/{id}",
          defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
      );
    }

    protected void Application_Start() {
      AreaRegistration.RegisterAllAreas();

      // Use LocalDB for Entity Framework by default
      Database.DefaultConnectionFactory = new SqlConnectionFactory("Data Source=(localdb)\v11.0; Integrated Security=True; MultipleActiveResultSets=True");

      RegisterGlobalFilters(GlobalFilters.Filters);
      RegisterRoutes(RouteTable.Routes);

      IBundleTransform javaScriptTransform;
      IBundleTransform cssTransform;
      
      #if DEBUG
        javaScriptTransform = new NoTransform("text/javascript");
        cssTransform = new NoTransform("text/css");
      #else
        javaScriptTransform = new JsMinifiy();
        cssTransform = new CssMinify();
      #endif

      var scriptBundle = new Bundle("~/ScriptBundle", javaScriptTransform);
      scriptBundle.AddFile("~/Scripts/jquery-1.7.2.js");
      scriptBundle.AddFile("~/Scripts/modernizr-2.5.3.js");
      scriptBundle.AddFile("~/Scripts/jquery.validate.js");
      scriptBundle.AddFile("~/Scripts/jquery.validate.unobtrusive.js");
      scriptBundle.AddFile("~/Scripts/bootstrap.js");
      BundleTable.Bundles.Add(scriptBundle);

      var cssBundle = new Bundle("~/Content/CssBundle", cssTransform);
      cssBundle.AddFile("~/Content/bootstrap.css");
      cssBundle.AddFile("~/Content/bootstrap-responsive.css");
      cssBundle.AddFile("~/Content/Site.css");
      BundleTable.Bundles.Add(cssBundle);

      //BundleTable.Bundles.RegisterTemplateBundles();
    }
  }
}