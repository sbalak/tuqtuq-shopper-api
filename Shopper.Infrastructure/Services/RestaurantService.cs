using Microsoft.EntityFrameworkCore;
using Shopper.Data;

namespace Shopper.Infrastructure
{
    public class RestaurantService : IRestaurantService
    {
        private readonly ShopperContext _context;

        public RestaurantService(ShopperContext context)
        {
            _context = context;
        }

        public async Task<RestaurantModel> GetRestaurant(int restaurantId)
        {
            var restaurant = await _context.Restaurants.Where(x => x.Id== restaurantId).Select(x => new RestaurantModel
            {
                Id = x.Id,
                Name = x.Name,
                Photo = x.Photo,
                Locality = x.Locality,
                City = x.City,
                Cuisine = x.Cuisine
            }).FirstOrDefaultAsync();

            return restaurant;
        }

        public async Task<List<CategorizedFoodItemModel>> GetFoodItems(int userId, int restaurantId, string? searchText = null)
        {
            var categorizedFoodItems = new List<CategorizedFoodItemModel>();
            var foodItems = new List<FoodItemModel>();

            var categories = await _context.Categories.Where(x => x.RestaurantId == restaurantId).OrderBy(x => x.Order).ToListAsync();

            if (searchText != null)
            {
                foodItems = await _context.FoodItems.Where(x => x.RestaurantId == restaurantId && x.Name.Contains(searchText))
                                        .Select(x => new FoodItemModel
                                        {
                                            Id = x.Id,
                                            CategoryId = x.CategoryId,
                                            Name = x.Name,
                                            Description = x.Description,
                                            Type = x.Type,
                                            Photo = x.Photo,
                                            TaxablePrice = x.TaxablePrice,
                                            Price = x.Price
                                        }).ToListAsync();
            }
            else
            {
                foodItems = await _context.FoodItems.Where(x => x.RestaurantId == restaurantId)
                                        .Select(x => new FoodItemModel
                                        {
                                            Id = x.Id,
                                            CategoryId = x.CategoryId,
                                            Name = x.Name,
                                            Description = x.Description,
                                            Type = x.Type,
                                            Photo = x.Photo,
                                            TaxablePrice = x.TaxablePrice,
                                            Price = x.Price
                                        }).ToListAsync();
            }

            var cartItems = await (from m in _context.Carts
                             join n in _context.CartItems on m.Id equals n.CartId
                             join o in _context.FoodItems on n.FoodItemId equals o.Id
                             where m.UserId == userId
                             select new
                             {
                                 FoodItemId = n.FoodItemId,
                                 TaxablePrice = o.TaxablePrice,
                                 Price = o.Price,
                                 Quantity = n.Quantity,
                                 TaxableAmount = n.Quantity * o.TaxablePrice,
                                 Amount = n.Quantity * o.Price
                             }).ToListAsync();

            foreach (var foodItem in foodItems)
            {
                var cartItem = cartItems.Where(x => x.FoodItemId == foodItem.Id && x.Quantity > 0).FirstOrDefault();

                if (cartItem != null)
                {
                    foodItem.Quantity = cartItem.Quantity;
                    foodItem.TaxableAmount = cartItem.TaxableAmount;
                    foodItem.Amount = cartItem.Amount;
                }
            }

            foreach (var category in categories)
            {
                var categorizedFoodItem = new CategorizedFoodItemModel();

                var foodItemsByCategory = foodItems.Where(x => x.CategoryId == category.Id).ToList();

                if (foodItemsByCategory.Count > 0)
                {
                    categorizedFoodItem.Title = category.Name;
                    categorizedFoodItem.Data = foodItemsByCategory;

                    categorizedFoodItems.Add(categorizedFoodItem);
                }
            }

            return categorizedFoodItems;
        }

        //public async Task<RestaurantMenuModel> GetRestaurantMenu(int userId, int restaurantId)
        //{
        //    RestaurantMenuModel menu = new RestaurantMenuModel();
        //    List<RestaurantMenuFoodModel> menuFoods = new List<RestaurantMenuFoodModel>();
        //    int totalQuantity = 0; decimal totalAmount = 0;

