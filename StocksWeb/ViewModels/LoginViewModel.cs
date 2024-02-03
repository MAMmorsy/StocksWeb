using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksWeb.ViewModels
{
    
    public class LoginViewModel
    {
        [Required(ErrorMessage ="Please type your username")]
        public string UserName { get; set; } = null!;
        [Required(ErrorMessage = "Please type your password")]
        public string Password { get; set; } = null!;
    }
}
