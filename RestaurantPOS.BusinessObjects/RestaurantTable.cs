namespace RestaurantPOS.BusinessObjects;

public class RestaurantTable
{
    public int TableId { get; set; }
    public string TableName { get; set; } = null!;
    public int Capacity { get; set; }
    public TableStatus Status { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public override string ToString() => TableName;
}
