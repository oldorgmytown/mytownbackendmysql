namespace mytown.Models.DTO_s
{
    public class ShopperExperienceCommentDto
    {
        public int ShopperExperienceCommentId { get; set; }

        public int ShopperExperienceId { get; set; }

        public int ShopperRegId { get; set; }

        public string ShopperName { get; set; }

        public string CommentText { get; set; }

        public bool IsAnonymous { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
