using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace wwwplatform.Extensions.Attributes
{
    public class FileExistsAttribute : ValidationAttribute
    {
        private bool required;
        private bool mustExist;
        private bool mustNotExist;

        public FileExistsAttribute(bool required = false, bool mustExist = true, bool mustNotExist = false)
        {
            this.required = required;
            this.mustExist = !mustNotExist;
            this.mustNotExist = mustNotExist;
            ErrorMessage = mustExist ? "The file specified does not exist." : "The file specified already exists.";
        }

        public override bool RequiresValidationContext => false;

        public override bool IsValid(object value)
        {
            string stringValue = Convert.ToString(value);
            bool result = false;
            if (string.IsNullOrEmpty(stringValue))
            {
                if (required)
                {
                    ErrorMessage = "{0} is required";
                    result = false;
                }
                else
                {
                    result = true;
                }
            }
            else
            {
                bool exists = Exists(stringValue);
                result = mustExist && exists || mustNotExist && !exists;
            }
            return result;
        }

        private bool Exists(string stringValue)
        {
            return File.Exists(HttpContext.Current.Server.MapPath(stringValue));
        }
    }
}