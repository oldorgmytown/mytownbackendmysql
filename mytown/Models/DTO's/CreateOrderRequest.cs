namespace mytown.Models.DTO_s;
using System.ComponentModel.DataAnnotations;

public class CreateOrderRequestddto
{
    public int? ShopperRegId { get; set; }

    public int? GuestRegId { get; set; }

    public bool IsGuestOrder { get; set; }

    public List<StoreShippingSelection> ShippingSelections { get; set; }

    public int? SelectedAltAddressId { get; set; } //null== dafult address

    public bool UseCart { get; set; } = true;

    public List<BuyNowItem>? Items { get; set; }
}

public class BuyNowItem
    {
        public int ProductId { get; set; }
        public int? SkuId { get; set; }
        public int Quantity { get; set; }
        
        public decimal ? Price { get; set; }
    }

