using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.DataAccessObjects;

public class ShiftDAO
{
    public static Shift? GetOpenShift(int userId)
    {
        using var context = new AppDbContext();
        return context.Shifts.FirstOrDefault(s => s.UserId == userId && s.ClosedAt == null);
    }

    public static Shift? GetShiftById(int shiftId)
    {
        using var context = new AppDbContext();
        return context.Shifts.FirstOrDefault(s => s.ShiftId == shiftId);
    }

    public static bool OpenShift(int userId, decimal openingCash)
    {
        using var context = new AppDbContext();
        try
        {
            context.Shifts.Add(new Shift
            {
                UserId = userId,
                OpenedAt = DateTime.Now,
                OpeningCash = openingCash
            });
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception (ex) as needed
            return false;
        }
    }

    public static bool CloseShift(int shiftId, decimal closingCash)
    {
        using var context = new AppDbContext();
        try
        {
            var shift = context.Shifts.FirstOrDefault(s => s.ShiftId == shiftId);
            if (shift == null) return false;
            shift.ClosedAt = DateTime.Now;
            shift.ClosingCash = closingCash;
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
