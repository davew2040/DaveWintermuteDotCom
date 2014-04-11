using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TheDaveSite.Utils
{
    public class MiscHelpers
    {
        public static string GetAppUrl(HttpRequestBase request, UrlHelper url)
        {
            return string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, url.Content("~"));
        }

        public static string getApplicationHost(HttpRequestBase request)
        {
            return request.Url.Scheme
                + "://"
                + request.Url.Authority
                + request.ApplicationPath;
        }
    }
}