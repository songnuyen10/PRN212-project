using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.Services;

public interface ITableService
{
    List<RestaurantTable> GetTables();
    bool SaveTable(RestaurantTable table);
    bool UpdateTable(RestaurantTable table);
    bool UpdateTableStatus(int tableId, TableStatus status);

    // Business rule: a table can only be flagged as awaiting payment while it has
    // an open order — not an arbitrary status flip.
    bool MarkAwaitingPayment(int tableId);
}
