using AsyncDemoWebAppComplete.App_Start;
using System;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;

namespace AsyncDemoWebAppComplete
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.DefaultConnectionLimit = 200;

            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}