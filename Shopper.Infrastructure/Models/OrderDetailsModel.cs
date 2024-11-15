namespace Shopper.Infrastructure
{
    public class OrderDetailsModel
    {
        public OrderDetailsModel()
        {
            OrderItems = new List<OrderDetailsFoodModel>();
        }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantLocality { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderDetailsFoodModel> OrderItems { get; set; }
    }
    
    public class OrderDetailsFoodModel
    {
        public int FoodItemId { get; set; }
        public string FoodName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
    }
}
