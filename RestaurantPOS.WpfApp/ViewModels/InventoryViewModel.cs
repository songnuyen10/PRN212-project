using System.Collections.ObjectModel;
using System.Windows;
using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Services;
using RestaurantPOS.WpfApp.MVVM;

namespace RestaurantPOS.WpfApp.ViewModels;

public class InventoryViewModel : ViewModelBase
{
    private readonly IIngredientService _ingredientService = new IngredientService();

    private ObservableCollection<Ingredient> _ingredients = new();
    public ObservableCollection<Ingredient> Ingredients
    {
        get => _ingredients;
        set => SetField(ref _ingredients, value);
    }

    private Ingredient? _selectedIngredient;
    public Ingredient? SelectedIngredient
    {
        get => _selectedIngredient;
        set
        {
            SetField(ref _selectedIngredient, value);
            if (value != null)
            {
                InputName = value.IngredientName;
                InputUnit = value.Unit;
                InputQuantity = value.QuantityInStock;
                InputThreshold = value.LowStockThreshold;
            }
        }
    }

    private string _inputName = string.Empty;
    public string InputName { get => _inputName; set => SetField(ref _inputName, value); }

    private string _inputUnit = string.Empty;
    public string InputUnit { get => _inputUnit; set => SetField(ref _inputUnit, value); }

    private decimal _inputQuantity;
    public decimal InputQuantity { get => _inputQuantity; set => SetField(ref _inputQuantity, value); }

    private decimal _inputThreshold;
    public decimal InputThreshold { get => _inputThreshold; set => SetField(ref _inputThreshold, value); }

    public RelayCommand AddCommand { get; }
    public RelayCommand UpdateCommand { get; }
    public RelayCommand DeleteCommand { get; }

    public InventoryViewModel()
    {
        AddCommand = new RelayCommand(_ => Add(), _ => !string.IsNullOrWhiteSpace(InputName) && SelectedIngredient == null);
        UpdateCommand = new RelayCommand(_ => Update(), _ => SelectedIngredient != null);
        DeleteCommand = new RelayCommand(_ => Delete(), _ => SelectedIngredient != null);

        Load();
    }

    private void Load()
    {
        Ingredients = new ObservableCollection<Ingredient>(_ingredientService.GetIngredients());
    }

    private void Add()
    {
        var ingredient = new Ingredient
        {
            IngredientName = InputName,
            Unit = InputUnit,
            QuantityInStock = InputQuantity,
            LowStockThreshold = InputThreshold
        };
        _ingredientService.SaveIngredient(ingredient);
        ResetInput();
        Load();
    }

    private void Update()
    {
        SelectedIngredient!.IngredientName = InputName;
        SelectedIngredient.Unit = InputUnit;
        SelectedIngredient.QuantityInStock = InputQuantity;
        SelectedIngredient.LowStockThreshold = InputThreshold;
        _ingredientService.UpdateIngredient(SelectedIngredient);
        ResetInput();
        Load();
    }

    private void Delete()
    {
        var confirm = MessageBox.Show($"Xóa nguyên liệu '{SelectedIngredient!.IngredientName}'?", "Xác nhận",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirm != MessageBoxResult.Yes) return;

        _ingredientService.DeleteIngredient(SelectedIngredient!.IngredientId);
        ResetInput();
        Load();
    }

    private void ResetInput()
    {
        SelectedIngredient = null;
        InputName = string.Empty;
        InputUnit = string.Empty;
        InputQuantity = 0;
        InputThreshold = 0;
    }
}
