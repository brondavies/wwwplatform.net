using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wwwplatform.Models.Serializers
{
    public class DynamicContractResolver : DefaultContractResolver
    {
        public IEnumerable<string> IncludedProperties = null;

        public IEnumerable<string> OmittedProperties = new string[] { "CreatedAt", "DeletedAt", "Permissions", "UpdatedBy" };

        public DynamicContractResolver(IEnumerable<string> included = null, IEnumerable<string> omitted = null):base()
        {
            IncludedProperties = included;
            if (omitted != null) OmittedProperties = omitted;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
            
            if (IncludedProperties != null)
            {
                properties = properties.Where(prop => IncludedProperties.Contains(prop.PropertyName)).ToList();
            }

            return properties.Where(prop => !OmittedProperties.Contains(prop.PropertyName)).ToList();
        }
    }
}