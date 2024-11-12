namespace Shopper.Infrastructure
{
    public interface IOrderService
    {
        Task<List<OrderListModel>> GetOrders(int userId);
        Task<OrderDetailsModel> GetOrder(int id);
        Task Confirm(int userId);
    }
}
