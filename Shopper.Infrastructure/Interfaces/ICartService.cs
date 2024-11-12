namespace Shopper.Infrastructure
{
    public interface ICartService
    {
        Task<CartDetailsModel> GetCart(int userId);
        Task Add(int userId, int restaurantId, int foodId);
        Task Remove(int userId, int restaurantId, int foodId);
    }
}
