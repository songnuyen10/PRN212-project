using Microsoft.EntityFrameworkCore;
using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.DataAccessObjects;

public class ShiftDAO
{
    public static Shift? GetOpenShift(int userId)
    {
        using var context = new AppDbContext();
        return context.Shifts.FirstOrDefault(s => s.UserId == userId && s.ClosedAt == null);
    }

    // Shifts are bucketed by OpenedAt, not ClosedAt — a shift opened 23:00 and closed
    // 02:00 the next day belongs to the day it opened, consistent with how OpenedAt
    // is used as the primary timestamp elsewhere on Shift.
    public static List<Shift> GetClosedShifts(DateTime from, DateTime to)
    {
        using var context = new AppDbContext();
        return context.Shifts
            .Include(s => s.User)
            .Include(s => s.Payments)
            .Where(s => s.ClosedAt != null && s.OpenedAt >= from && s.OpenedAt <= to)
            .OrderByDescending(s => s.OpenedAt)
            .ToList();
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
            AppLogger.LogError($"{nameof(ShiftDAO)}.{nameof(OpenShift)}", ex);
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
            AppLogger.LogError($"{nameof(ShiftDAO)}.{nameof(CloseShift)}", ex);
            return false;
        }
    }
}
