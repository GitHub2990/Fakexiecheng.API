using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Fakexiecheng.API.Dtos
{
    public class RegisterDto
    {
        [Required]//Required注释  为必填字段
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password),ErrorMessage ="输入的密码不一致")]
        public string CofirmPassword { get; set; }
    }
}
