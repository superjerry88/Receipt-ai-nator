namespace HackathonReceiptScanner
{
    public class Receipt
    {
        public List<Items> Items { get; set; }
        public string ShopName { get; set; }
        public string ShopRegisteredNumber { get; set; }
        public string Address { get; set; }
        public DateTime DateTime { get; set; }

        public string OperatorName { get; set; }
        public int ReturnPeriod { get; set; }//IF valid

        public double IsTaxShown { get; set; }
        public double TaxPercentage { get; set; }
        public double TotalTax { get; set; }
        public double TotalPrice { get; set; }
        public double TotalPriceWithoutTax { get; set; }
        public Dictionary<string,string> OtherValues { get; set; }
    }
    public class Items
    {
        public string? Name { get; set; }
        public string? Category { get; set; } //Food, Clothing, Service ...
        public string? Description { get; set; } //GPT created description
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice => Price * Quantity;
        public bool TaxInclusive {get; set; }
    }
}
