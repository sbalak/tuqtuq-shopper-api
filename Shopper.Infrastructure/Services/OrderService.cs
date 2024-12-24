using Microsoft.EntityFrameworkCore;
using Shopper.Data;
using System.Globalization;

namespace Shopper.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly ShopperContext _context;

        public OrderService(ShopperContext context)
        {
            _context = context;
        }

        public async Task<List<OrderModel>> GetOrders(int userId, int? page = 1, int? pageSize = 10)
        {
            List<OrderModel> orders = new List<OrderModel>();

            int offset = (Convert.ToInt32(page) - 1) * Convert.ToInt32(pageSize);
            int fetch = Convert.ToInt32(page) * Convert.ToInt32(pageSize);

            var ordersList = await (from m in _context.Orders
                                    join n in _context.Restaurants on m.RestaurantId equals n.Id
                                    where m.UserId == userId
                                    select new
                                    {
                                        OrderId = m.Id,
                                        RestaurantId = n.Id,
                                        RestaurantName = n.Name,
                                        RestaurantLocality = n.Locality,
                                        Status = m.Status,
                                        TotalTaxableAmount = m.TaxableAmount,
                                        TotalAmount = m.Amount,
                                        PrimaryTaxAmount = m.PrimaryTaxAmount,
                                        SecondaryTaxAmount = m.SecondaryTaxAmount,
                                        DateOrdered = m.DateOrdered,
                                        FormattedDateOrdered = m.DateOrdered.ToString("dd MMM yy, hh:mm tt")
                                    }).OrderByDescending(m => m.DateOrdered).Skip(offset).Take(fetch).ToListAsync();

            foreach (var ordersItem in ordersList)
            {
                OrderModel order = new OrderModel()
                {
                    OrderId = ordersItem.OrderId,
                    RestaurantId = ordersItem.RestaurantId,
                    RestaurantName = ordersItem.RestaurantName,
                    RestaurantLocality = ordersItem.RestaurantLocality,
                    Status = ordersItem.Status,
                    TotalPrimaryTaxAmount = Assist.Rupee(ordersItem.PrimaryTaxAmount),
                    TotalSecondaryTaxAmount = Assist.Rupee(ordersItem.SecondaryTaxAmount),
                    TotalTaxAmount = Assist.Rupee(ordersItem.PrimaryTaxAmount + ordersItem.SecondaryTaxAmount),
                    TotalTaxableAmount = Assist.Rupee(ordersItem.TotalTaxableAmount),
                    TotalAmount = Assist.Rupee(ordersItem.TotalAmount),
                    DateOrdered = ordersItem.FormattedDateOrdered
                };

                var foodItems = await (from m in _context.Orders
                                  join n in _context.OrderItems on m.Id equals n.OrderId
                                  join o in _context.FoodItems on n.FoodItemId equals o.Id
                                  where m.Id == order.OrderId
                                  select new
                                  {
                                      FoodName = o.Name,
                                      Type = o.Type,
                                      Quantity = n.Quantity,
                                      TaxableAmount = n.TaxableAmount,
                                      Amount = n.Amount
                                  }).ToListAsync();

                if (foodItems.Count > 0)
                {
                    List<OrderItemModel> orderItems = new List<OrderItemModel>();

                    foreach (var foodItem in foodItems)
                    {
                        OrderItemModel orderItem = new OrderItemModel()
                        {
                            FoodName = foodItem.FoodName,
                            Quantity = foodItem.Quantity,
                            Type = foodItem.Type,
                            TaxablePrice = Assist.Rupee(foodItem.TaxableAmount / foodItem.Quantity),
                            Price = Assist.Rupee(foodItem.Amount / foodItem.Quantity),
                            TaxableAmount = Assist.Rupee(foodItem.TaxableAmount),
                            Amount = Assist.Rupee(foodItem.Amount)

                        };

                        orderItems.Add(orderItem);
                    }

                    order.OrderItems = orderItems;
                }

                orders.Add(order);
            }

            return orders;
        }

        public async Task<OrderModel> GetOrder(int id)
        {
            OrderModel order = new OrderModel();
            List<OrderItemModel> orderItems = new List<OrderItemModel>();

            var orderDetails = await (from m in _context.Orders
                                      join n in _context.Restaurants on m.RestaurantId equals n.Id
                                      where m.Id == id
                                      select new
                                      {
                                          OrderId = m.Id,
                                          RestaurantId = n.Id,
                                          RestaurantName = n.Name,
                                          RestaurantLocality = n.Locality,
                                          Status = m.Status,
                                          TotalTaxableAmount = m.TaxableAmount,
                                          TotalAmount = m.Amount,
                                          PrimaryTaxAmount = m.PrimaryTaxAmount,
                                          SecondaryTaxAmount = m.SecondaryTaxAmount,
                                          DateOrdered = m.DateOrdered,
                                          FormattedDateOrdered = m.DateOrdered.ToString("dd MMM yy, hh:mm tt")
                                      }).FirstOrDefaultAsync();

            var foodItems = await (from m in _context.Orders
                              join n in _context.OrderItems on m.Id equals n.OrderId
                              join o in _context.FoodItems on n.FoodItemId equals o.Id
                              where m.Id == id
                              select new
                              {
                                  FoodName = o.Name,
                                  Type = o.Type,
                                  Quantity = n.Quantity,
                                  TaxableAmount = n.TaxableAmount,
                                  Amount = n.Amount
                              }).ToListAsync();

            if (orderDetails != null && foodItems.Count > 0)
            {
                order.OrderId = orderDetails.OrderId;
                order.RestaurantId = orderDetails.RestaurantId;
                order.RestaurantName = orderDetails.RestaurantName;
                order.RestaurantLocality = orderDetails.RestaurantLocality;
                order.Status = orderDetails.Status;
                order.TotalPrimaryTaxAmount = Assist.Rupee(orderDetails.PrimaryTaxAmount);
                order.TotalSecondaryTaxAmount = Assist.Rupee(orderDetails.SecondaryTaxAmount);
                order.TotalTaxAmount = Assist.Rupee(orderDetails.PrimaryTaxAmount + orderDetails.SecondaryTaxAmount);
                order.TotalTaxableAmount = Assist.Rupee(orderDetails.TotalTaxableAmount);
                order.TotalAmount = Assist.Rupee(orderDetails.TotalAmount);
                order.DateOrdered = orderDetails.FormattedDateOrdered;

                foreach (var foodItem in foodItems)
                {
                    OrderItemModel orderItem = new OrderItemModel()
                    {
                        FoodName = foodItem.FoodName,
                        Quantity = foodItem.Quantity,
                        Type = foodItem.Type,
                        TaxablePrice = Assist.Rupee(foodItem.TaxableAmount / foodItem.Quantity),
                        Price = Assist.Rupee(foodItem.Amount / foodItem.Quantity),
                        TaxableAmount = Assist.Rupee(foodItem.TaxableAmount),
                        Amount = Assist.Rupee(foodItem.Amount)

                    };

                    orderItems.Add(orderItem);
                }

                order.OrderItems = orderItems;

            }

            return order;
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
                                 select new
                                 {
                                     FoodItemId = n.FoodItemId,
                                     FoodName = o.Name,
                                     Price = o.Price,
                                     TaxableAmount = ((n.Quantity * o.Price) / (100 + o.Restaurant.PrimaryTaxRate + o.Restaurant.SecondaryTaxRate)) * 100,
                                     Amount = n.Quantity * o.Price,
                                     PrimaryTaxAmount = ((((n.Quantity * o.Price) / (100 + o.Restaurant.PrimaryTaxRate + o.Restaurant.SecondaryTaxRate)) * 100) * o.Restaurant.PrimaryTaxRate) / 100,
                                     SecondaryTaxAmount = ((((n.Quantity * o.Price) / (100 + o.Restaurant.PrimaryTaxRate + o.Restaurant.SecondaryTaxRate)) * 100) * o.Restaurant.SecondaryTaxRate) / 100,
                                     Quantity = n.Quantity
                                 }).ToListAsync();

                if (foodItems != null)
                {
                    Order order = new Order()
                    {
                        RestaurantId = cart.RestaurantId,
                        UserId = userId,
                        Status = "New",
                        TaxableAmount = foodItems.Sum(x => x.TaxableAmount),
                        Amount = foodItems.Sum(x => x.Amount),
                        PrimaryTaxAmount = foodItems.Sum(x => x.PrimaryTaxAmount),
                        SecondaryTaxAmount = foodItems.Sum(x => x.SecondaryTaxAmount),
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
                            TaxableAmount = item.TaxableAmount,
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
}
