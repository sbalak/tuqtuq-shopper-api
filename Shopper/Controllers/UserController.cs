using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopper.Models;
using Shopper.ViewModels;

namespace Shopper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private ShopperContext _context;

        public UserController(ShopperContext context)
        {
            _context = context;
        }

        [HttpGet("Id")]
        public int Id(string email)
        {
            var userId = _context.Users.Where(x => x.Email == email).Select(x => x.Id).FirstOrDefault();
            return userId;
        }

        [HttpGet("Details")]
        public User Details(int userId)
        {
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            return user;
        }

        [HttpPost("Update")]
        public bool Update(int userId, string firstName, string lastName)
        {
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();

            user.FirstName = firstName;
            user.LastName = lastName;

            _context.Users.Update(user);
            _context.SaveChanges();

            return true;
        }

        [HttpGet("Coordinates")]
        public dynamic Coordinates(int userId)
        {
            var coordinates = _context.Users.Where(x => x.Id == userId).Select(x => new UserCoordinatesViewModel { Latitude = x.Latitude, Longitude = x.Longitude }).FirstOrDefault();
            return coordinates;
        }

        [HttpPost("SetCoordinates")]
        public bool SetCoordinates(int userId, double latitude, double longitude)
        {
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();

            user.Latitude = latitude;
            user.Longitude = longitude;

            _context.Users.Update(user);
            _context.SaveChanges();

            return true;
        }
    }
}
