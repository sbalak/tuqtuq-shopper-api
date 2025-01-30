namespace Shopper.Infrastructure
{
    public interface IOfferService
    {
        Task<List<OfferModel>> GetOffers(decimal cartValue, int restaurantId);
        Task<OfferModel> GetSuitableOffer(decimal cartValue, int restaurantId);
    }
}
