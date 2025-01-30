using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopper.Infrastructure;

namespace Shopper.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OfferController : ControllerBase
    {
        private IOfferService _offer;

        public OfferController(IOfferService offer)
        {
            _offer = offer;
        }

        [AllowAnonymous]
        [HttpGet("List")]
        public async Task<List<OfferModel>> List(decimal cartValue, int restaurantId)
        {
            var offers = await _offer.GetOffers(cartValue, restaurantId);
            return offers;
        }

        [AllowAnonymous]
        [HttpGet("Match")]
        public async Task<OfferModel> Match(decimal cartValue, int restaurantId)
        {
            var offer = await _offer.GetSuitableOffer(cartValue, restaurantId);
            return offer;
        }
    }
}
