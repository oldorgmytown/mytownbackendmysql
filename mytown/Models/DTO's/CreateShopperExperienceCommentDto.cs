namespace mytown.Models.DTO_s
{
    public class CreateShopperExperienceCommentDto
    {
        public int ShopperExperienceId { get; set; }

        public int ShopperRegId { get; set; }

        public string CommentText { get; set; }

        public bool IsAnonymous { get; set; }
    }
}
