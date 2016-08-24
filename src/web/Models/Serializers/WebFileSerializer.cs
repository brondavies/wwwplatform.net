using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wwwplatform.Models.Serializers
{
    public class WebFileSerializer : AuditableSerializer
    {
        public WebFileSerializer() :base()
        {
            OmittedProperties = OmittedProperties.Concat(new string[] { "Location" });
        }
    }
}
