
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksWeb.ViewModels
{
    public class InvoiceItemsViewModel
    {
        public int InvoiceDetailsId { get; set; }
        public int InvoiceId { get; set; }
        public string StoreName { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int UnitId { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public decimal Discount { get; set; }
        public decimal Net { get; set; }
    }
}
