using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.Repositories;

public interface IPaymentRepository
{
    bool CheckoutOrder(int orderId, int cashierUserId, PaymentMethod method, int? shiftId);
    List<Payment> GetPaymentsByShiftId(int shiftId);
    List<Payment> GetPaymentsBetween(DateTime from, DateTime to);
}
