namespace Shopper.Infrastructure
{
    public interface IRestaurantService
    {
        Task<List<RestaurantModel>> GetRestaurants(string? query = "", int? page = 1, int? pageSize = 10);
        Task<List<RestaurantModel>> GetRestaurantsRecentlyVisited(int? page = 1, int? pageSize = 10);
        Task<RestaurantModel> GetRestaurant(int restaurantId);
        Task<List<CategorizedFoodItemModel>> GetFoodItems(int userId, int restaurantId, string? searchText = null);
    }
}
