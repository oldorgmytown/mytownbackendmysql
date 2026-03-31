namespace mytown.Models.DTO_s;
using System.ComponentModel.DataAnnotations;

public class CreateOrderRequestddto
{
  
        [Required]
        public int ShopperRegId { get; set; }

        [Required]
        public List<StoreShippingSelection> ShippingSelections { get; set; }

        public int? SelectedAltAddressId { get; set; }
    //null == default address

        //  NEW
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

