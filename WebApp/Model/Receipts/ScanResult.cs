namespace WebApp.Model.Receipts;

public class ScanResult
{
    public bool IsReceiptVisible { get; set; }
    public List<Receipt> Receipts { get; set; } = new List<Receipt>();

    public List<string> Remarks { get; set; } = new List<string>(); //Assumptions,Notes, etc.
}