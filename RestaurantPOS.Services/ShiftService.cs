using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Repositories;

namespace RestaurantPOS.Services;

public class ShiftService : IShiftService
{
    private readonly IShiftRepository _shiftRepository;
    private readonly IPaymentRepository _paymentRepository;

    public ShiftService()
    {
        _shiftRepository = new ShiftRepository();
        _paymentRepository = new PaymentRepository();
    }

    // Test seam only — production code always uses the parameterless constructor.
    public ShiftService(IShiftRepository shiftRepository, IPaymentRepository paymentRepository)
    {
        _shiftRepository = shiftRepository;
        _paymentRepository = paymentRepository;
    }

    public Shift? GetOpenShift(int userId) => _shiftRepository.GetOpenShift(userId);

    public bool OpenShift(int userId, decimal openingCash) => _shiftRepository.OpenShift(userId, openingCash);

    public bool CloseShift(int shiftId, decimal closingCash) => _shiftRepository.CloseShift(shiftId, closingCash);

    public ShiftReconciliation GetReconciliation(int shiftId)
    {
        var shift = _shiftRepository.GetShiftById(shiftId);
        var openingCash = shift?.OpeningCash ?? 0m;

        var cashPayments = _paymentRepository.GetPaymentsByShiftId(shiftId)
            .Where(p => p.Method == PaymentMethod.Cash)
            .Sum(p => p.AmountPaid);

        return new ShiftReconciliation(openingCash, cashPayments, openingCash + cashPayments);
    }
}
