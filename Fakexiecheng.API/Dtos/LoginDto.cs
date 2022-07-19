using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Fakexiecheng.API.Dtos
{
    public class LoginDto
    {
        [Required]//必填
        public string Email { get; set; }

        [Required]
        public string password { get; set; }
    }
}
