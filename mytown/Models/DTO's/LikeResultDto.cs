namespace mytown.Models.DTO_s
{
    public class LikeResultDto
    {
        public bool Liked { get; set; }       // true = now liked, false = like removed
        public int LikeCount { get; set; }
    }
}
