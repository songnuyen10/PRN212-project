using RestaurantPOS.BusinessObjects;
using RestaurantPOS.DataAccessObjects;

namespace RestaurantPOS.Repositories;

public class ShiftRepository : IShiftRepository
{
    public Shift? GetOpenShift(int userId) => ShiftDAO.GetOpenShift(userId);

    public Shift? GetShiftById(int shiftId) => ShiftDAO.GetShiftById(shiftId);

    public bool OpenShift(int userId, decimal openingCash) => ShiftDAO.OpenShift(userId, openingCash);

    public bool CloseShift(int shiftId, decimal closingCash) => ShiftDAO.CloseShift(shiftId, closingCash);
}
