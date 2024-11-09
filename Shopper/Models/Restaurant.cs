namespace Shopper.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public string LegalName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Locality { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public string Cuisine { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
    public class FoodItem
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string? Photo { get; set; }
        public decimal Price { get; set; }
        public Restaurant Restaurant { get; set; }
    }
}
