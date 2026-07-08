using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.Services;

public record RevenuePoint(DateTime Date, decimal Revenue);
public record TopSellerLine(string ItemName, int QuantitySold, decimal Revenue);

public interface IDashboardService
{
    List<RevenuePoint> GetDailyRevenue(DateTime from, DateTime to);
    List<TopSellerLine> GetTopSellers(DateTime from, DateTime to, int top = 5);
    List<Ingredient> GetLowStockIngredients();
}
