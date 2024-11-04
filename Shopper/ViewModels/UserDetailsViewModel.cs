﻿namespace Shopper.ViewModels
{
    public class UserDetailsViewModel
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        double Latitude { get; set; }
        double Longitude { get; set; }
        string UserName { get; set; }
        string Email { get; set; }
        bool EmailConfirmed { get; set; }
        string PhoneNumber { get; set; }
        bool PhoneNumberConfirmed { get; set; }
    }

    public class UserCoordinatesViewModel
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
