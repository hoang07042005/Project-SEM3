using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eAdministrationLabs.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; }

        // Danh sách các vai trò người dùng có thể chọn
        public List<int> SelectedRoleIds { get; set; }
    }

}
