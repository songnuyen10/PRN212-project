using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Services;
using Xunit;

namespace RestaurantPOS.Tests;

public class PaymentServiceTests
{
    [Fact]
    public void Checkout_ReturnsFalse_WhenOrderIsNotOpen()
    {
        var orderRepository = new FakeOrderRepository();
        orderRepository.Seed(new Order { OrderId = 1, Status = OrderStatus.Paid });
        var paymentRepository = new FakePaymentRepository();
        var service = new PaymentService(paymentRepository, orderRepository, new FakeShiftRepository());

        var result = service.Checkout(orderId: 1, cashierUserId: 1, PaymentMethod.Cash);

        Assert.False(result);
        Assert.False(paymentRepository.CheckoutOrderWasCalled);
    }

    [Fact]
    public void Checkout_StampsTheCashiersCurrentOpenShift()
    {
        var orderRepository = new FakeOrderRepository();
        orderRepository.Seed(new Order { OrderId = 1, Status = OrderStatus.Open });
        var shiftRepository = new FakeShiftRepository();
        shiftRepository.Seed(new Shift { ShiftId = 42, UserId = 1, OpeningCash = 500_000 });
        var paymentRepository = new FakePaymentRepository();
        var service = new PaymentService(paymentRepository, orderRepository, shiftRepository);

        var result = service.Checkout(orderId: 1, cashierUserId: 1, PaymentMethod.Cash);

        Assert.True(result);
        Assert.Equal(42, paymentRepository.CheckoutOrderWasCalledWithShiftId);
    }

    [Fact]
    public void Checkout_AllowsPayment_WhenCashierHasNoOpenShift()
    {
        var orderRepository = new FakeOrderRepository();
        orderRepository.Seed(new Order { OrderId = 1, Status = OrderStatus.Open });
        var paymentRepository = new FakePaymentRepository();
        var service = new PaymentService(paymentRepository, orderRepository, new FakeShiftRepository());

        var result = service.Checkout(orderId: 1, cashierUserId: 1, PaymentMethod.Cash);

        Assert.True(result);
        Assert.Null(paymentRepository.CheckoutOrderWasCalledWithShiftId);
    }
}
