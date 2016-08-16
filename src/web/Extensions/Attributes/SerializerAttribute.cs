using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace wwwplatform.Extensions.Attributes
{
    public class SerializerAttribute : ActionFilterAttribute
    {
        public Type Serializer;
        public SerializerAttribute(Type serializer) : base()
        {
            Serializer = serializer;
        }
    }
}