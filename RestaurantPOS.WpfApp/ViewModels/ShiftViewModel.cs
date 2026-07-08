using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Services;
using RestaurantPOS.WpfApp.MVVM;

namespace RestaurantPOS.WpfApp.ViewModels;

public class ShiftViewModel : ViewModelBase
{
    private readonly IShiftService _shiftService = new ShiftService();
    private readonly int _userId = SessionContext.CurrentUser!.UserId;

    private Shift? _openShift;
    public Shift? OpenShift
    {
        get => _openShift;
        private set => SetField(ref _openShift, value);
    }

    public bool HasOpenShift => OpenShift != null;

    private ShiftReconciliation? _reconciliation;
    public ShiftReconciliation? Reconciliation
    {
        get => _reconciliation;
        private set => SetField(ref _reconciliation, value);
    }

    private decimal _openingCash;
    public decimal OpeningCash { get => _openingCash; set => SetField(ref _openingCash, value); }

    private decimal _closingCash;
    public decimal ClosingCash
    {
        get => _closingCash;
        set
        {
            SetField(ref _closingCash, value);
            OnPropertyChanged(nameof(Variance));
        }
    }

    // Positive = more cash counted than expected, negative = short.
    public decimal? Variance => Reconciliation == null ? null : ClosingCash - Reconciliation.ExpectedCash;

    public RelayCommand OpenShiftCommand { get; }
    public RelayCommand CloseShiftCommand { get; }

    public ShiftViewModel()
    {
        OpenShiftCommand = new RelayCommand(_ => OpenNewShift(), _ => !HasOpenShift);
        CloseShiftCommand = new RelayCommand(_ => CloseCurrentShift(), _ => HasOpenShift);

        Load();
    }

    private void Load()
    {
        OpenShift = _shiftService.GetOpenShift(_userId);
        Reconciliation = OpenShift == null ? null : _shiftService.GetReconciliation(OpenShift.ShiftId);
        OnPropertyChanged(nameof(HasOpenShift));
        OnPropertyChanged(nameof(Variance));
    }

    private void OpenNewShift()
    {
        _shiftService.OpenShift(_userId, OpeningCash);
        OpeningCash = 0;
        Load();
    }

    private void CloseCurrentShift()
    {
        _shiftService.CloseShift(OpenShift!.ShiftId, ClosingCash);
        ClosingCash = 0;
        Load();
    }
}
