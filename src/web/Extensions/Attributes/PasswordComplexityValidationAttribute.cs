using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using wwwplatform.Models;

namespace wwwplatform.Extensions.Attributes
{
    public class PasswordComplexityValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            int upper = int.Parse(Settings.GetConfig("password:RequiresUppercase", "0"));
            int lower = int.Parse(Settings.GetConfig("password:RequiresLowercase", "0"));
            int number = int.Parse(Settings.GetConfig("password:RequiresNumber", "0"));
            int special = int.Parse(Settings.GetConfig("password:RequiresSpecial", "0"));
            int upperReq = upper;
            int lowerReq = lower;
            int numberReq = number;
            int specialReq = special;

            string password = value.ToString();
            foreach (char c in password)
            {
                if (c >= 'A' && c <= 'Z')
                {
                    upper--;
                }
                else
                if (c >= 'a' && c <= 'z')
                {
                    lower--;
                }
                else
                if (c >= '0' && c <= '9')
                {
                    number--;
                }
                else
                {
                    special--;
                }
            }
            if (upper > 0 || lower > 0 || number > 0 || special > 0)
            {
                return new ValidationResult(ErrorMessage ?? GetErrorMessage(validationContext));
            }
            return ValidationResult.Success;
        }

        private string GetErrorMessage(ValidationContext context)
        {
            return string.Format(Settings.GetConfig("password:ValidationErrorMessage"), context.DisplayName);
        }
    }
}