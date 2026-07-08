using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.Repositories;

public interface IRestaurantTableRepository
{
    List<RestaurantTable> GetTables();
    RestaurantTable? GetTableById(int tableId);
    bool SaveTable(RestaurantTable table);
    bool UpdateTable(RestaurantTable table);
    bool UpdateTableStatus(int tableId, TableStatus status);
}
