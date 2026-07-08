namespace RestaurantPOS.BusinessObjects;

public class MenuCategory
{
    public int MenuCategoryId { get; set; }
    public string CategoryName { get; set; } = null!;

    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

    public override string ToString() => CategoryName;
}
