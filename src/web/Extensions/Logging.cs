using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wwwplatform.Extensions.Logging
{
    public class Log
    {
        public static Elmah.ErrorLog elmah
        {
            get
            {
                return Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current);
            }
        }

        public static string Error(Exception exception)
        {
            try
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);
                return elmah.Log(new Elmah.Error(exception));
            }
            catch
            {

            }
            return null;
        }

        public static string Error(string message, params object[] args)
        {
            try
            {
                Exception exception;
                if (args.Length == 1)
                {
                    exception = new Exception(string.Format(message,
                        JsonConvert.SerializeObject(args[0], Formatting.Indented,
                            new JsonSerializerSettings()
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                CheckAdditionalContent = false,
                                TypeNameHandling = TypeNameHandling.None
                            })
                        )
                    );
                }
                else
                {
                    exception = new Exception(string.Format(message, args));
                }
                return Error(exception);
            }
            catch { }
            return string.Empty;
        }
    }
}