using Microsoft.EntityFrameworkCore;
using Shopper.Data;

namespace Shopper.Infrastructure
{
    public class UserService : IUserService
    {
        private readonly ShopperContext _context;

        public UserService(ShopperContext context)
        {
            _context = context;
        }

        public async Task<int> GetUserId(string email)
        {
            var userId = await _context.Users.Where(x => x.Email == email).Select(x => x.Id).FirstOrDefaultAsync();
            return userId;
        }

        public async Task<User> GetUser(int userId)
        {
            var user = await _context.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
            return user;
        }

        public async Task<bool> Update(int userId, string firstName, string lastName)
        {
            var user = await _context.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();

            user.FirstName = firstName;
            user.LastName = lastName;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<UserCoordinatesModel> GetCoordinates(int userId)
        {
            var coordinates = await _context.Users.Where(x => x.Id == userId).Select(x => new UserCoordinatesModel { Latitude = x.Latitude, Longitude = x.Longitude }).FirstOrDefaultAsync();
            return coordinates;
        }

        public async Task<bool> SetCoordinates(int userId, double latitude, double longitude)
        {
            var user = await _context.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();

            user.Latitude = latitude;
            user.Longitude = longitude;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
