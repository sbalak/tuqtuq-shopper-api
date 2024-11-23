namespace Shopper.Infrastructure
{
    public interface IOrderService
    {
        Task<List<OrderModel>> GetOrders(int userId);
        Task<OrderModel> GetOrder(int id);
        Task Confirm(int userId);
    }
}
