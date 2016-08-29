using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wwwplatform.Models.Serializers
{
    public class ApplicationUserSerializer : DynamicContractResolver
    {
        public ApplicationUserSerializer()
            : base(included: new[] { "Id", "FirstName", "LastName", "UserName" }) { }
    }
}