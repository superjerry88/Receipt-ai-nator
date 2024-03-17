namespace WebApp.Model.Receipts
{
    public class ScanResult
    {
        public bool IsReceiptVisible { get; set; }
        public List<Receipt> Receipts { get; set; } = new List<Receipt>();
        public string Remarks { get; set; } //Put any remarks here like assumptions made, etc.
    }
    public class Receipt
    {
        public List<Items> Items { get; set; }
        public string ShopName { get; set; }
        public string ShopAddress { get; set; }
        public string ReceiptId { get; set; } //Empty if not applicable
        public DateTime DateTime { get; set; }
        public string NameOnReceipt { get; set; }

        public string OperatorName { get; set; }
        public int ReturnPeriod { get; set; } //0 if not applicable

        public double IsTaxShown { get; set; }
        public double TotalTax { get; set; }
        public double TotalPrice { get; set; }
        public double TotalPriceWithoutTax { get; set; }
        public Dictionary<string, string> OtherValues { get; set; }
    }
    public class Items
    {
        public string? Name { get; set; } //Put original name here without translating
        public string? ProductCode { get; set; } //Put if applicable
        public string? Category { get; set; } //Used to predefine list
        public string? Description { get; set; } //Generate a short description for the given name.
        public double PricePerItem { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice => PricePerItem * Quantity;
        public bool TaxInclusive { get; set; }
        public double Confidence { get; set; } // 0.0 = no confidence, 0.25 = low confidence, 0.75 = high confidence ,1.0 = full confidence
    }
}
