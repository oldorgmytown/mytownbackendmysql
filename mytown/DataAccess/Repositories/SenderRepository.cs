using Microsoft.EntityFrameworkCore;

using mytown.DataAccess.Interfaces;
using mytown.DTOs;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;
using Stripe;

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
        public async Task<MatchingTransporterDto?> GetMatchingTransportersAsync(int senderOrderId)
        {
            var order = await _context.SenderOrders
                .FirstOrDefaultAsync(x => x.SenderOrderId == senderOrderId);

            if (order == null)
                throw new Exception("Sender order not found");

            DateTime bookingDateTime =
                TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
                    DateTime.UtcNow,
                    "India Standard Time");

            // Common matching query
            var matchingQuery = _context.TransporterTravelPlans
                .Include(x => x.TransporterRegister)
                .Where(x =>
                    x.IsActive &&
                    x.PlanStatus == "Available" &&
                    x.StartDate > bookingDateTime &&

                    x.StartTown.ToLower() == order.PickupTown.ToLower() &&
                    x.StartCity.ToLower() == order.PickupCity.ToLower() &&
                    x.StartState.ToLower() == order.PickupState.ToLower() &&
                    x.StartCountry.ToLower() == order.PickupCountry.ToLower() &&

                    x.DestinationTown.ToLower() == order.ReceiverTown.ToLower() &&
                    x.DestinationCity.ToLower() == order.ReceiverCity.ToLower() &&
                    x.DestinationState.ToLower() == order.ReceiverState.ToLower() &&
                    x.DestinationCountry.ToLower() == order.ReceiverCountry.ToLower() &&

                    (!order.IsFragile || x.AcceptsFragile) &&
                    (!order.IsPerishable || x.AcceptsPerishable)
                );

            // First priority -> weight matching
            var exactWeightMatch = await matchingQuery
                .Where(x => x.MaxWeightKg >= order.PackageWeight)
                .OrderBy(x => x.CreatedAt)
                .Select(x => new MatchingTransporterDto
                {
                    PlanId = x.PlanId,
                    TransporterRegId = x.TransporterRegId,
                    TransporterName = x.TransporterRegister.TransporterName,
                    Email = x.TransporterRegister.Email,
                    PhoneNumber = x.TransporterRegister.PhoneNumber,
                    VehicleType = x.VehicleType,
                    VehicleName = x.VehicleName,
                    MaxWeightKg = x.MaxWeightKg,
                    StartDate = x.StartDate,
                    ArrivalDate = x.ArrivalDate,
                    PreferredContact = x.PreferredContact,
                    Message = "Matching transporter found"
                })
                .FirstOrDefaultAsync();

            if (exactWeightMatch != null)
                return exactWeightMatch;

            // Second priority -> all matched except weight
            var overweightTransporter = await matchingQuery
             .OrderByDescending(x => x.MaxWeightKg)
             .ThenBy(x => x.CreatedAt)
             .Select(x => new MatchingTransporterDto
             {
                 PlanId = x.PlanId,
                 TransporterRegId = x.TransporterRegId,
                 TransporterName = x.TransporterRegister.TransporterName,
                 Email = x.TransporterRegister.Email,
                 PhoneNumber = x.TransporterRegister.PhoneNumber,
                 VehicleType = x.VehicleType,
                 VehicleName = x.VehicleName,
                 MaxWeightKg = x.MaxWeightKg,
                 StartDate = x.StartDate,
                 ArrivalDate = x.ArrivalDate,
                 PreferredContact = x.PreferredContact,
                 Message = "Package weight exceeds transporter maximum weight limit"
             })
             .FirstOrDefaultAsync();

            return overweightTransporter;
        }
        // changed as per transporetr columns - town, city, state, country

        public async Task<SenderOrderSummaryDto> GetOrderSummaryAsync(
    SenderOrderSummaryRequestDto dto)
        {
            var order = await _context.SenderOrders
                .FirstOrDefaultAsync(x => x.SenderOrderId == dto.SenderOrderId);

            if (order == null)
                throw new Exception("Order not found");

            var sender = await _context.SenderRegisters
                .FirstOrDefaultAsync(x => x.SenderRegId == order.SenderRegId);

            var transporter = await _context.TransporterRegisters
                .FirstOrDefaultAsync(x => x.TransporterRegId == dto.TransporterRegId);

            var plan = await _context.TransporterTravelPlans
                .FirstOrDefaultAsync(x => x.PlanId == dto.TransporterPlanId);

            decimal transportCharge = 50;
            decimal gstAmount = transportCharge * 0.18m;
            decimal totalAmount = transportCharge + gstAmount;

            string startLocation =
                $"{plan.StartTown}, {plan.StartCity}, {plan.StartState}, {plan.StartCountry}";

            string destinationLocation =
                $"{plan.DestinationTown}, {plan.DestinationCity}, {plan.DestinationState}, {plan.DestinationCountry}";

            return new SenderOrderSummaryDto
            {
                ProductName = order.ProductName,
                ProductCost = order.ProductCost,

                PackageLength = order.PackageLength,
                PackageWidth = order.PackageWidth,
                PackageHeight = order.PackageHeight,
                PackageWeight = order.PackageWeight,

                SenderName = sender.SenderName,
                SenderPhone = sender.PhoneNumber,

                PickupAddress = order.PickupAddress,
                PickupDate = order.PickupDate,
                PickupTime = order.PickupTime,

                ReceiverName = order.ReceiverName,
                ReceiverPhone = order.ReceiverPhone,
                ReceiverAddress = order.ReceiverAddress,

                TransporterRegId = transporter.TransporterRegId,
                TransporterName = transporter.TransporterName,
                TransporterPhone = transporter.PhoneNumber,

                VehicleType = plan.VehicleType,

                StartLocation = startLocation,
                Destination = destinationLocation,

                TransportCharge = transportCharge,
                GstAmount = gstAmount,
                TotalAmount = totalAmount
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

        public async Task<SenderOrderConfirmationDto>
      GetOrderConfirmationAsync(
          int senderOrderId)
        {
            var order =
                await _context.SenderOrders
                .FirstOrDefaultAsync(x =>
                    x.SenderOrderId == senderOrderId);

            if (order == null)
                throw new Exception("Order not found");

            var transporter =
                await _context.TransporterRegisters
                .FirstOrDefaultAsync(x =>
                    x.TransporterRegId == order.TransporterRegId);

            var sender =
                await _context.SenderRegisters
                .FirstOrDefaultAsync(x =>
                    x.SenderRegId == order.SenderRegId);

var plan =
                await _context.TransporterTravelPlans
                .FirstOrDefaultAsync(x =>
                    x.PlanId == order.TransporterPlanId);

            if (transporter == null)
                throw new Exception("Transporter record not found.");
            if (plan == null)
                throw new Exception("Travel plan not found.");
            if (sender == null)
                throw new Exception("Sender record not found.");

            return new SenderOrderConfirmationDto
            {
                SenderOrderId = order.SenderOrderId,
                BookingDate = order.CreatedAt,

                ProductName = order.ProductName,
                Dimensions = $"{order.PackageLength ?? 0} x {order.PackageWidth ?? 0} x {order.PackageHeight ?? 0} cm",
                Weight = $"{order.PackageWeight ?? 0} kg",
                DeclaredValue = order.ProductCost,

                PickupAddress = order.PickupAddress,
                PickupDate = order.PickupDate,
                PickupTime = order.PickupTime,

                ReceiverName = order.ReceiverName,
                ReceiverPhone = order.ReceiverPhone,
                DeliveryAddress = order.ReceiverAddress,

                EstimatedDeliveryDate = plan.ArrivalDate,

                TransporterName = transporter.TransporterName,
                TransporterPhone = transporter.PhoneNumber,
                VehicleType = plan.VehicleType,

                TransportationCharge = 50,
                PaymentMethod = "Online Payment",

                SenderName = sender.SenderName,
                SenderPhone = sender.PhoneNumber
            };
        }

        // sender package delivery status

        public async Task<bool>
        UpdateSenderPackageDeliveryStatusAsync(
            UpdateSenderPackageDeliveryStatusDto dto)
        {
            var order =
                await _context.SenderOrders
                .FirstOrDefaultAsync(x =>
                    x.SenderOrderId ==
                    dto.SenderOrderId);

            if (order == null)
                throw new Exception("Order not found");

            if (!order.TransporterRegId.HasValue)
                throw new Exception(
                    "Transporter not assigned");

            if (order.DeliveryStatus ==
                dto.DeliveryStatus)
                throw new Exception(
                    "Status already updated");

            order.DeliveryStatus =
                dto.DeliveryStatus;

            _context.SenderDBNotifications.Add(
                new SenderDBNotifications
                {
                    SenderRegId =
                        order.SenderRegId,

                    Title =
                        "Shipment Status Updated",

                    Message =
                        $"Your shipment #{order.SenderOrderId} is now {dto.DeliveryStatus}.",

                    IsRead = false,

                    CreatedDate =
                        DateTime.UtcNow
                });

            _context.TransporterDBNotifications.Add(
                new TransporterDBNotifications
                {
                    TransporterRegId =
                        order.TransporterRegId.Value,

                    Title =
                        "Shipment Status Updated",

                    Message =
                        $"Shipment #{order.SenderOrderId} status updated to {dto.DeliveryStatus}.",

                    IsRead = false,

                    CreatedDate =
                        DateTime.UtcNow
                });

            await _context.SaveChangesAsync();

            return true;
        }

        // update notifications

        public async Task AddSenderNotificationAsync(
    SenderDBNotifications notification)
        {
            await _context
                .SenderDBNotifications
                .AddAsync(notification);
        }

        public async Task AddTransporterNotificationAsync(
    TransporterDBNotifications notification)
        {
            await _context
                .TransporterDBNotifications
                .AddAsync(notification);
        }

        // get trasprter email
        public async Task<TransporterEmailDto>
GetTransporterByIdAsync(int transporterId)
        {
            return await _context.TransporterRegisters
                .Where(t => t.TransporterRegId == transporterId)
                .Select(t => new TransporterEmailDto
                {
                    TransporterId = t.TransporterRegId,
                    TransporterName = t.TransporterName,
                    Email = t.Email
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<SenderOrdersTabDto>>
 GetSenderOrdersAsync(int senderId, string orderStatus)
        {
            var today = DateTime.UtcNow.Date;

            var query =
                from o in _context.SenderOrders

                join t in _context.TransporterRegisters
                on o.TransporterRegId equals t.TransporterRegId
                into transporterGroup

                from transporter in transporterGroup.DefaultIfEmpty()

                where o.SenderRegId == senderId

                select new SenderOrdersTabDto
                {
                    SenderOrderId = o.SenderOrderId,

                    ProductName = o.ProductName,

                    BookingDate = o.CreatedAt,

                    PickupLocation = o.PickupAddress,

                    DeliveryLocation = o.ReceiverAddress,

                    DeliveryStatus = o.DeliveryStatus,

                    OrderType =
                        o.DeliveryStatus == "Delivered"
                            ? "Delivered"
                            : o.DeliveryStatus == "Pending"
                                ? (o.CreatedAt.Date == today
                                    ? "New"
                                    : "Pending")
                                : "InProgress",

                    TransporterName =
                        transporter != null
                            ? transporter.TransporterName
                            : null,

                    TransporterPhone =
                        transporter != null
                            ? transporter.PhoneNumber
                            : null,
                    TrackingId = o.TrackingId
                };

            // NEW = Today's pending orders
            if (orderStatus == "New")
            {
                query = query.Where(x =>
                    x.DeliveryStatus == "Pending" &&
                    x.BookingDate.Date == today);
            }

            // PENDING = Previous pending orders
            else if (orderStatus == "Pending")
            {
                query = query.Where(x =>
                    x.DeliveryStatus == "Pending" &&
                    x.BookingDate.Date <= today);
            }

            //sender In Progress status error fixed - changed to In Progress from InProgress

            else if (orderStatus == "In Progress")
            {
                query = query.Where(x =>
                    x.DeliveryStatus.ToLower() == "pickedup" ||
                    x.DeliveryStatus.ToLower() == "inprogress" ||
                    x.DeliveryStatus.ToLower() == "in progress" ||
                    x.DeliveryStatus.ToLower() == "intransit" ||
                    x.DeliveryStatus.ToLower() == "in transit");
            }

            // DELIVERED
            else if (orderStatus == "Delivered")
            {
                query = query.Where(x =>
                    x.DeliveryStatus == "Delivered");
            }

            return await query
                .OrderByDescending(x => x.BookingDate)
                .ToListAsync();
        }
        public async Task<SenderRegisterDto?> GetSenderProfileAsync(int senderRegId)
        {
            return await _context.SenderRegisters
                .Where(x => x.SenderRegId == senderRegId)
                .Select(x => new SenderRegisterDto
                {
                    SenderId = x.SenderRegId,
                    SenderName = x.SenderName,
                    Email = x.Email,

                    // Usually don't expose password in API
                    Password = "",

                    Address = x.Address,
                    Town = x.Town,
                    City = x.City,
                    State = x.State,
                    Country = x.Country,
                    PostalCode = x.PostalCode,
                    PhoneNumber = x.PhoneNumber,

                    Status = x.Status,
                    SenderRegDate = x.SenderRegDate,
                    IsEmailVerified = x.IsEmailVerified
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateSenderProfileAsync(
    int senderRegId,
    UpdateSenderProfileDto dto)
        {
            var sender = await _context.SenderRegisters
                .FirstOrDefaultAsync(x =>
                    x.SenderRegId == senderRegId);

            if (sender == null)
                return false;

            sender.SenderName = dto.SenderName;
            sender.Address = dto.Address;
            sender.Town = dto.Town;
            sender.City = dto.City;
            sender.State = dto.State;
            sender.Country = dto.Country;
            sender.PostalCode = dto.PostalCode;
            sender.PhoneNumber = dto.PhoneNumber;

            await _context.SaveChangesAsync();

            return true;
        }

        // -------------------------------------------------------------------------
        // NOTIFICATIONS
        // -------------------------------------------------------------------------

        public async Task<List<SenderDBNotifications>>
        GetUnreadNotificationsAsync(int senderId)
        {
            return await _context.SenderDBNotifications
                .Where(n =>
                    n.SenderRegId == senderId &&
                    !n.IsRead)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task MarkAllAsReadAsync(int senderId)
        {
            var notifications =
                await _context.SenderDBNotifications
                .Where(n =>
                    n.SenderRegId == senderId &&
                    !n.IsRead)
                .ToListAsync();

            notifications.ForEach(n =>
                n.IsRead = true);

            await _context.SaveChangesAsync();
        }

        public async Task MarkEachNotificationReadAsync(int notificationId)
        {
            var notification =
                await _context.SenderDBNotifications
                .FirstOrDefaultAsync(n =>
                    n.NotificationId == notificationId);

            if (notification != null)
            {
                notification.IsRead = true;

                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<SenderAlternateAddressDto>>
GetAddressesBySenderIdAsync(int senderRegId)
        {
            return await _context.SenderAlternateAddresses
                .Where(x =>
                    x.SenderRegId == senderRegId &&
                    !x.IsDeleted)

                .Select(x => new SenderAlternateAddressDto
                {
                    AltAddressId = x.AltAddressId,
                    SenderRegId = x.SenderRegId,
                    AltName = x.AltName,
                    AltPhoneNumber = x.AltPhoneNumber,
                    AltAddress = x.AltAddress,
                    AltTown = x.AltTown,
                    AltCity = x.AltCity,
                    AltState = x.AltState,
                    AltCountry = x.AltCountry,
                    AltPostalCode = x.AltPostalCode,
                    DeliveryNotes = x.DeliveryNotes
                })
                .ToListAsync();
        }


        public async Task<SenderAlternateAddressDto>
        AddAddressAsync(SenderAlternateAddress address)
        {
            SenderAlternateAddress entity;

            if (address.AltAddressId > 0)
            {
                entity = await _context.SenderAlternateAddresses
                    .FirstOrDefaultAsync(x =>
                        x.AltAddressId == address.AltAddressId &&
                        x.SenderRegId == address.SenderRegId)

                    ?? throw new Exception("Address not found");

                entity.AltName = address.AltName;
                entity.AltPhoneNumber = address.AltPhoneNumber;
                entity.AltAddress = address.AltAddress;
                entity.AltTown = address.AltTown;
                entity.AltCity = address.AltCity;
                entity.AltState = address.AltState;
                entity.AltCountry = address.AltCountry;
                entity.AltPostalCode = address.AltPostalCode;
                entity.DeliveryNotes = address.DeliveryNotes;
            }
            else
            {
                entity = address;

                _context.SenderAlternateAddresses
                    .Add(entity);
            }

            await _context.SaveChangesAsync();

            return new SenderAlternateAddressDto
            {
                AltAddressId = entity.AltAddressId,
                SenderRegId = entity.SenderRegId,
                AltName = entity.AltName,
                AltPhoneNumber = entity.AltPhoneNumber,
                AltAddress = entity.AltAddress,
                AltTown = entity.AltTown,
                AltCity = entity.AltCity,
                AltState = entity.AltState,
                AltCountry = entity.AltCountry,
                AltPostalCode = entity.AltPostalCode,
                DeliveryNotes = entity.DeliveryNotes
            };
        }


        public async Task<bool>
        DeleteAddressAsync(int id)
        {
            var address =
                await _context.SenderAlternateAddresses
                .FirstOrDefaultAsync(x =>
                    x.AltAddressId == id &&
                    !x.IsDeleted);

            if (address == null)
                return false;

            address.IsDeleted = true;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}