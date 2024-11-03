using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopper.Models;
using Shopper.ViewModels;

namespace Shopper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private ShopperContext _context;

        public CartController(ShopperContext context)
        {
            _context = context;
        }

        [HttpGet("Add")]
        public void Add(int userId, int restaurantId, int foodId)
        {
            var cart = _context.Carts.Where(x => x.UserId == userId).FirstOrDefault();
            int? cartId = null;

            if (cart == null)
            {
                Cart newCart = new Cart()
                {
                    UserId = userId,
                    RestaurantId = restaurantId
                };

                _context.Carts.Add(newCart);
                _context.SaveChanges();

                CartItem newCartItem = new CartItem()
                {
                    CartId = newCart.Id,
                    FoodItemId = foodId,
                    Quantity = 1
                };

                _context.CartItems.Add(newCartItem);
                _context.SaveChanges();

                cartId = newCart.Id;
            }
            else
            {
                if (cart.RestaurantId == restaurantId)
                {
                    var cartItem = _context.CartItems.Where(x => x.CartId == cart.Id && x.FoodItemId == foodId).FirstOrDefault();

                    if (cartItem == null)
                    {
                        CartItem newCartItem = new CartItem()
                        {
                            CartId = cart.Id,
                            FoodItemId = foodId,
                            Quantity = 1
                        };

                        _context.CartItems.Add(newCartItem);
                        _context.SaveChanges();
                    }
                    else
                    {
                        cartItem.Quantity += 1;

                        _context.CartItems.Update(cartItem);
                        _context.SaveChanges();
                    }

                    cartId = cart.Id;
                }
                else
                {
                    var cartItems = _context.CartItems.Where(x => x.CartId == cart.Id).ToList();

                    _context.CartItems.RemoveRange(cartItems);
                    _context.SaveChanges();

                    _context.Carts.Remove(cart);
                    _context.SaveChanges();


                    Cart newCart = new Cart()
                    {
                        UserId = userId,
                        RestaurantId = restaurantId
                    };

                    _context.Carts.Add(newCart);
                    _context.SaveChanges();

                    CartItem newCartItem = new CartItem()
                    {
                        CartId = newCart.Id,
                        FoodItemId = foodId,
                        Quantity = 1
                    };

                    _context.CartItems.Add(newCartItem);
                    _context.SaveChanges();

                    cartId = newCart.Id;
                }
            }
        }

        [HttpGet("Remove")]
        public void Remove(int userId, int restaurantId, int foodId)
        {
            var cart = _context.Carts.Where(x => x.UserId == userId).FirstOrDefault();

            if (cart != null)
            {
                var cartItem = _context.CartItems.Where(x => x.CartId == cart.Id && x.FoodItemId == foodId).FirstOrDefault();

                if (cartItem != null)
                {
                    if (cartItem.Quantity > 1)
                    {
                        cartItem.Quantity -= 1;

                        _context.CartItems.Update(cartItem);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.CartItems.Remove(cartItem);
                        _context.SaveChanges();

                    }
                }

                var cartItems = _context.CartItems.Where(x => x.CartId == cart.Id).ToList();

                if (cartItems.Count() == 0)
                {
                    _context.Carts.Remove(cart);
                    _context.SaveChanges();
                }
            }
        }

        [HttpGet("Details")]
        public CartDetailsViewModel Details(int userId)
        {
            CartDetailsViewModel model = new CartDetailsViewModel();

            var foodItems = (from m in _context.Carts
                            join n in _context.CartItems on m.Id equals n.CartId
                            join o in _context.FoodItems on n.FoodItemId equals o.Id
                            where m.UserId == userId
                            select new CartDetailsFoodViewModel
                            {
                                FoodItemId = n.FoodItemId,
                                FoodName = o.Name,
                                Quantity = n.Quantity,
                                ItemPrice = o.Price,
                                TotalPrice = n.Quantity * o.Price
                            }).ToList();

            if (foodItems.Count > 0)
            {
                var restaurant = (from m in _context.Carts
                                  join n in _context.Restaurants on m.RestaurantId equals n.Id
                                  where m.UserId == userId
                                  select new
                                  {
                                      RestaurantId = n.Id,
                                      RestaurantName = n.Name,
                                      RestaurantLocality = n.Locality
                                  }).FirstOrDefault();

                model.RestaurantId = restaurant.RestaurantId;
                model.RestaurantName = restaurant.RestaurantName;
                model.RestaurantLocality = restaurant.RestaurantLocality;
            }

            model.FoodItems = foodItems;
            model.TotalQuantity = foodItems.Sum(x => x.Quantity);
            model.TotalPrice = foodItems.Sum(x => x.TotalPrice);

            return model;
        }
    }
}
