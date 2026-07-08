using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.Services;

// Expected cash = what should be in the drawer if nothing went wrong: the opening
// float plus every cash-method payment taken during the shift. Compared against what
// the cashier actually counts (Shift.ClosingCash) to catch over/short at close-out.
public record ShiftReconciliation(decimal OpeningCash, decimal CashPayments, decimal ExpectedCash);

public interface IShiftService
{
    Shift? GetOpenShift(int userId);
    bool OpenShift(int userId, decimal openingCash);
    bool CloseShift(int shiftId, decimal closingCash);
    ShiftReconciliation GetReconciliation(int shiftId);
}
