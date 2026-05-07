using Microsoft.EntityFrameworkCore;

using mytown.DataAccess.Interfaces;
using mytown.DTOs;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;

namespace mytown.DataAccess.Implementations
{
    public class SenderRepository : ISenderRepository
    {
        private readonly AppDbContext _context;

        public SenderRepository(AppDbContext context)
        {
            _context = context;
        }

        // ---------------- EMAIL CHECK ----------------
        public async Task<(bool isTaken, string message)> IsEmailTaken(string email)
        {
            var sender = await _context.SenderRegisters
                .FirstOrDefaultAsync(s => s.Email.ToLower() == email.ToLower());

            if (sender == null || sender.Status == "Deactivated")
                return (false, null);

            if (sender.Status == "Blocked")
                return (true, "This email is blocked. Please contact support.");

            return (true, null);
        }

        // ---------------- PENDING SENDER VERIFICATION ----------------
        public async Task SavePendingSenderVerification(PendingSenderVerification pending)
        {
            _context.PendingSenderVerifications.Add(pending);
            await _context.SaveChangesAsync();
        }

        public async Task<PendingSenderVerification> FindPendingSenderVerificationByToken(string token)
        {
            return await _context.PendingSenderVerifications
                .FirstOrDefaultAsync(p => p.Token == token);
        }

        public async Task<PendingSenderVerification> FindPendingSenderVerificationByEmail(string email)
        {
            return await _context.PendingSenderVerifications
                .FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower()
                                       && p.ExpiryDate > DateTime.UtcNow);
        }

        public async Task DeletePendingSenderVerification(string token)
        {
            var pending = await _context.PendingSenderVerifications
                .FirstOrDefaultAsync(p => p.Token == token);

            if (pending != null)
            {
                _context.PendingSenderVerifications.Remove(pending);
                await _context.SaveChangesAsync();
            }
        }

        // ---------------- REGISTER SENDER ----------------
        public async Task<SenderRegister> RegisterSender(SenderRegister sender)
        {
            try
            {
                _context.SenderRegisters.Add(sender);
                await _context.SaveChangesAsync();
                return sender;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Database Update Exception: " + ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("Inner Exception: " + ex.InnerException.Message);

                throw new Exception("There was an error saving the sender registration to the database.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Exception: " + ex.Message);
                throw new Exception("An unexpected error occurred during sender registration.");
            }
        }

        // ---------------- GET BY ID ----------------
        public async Task<SenderRegister> GetSenderByIdAsync(int senderRegId)
        {
            return await _context.SenderRegisters
                .FirstOrDefaultAsync(s => s.SenderRegId == senderRegId);
        }

        // ---------------- CREATE SENDER ORDER ----------------

        public async Task<int> CreateSenderOrderAsync(CreateSenderOrderDto dto)
        {
            var order = new SenderOrder
            {
                SenderRegId = dto.SenderId,

                ProductName = dto.ProductName,
                ProductCost = dto.ProductCost,

                PackageLength = dto.PackageLength,
                PackageWidth = dto.PackageWidth,
                PackageHeight = dto.PackageHeight,
                PackageWeight = dto.PackageWeight,

                IsFragile = dto.IsFragile,
                IsPerishable = dto.IsPerishable,

                SpecialInstructions = dto.SpecialInstructions,

                PickupAddress = dto.PickupAddress,
                PickupTown = dto.PickupTown,
                PickupCity = dto.PickupCity,
                PickupState = dto.PickupState,
                PickupCountry = dto.PickupCountry,
                PickupPincode = dto.PickupPincode,

                PickupDate = dto.PickupDate,
                PickupTime = dto.PickupTime,

                ReceiverName = dto.ReceiverName,
                ReceiverPhone = dto.ReceiverPhone,

                ReceiverAddress = dto.ReceiverAddress,
                ReceiverTown = dto.ReceiverTown,
                ReceiverCity = dto.ReceiverCity,
                ReceiverState = dto.ReceiverState,
                ReceiverCountry = dto.ReceiverCountry,
                ReceiverPincode = dto.ReceiverPincode
            };

            _context.SenderOrders.Add(order);

            await _context.SaveChangesAsync();

            return order.SenderOrderId;
        }

