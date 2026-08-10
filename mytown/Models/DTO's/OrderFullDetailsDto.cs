using System;
using System.Collections.Generic;

namespace mytown.Models.DTO_s
{
    public class OrderFullDetailsDto
    {
        // Store Order
        public int StoreOrderId { get; set; }
        public string StoreOrderCode { get; set; }
        public string CourierType { get; set; }
        public decimal StoreTotalAmount { get; set; }
        public string StoreOrderStatus { get; set; }

        // Order
        public int OrderId { get; set; }
        public decimal OrderTotalAmount { get; set; }
        public string ShippingType { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsGuestOrder { get; set; }

        // Shopper
        public int? ShopperRegId { get; set; }
        public string? ShopperUsername { get; set; }
        public string? ShopperEmail { get; set; }
        public string? ShopperPhoneNumber { get; set; }

        // Guest (if this order was placed as guest)
        public int? GuestRegId { get; set; }

        // Business
        public int BusRegId { get; set; }
        public string BusinessName { get; set; }
        public string BusEmail { get; set; }
        public string BusMobileNo { get; set; }
        public string BusinessTown { get; set; }
        public string BusinessCity { get; set; }
        public string BusinessState { get; set; }
        public string BusinessCountry { get; set; }

        // Shipping / Tracking (from shipping_details)
        public string? TrackingId { get; set; }
        public string? ShippingStatus { get; set; }
        public int? EstimatedDays { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public decimal? ShippingCost { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? DeliveryProofFileName { get; set; }
        public int? TransporterRegId { get; set; }
        public int? BranchId { get; set; }

        // Items in this store order
        public List<OrderFullDetailItemDto> Items { get; set; } = new();
    }

    public class OrderFullDetailItemDto
    {
        public int OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public int SkuId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}