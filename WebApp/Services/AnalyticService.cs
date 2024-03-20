using WebApp.Model;
using WebApp.Model.Receipts;

namespace WebApp.Services;

public class AnalyticService
{
    public Task<Analytic> GetAnalytics(User user)
    {
        var jobsOfUser = RezApi.Jobs.GetJobsByUser(user);
        var jobs = jobsOfUser.Where(x => x.IsCompleted).ToList();
        List<ScanResult> results = jobs.Where(x => x.Result!=null).Select(x => x.Result).ToList()!;
        var receipts = results.Where(x => x.Receipts.Any()).SelectMany(x => x.Receipts).ToList();

        var analytic = new Analytic()
        {
            UserId = user.Id,
            TotalScans = jobsOfUser.Count,

            TotalReceiptsAllTime = receipts.Count,
            TotalReceiptsThisMonth = receipts.Count(x => x.DateTime.Month == DateTime.Now.Month),
            TotalReceiptsToday = receipts.Count(x => x.DateTime.Date == DateTime.Now.Date),

            TotalSpentAllTime = receipts.Sum(x => x.TotalPrice),
            TotalSpentThisMonth = receipts.Where(x => x.DateTime.Month == DateTime.Now.Month).Sum(x => x.TotalPrice),
            TotalSpentToday = receipts.Where(x => x.DateTime.Date == DateTime.Now.Date).Sum(x => x.TotalPrice),

            TotalTaxPaidAllTime = receipts.Sum(x => x.TotalTax),
            TotalTaxPaidThisMonth = receipts.Where(x => x.DateTime.Month == DateTime.Now.Month).Sum(x => x.TotalTax),
            TotalTaxPaidToday = receipts.Where(x => x.DateTime.Date == DateTime.Now.Date).Sum(x => x.TotalTax),

            CategorySpendingAllTime = receipts.GroupBy(x => x.Category).ToDictionary(x => x.Key, x => x.Sum(x => x.TotalPrice)),
            CategorySpendingThisMonth = receipts.Where(x => x.DateTime.Month == DateTime.Now.Month).GroupBy(x => x.Category).ToDictionary(x => x.Key, x => x.Sum(x => x.TotalPrice)),
            TopSpendingAllTime = receipts.OrderByDescending(x => x.TotalPrice).Take(5).ToList(),
            TopSpendingThisMonth = receipts.Where(x => x.DateTime.Month == DateTime.Now.Month).OrderByDescending(x => x.TotalPrice).Take(5).ToList()
        };
        return Task.FromResult(analytic);
    }
}

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
    public Dictionary<string,double> CategorySpendingThisMonth { get; set; } = new Dictionary<string, double>();

    public List<Receipt> TopSpendingAllTime { get; set; } = new List<Receipt>();
    public List<Receipt> TopSpendingThisMonth { get; set; } = new List<Receipt>();
}