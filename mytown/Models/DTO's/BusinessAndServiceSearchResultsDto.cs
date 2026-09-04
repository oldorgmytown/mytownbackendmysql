using mytown.Models;

namespace mytown.DTOs
{
    public class BusinessAndServiceSearchResultsDto
    {
        public List<BusinessProfile> BusinessProfiles { get; set; } = new();

        public List<ServiceProfile> ServiceProfiles { get; set; } = new();
    }
}