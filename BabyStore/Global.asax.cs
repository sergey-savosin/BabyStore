﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace BabyStore
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // custom CultureInfo for editing decimals in Price fields
            var newCulture = (CultureInfo)CultureInfo.CreateSpecificCulture("En-us");// .CurrentCulture.Clone();
            newCulture.NumberFormat.NumberGroupSeparator = "~";

            System.Threading.Thread.CurrentThread.CurrentCulture = newCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = newCulture;
        }
    }
}
