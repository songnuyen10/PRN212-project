using RestaurantPOS.BusinessObjects;
using RestaurantPOS.WpfApp.MVVM;

namespace RestaurantPOS.WpfApp.ViewModels;

// A menu item picked in the cart but not yet sent to the kitchen (not persisted).
public class CartLine : ViewModelBase
{
    public MenuItem MenuItem { get; set; } = null!;

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set
        {
            SetField(ref _quantity, value);
            OnPropertyChanged(nameof(Subtotal));
        }
    }

    public decimal Subtotal => MenuItem.Price * Quantity;
}
