namespace WebApp.Model.Receipts;

public class SimpleItem
{
    public SimpleItem(Items item)
    {
        Name = item.Name!;
        Quantity = item.Quantity;
        TotalPrice = item.TotalPrice;
    }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public double TotalPrice  { get; set; }
}