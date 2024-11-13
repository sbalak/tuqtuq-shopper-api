using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopper.Infrastructure;

namespace Shopper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RestaurantController : Controller
    {
        private readonly IRestaurantService _restaurant;

        public RestaurantController(IRestaurantService restaurant) 
        {
            _restaurant = restaurant;
        }

        [HttpGet("List")]
        public async Task<List<RestaurantListModel>> List()
        {
            var restaurants = await _restaurant.GetRestaurants();
            return restaurants;
        }
        
        [HttpGet("Details")]
        public async Task<RestaurantMenuModel> Details(int userId, int restaurantId)
        {
            var restaurant = await _restaurant.GetRestaurantMenu(userId, restaurantId);
            return restaurant;
        }

        [HttpGet("Filter")]
        public async Task<RestaurantMenuModel> Details(int userId, int restaurantId, string searchText)
        {
            var restaurant = await _restaurant.FilterRestaurantMenu(userId, restaurantId, searchText);
            return restaurant;
        }
    }
}
