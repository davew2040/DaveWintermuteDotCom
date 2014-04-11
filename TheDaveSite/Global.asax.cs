using DataAccess;
using DataAccess.Models;
using DataAccess.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TheDaveSite.Models;
using WebMatrix.WebData;

namespace TheDaveSite
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            using (var db = new DaveAppContext())
            {
                db.Database.CreateIfNotExists();
                var poke = db.UserProfiles.FirstOrDefault();
            }


            if (!WebSecurity.Initialized)
            {
                WebSecurity.InitializeDatabaseConnection(DatabaseUtils.CurrentDataDatabaseConnectionString, "UserProfile", "UserId", "UserName", autoCreateTables: true);
            }

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }
    }
}