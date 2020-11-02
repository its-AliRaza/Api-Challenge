using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ApiChallenge.Helpers
{
    public class Utils
    {
        public static string connStr = ConfigurationManager.ConnectionStrings["Challenge"].ConnectionString;
    }
}