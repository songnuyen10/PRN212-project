using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.Services;

public interface IPaymentService
{
    bool Checkout(int orderId, int cashierUserId, PaymentMethod method);
    List<Payment> GetPaymentsBetween(DateTime from, DateTime to);
}
