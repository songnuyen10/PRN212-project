namespace RestaurantPOS.BusinessObjects;

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public byte[] PasswordHash { get; set; } = null!;
    public byte[] PasswordSalt { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public UserRole Role { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();
}
