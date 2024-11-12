using Microsoft.AspNetCore.Identity;

namespace Shopper.Data
{
    public class User : IdentityUser<int>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    public class UserClaim : IdentityUserClaim<int> { }

    public class UserLogin : IdentityUserLogin<int> { }

    public class UserToken : IdentityUserToken<int> { }

    public class Role : IdentityRole<int> { }

    public class RoleClaim : IdentityRoleClaim<int> { }

    public class UserRole : IdentityUserRole<int> { }
}
