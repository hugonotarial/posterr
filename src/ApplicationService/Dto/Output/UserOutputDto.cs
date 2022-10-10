namespace ApplicationService.Dto.Output
{
    public class UserOutputDto
    {
        public string UserName { get; set; }
        public string CreatedAt { get; set; }

        public int NumberOfFollowers { get; set; }
        public int NumberFollowing { get; set; }
        public int NumberOfPosts { get; set; }
    }
}
