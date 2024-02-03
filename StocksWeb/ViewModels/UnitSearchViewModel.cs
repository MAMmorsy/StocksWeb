using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksWeb.ViewModels
{
    public class UnitSearchViewModel
    {
        public int ProductId { get; set; }
        public int StoreId { get; set; }
    }
    public class EncUnitSearchViewModel
    {
        public string ProductId { get; set; }
        public string StoreId { get; set; }
    }
}
