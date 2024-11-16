namespace Shopper.Infrastructure
{
    public interface IRestaurantService
    {
        Task<List<RestaurantModel>> GetRestaurants();
        Task<RestaurantModel> GetRestaurant(int restaurantId);
        Task<List<FoodItemModel>> GetFoodItems(int userId, int restaurantId);
    }
}
