using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Repositories;

namespace RestaurantPOS.Tests;

public class FakeShiftRepository : IShiftRepository
{
    private readonly Dictionary<int, Shift> _shifts = new();

    public void Seed(Shift shift) => _shifts[shift.ShiftId] = shift;

    public Shift? GetOpenShift(int userId) => _shifts.Values.FirstOrDefault(s => s.UserId == userId && s.ClosedAt == null);

    public Shift? GetShiftById(int shiftId) => _shifts.GetValueOrDefault(shiftId);

    public bool OpenShift(int userId, decimal openingCash) => true;

    public bool CloseShift(int shiftId, decimal closingCash) => true;
}
