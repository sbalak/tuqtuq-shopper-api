namespace Shopper.Infrastructure
{
    public interface IRestaurantService
    {
        Task<List<RestaurantModel>> GetRestaurants(double latitude, double longitude, string? query = "", int? page = 1, int? pageSize = 10);
        Task<List<RestaurantRecentlyVisitedModel>> GetRestaurantsRecentlyVisited(int userId, double latitude, double longitude, int? page = 1, int? pageSize = 10);
        Task<RestaurantModel> GetRestaurant(int restaurantId);
        Task<List<CategorizedFoodItemModel>> GetFoodItems(int userId, int restaurantId, string? searchText = null);
    }
}
