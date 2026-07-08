using RestaurantPOS.Repositories;

namespace RestaurantPOS.Services;

public class DashboardService : IDashboardService
{
    private readonly IPaymentRepository _paymentRepository;

    public DashboardService()
    {
        _paymentRepository = new PaymentRepository();
    }

    // Test seam only — production code always uses the parameterless constructor.
    public DashboardService(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public List<RevenuePoint> GetDailyRevenue(DateTime from, DateTime to)
    {
        var payments = _paymentRepository.GetPaymentsBetween(from, to);
        return payments
            .GroupBy(p => p.PaidAt.Date)
            .OrderBy(g => g.Key)
            .Select(g => new RevenuePoint(g.Key, g.Sum(p => p.AmountPaid)))
            .ToList();
    }

    public List<TopSellerLine> GetTopSellers(DateTime from, DateTime to, int top = 5)
    {
        var payments = _paymentRepository.GetPaymentsBetween(from, to);
        return payments
            .SelectMany(p => p.Order.OrderItems)
            .GroupBy(i => i.MenuItem.ItemName)
            .Select(g => new TopSellerLine(g.Key, g.Sum(i => i.Quantity), g.Sum(i => i.Quantity * i.UnitPrice)))
            .OrderByDescending(l => l.QuantitySold)
            .Take(top)
            .ToList();
    }
}
