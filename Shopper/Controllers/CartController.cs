using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopper.Infrastructure;

namespace Shopper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private ICartService _cart;

        public CartController(ICartService cart)
        {
            _cart = cart;
        }

        [HttpGet("Add")]
        public async Task Add(int userId, int restaurantId, int foodId)
        {
            await _cart.Add(userId, restaurantId, foodId);
        }

        [HttpGet("Remove")]
        public async Task Remove(int userId, int restaurantId, int foodId)
        {
            await _cart.Remove(userId, restaurantId, foodId);
        }

        [HttpGet("Details")]
        public async Task<CartDetailsModel> Details(int userId)
        {
            var cart = await _cart.GetCart(userId);
            return cart;
        }
    }
}