        //    var restaurant = await _context.Restaurants.Where(x => x.Id == restaurantId).Select(x => new RestaurantModel
        //    {
        //        Id = x.Id,
        //        Name = x.Name,
        //        Photo = x.Photo,
        //        Locality = x.Locality,
        //        City = x.City,
        //        Cuisine = x.Cuisine
        //    }).FirstOrDefaultAsync();

        //    if (restaurant != null)
        //    {
        //        var foodItems = _context.FoodItems.Where(x => x.RestaurantId == restaurantId)
        //                                .Select(x => new FoodItemModel
        //                                {
        //                                    Id = x.Id,
        //                                    Name = x.Name,
        //                                    Description = x.Description,
        //                                    Type = x.Type,
        //                                    Photo = x.Photo,
        //                                    Price = x.Price
        //                                }).ToList();

        //        var cartItems = (from m in _context.Carts
        //                         join n in _context.CartItems on m.Id equals n.CartId
        //                         join o in _context.FoodItems on n.FoodItemId equals o.Id
        //                         where m.UserId == userId
        //                         select new CartItemModel
        //                         {
        //                             FoodItemId = n.FoodItemId,
        //                             FoodName = o.Name,
        //                             Price = o.Price,
        //                             Amount = n.Quantity * o.Price,
        //                             Quantity = n.Quantity
        //                         }).ToList();

        //        foreach (var foodItem in foodItems)
        //        {
        //            RestaurantMenuFoodModel menuFood = new RestaurantMenuFoodModel();
        //            var cartItem = cartItems.Where(x => x.FoodItemId == foodItem.Id && x.Quantity > 0).FirstOrDefault();

        //            if (cartItem != null)
        //            {
        //                menuFood.Amount = cartItem.Quantity * foodItem.Price;
        //                menuFood.Quantity = cartItem.Quantity;
        //            }

        //            menuFood.FoodItem = foodItem;

        //            menuFoods.Add(menuFood);
        //        }

        //        menu.Restaurant = restaurant;
        //        menu.FoodItems = menuFoods;
        //        menu.TotalQuantity = cartItems.Sum(x => x.Quantity);
        //        menu.TotalAmount = cartItems.Sum(x => x.Amount);
        //    }

        //    return menu;
        //}

        //public async Task<RestaurantMenuModel> FilterRestaurantMenu(int userId, int restaurantId, string searchText)
        //{
        //    RestaurantMenuModel menu = new RestaurantMenuModel();
        //    List<RestaurantMenuFoodModel> menuFoods = new List<RestaurantMenuFoodModel>();
        //    int totalQuantity = 0; decimal totalAmount = 0;

        //    var restaurant = await _context.Restaurants.Where(x => x.Id == restaurantId).Select(x => new RestaurantModel
        //    {
        //        Id = x.Id,
        //        Name = x.Name,
        //        Photo = x.Photo,
        //        Locality = x.Locality,
        //        City = x.City,
        //        Cuisine = x.Cuisine
        //    }).FirstOrDefaultAsync();

        //    if (restaurant != null)
        //    {
        //        var foodItems = _context.FoodItems.Where(x => x.RestaurantId == restaurantId && x.Name.Contains(searchText))
        //                                .Select(x => new FoodItemModel
        //                                {
        //                                    Id = x.Id,
        //                                    Name = x.Name,
        //                                    Description = x.Description,
        //                                    Type = x.Type,
        //                                    Photo = x.Photo,
        //                                    Price = x.Price
        //                                }).ToList();

        //        var cartItems = (from m in _context.Carts
        //                         join n in _context.CartItems on m.Id equals n.CartId
        //                         join o in _context.FoodItems on n.FoodItemId equals o.Id
        //                         where m.UserId == userId
        //                         select new CartItemModel
        //                         {
        //                             FoodItemId = n.FoodItemId,
        //                             FoodName = o.Name,
        //                             Price = o.Price,
        //                             Amount = n.Quantity * o.Price,
        //                             Quantity = n.Quantity
        //                         }).ToList();

        //        foreach (var foodItem in foodItems)
        //        {
        //            RestaurantMenuFoodModel menuFood = new RestaurantMenuFoodModel();
        //            var cartItem = cartItems.Where(x => x.FoodItemId == foodItem.Id && x.Quantity > 0).FirstOrDefault();

        //            if (cartItem != null)
        //            {
        //                menuFood.Amount = cartItem.Quantity * foodItem.Price;
        //                menuFood.Quantity = cartItem.Quantity;
        //            }

