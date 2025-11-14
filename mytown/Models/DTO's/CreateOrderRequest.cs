namespace mytown.Models.DTO_s;
using System.ComponentModel.DataAnnotations;

public class CreateOrderRequestddto
{
    [Required(ErrorMessage = "ShopperRegId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "ShopperRegId must be a valid positive number.")]
    public int ShopperRegId { get; set; }

    [Required(ErrorMessage = "ShippingSelections are required.")]
    [MinLength(1, ErrorMessage = "At least one store shipping selection is required.")]
    public List<StoreShippingSelection> ShippingSelections { get; set; }
}
