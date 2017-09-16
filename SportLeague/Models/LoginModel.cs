using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SportLeague.Models
{
    public class LoginModel
    {
        [Required]
        [StringLength(20)]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(40)]
        public string Password { get; set; }

    }
}