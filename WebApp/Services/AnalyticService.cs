using WebApp.Model;
using WebApp.Model.Receipts;

namespace WebApp.Services;

public class AnalyticService
{
    public Task<Analytic> GetAnalytics(User user)
    {
        var receipts = RezApi.Jobs.GetReceiptByUser(user);

        Console.WriteLine($"RE: {receipts.FirstOrDefault()?.TaskId} | {receipts.FirstOrDefault()?.ReceiptIndex}");
        var analytic = new Analytic()
        {
            UserId = user.Id,
            TotalScans = receipts.Select(c=>c.TaskId).Distinct().Count(),

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