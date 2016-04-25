using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace wwwplatform.Models
{
    [Table(name: "SitePage")]
    public class SitePage : Auditable
    {
        public SitePage() : base() { }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [Required]
        [MaxLength(150)]
        public string Description { get; set; }

        [DisplayName("Publication Date")]
        public DateTime PubDate { get; set; }

        [DataType(DataType.Html), UIHint("tinymce_jquery_full"), AllowHtml]
        [DisplayName("Page Body")]
        public string HTMLBody { get; set; }

        public int Order { get; set; }

        [MaxLength(150)]
        public string Slug { get; set; }

        public Nullable<long> ParentPageId { get; set; }

        [ForeignKey("ParentPageId")]
        [DisplayName("Parent Page")]
        public virtual SitePage ParentPage { get; set; }

        [InverseProperty("ParentPage")]
        [DisplayName("Parent Page")]
        public virtual ICollection<SitePage> SubPages { get; set; }

        [DefaultValue(true)]
        [DisplayName("Show In Navigation")]
        public bool ShowInNavigation { get; set; }
        
        [DisplayName("Set As Home Page")]
        public bool HomePage { get; set; }

        internal void Update(SitePage sitePage)
        {
            Name = sitePage.Name;
            Description = sitePage.Description;
            PubDate = sitePage.PubDate;
            HTMLBody = sitePage.HTMLBody;
            Order = sitePage.Order;
            ParentPageId = sitePage.ParentPageId;
            ShowInNavigation = sitePage.ShowInNavigation;
            HomePage = sitePage.HomePage;
        }
    }
}
