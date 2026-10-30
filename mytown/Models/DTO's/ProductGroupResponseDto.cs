using System.Text.Json.Serialization;

namespace mytown.Models.DTO_s
{
    public class ProductGroupResponseDto
    {
       
            [JsonPropertyName("prod_group_id")]
            public int ProdGroupId { get; set; }

            [JsonPropertyName("prod_group_name")]
            public string ProdGroupName { get; set; }
        
    }
}
