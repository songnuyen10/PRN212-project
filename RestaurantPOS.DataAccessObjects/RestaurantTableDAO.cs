using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.DataAccessObjects;

public class RestaurantTableDAO
{
    public static List<RestaurantTable> GetTables()
    {
        using var context = new AppDbContext();
        return context.RestaurantTables.OrderBy(t => t.TableName).ToList();
    }

    public static RestaurantTable? GetTableById(int tableId)
    {
        using var context = new AppDbContext();
        return context.RestaurantTables.FirstOrDefault(t => t.TableId == tableId);
    }

    public static bool SaveTable(RestaurantTable table)
    {
        using var context = new AppDbContext();
        try
        {
            context.RestaurantTables.Add(table);
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception (ex) as needed
            return false;
        }
    }

    public static bool UpdateTable(RestaurantTable table)
    {
        using var context = new AppDbContext();
        try
        {
            context.RestaurantTables.Update(table);
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception (ex) as needed
            return false;
        }
    }

    public static bool UpdateTableStatus(int tableId, TableStatus status)
    {
        using var context = new AppDbContext();
        try
        {
            var table = context.RestaurantTables.FirstOrDefault(t => t.TableId == tableId);
            if (table == null) return false;
            table.Status = status;
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception (ex) as needed
            return false;
        }
    }
}
