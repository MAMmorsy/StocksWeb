namespace StocksWeb.ViewModels
{
    public class CheckQuantityViewModel
    {
        public List<ProductQuantityViewModel> Data { get; set; }
        public int StoreId { get; set; }
        
    }

    public class ProductQuantityViewModel
    {
        public int ProductId { get; set; }
        public int UnitId { get; set; }
        public int Quantity { get; set; }
    }
}
