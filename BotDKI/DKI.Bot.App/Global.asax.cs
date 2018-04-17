using DKI.Bot.App.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace DKI.Bot.App
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            //ObjectContainer.Register<RedisDB>(new RedisDB (ConfigurationManager.AppSettings["RedisCon"]));
        }
    }
}
