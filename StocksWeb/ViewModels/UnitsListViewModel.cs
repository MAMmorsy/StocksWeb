using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksWeb.ViewModels
{
    public class UnitsListViewModel
    {
        public int UnitId { get; set; }
        public string? UnitName { get; set; }
    }
}
