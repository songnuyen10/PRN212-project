using System.Windows.Threading;
using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Services;
using RestaurantPOS.WpfApp.MVVM;

namespace RestaurantPOS.WpfApp.ViewModels;

public class ShiftViewModel : ViewModelBase
{
    private readonly IShiftService _shiftService = new ShiftService();
    private readonly IUserService _userService = new UserService();
    private readonly int _userId = SessionContext.CurrentUser!.UserId;
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };

    public string UserFullName { get; } = SessionContext.CurrentUser!.FullName;

    private Shift? _openShift;
    public Shift? OpenShift
    {
        get => _openShift;
        private set => SetField(ref _openShift, value);
    }

    public bool HasOpenShift => OpenShift != null;

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetField(ref _errorMessage, value);
    }

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

    private string _elapsedText = string.Empty;
    public string ElapsedText { get => _elapsedText; private set => SetField(ref _elapsedText, value); }

    private string _scheduledStartInput;
    public string ScheduledStartInput { get => _scheduledStartInput; set => SetField(ref _scheduledStartInput, value); }

    private string _scheduledEndInput;
    public string ScheduledEndInput { get => _scheduledEndInput; set => SetField(ref _scheduledEndInput, value); }

    private bool _hasSummary;
    public bool HasSummary { get => _hasSummary; private set => SetField(ref _hasSummary, value); }

    private ShiftSummary? _lastClosedSummary;
    public ShiftSummary? LastClosedSummary
    {
        get => _lastClosedSummary;
        private set => SetField(ref _lastClosedSummary, value);
    }

    public bool ShowNoShiftPanel => !HasOpenShift && !HasSummary;

    public RelayCommand OpenShiftCommand { get; }
    public RelayCommand CloseShiftCommand { get; }
    public RelayCommand SaveScheduledHoursCommand { get; }
    public RelayCommand DismissSummaryCommand { get; }

    public ShiftViewModel()
    {
        var user = SessionContext.CurrentUser!;
        _scheduledStartInput = user.ScheduledStartTime?.ToString(@"hh\:mm") ?? "";
        _scheduledEndInput = user.ScheduledEndTime?.ToString(@"hh\:mm") ?? "";

        OpenShiftCommand = new RelayCommand(_ => OpenNewShift(), _ => !HasOpenShift);
        CloseShiftCommand = new RelayCommand(_ => CloseCurrentShift(), _ => HasOpenShift);
        SaveScheduledHoursCommand = new RelayCommand(_ => SaveScheduledHours());
        DismissSummaryCommand = new RelayCommand(_ => DismissSummary());

        _timer.Tick += (_, _) => UpdateElapsedText();

        Load();
    }

    private void Load()
    {
        OpenShift = _shiftService.GetOpenShift(_userId);
        Reconciliation = OpenShift == null ? null : _shiftService.GetReconciliation(OpenShift.ShiftId);
        _timer.IsEnabled = HasOpenShift;
        UpdateElapsedText();
        OnPropertyChanged(nameof(HasOpenShift));
        OnPropertyChanged(nameof(Variance));
        OnPropertyChanged(nameof(ShowNoShiftPanel));
    }

    // TimeSpan's "hh" format specifier is hours-within-day (0-23), not total
    // hours — a shift left open past 24h would silently lose its day count.
    private void UpdateElapsedText()
    {
        if (OpenShift == null)
        {
            ElapsedText = string.Empty;
            return;
        }
        var elapsed = DateTime.Now - OpenShift.OpenedAt;
        ElapsedText = $"{(int)elapsed.TotalHours:D2}:{elapsed:mm\\:ss}";
    }

    public void StopTimer() => _timer.Stop();

    private void OpenNewShift()
    {
        if (!_shiftService.OpenShift(_userId, OpeningCash))
        {
            ErrorMessage = "Không thể mở ca — có thể bạn đã có một ca đang mở.";
            Load();
            return;
        }
        ErrorMessage = string.Empty;
        OpeningCash = 0;
        Load();
    }

    private void CloseCurrentShift()
    {
        var openShift = OpenShift!;
        if (!_shiftService.CloseShift(openShift.ShiftId, ClosingCash))
        {
            ErrorMessage = "Không thể đóng ca — vui lòng thử lại.";
            Load();
            return;
        }

        var user = SessionContext.CurrentUser!;
        LastClosedSummary = new ShiftSummary
        {
            UserFullName = user.FullName,
            OpeningCash = openShift.OpeningCash,
            ClosingCash = ClosingCash,
            OpenedAt = openShift.OpenedAt,
            ClosedAt = DateTime.Now,
            ScheduledStart = user.ScheduledStartTime,
            ScheduledEnd = user.ScheduledEndTime
        };
        HasSummary = true;
        ErrorMessage = string.Empty;
        ClosingCash = 0;
        Load();
    }

    private void DismissSummary()
    {
        LastClosedSummary = null;
        HasSummary = false;
        OnPropertyChanged(nameof(ShowNoShiftPanel));
    }

    private void SaveScheduledHours()
    {
        if (!TimeSpan.TryParse(ScheduledStartInput, out var start) || !TimeSpan.TryParse(ScheduledEndInput, out var end)
            || start >= TimeSpan.FromDays(1) || end >= TimeSpan.FromDays(1))
        {
            ErrorMessage = "Giờ không hợp lệ — nhập theo định dạng HH:mm.";
            return;
        }
        if (end <= start)
        {
            ErrorMessage = "Giờ kết thúc phải sau giờ bắt đầu.";
            return;
        }
        if (!_userService.UpdateScheduledHours(_userId, start, end))
        {
            ErrorMessage = "Không thể lưu giờ ca quy định.";
            return;
        }
        SessionContext.CurrentUser!.ScheduledStartTime = start;
        SessionContext.CurrentUser!.ScheduledEndTime = end;
        ErrorMessage = string.Empty;
    }
}
