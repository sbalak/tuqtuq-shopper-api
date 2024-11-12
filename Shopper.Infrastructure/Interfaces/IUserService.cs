using Shopper.Data;

namespace Shopper.Infrastructure
{
    public interface IUserService
    {
        Task<int> GetUserId(string email);
        Task<User> GetUser(int userId);
        Task<bool> Update(int userId, string firstName, string lastName);
        Task<UserCoordinatesModel> GetCoordinates(int userId);
        Task<bool> SetCoordinates(int userId, double latitude, double longitude);
    }
}
