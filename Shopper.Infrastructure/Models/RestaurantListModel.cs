namespace Shopper.Infrastructure
{
    public class RestaurantListModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public string Locality { get; set; }
        public string City { get; set; }
        public string Cuisine { get; set; }
        public double Distance { get; set; }
    }
}
