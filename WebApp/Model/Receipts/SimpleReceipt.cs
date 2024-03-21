namespace WebApp.Model.Receipts;

public class SimpleReceipt
{
    public SimpleReceipt(Receipt receipt)
    {
        Items = new List<SimpleItem>(receipt.Items.Select(c => new SimpleItem(c)));
        ShopName = receipt.ShopName;
        ShopAddress = receipt.ShopAddress;
        DateTime = receipt.DateTime;
        TotalPrice = (int)receipt.TotalPrice;
        Category = receipt.Category;
        OtherValues = receipt.OtherValues;
    }
    public List<SimpleItem> Items { get; set; }
    public string ShopName { get; set; }
    public string ShopAddress { get; set; }
    public DateTime DateTime { get; set; }
    public double TotalPrice { get; set; }
    public string Category { get; set; }
    public Dictionary<string, string> OtherValues { get; set; }
}