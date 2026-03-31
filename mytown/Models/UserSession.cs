namespace mytown.Models
{
    public class UserSession
    {
        public int Id { get; set; }
        public int UserId { get; set; }            // ShopperRegId or BusRegId or CourierId
        public string UserType { get; set; }       // "Shopper" / "Business" / "Courier"
        public string SessionGuid { get; set; }    // GUID as string
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }   // optional
        public string? IpAddress { get; set; }     // optional
        public string? DeviceInfo { get; set; }
    }
}
