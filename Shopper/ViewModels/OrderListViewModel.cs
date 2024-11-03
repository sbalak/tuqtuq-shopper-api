namespace Shopper.ViewModels
{
    public class OrderListViewModel
    {
        public OrderListViewModel()
        {
            OrderItems = new List<OrderListFoodViewModel>();
        }
        public int OrderId { get; set; }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantLocality { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderListFoodViewModel> OrderItems { get; set; }
    }

    public class OrderListFoodViewModel
    {
        public string FoodName { get; set; }
        public int Quantity { get; set; }
    }
}
