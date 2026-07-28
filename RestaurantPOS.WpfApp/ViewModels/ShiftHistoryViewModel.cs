using System.Collections.ObjectModel;
using System.Diagnostics;
using RestaurantPOS.DataAccessObjects;
using RestaurantPOS.Services;
using RestaurantPOS.WpfApp.MVVM;
using RestaurantPOS.WpfApp.Reports;

namespace RestaurantPOS.WpfApp.ViewModels;

public class ShiftHistoryViewModel : ViewModelBase
{
    private readonly IShiftService _shiftService = new ShiftService();

    public ObservableCollection<ShiftReport> Shifts { get; } = new();

    private ShiftReport? _selectedShift;
    public ShiftReport? SelectedShift
    {
        get => _selectedShift;
        set => SetField(ref _selectedShift, value);
    }

    private DateTime _fromDate = DateTime.Today.AddDays(-6);
    public DateTime FromDate
    {
        get => _fromDate;
        set => SetField(ref _fromDate, value);
    }

    private DateTime _toDate = DateTime.Today;
    public DateTime ToDate
    {
        get => _toDate;
        set => SetField(ref _toDate, value);
    }

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetField(ref _errorMessage, value);
    }

    public RelayCommand RefreshCommand { get; }
    public RelayCommand PrintCommand { get; }

    public ShiftHistoryViewModel()
    {
        RefreshCommand = new RelayCommand(_ => Load());
        PrintCommand = new RelayCommand(_ => Print(), _ => SelectedShift != null);
        Load();
    }

    // Shifts are bucketed by their opening day (see ShiftDAO.GetClosedShifts) — a
    // shift opened late at night and closed after midnight stays under the day it opened.
    private void Load()
    {
        var toExclusive = ToDate.AddDays(1).AddTicks(-1);
        Shifts.Clear();
        foreach (var shift in _shiftService.GetShiftHistory(FromDate, toExclusive))
        {
            Shifts.Add(shift);
        }
    }

    private void Print()
    {
        if (SelectedShift == null) return;
        try
        {
            var pdfPath = ShiftReportBuilder.BuildPdf(SelectedShift);
            Process.Start(new ProcessStartInfo(pdfPath) { UseShellExecute = true });
            ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            AppLogger.LogError($"{nameof(ShiftHistoryViewModel)}.{nameof(Print)}", ex);
            ErrorMessage = "Không thể in báo cáo ca.";
        }
    }
}
