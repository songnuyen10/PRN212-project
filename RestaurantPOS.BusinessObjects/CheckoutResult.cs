namespace RestaurantPOS.BusinessObjects;

public enum CheckoutResult
{
    Success,
    OrderNotOpen,
    InsufficientStock,
    Conflict,
    Error
}
