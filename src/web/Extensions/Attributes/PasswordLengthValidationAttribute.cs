using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wwwplatform.Extensions.Attributes
{
    public class PasswordLengthValidationAttribute : StringLengthAttribute
    {
        public PasswordLengthValidationAttribute() : base (100)
        {
            MinimumLength = 8;
            ErrorMessage = "The {0} must be at least {2} characters long.";
        }
    }
}
