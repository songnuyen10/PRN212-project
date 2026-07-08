using RestaurantPOS.BusinessObjects;
using RestaurantPOS.DataAccessObjects;

namespace RestaurantPOS.Repositories;

public class RestaurantTableRepository : IRestaurantTableRepository
{
    public List<RestaurantTable> GetTables() => RestaurantTableDAO.GetTables();

    public RestaurantTable? GetTableById(int tableId) => RestaurantTableDAO.GetTableById(tableId);

    public bool SaveTable(RestaurantTable table) => RestaurantTableDAO.SaveTable(table);

    public bool UpdateTable(RestaurantTable table) => RestaurantTableDAO.UpdateTable(table);

    public bool UpdateTableStatus(int tableId, TableStatus status) => RestaurantTableDAO.UpdateTableStatus(tableId, status);
}
