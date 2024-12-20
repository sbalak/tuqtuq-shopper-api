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
        public async Task<CartModel> Details(int userId)
        {
            var cart = await _cart.GetCart(userId);
            return cart;
        }

        [HttpGet("Value")]
        public async Task<CartValueModel> Value(int userId, int restaurantId)
        {
            var cartValue = await _cart.GetCartValue(userId, restaurantId);
            return cartValue;
        }
    }
}
