namespace Shopper.Infrastructure
{
    public class CartModel
    {
        public CartModel()
        {
            CartItems = new List<CartItemModel>();
        }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantLocality { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalPrimaryTaxAmount { get; set; }
        public decimal TotalSecondaryTaxAmount { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public List<CartItemModel> CartItems { get; set; }
    }
    
    public class CartItemModel
    {
        public int FoodItemId { get; set; }
        public string FoodName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
    }

    public class CartValueModel
    {
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
    }
}
