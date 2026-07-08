using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Repositories;

namespace RestaurantPOS.Services;

public class TableService : ITableService
{
    private readonly IRestaurantTableRepository _tableRepository;
    private readonly IOrderRepository _orderRepository;

    public TableService()
    {
        _tableRepository = new RestaurantTableRepository();
        _orderRepository = new OrderRepository();
    }

    // Test seam only — production code always uses the parameterless constructor.
    public TableService(IRestaurantTableRepository tableRepository, IOrderRepository orderRepository)
    {
        _tableRepository = tableRepository;
        _orderRepository = orderRepository;
    }

    public List<RestaurantTable> GetTables() => _tableRepository.GetTables();

    public bool SaveTable(RestaurantTable table) => _tableRepository.SaveTable(table);

    public bool UpdateTable(RestaurantTable table) => _tableRepository.UpdateTable(table);

    public bool UpdateTableStatus(int tableId, TableStatus status) => _tableRepository.UpdateTableStatus(tableId, status);

    public bool MarkAwaitingPayment(int tableId)
    {
        var openOrder = _orderRepository.GetOpenOrderByTable(tableId);
        if (openOrder == null) return false;

        return _tableRepository.UpdateTableStatus(tableId, TableStatus.AwaitingPayment);
    }
}
