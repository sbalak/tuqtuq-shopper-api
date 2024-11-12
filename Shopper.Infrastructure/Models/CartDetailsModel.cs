namespace Shopper.Infrastructure
{
    public class CartDetailsModel
    {
        public CartDetailsModel()
        {
            FoodItems = new List<CartDetailsFoodModel>();
        }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantLocality { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalPrice { get; set; }
        public List<CartDetailsFoodModel> FoodItems { get; set; }
    }
    
    public class CartDetailsFoodModel
    {
        public int FoodItemId { get; set; }
        public string FoodName { get; set; }
        public int Quantity { get; set; }
        public decimal ItemPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
