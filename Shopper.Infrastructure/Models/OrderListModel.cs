namespace Shopper.Infrastructure
{
    public class OrderListModel
    {
        public OrderListModel()
        {
            OrderItems = new List<OrderListFoodModel>();
        }
        public int OrderId { get; set; }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantLocality { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderListFoodModel> OrderItems { get; set; }
    }

    public class OrderListFoodModel
    {
        public string FoodName { get; set; }
        public int Quantity { get; set; }
    }
}
