namespace Shopper.Infrastructure
{
    public class OfferModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Value { get; set; }
        public decimal TaxableAmount { get; set; }
        public decimal DeductableAmount { get; set; }
        public decimal TotalTaxableAmount { get; set; }
        public bool IsValid { get; set; }
    }
}
