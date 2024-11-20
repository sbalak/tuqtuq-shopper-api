using Microsoft.EntityFrameworkCore;
using Shopper.Data;

namespace Shopper.Infrastructure
{
    public class CartService : ICartService
    {
        private readonly ShopperContext _context;

        public CartService(ShopperContext context)
        {
            _context = context;
        }

        public async Task<CartModel> GetCart(int userId)
        {
            CartModel cart = new CartModel();
            decimal primaryTaxRate = 0, secondaryTaxRate = 0;

            var foodItems = await (from m in _context.Carts
                             join n in _context.CartItems on m.Id equals n.CartId
                             join o in _context.FoodItems on n.FoodItemId equals o.Id
                             where m.UserId == userId
                             select new CartItemModel
                             {
                                 FoodItemId = n.FoodItemId,
                                 FoodName = o.Name,
                                 Quantity = n.Quantity,
                                 Price = o.Price,
                                 Amount = n.Quantity * o.Price
                             }).ToListAsync();

            if (foodItems.Count > 0)
            {
                var restaurant = await (from m in _context.Carts
                                  join n in _context.Restaurants on m.RestaurantId equals n.Id
                                  where m.UserId == userId
                                  select new
                                  {
                                      RestaurantId = n.Id,
                                      RestaurantName = n.Name,
                                      RestaurantLocality = n.Locality,
                                      RestauarantPrimaryTaxRate = n.PrimaryTaxRate,
                                      RestaurantSecondaryTaxRate = n.SecondaryTaxRate
                                  }).FirstOrDefaultAsync();

                cart.RestaurantId = restaurant.RestaurantId;
                cart.RestaurantName = restaurant.RestaurantName;
                cart.RestaurantLocality = restaurant.RestaurantLocality;

                primaryTaxRate = restaurant.RestauarantPrimaryTaxRate;
                secondaryTaxRate = restaurant.RestaurantSecondaryTaxRate;
            }

            var primaryTax = (foodItems.Sum(x => x.Amount) * primaryTaxRate) / 100;
            var secondaryTax = (foodItems.Sum(x => x.Amount) * secondaryTaxRate) / 100;
            var totalTax = primaryTax + secondaryTax;
            var totalAmount = foodItems.Sum(x => x.Amount) + totalTax; 

            cart.CartItems = foodItems;
            cart.TotalPrimaryTaxAmount = Math.Round(primaryTax, 2);
            cart.TotalSecondaryTaxAmount = Math.Round(secondaryTax, 2);
            cart.TotalTaxAmount = Math.Round(totalTax, 2);
            cart.TotalQuantity = foodItems.Sum(x => x.Quantity);
            cart.TotalAmount = Math.Round(totalAmount, 2);

            return cart;
        }

        public async Task<CartValueModel> GetCartValue(int userId, int restaurantId)
        {
            var cartItems = await _context.CartItems.Where(x => x.Cart.UserId == userId && x.Cart.RestaurantId == restaurantId).Select(x => new 
            {
                Quantity = x.Quantity,
                Amount = x.FoodItem.Price * x.Quantity
            }).ToListAsync();

            var cartValue = new CartValueModel()
            {
                Quantity = cartItems.Sum(x => x.Quantity),
                Amount = cartItems.Sum(x => x.Amount)
            };

            return cartValue;
        }

        public async Task Add(int userId, int restaurantId, int foodId)
        {
            var cart = await _context.Carts.Where(x => x.UserId == userId).FirstOrDefaultAsync();
            int? cartId = null;

            if (cart == null)
            {
                Cart newCart = new Cart()
                {
                    UserId = userId,
                    RestaurantId = restaurantId,
                    DateCreated = DateTime.Now
                };

                await _context.Carts.AddAsync(newCart);
                await _context.SaveChangesAsync();

                CartItem newCartItem = new CartItem()
                {
                    CartId = newCart.Id,
                    FoodItemId = foodId,
                    Quantity = 1
                };

                await _context.CartItems.AddAsync(newCartItem);
                await _context.SaveChangesAsync();

                cartId = newCart.Id;
            }
            else
            {
                if (cart.RestaurantId == restaurantId)
                {
                    var cartItem = await _context.CartItems.Where(x => x.CartId == cart.Id && x.FoodItemId == foodId).FirstOrDefaultAsync();

                    if (cartItem == null)
                    {
                        CartItem newCartItem = new CartItem()
                        {
                            CartId = cart.Id,
                            FoodItemId = foodId,
                            Quantity = 1
                        };

                        await _context.CartItems.AddAsync(newCartItem);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        cartItem.Quantity += 1;

                        _context.CartItems.Update(cartItem);
                        await _context.SaveChangesAsync();
                    }

                    cartId = cart.Id;
                }
                else
                {
                    var cartItems = await _context.CartItems.Where(x => x.CartId == cart.Id).ToListAsync();

                    _context.CartItems.RemoveRange(cartItems);
                    await _context.SaveChangesAsync();

                    _context.Carts.Remove(cart);
                    await _context.SaveChangesAsync();


                    Cart newCart = new Cart()
                    {
                        UserId = userId,
                        RestaurantId = restaurantId,
                        DateCreated = DateTime.Now
                    };

                    await _context.Carts.AddAsync(newCart);
                    await _context.SaveChangesAsync();

                    CartItem newCartItem = new CartItem()
                    {
                        CartId = newCart.Id,
                        FoodItemId = foodId,
                        Quantity = 1
                    };

                    await _context.CartItems.AddAsync(newCartItem);
                    await _context.SaveChangesAsync();

                    cartId = newCart.Id;
                }
            }
        }

        public async Task Remove(int userId, int restaurantId, int foodId)
        {
            var cart = await _context.Carts.Where(x => x.UserId == userId).FirstOrDefaultAsync();

            if (cart != null)
            {
                var cartItem = await _context.CartItems.Where(x => x.CartId == cart.Id && x.FoodItemId == foodId).FirstOrDefaultAsync();

                if (cartItem != null)
                {
                    if (cartItem.Quantity > 1)
                    {
                        cartItem.Quantity -= 1;

                        _context.CartItems.Update(cartItem);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        _context.CartItems.Remove(cartItem);
                        await _context.SaveChangesAsync();

                    }
                }

                var cartItems = await _context.CartItems.Where(x => x.CartId == cart.Id).ToListAsync();

                if (cartItems.Count() == 0)
                {
                    _context.Carts.Remove(cart);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