        // geting transorter matching
        public async Task<List<MatchingTransporterDto>>
     GetMatchingTransportersAsync(int senderOrderId)
        {
            var order = await _context.SenderOrders
                .FirstOrDefaultAsync(x =>
                    x.SenderOrderId == senderOrderId);

            if (order == null)
                throw new Exception("Sender order not found");

            var transporters =
                await _context.TransporterTravelPlans
                .Include(x => x.TransporterRegister)

                // active plan only
                .Where(x => x.IsActive)

                // available only
                .Where(x =>
                    x.PlanStatus == "Available")

                // pickup city match
                .Where(x =>
                    x.StartLocation.Contains(order.PickupCity))

                // destination city match
                .Where(x =>
                    x.Destination.Contains(order.ReceiverCity))

                // weight check
                .Where(x =>
                    x.MaxWeightKg >= order.PackageWeight)

                // fragile check
                .Where(x =>
                    !order.IsFragile ||
                    x.AcceptsFragile)

                // perishable check
                .Where(x =>
                    !order.IsPerishable ||
                    x.AcceptsPerishable)

                // date check
                .Where(x =>
                    order.PickupDate >= x.StartDate &&
                    order.PickupDate <= x.ArrivalDate)

                .Select(x => new MatchingTransporterDto
                {
                    PlanId = x.PlanId,

                    TransporterRegId =
                        x.TransporterRegId,

                    TransporterName =
                        x.TransporterRegister.TransporterName,

                    Email =
                        x.TransporterRegister.Email,

                    PhoneNumber =
                        x.TransporterRegister.PhoneNumber,

                    VehicleType =
                        x.VehicleType,

                    VehicleName =
                        x.VehicleName,

                    MaxWeightKg =
                        x.MaxWeightKg,

                    StartDate =
                        x.StartDate,

                    ArrivalDate =
                        x.ArrivalDate,

                    PreferredContact =
                        x.PreferredContact
                })

                .ToListAsync();

            return transporters;
        }

        public async Task<SenderOrderSummaryDto>
    GetOrderSummaryAsync(
        SenderOrderSummaryRequestDto dto)
        {
            var order =
                await _context.SenderOrders
                .FirstOrDefaultAsync(x =>
                    x.SenderOrderId ==
                    dto.SenderOrderId);

            if (order == null)
                throw new Exception("Order not found");

            var transporter =
                await _context.TransporterRegisters
                .FirstOrDefaultAsync(x =>
                    x.TransporterRegId ==
                    dto.TransporterRegId);

            if (transporter == null)
                throw new Exception("Transporter not found");

            var plan =
                await _context.TransporterTravelPlans
                .FirstOrDefaultAsync(x =>
                    x.PlanId ==
                    dto.TransporterPlanId);

            if (plan == null)
                throw new Exception("Plan not found");

            // Example pricing
            decimal weight =
                order.PackageWeight ?? 1;

            decimal baseAmount =
                weight * 20;

            decimal gstAmount =
                (baseAmount * 18) / 100;

            decimal totalAmount =
                baseAmount + gstAmount;

            var sender =
                await _context.SenderRegisters
                .FirstOrDefaultAsync(x =>
                    x.SenderRegId ==
                    order.SenderRegId);

            return new SenderOrderSummaryDto
            {
                // Sender
                SenderName =
                    sender.SenderName,

                SenderPhone =
                    sender.PhoneNumber,

                PickupAddress =
                    order.PickupAddress,

                // Receiver
                ReceiverName =
                    order.ReceiverName,

                ReceiverPhone =
                    order.ReceiverPhone,

                ReceiverAddress =
                    order.ReceiverAddress,

                // Product
                ProductName =
                    order.ProductName,

                ProductCost =
                    order.ProductCost,

                PackageWeight =
                    order.PackageWeight,

                IsFragile =
                    order.IsFragile,

                IsPerishable =
                    order.IsPerishable,

                // Transporter
                TransporterName =
                    transporter.TransporterName,

                TransporterEmail =
                    transporter.Email,

                TransporterPhone =
                    transporter.PhoneNumber,

                VehicleType =
                    plan.VehicleType,

                VehicleName =
                    plan.VehicleName,

                // Charges
                BaseAmount =
                    baseAmount,

                GstAmount =
                    gstAmount,

                TotalAmount =
                    totalAmount
            };
        }

        // select transporter
        public async Task<bool> SelectTransporterAsync(
    SelectTransporterDto dto)
        {
            var order =
                await _context.SenderOrders
                .FirstOrDefaultAsync(x =>
                    x.SenderOrderId ==
                    dto.SenderOrderId);

            if (order == null)
                throw new Exception("Order not found");

            order.TransporterRegId =
                dto.TransporterRegId;

            order.TransporterPlanId =
                dto.TransporterPlanId;

            order.OrderStatus =
                "TransporterSelected";

            await _context.SaveChangesAsync();

            return true;
        }

        // sender payment

        public async Task<SenderOrder>
             GetSenderOrderAsync(int senderOrderId)
        {
            return await _context.SenderOrders
                .FirstOrDefaultAsync(x =>
                    x.SenderOrderId ==
                    senderOrderId);
        }

        public async Task AddSenderOrderPaymentAsync(
            SenderOrderPayment payment)
        {
            await _context
                .SenderOrderPayments
                .AddAsync(payment);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}