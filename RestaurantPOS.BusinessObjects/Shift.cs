namespace RestaurantPOS.BusinessObjects;

public class Shift
{
    public int ShiftId { get; set; }

    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public DateTime OpenedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public decimal OpeningCash { get; set; }
    public decimal? ClosingCash { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
