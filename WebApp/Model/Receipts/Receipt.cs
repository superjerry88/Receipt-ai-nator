using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace WebApp.Model.Receipts;

public class Receipt
{
    public List<Items> Items { get; set; } = new();
    public string ShopName { get; set; } = "";
    public string ShopAddress { get; set; } = "";
    public string ReceiptId { get; set; } = "";
    public DateTime DateTime { get; set; }
    public string NameOnReceipt { get; set; } = "";

    public string OperatorName { get; set; } = "";
    public int ReturnPeriod { get; set; }

    public double TotalTax { get; set; }
    public double TotalServiceCharge { get; set; }
    public double TotalDiscount { get; set; }
    public double TotalPriceWithoutTax { get; set; }
    public double TotalPrice { get; set; }
    public string Category { get; set; } = "";
    public Dictionary<string, string> OtherValues { get; set; } = new Dictionary<string, string>();

    [BsonIgnore]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public string TaskId { get; set; } = "";

    [BsonIgnore]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public int ReceiptIndex { get; set; } = 0;
}