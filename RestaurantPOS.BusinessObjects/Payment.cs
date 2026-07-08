namespace RestaurantPOS.BusinessObjects;

public enum PaymentMethod
{
    Cash,
    BankTransfer
}

public class Payment
{
    public int PaymentId { get; set; }

    public int OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;

    public int CashierUserId { get; set; }
    public virtual User CashierUser { get; set; } = null!;

    // Which cashier shift this payment counts toward for cash reconciliation.
    // Nullable: a payment can still be taken with no shift open (see ShiftService).
    public int? ShiftId { get; set; }
    public virtual Shift? Shift { get; set; }

    public decimal AmountPaid { get; set; }
    public PaymentMethod Method { get; set; }
    public DateTime PaidAt { get; set; }
}
