using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using RestaurantPOS.Services;
using RestaurantPOS.WpfApp.MVVM;

namespace RestaurantPOS.WpfApp.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private readonly IDashboardService _dashboardService = new DashboardService();

    public ISeries[] Series { get; private set; } = [];
    public Axis[] XAxes { get; private set; } = [];

    public ObservableCollection<TopSellerLine> TopSellers { get; } = new();

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

    public RelayCommand RefreshCommand { get; }

    public DashboardViewModel()
    {
        RefreshCommand = new RelayCommand(_ => Load());
        Load();
    }

    private void Load()
    {
        var toExclusive = ToDate.AddDays(1).AddTicks(-1);
        var revenue = _dashboardService.GetDailyRevenue(FromDate, toExclusive);

        Series = new ISeries[]
        {
            new ColumnSeries<decimal>
            {
                Values = revenue.Select(r => r.Revenue).ToArray(),
                Name = "Doanh thu"
            }
        };
        XAxes = new[]
        {
            new Axis { Labels = revenue.Select(r => r.Date.ToString("dd/MM")).ToArray() }
        };
        OnPropertyChanged(nameof(Series));
        OnPropertyChanged(nameof(XAxes));

        TopSellers.Clear();
        foreach (var line in _dashboardService.GetTopSellers(FromDate, toExclusive))
        {
            TopSellers.Add(line);
        }
    }
}
