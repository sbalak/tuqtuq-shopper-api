using Microsoft.EntityFrameworkCore;
using Shopper.Data;

namespace Shopper.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly ShopperContext _context;

        public OrderService(ShopperContext context)
        {
            _context = context;
        }

        public async Task<List<OrderListModel>> GetOrders(int userId)
        {
            var orderList = await (from m in _context.Orders
                             join n in _context.Restaurants on m.RestaurantId equals n.Id
                             where m.UserId == userId
                             select new OrderListModel
                             {
                                 OrderId = m.Id,
                                 RestaurantId = n.Id,
                                 RestaurantName = n.Name,
                                 RestaurantLocality = n.Locality,
                                 TotalAmount = m.Amount
                             }).ToListAsync();

            foreach (var order in orderList)
            {
                var orderItems = await (from m in _context.Orders
                                  join n in _context.OrderItems on m.Id equals n.OrderId
                                  join o in _context.FoodItems on n.FoodItemId equals o.Id
                                  where m.Id == order.OrderId
                                  select new OrderListFoodModel
                                  {
                                      FoodName = o.Name,
                                      Quantity = n.Quantity
                                  }).ToListAsync();
                order.OrderItems = orderItems;

            }

            return orderList;
        }

        public async Task<OrderDetailsModel> GetOrder(int id)
        {
            OrderDetailsModel model = new OrderDetailsModel();

            var orderItems = await (from m in _context.Orders
                              join n in _context.OrderItems on m.Id equals n.OrderId
                              join o in _context.FoodItems on n.FoodItemId equals o.Id
                              where m.Id == id
                              select new OrderDetailsFoodModel
                              {
                                  FoodItemId = n.FoodItemId,
                                  FoodName = o.Name,
                                  Quantity = n.Quantity,
                                  Price = o.Price,
                                  Amount = n.Quantity * o.Price
                              }).ToListAsync();

            if (orderItems.Count > 0)
            {
                var orderDetails = await (from m in _context.Orders
                                    join n in _context.Restaurants on m.RestaurantId equals n.Id
                                    where m.Id == id
                                    select new
                                    {
                                        RestaurantId = n.Id,
                                        RestaurantName = n.Name,
                                        RestaurantLocality = n.Locality,
                                        m.Amount
                                    }).FirstOrDefaultAsync();

                model.RestaurantId = orderDetails.RestaurantId;
                model.RestaurantName = orderDetails.RestaurantName;
                model.RestaurantLocality = orderDetails.RestaurantLocality;
                model.TotalAmount = orderDetails.Amount;
            }

            model.OrderItems = orderItems;
            model.TotalQuantity = orderItems.Sum(x => x.Quantity);

            return model;
        }

        public async Task Confirm(int userId)
        {
            List<OrderItem> orderItems = new List<OrderItem>();

            var cart = await  _context.Carts.Where(x => x.UserId == userId).FirstOrDefaultAsync();

            if (cart != null)
            {
                var cartItems = await _context.CartItems.Where(x => x.CartId == cart.Id).ToListAsync();

                var foodItems = await (from m in _context.Carts
                                 join n in _context.CartItems on m.Id equals n.CartId
                                 join o in _context.FoodItems on n.FoodItemId equals o.Id
                                 where m.UserId == userId && m.RestaurantId == cart.RestaurantId
                                 select new OrderDetailsFoodModel
                                 {
                                     FoodItemId = n.FoodItemId,
                                     FoodName = o.Name,
                                     Price = o.Price,
                                     Amount = n.Quantity * o.Price,
                                     Quantity = n.Quantity
                                 }).ToListAsync();


                Order order = new Order()
                {
                    RestaurantId = cart.RestaurantId,
                    UserId = userId,
                    Amount = foodItems.Sum(x => x.Amount),
                    DateOrdered = DateTime.Now
                };

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                foreach (var item in foodItems)
                {
                    OrderItem orderItem = new OrderItem()
                    {
                        FoodItemId = item.FoodItemId,
                        OrderId = order.Id,
                        Quantity = item.Quantity,
                        Amount = item.Amount
                    };

                    orderItems.Add(orderItem);
                }

                await _context.OrderItems.AddRangeAsync(orderItems);
                await _context.SaveChangesAsync();

                _context.CartItems.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }
    }
}
