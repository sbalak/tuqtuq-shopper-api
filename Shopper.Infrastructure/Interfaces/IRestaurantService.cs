namespace Shopper.Infrastructure
{
    public interface IRestaurantService
    {
        Task<List<RestaurantListModel>> GetRestaurants();

        Task<RestaurantMenuModel> GetRestaurantMenu(int userId, int restaurantId);
        Task<RestaurantMenuModel> FilterRestaurantMenu(int userId, int restaurantId, string searchText);
    }
}
