namespace Shopper.ViewModels
{
    public class CartDetailsViewModel
    {
        public CartDetailsViewModel()
        {
            FoodItems = new List<CartDetailsFoodViewModel>();
        }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantLocality { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalPrice { get; set; }
        public List<CartDetailsFoodViewModel> FoodItems { get; set; }
    }
    
    public class CartDetailsFoodViewModel
    {
        public int FoodItemId { get; set; }
        public string FoodName { get; set; }
        public int Quantity { get; set; }
        public decimal ItemPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
