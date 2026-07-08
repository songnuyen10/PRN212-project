using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Repositories;

namespace RestaurantPOS.Tests;

public class FakeRestaurantTableRepository : IRestaurantTableRepository
{
    private readonly Dictionary<int, RestaurantTable> _tables = new();

    public void Seed(RestaurantTable table) => _tables[table.TableId] = table;

    public List<RestaurantTable> GetTables() => _tables.Values.ToList();

    public RestaurantTable? GetTableById(int tableId) => _tables.GetValueOrDefault(tableId);

    public bool SaveTable(RestaurantTable table) => true;

    public bool UpdateTable(RestaurantTable table) => true;

    public bool UpdateTableStatus(int tableId, TableStatus status)
    {
        if (_tables.TryGetValue(tableId, out var table)) table.Status = status;
        return true;
    }
}
