using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Services;
using Xunit;

namespace RestaurantPOS.Tests;

public class ShiftServiceTests
{
    [Fact]
    public void OpenShift_ReturnsFalse_WhenUserAlreadyHasAnOpenShift()
    {
        var shiftRepository = new FakeShiftRepository();
        shiftRepository.Seed(new Shift { ShiftId = 1, UserId = 7, OpeningCash = 100_000 });
        var service = new ShiftService(shiftRepository, new FakePaymentRepository());

        var result = service.OpenShift(userId: 7, openingCash: 200_000);

        Assert.False(result);
    }

    [Fact]
    public void GetReconciliation_ExpectedCash_IsOpeningCashPlusCashPayments()
    {
        var shiftRepository = new FakeShiftRepository();
        shiftRepository.Seed(new Shift { ShiftId = 1, OpeningCash = 500_000 });
        var paymentRepository = new FakePaymentRepository();
        paymentRepository.Seed(new Payment { ShiftId = 1, AmountPaid = 45_000, Method = PaymentMethod.Cash });
        var service = new ShiftService(shiftRepository, paymentRepository);

        var result = service.GetReconciliation(1);

        Assert.Equal(500_000, result.OpeningCash);
        Assert.Equal(45_000, result.CashPayments);
        Assert.Equal(545_000, result.ExpectedCash);
    }

    [Fact]
    public void GetReconciliation_ExcludesBankTransferPayments()
    {
        var shiftRepository = new FakeShiftRepository();
        shiftRepository.Seed(new Shift { ShiftId = 1, OpeningCash = 500_000 });
        var paymentRepository = new FakePaymentRepository();
        paymentRepository.Seed(new Payment { ShiftId = 1, AmountPaid = 45_000, Method = PaymentMethod.Cash });
        paymentRepository.Seed(new Payment { ShiftId = 1, AmountPaid = 999_000, Method = PaymentMethod.BankTransfer });
        var service = new ShiftService(shiftRepository, paymentRepository);

        var result = service.GetReconciliation(1);

        Assert.Equal(45_000, result.CashPayments);
        Assert.Equal(545_000, result.ExpectedCash);
    }
}
