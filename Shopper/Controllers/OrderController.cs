using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopper.Models;
using Shopper.ViewModels;

namespace Shopper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private ShopperContext _context;

        public OrderController(ShopperContext context)
        {
            _context = context;
        }

        [HttpGet("Confirm")]
        public void Confirm(int userId)
        {
            List<OrderItem> orderItems = new List<OrderItem>();

            var cart = _context.Carts.Where(x => x.UserId == userId).FirstOrDefault();

            if (cart != null)
            {
                var cartItems = _context.CartItems.Where(x => x.CartId == cart.Id).ToList();

                var foodItems = (from m in _context.Carts
                                 join n in _context.CartItems on m.Id equals n.CartId
                                 join o in _context.FoodItems on n.FoodItemId equals o.Id
                                 where m.UserId == userId && m.RestaurantId == cart.RestaurantId
                                 select new OrderDetailsFoodViewModel
                                 {
                                     FoodItemId = n.FoodItemId,
                                     FoodName = o.Name,
                                     ItemPrice = o.Price,
                                     TotalPrice = n.Quantity * o.Price,
                                     Quantity = n.Quantity
                                 }).ToList();


                Order order = new Order()
                {
                    RestaurantId = cart.RestaurantId,
                    UserId = userId,
                    TotalPrice = foodItems.Sum(x => x.TotalPrice),
                    DateOrdered = DateTime.Now
                };

                _context.Orders.Add(order);
                _context.SaveChanges();

                foreach (var item in foodItems)
                {
                    OrderItem orderItem = new OrderItem()
                    {
                        FoodItemId = item.FoodItemId,
                        OrderId = order.Id,
                        Quantity = item.Quantity,
                        Price = item.TotalPrice
                    };

                    orderItems.Add(orderItem);
                }

                _context.OrderItems.AddRange(orderItems);
                _context.SaveChanges();

                _context.CartItems.RemoveRange(cartItems);
                _context.SaveChanges();

                _context.Carts.Remove(cart);
                _context.SaveChanges();
            }
        }

        [HttpGet("Details")]
        public OrderDetailsViewModel Details(int id)
        {
            OrderDetailsViewModel model = new OrderDetailsViewModel();

            var orderItems = (from m in _context.Orders
                            join n in _context.OrderItems on m.Id equals n.OrderId
                            join o in _context.FoodItems on n.FoodItemId equals o.Id
                            where m.Id == id
                            select new OrderDetailsFoodViewModel
                            {
                                FoodItemId = n.FoodItemId,
                                FoodName = o.Name,
                                Quantity = n.Quantity,
                                ItemPrice = o.Price,
                                TotalPrice = n.Quantity * o.Price
                            }).ToList();

            if (orderItems.Count > 0)
            {
                var orderDetails = (from m in _context.Orders
                                    join n in _context.Restaurants on m.RestaurantId equals n.Id
                                    where m.Id == id
                                    select new
                                    {
                                        RestaurantId = n.Id,
                                        RestaurantName = n.Name,
                                        RestaurantLocality = n.Locality,
                                        m.TotalPrice
                                    }).FirstOrDefault();

                model.RestaurantId = orderDetails.RestaurantId;
                model.RestaurantName = orderDetails.RestaurantName;
                model.RestaurantLocality = orderDetails.RestaurantLocality;
                model.TotalPrice = orderDetails.TotalPrice;
            }

            model.OrderItems = orderItems;
            model.TotalQuantity = orderItems.Sum(x => x.Quantity);

            return model;
        }

        [HttpGet("List")]
        public List<OrderListViewModel> List(int userId)
        {
            var orderList = (from m in _context.Orders
                             join n in _context.Restaurants on m.RestaurantId equals n.Id
                             where m.UserId == userId
                             select new OrderListViewModel
                             {
                                 OrderId = m.Id,
                                 RestaurantId = n.Id,
                                 RestaurantName = n.Name,
                                 RestaurantLocality = n.Locality,
                                 TotalPrice = m.TotalPrice
                             }).ToList();

            foreach (var order in orderList)
            {
                var orderItems = (from m in _context.Orders
                                 join n in _context.OrderItems on m.Id equals n.OrderId
                                 join o in _context.FoodItems on n.FoodItemId equals o.Id
                                 where m.Id == order.OrderId
                                 select new OrderListFoodViewModel
                                 {
                                     FoodName = o.Name,
                                     Quantity = n.Quantity
                                 }).ToList();
                order.OrderItems = orderItems;

            }

            return orderList;
        }
    }
}
