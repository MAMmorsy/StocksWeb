using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksWeb.ViewModels
{
    public class ProductQuantitySearchViewModel
    {
        public int ProductId { get; set; }
        public int StoreId { get; set; }
        public int UnitId { get; set; }
    }
    public class EncProductQuantitySearchViewModel
    {
        public string ProductId { get; set; }
        public string StoreId { get; set; }
        public string UnitId { get; set; }
    }
}
