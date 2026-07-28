using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.Services;

// Expected cash = what should be in the drawer if nothing went wrong: the opening
// float plus every cash-method payment taken during the shift. Compared against what
// the cashier actually counts (Shift.ClosingCash) to catch over/short at close-out.
public record ShiftReconciliation(decimal OpeningCash, decimal CashPayments, decimal ExpectedCash);

// Full snapshot of a shift for display/printing — the post-close summary panel and
// the admin history grid both bind this. Init-only properties (not a positional
// record): OpeningCash/ClosingCash/CashPayments are all decimal and OpenedAt/ClosedAt
// are both DateTime, so positional args would let a transposition compile silently.
public class ShiftReport
{
    public int ShiftId { get; init; }
    public string UserFullName { get; init; } = "";
    public DateTime OpenedAt { get; init; }
    public DateTime ClosedAt { get; init; }
    public decimal OpeningCash { get; init; }
    public decimal ClosingCash { get; init; }
    public decimal CashPayments { get; init; }
    public TimeSpan? ScheduledStart { get; init; }
    public TimeSpan? ScheduledEnd { get; init; }

    public decimal ExpectedCash => OpeningCash + CashPayments;
    public decimal Variance => ClosingCash - ExpectedCash;
    public TimeSpan WorkedDuration => ClosedAt - OpenedAt;
}

public interface IShiftService
{
    Shift? GetOpenShift(int userId);
    bool OpenShift(int userId, decimal openingCash);
    bool CloseShift(int shiftId, decimal closingCash);
    ShiftReconciliation GetReconciliation(int shiftId);
    List<ShiftReport> GetShiftHistory(DateTime from, DateTime to);
}
