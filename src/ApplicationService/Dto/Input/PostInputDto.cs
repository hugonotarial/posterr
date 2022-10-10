namespace ApplicationService.Dto.Input
{
    public class PostInputDto
    {
        public string Message { get; set; }
        public string Type { get; set; } = "regular_post";
        public int? PostOriginId { get; set; }
    }
}
