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

    [Fact]
    public void GetShiftHistory_ComputesExpectedCashAndVariance()
    {
        var shiftRepository = new FakeShiftRepository();
        var opened = new DateTime(2026, 7, 20, 8, 0, 0);
        var closed = new DateTime(2026, 7, 20, 16, 0, 0);
        shiftRepository.Seed(new Shift
        {
            ShiftId = 1,
            User = new User { FullName = "Cashier One" },
            OpenedAt = opened,
            ClosedAt = closed,
            OpeningCash = 500_000,
            ClosingCash = 560_000,
            Payments = new List<Payment> { new() { AmountPaid = 45_000, Method = PaymentMethod.Cash } }
        });
        var service = new ShiftService(shiftRepository, new FakePaymentRepository());

        var result = service.GetShiftHistory(opened.Date, opened.Date.AddDays(1).AddTicks(-1));

        var report = Assert.Single(result);
        Assert.Equal("Cashier One", report.UserFullName);
        Assert.Equal(45_000, report.CashPayments);
        Assert.Equal(545_000, report.ExpectedCash);
        Assert.Equal(15_000, report.Variance);
    }

    [Fact]
    public void GetShiftHistory_ExcludesBankTransferFromCashPayments()
    {
        var shiftRepository = new FakeShiftRepository();
        var opened = new DateTime(2026, 7, 20, 8, 0, 0);
        shiftRepository.Seed(new Shift
        {
            ShiftId = 1,
            User = new User { FullName = "Cashier One" },
            OpenedAt = opened,
            ClosedAt = opened.AddHours(8),
            OpeningCash = 500_000,
            Payments = new List<Payment>
            {
                new() { AmountPaid = 45_000, Method = PaymentMethod.Cash },
                new() { AmountPaid = 999_000, Method = PaymentMethod.BankTransfer }
            }
        });
        var service = new ShiftService(shiftRepository, new FakePaymentRepository());

        var result = service.GetShiftHistory(opened.Date, opened.Date.AddDays(1).AddTicks(-1));

        var report = Assert.Single(result);
        Assert.Equal(45_000, report.CashPayments);
        Assert.Equal(545_000, report.ExpectedCash);
    }

    [Fact]
    public void GetShiftHistory_ExcludesOpenShifts()
    {
        var shiftRepository = new FakeShiftRepository();
        var opened = new DateTime(2026, 7, 20, 8, 0, 0);
        shiftRepository.Seed(new Shift
        {
            ShiftId = 1,
            User = new User { FullName = "Cashier One" },
            OpenedAt = opened,
            ClosedAt = null,
            OpeningCash = 500_000
        });
        var service = new ShiftService(shiftRepository, new FakePaymentRepository());

        var result = service.GetShiftHistory(opened.Date, opened.Date.AddDays(1).AddTicks(-1));

        Assert.Empty(result);
    }
}
