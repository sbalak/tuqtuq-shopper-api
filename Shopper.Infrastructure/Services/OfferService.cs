using Microsoft.EntityFrameworkCore;
using Shopper.Data;
using System.Globalization;

namespace Shopper.Infrastructure
{
    public class OfferService : IOfferService
    {
        private readonly ShopperContext _context;

        public OfferService(ShopperContext context)
        {
            _context = context;
        }

        public async Task<List<OfferModel>> GetOffers(decimal cartValue, int restaurantId)
        {
            List<OfferModel> offers = new List<OfferModel>();
            var promotions = _context
                                .Offers
                                .Where(m => 
                                            m.Type == "promotion" && 
                                            (m.Threshold == null || m.Threshold <= cartValue) && 
                                            m.IsDisabled == false &&
                                            m.RestaurantId == restaurantId)
                                .ToList();

            var discounts = _context
                                .Offers
                                .Where(m => 
                                            m.Type == "discount" &&
                                            (m.Threshold == null || m.Threshold <= cartValue) &&
                                            m.IsDisabled == false &&
                                            m.RestaurantId == restaurantId)
                                .ToList();

            var cashdiscounts = _context
                                .Offers
                                .Where(m => 
                                            m.Type == "cash" &&
                                            m.Threshold <= cartValue &&
                                            m.IsDisabled == false &&
                                            m.RestaurantId == restaurantId)
                                .ToList();

            foreach (var promotion in promotions)
            {
                decimal deduction = (cartValue * ((decimal)promotion.Value / 100));

                OfferModel offer = new OfferModel()
                {
                    Name = promotion.Name,
                    Type = promotion.Type,
                    Value = promotion.Value,
                    TaxableAmount = cartValue,
                    DeductableAmount = deduction,
                    TotalTaxableAmount = cartValue - deduction,
                    IsValid = false,
                };
                
                if (promotion.UpperLimit == null || promotion.UpperLimit >= deduction)
                {
                    offer.IsValid = true;
                }

                offers.Add(offer);
            }

            foreach (var discount in discounts)
            {
                decimal deduction = (cartValue * ((decimal)discount.Value / 100));

                OfferModel offer = new OfferModel()
                {
                    Name = discount.Name,
                    Type = discount.Type,
                    Value = discount.Value,
                    TaxableAmount = cartValue,
                    DeductableAmount = deduction,
                    TotalTaxableAmount = cartValue - deduction,
                    IsValid = false,
                };

                if (discount.UpperLimit == null || discount.UpperLimit >= deduction)
                {
                    offer.IsValid = true;
                }

                offers.Add(offer);
            }

            foreach (var cash in cashdiscounts)
            {
                decimal deduction = (decimal)cash.Value;

                OfferModel offer = new OfferModel()
                {
                    Name = cash.Name,
                    Type = cash.Type,
                    Value = cash.Value,
                    TaxableAmount = cartValue,
                    DeductableAmount = deduction,
                    TotalTaxableAmount = cartValue - deduction,
                    IsValid = true,
                };

                offers.Add(offer);
            }

            return offers;
        }

        public async Task<OfferModel> GetSuitableOffer(decimal cartValue, int restaurantId)
        {
            List<OfferModel> offers = await GetOffers(cartValue, restaurantId);
            OfferModel offer = new OfferModel();

            if (offers.Count > 1)
            {
                offer = offers.Where(m => m.IsValid)
                              .OrderByDescending(m => m.TotalTaxableAmount)
                              .FirstOrDefault();

            }

            return offer;

        }
    }
}
