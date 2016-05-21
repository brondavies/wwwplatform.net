using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wwwplatform.Models.ViewModels
{
    [NotMapped]
    public class UserEditorModel : RegisterViewModel
    {
        public UserEditorModel() { }
        public UserEditorModel(ApplicationUser user)
        {
            Id = user.Id;
            Email = user.Email;
            EmailConfirmed = user.EmailConfirmed;
            FirstName = user.FirstName;
            LastName = user.LastName;
            LockoutEnabled = user.LockoutEnabled;
            LockoutEndDateUtc = user.LockoutEndDateUtc;
            PhoneNumber = user.PhoneNumber;
            PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            TwoFactorEnabled = user.TwoFactorEnabled;
            UserName = user.UserName;
            //required but not used for edit
            ConfirmPassword =
            Password = "**********";
            if (user.Id != null)
            {
                permissions = user.Roles.Select(r => r.RoleId).ToArray();
            }
            else
            {
                permissions = new string[] { }; 
            }
        }
        
        public string Id { get; set; }
        
        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }
        
        [Display(Name = "Account Lockout")]
        public bool LockoutEnabled { get; set; }

        [Display(Name = "Account Lockout Ends")]
        public DateTime? LockoutEndDateUtc { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Phone Number Confirmed")]
        public bool PhoneNumberConfirmed { get; set; }

        [Display(Name = "Two-factor Authentication")]
        public bool TwoFactorEnabled { get; set; }

        public string FullName()
        {
            return (FirstName + " " + LastName).Trim();
        }

        public string[] permissions { get; set; }
    }
}
