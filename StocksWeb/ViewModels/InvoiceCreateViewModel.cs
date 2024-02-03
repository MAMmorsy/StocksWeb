
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
        public decimal Total { get; set; }
        public decimal Taxes { get; set; }
        public decimal Net { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalDiscount { get; set; }
        public List<InvoiceItemsCreateDTO> items { get; set; } = null!;
    }

    public class InvoiceItemsCreateDTO 
    {
        public string? ProductName { get; set; }
        public string? UnitName { get; set; }
        public int StoreProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public decimal Discount { get; set; }
        public decimal Net { get; set; }
    }
}
