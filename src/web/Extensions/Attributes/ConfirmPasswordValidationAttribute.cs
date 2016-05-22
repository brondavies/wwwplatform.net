using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wwwplatform.Extensions.Attributes
{
    public class ConfirmPasswordValidationAttribute : CompareAttribute
    {
        public ConfirmPasswordValidationAttribute(string otherField = "NewPassword") : base(otherField)
        {
            ErrorMessage = "The new password and confirmation password do not match.";
        }
    }
}
