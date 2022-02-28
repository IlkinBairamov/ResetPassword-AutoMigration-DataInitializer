using FrontToBack.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.ViewModels
{
    public class AccountViewModel   
    {
        public User user { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password),Compare("Password")]
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }
    }
}
