using RestaurantPOS.BusinessObjects;
using RestaurantPOS.DataAccessObjects;

namespace RestaurantPOS.Repositories;

public class PaymentRepository : IPaymentRepository
{
    public CheckoutResult CheckoutOrder(int orderId, int cashierUserId, PaymentMethod method, int? shiftId) =>
        PaymentDAO.CheckoutOrder(orderId, cashierUserId, method, shiftId);

    public List<Payment> GetPaymentsByShiftId(int shiftId) =>
        PaymentDAO.GetPaymentsByShiftId(shiftId);

    public List<Payment> GetPaymentsBetween(DateTime from, DateTime to) =>
        PaymentDAO.GetPaymentsBetween(from, to);
}
