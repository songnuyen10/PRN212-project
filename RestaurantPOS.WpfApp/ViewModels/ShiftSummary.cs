namespace RestaurantPOS.WpfApp.ViewModels;

// Snapshot of a just-closed shift for the summary panel — not persisted, built
// from OpenShift + SessionContext.CurrentUser right before Load() clears them.
public class ShiftSummary
{
    public string UserFullName { get; init; } = "";
    public decimal OpeningCash { get; init; }
    public decimal ClosingCash { get; init; }
    public DateTime OpenedAt { get; init; }
    public DateTime ClosedAt { get; init; }
    public TimeSpan? ScheduledStart { get; init; }
    public TimeSpan? ScheduledEnd { get; init; }

    public TimeSpan WorkedDuration => ClosedAt - OpenedAt;
}
