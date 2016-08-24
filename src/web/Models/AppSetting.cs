using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wwwplatform.Models
{
    public class AppSetting
    {
        [Key, Column(Order = 0)]
        public string Name { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }
    }
}
