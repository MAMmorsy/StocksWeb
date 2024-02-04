
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksWeb.ViewModels
{
    public class InvoiceCreateViewModel
    {
        public string InvoiceNo { get; set; } = null!;
        public DateTime InvoiceDate { get; set; }= DateTime.Now;
        public int UserId { get; set; }
        public decimal totalTotal { get; set; }
        public decimal Taxes { get; set; }
        public decimal totalNet { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalDiscount { get; set; }
        public List<int> productList { get; set; }
        public List<int> unitList { get; set; }
        public List<decimal> price { get; set; }
        public List<int> quantity { get; set; }
        public List<decimal> total { get; set; }
        public List<decimal> discount { get; set; }
        public List<decimal> net { get; set; }
    }

}
