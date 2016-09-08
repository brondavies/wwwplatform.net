using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wwwplatform.Models.Support
{
    public class SkinDefinition
    {
        public Dictionary<string, List<string>> scripts { get; set; }
        public Dictionary<string, List<string>> css { get; set; }
        public string layout { get; set; }

        internal static SkinDefinition Load(string skinFile)
        {
            return JsonConvert.DeserializeObject<SkinDefinition>(File.ReadAllText(skinFile));
        }
    }
}
