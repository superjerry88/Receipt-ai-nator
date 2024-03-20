using WebApp.Model.Receipts;

namespace WebApp.Model;

public class Analytic
{
    public string UserId { get; set; } = "";

    public int TotalScans { get; set; }

    public int TotalReceiptsAllTime { get; set; }
    public int TotalReceiptsThisMonth { get; set; }
    public int TotalReceiptsToday { get; set; }

    public double TotalSpentAllTime { get; set; }
    public double TotalSpentThisMonth { get; set; }
    public double TotalSpentToday { get; set; }

    public double TotalTaxPaidAllTime { get; set; }
    public double TotalTaxPaidThisMonth { get; set; }
    public double TotalTaxPaidToday { get; set; }

    public Dictionary<string, double> CategorySpendingAllTime { get; set; } = new Dictionary<string, double>();
    public Dictionary<string, double> CategorySpendingThisMonth { get; set; } = new Dictionary<string, double>();

    public List<Receipt> TopSpendingAllTime { get; set; } = new List<Receipt>();
    public List<Receipt> TopSpendingThisMonth { get; set; } = new List<Receipt>();
}