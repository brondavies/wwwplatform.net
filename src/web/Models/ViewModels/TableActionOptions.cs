using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace wwwplatform.Models.ViewModels
{
    public class TableActionOptions : ViewDataDictionary
    {
        public bool showDelete
        {
            get { return this["showDelete"] == null; }
            set { if(!value) this["showDelete"] = value; }
        }

        public string deleteLabel
        {
            get { return this["deleteLabel"] as string; }
            set { this["deleteLabel"] = value; }
        }

        public bool showDetails
        {
            get { return this["showDetails"] == null; }
            set { if (!value) this["showDetails"] = value; }
        }

        public string detailLabel
        {
            get { return this["detailLabel"] as string; }
            set { this["detailLabel"] = value; }
        }

        public bool showEdit
        {
            get { return this["showEdit"] == null; }
            set { if (!value) this["showEdit"] = value; }
        }

        public string editLabel
        {
            get { return this["editLabel"] as string; }
            set { this["editLabel"] = value; }
        }

        public string Controller
        {
            get { return (string)this["Controller"]; }
            set { this["Controller"] = value; }
        }
    }
}