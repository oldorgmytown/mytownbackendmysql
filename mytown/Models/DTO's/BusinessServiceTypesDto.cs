namespace mytown.Models.DTO_s
{
    public class BusinessServiceTypesDto
    {
        public int BusServId { get; set; }
        public string BusinessServiceName { get; set; }

        public List<string> ServiceTypeNames { get; set; } = new();
    }
}
