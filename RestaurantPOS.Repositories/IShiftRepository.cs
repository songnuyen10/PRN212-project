using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.Repositories;

public interface IShiftRepository
{
    Shift? GetOpenShift(int userId);
    Shift? GetShiftById(int shiftId);
    bool OpenShift(int userId, decimal openingCash);
    bool CloseShift(int shiftId, decimal closingCash);
}