        //            menuFood.FoodItem = foodItem;

        //            menuFoods.Add(menuFood);
        //        }

        //        menu.Restaurant = restaurant;
        //        menu.FoodItems = menuFoods;
        //        menu.TotalQuantity = cartItems.Sum(x => x.Quantity);
        //        menu.TotalAmount = cartItems.Sum(x => x.Amount);
        //    }

        //    return menu;
        //}

        /*
         
        SELECT [Id], [Name], [Photo], [LegalName], [AddressLine1], [AddressLine2], [Locality], [City], [Postcode], [Cuisine], [Latitude], [Longitude], ROUND([Distance], 2) AS [Distance]
        FROM (
	        SELECT z.[Id], z.[Name], z.[Photo], z.[LegalName], z.[AddressLine1], z.[AddressLine2], z.[Locality], z.[City], z.[Postcode], z.[Cuisine], z.[Latitude], z.[Longitude], p.[Radius],
		           p.[DistanceUnit]
				        * DEGREES(ACOS(LEAST(1.0, COS(RADIANS(p.[LatPoint]))
                        * COS(RADIANS(z.[Latitude]))
                        * COS(RADIANS(p.[LongPoint] - z.[Longitude]))
                        + SIN(RADIANS(p.[LatPoint]))
                        * SIN(RADIANS(z.[Latitude]))))) AS [Distance]
	        FROM [Restaurants] AS z
	        JOIN (
		        SELECT 13.089877255747481 AS [LatPoint], 
		               80.19443538203495 AS [LongPoint],
			           50.0 AS [Radius],
			           111.045 AS [DistanceUnit]
            ) AS p ON 1=1
	        WHERE z.[Latitude] BETWEEN p.[LatPoint] - (p.[Radius] / p.[DistanceUnit]) AND 
	                                  p.[LatPoint] + (p.[Radius] / p.[DistanceUnit]) AND
		          z.[Longitude] BETWEEN p.[LongPoint] - (p.[Radius] / (p.[DistanceUnit] * COS(RADIANS(p.[LatPoint])))) AND 
							           p.[LongPoint] + (p.[Radius] / (p.[DistanceUnit] * COS(RADIANS(p.[LatPoint]))))
        ) AS d
        WHERE [Distance] <= [Radius]
        ORDER BY [Distance]

        */

        public async Task<List<RestaurantModel>> GetRestaurants()
        {
            var restaurants = await _context.Database.SqlQuery<RestaurantModel>($"SELECT [Id], [Name], [Photo], [LegalName], [AddressLine1], [AddressLine2], [Locality], [City], [Postcode], [Cuisine], [Latitude], [Longitude], ROUND([Distance], 2) AS [Distance] FROM (SELECT z.[Id], z.[Name], z.[Photo], z.[LegalName], z.[AddressLine1], z.[AddressLine2], z.[Locality], z.[City], z.[Postcode], z.[Cuisine], z.[Latitude], z.[Longitude], p.[Radius], p.[DistanceUnit] * DEGREES(ACOS(LEAST(1.0, COS(RADIANS(p.[LatPoint])) * COS(RADIANS(z.[Latitude])) * COS(RADIANS(p.[LongPoint] - z.[Longitude])) + SIN(RADIANS(p.[LatPoint])) * SIN(RADIANS(z.[Latitude]))))) AS [Distance] FROM [Restaurants] AS z JOIN (SELECT 13.089877255747481 AS [LatPoint], 80.19443538203495 AS [LongPoint], 50.0 AS [Radius], 111.045 AS [DistanceUnit]) AS p ON 1=1 WHERE z.[Latitude] BETWEEN p.[LatPoint] - (p.[Radius] / p.[DistanceUnit]) AND  p.[LatPoint] + (p.[Radius] / p.[DistanceUnit]) AND z.[Longitude] BETWEEN p.[LongPoint] - (p.[Radius] / (p.[DistanceUnit] * COS(RADIANS(p.[LatPoint])))) AND p.[LongPoint] + (p.[Radius] / (p.[DistanceUnit] * COS(RADIANS(p.[LatPoint]))))) AS d WHERE [Distance] <= [Radius] ORDER BY [Distance]").ToListAsync();
            return restaurants;
        }
    }
}
