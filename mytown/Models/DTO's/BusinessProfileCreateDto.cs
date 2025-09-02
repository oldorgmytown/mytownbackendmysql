namespace mytown.Models.DTO_s
{
    public class BusinessProfileCreateDto
    {
      
            public int BusRegId { get; set; }
            public string BusinessUsername { get; set; }
            public string BusinessLocation { get; set; }
            public string BusinessAbout { get; set; }
            public string BusTime { get; set; }
            public string BusinessServiceName { get; set; }
            public string BusinessCategoryName { get; set; }
        public string ProfileStatus { get; set; }

        //public IFormFile BannerFile { get; set; }
        //    public IFormFile LogoFile { get; set; }
        

    }
}
