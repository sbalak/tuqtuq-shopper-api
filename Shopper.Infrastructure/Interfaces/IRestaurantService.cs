namespace Shopper.Infrastructure
{
    public interface IRestaurantService
    {
        Task<List<RestaurantModel>> GetRestaurants();
        Task<RestaurantModel> GetRestaurant(int restaurantId);
        Task<List<CategorizedFoodItemModel>> GetFoodItems(int userId, int restaurantId, string? searchText = null);
    }
}
