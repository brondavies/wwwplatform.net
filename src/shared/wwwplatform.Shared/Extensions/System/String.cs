using System;
using System.Text;
using System.Web;

namespace wwwplatform.Shared.Extensions.System
{
    public static class String
    {
        /// <summary>
        /// Converts a file path into an app-relative URI string
        /// </summary>
        /// <returns>A new string with the absolute URL</returns>
        public static string ToAppPath(this string filename, HttpContextBase context = null)
        {
            if (filename == null) { filename = "";  }
            string appPath = (context ?? new HttpContextWrapper(HttpContext.Current)).Server.MapPath("~");
            return filename.Replace(appPath, "").Replace("\\", "/");
        }

        /// <summary>
        /// Changes the url fragment or virtual path into an app-relative URI string
        /// </summary>
        /// <returns>A new string with the http(s): protocol prefixed</returns>
        public static string ResolveUrl(this string url, HttpContextBase context = null)
        {
            return (context ?? new HttpContextWrapper(HttpContext.Current)).Response.ApplyAppPathModifier(url);
        }

        /// <summary>
        /// Resolves the string as a relative url and applies an http: or https: protocol to the url if it needs it
        /// </summary>
        /// <param name="makeSsl">Whether to force the https: protocol to be used</param>
        /// <returns>A new string with the http(s): protocol, server, and port prefixed if needed to the resolved path</returns>
        public static string ToAbsoluteUrl(this string url, HttpContextBase context = null, bool makeSsl = false)
        {
            if (url == null) return null;
            if (url.StartsWith("http:") || url.StartsWith("https:"))
            {
                return url;
            }

            var httpContext = context ?? new HttpContextWrapper(HttpContext.Current);
            string newUrl = url;
            string scheme = makeSsl ? "https://" : httpContext.Request.Url.GetLeftPart(UriPartial.Scheme);
            string server = httpContext.Request.Url.Host;
            int port = httpContext.Request.Url.Port;
            if (port > 0 && port != 80 && port != 443)
            {
                server = server + ":" + port;
            }
            string rightPart = url.ResolveUrl(context);

            newUrl = scheme + server + rightPart;

            return newUrl;
        }

        /// <summary>
        /// Applies an http: or https: protocol to the string if it needs it
        /// </summary>
        /// <param name="useSsl">Whether to use https: if http and https are not found</param>
        /// <returns>A new string with the http: protocol prefixed</returns>
        public static string MakeHttp(this string url, bool useSsl = false)
        {
            if (!string.IsNullOrEmpty(url))
            {
                string protocol = useSsl ? "https:" : "http:";
                return url.StartsWith("http") ? url :
                    protocol + (url.StartsWith("//") ? url :
                        "//" + url);
            }
            return url;
        }

        /// <summary>
        /// Analyzes a url and resolves it to the local file system path
        /// </summary>
        /// <returns>A new string representing the local file system path of the url</returns>
        public static string ResolveLocalPath(this string url, HttpContextBase context = null)
        {
            var httpContext = context ?? new HttpContextWrapper(HttpContext.Current);
            return httpContext.Server.MapPath(
                httpContext.Response.ApplyAppPathModifier(url)
            );
        }

        /// <summary>
        /// Decodes and returns the original value of a base64-encoded string
        /// </summary>
        /// <returns>A new string representing the original value of a base64-encoded string</returns>
        public static string FromBase64String(this string value)
        {
            if (string.IsNullOrEmpty(value)) { return value; }
            return Encoding.ASCII.GetString(Convert.FromBase64String(value));
        }

        /// <summary>
        /// Returns the base64-encoded representation of the string
        /// </summary>
        /// <param name="options"></param>
        /// <returns>A new string representing the base64-encoded value of the string</returns>
        public static string ToBase64String(this string value, Base64FormattingOptions options = Base64FormattingOptions.None)
        {
            if (string.IsNullOrEmpty(value)) { return value; }
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(value), options);
        }
    }
}
