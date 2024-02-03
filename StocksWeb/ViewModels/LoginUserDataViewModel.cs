using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksWeb.ViewModels
{
    public class LoginUserDataViewModel
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; } = null!;
        public string RoleName { get; set; } = null!;
    }

    public class LoginResponseViewModel : LoginUserDataViewModel
    {
        public string Token { get; set; } = string.Empty;
    }
}
