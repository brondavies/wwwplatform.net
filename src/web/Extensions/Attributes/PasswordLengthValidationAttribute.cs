using System.ComponentModel.DataAnnotations;
using wwwplatform.Models;

namespace wwwplatform.Extensions.Attributes
{
    public class PasswordLengthValidationAttribute : StringLengthAttribute
    {
        public PasswordLengthValidationAttribute() : base (100)
        {
            MinimumLength = int.Parse(Settings.GetConfig("password:MinmumLength", "8"));
            ErrorMessage = "The {0} must be at least {2} characters long.";
        }
    }
}
