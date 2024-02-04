namespace StocksWeb.ViewModels
{
    public class InvoiceObjectViewModel
    {
        public string InvoiceNo { get; set; } = null!;
        public DateTime InvoiceDate { get; set; }
        public int UserId { get; set; }
        public decimal Total { get; set; }
        public decimal Taxes { get; set; }
        public decimal Net { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalDiscount { get; set; }
        public List<InvoiceItemsCreateViewModel> items { get; set; } = null!;
    }

    public class InvoiceItemsCreateViewModel
    {
        public int StoreProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public decimal Discount { get; set; }
        public decimal Net { get; set; }
    }
}
