namespace WebApp.Model.Receipts
{
    public class Items
    {
        public string? Name { get; set; } //Put name as it is in the receipt
        public string? ProductCode { get; set; }
        public string? Category { get; set; } //Used to predefine list
        public string? Description { get; set; } //Generate a short description for the given name.
        public double PricePerItem { get; set; }//0 if not listed.
        public int Quantity { get; set; }
        public double TotalPrice => PricePerItem * Quantity;
        public bool TaxInclusive { get; set; }
        public double Confidence { get; set; } // 0.0 = no confidence, 0.25 = low confidence, 0.5 = middle ,0.75 = high confidence ,1.0 = full confidence
    }
}
