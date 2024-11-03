using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopper.Models;
using Shopper.ViewModels;

namespace Shopper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RestaurantController : Controller
    {
        private ShopperContext _context;

        public RestaurantController(ShopperContext context) 
        {
            _context = context;
        }

        [HttpGet("List")]
        public List<Restaurant> List()
        {
            var restaurants = _context.Restaurants.ToList();
            return restaurants;
        }

        [HttpGet("ListByCuisine")]
        public List<Restaurant> ListByCuisine(string cuisine)
        {
            var restaurants = _context.Restaurants.Where(m => m.Cuisine.Contains(cuisine)).ToList();
            return restaurants;
        }

        [HttpGet("Details")]
        public RestaurantMenuViewModel Details(int userId, int restaurantId)
        {
            RestaurantMenuViewModel model = new RestaurantMenuViewModel();
            int totalQuantity = 0; decimal totalPrice = 0;

            var restaurant = _context.Restaurants.Where(x => x.Id == restaurantId).Select(x => new 
                                     { 
                                        x.Id, 
                                        x.Name,
                                        x.Photo,
                                        x.LegalName,
                                        x.AddressLine1,
                                        x.AddressLine2,
                                        x.Locality, 
                                        x.City,
                                        x.Postcode,
                                        x.Cuisine,
                                        x.Latitude,
                                        x.Longitude
                                     }).FirstOrDefault();

            if (restaurant != null)
            {

                var foodItems = _context.FoodItems.Where(x => x.RestaurantId == restaurantId)
                                        .Select(x => new RestaurantMenuFoodViewModel
                                        {
                                            Id = x.Id,
                                            Name = x.Name,
                                            Photo = x.Photo,
                                            ItemPrice = x.Price
                                        }).ToList();

                var cartItems = (from m in _context.Carts
                                 join n in _context.CartItems on m.Id equals n.CartId
                                 join o in _context.FoodItems on n.FoodItemId equals o.Id
                                 where m.UserId == userId
                                 select new CartDetailsFoodViewModel
                                 {
                                     FoodItemId = n.FoodItemId,
                                     FoodName = o.Name,
                                     ItemPrice = o.Price,
                                     TotalPrice = n.Quantity * o.Price,
                                     Quantity = n.Quantity
                                 }).ToList();

                foreach (var foodItem in foodItems)
                {
                    var cartItem = cartItems.Where(x => x.FoodItemId == foodItem.Id && x.Quantity > 0).FirstOrDefault();

                    if (cartItem != null)
                    {
                        foodItem.TotalPrice = cartItem.Quantity * foodItem.ItemPrice;
                        foodItem.Quantity = cartItem.Quantity;

                        totalPrice += foodItem.TotalPrice;
                        totalQuantity += foodItem.Quantity;
                    }
                }

                model.Id = restaurant.Id;
                model.Name = restaurant.Name;
                model.Photo = restaurant.Photo;
                model.LegalName = restaurant.LegalName;
                model.AddressLine1 = restaurant.AddressLine1;
                model.AddressLine2 = restaurant.AddressLine2;
                model.Locality = restaurant.Locality;
                model.City = restaurant.City;
                model.Postcode = restaurant.Postcode;
                model.Cuisine = restaurant.Cuisine;
                model.Latitude = restaurant.Latitude;
                model.Longitude = restaurant.Longitude;
                model.FoodItems = foodItems;
                model.TotalQuantity = totalQuantity;
                model.TotalPrice = totalPrice;
            }

            return model;
        }
    }
}
