namespace Shopper.Infrastructure
{
    public interface IRestaurantService
    {
        Task<List<RestaurantListModel>> GetRestaurants();

        Task<RestaurantMenuModel> GetRestaurantMenu(int userId, int restaurantId);
    }
}
