namespace ApplicationService.Dto.Output
{
    public class PostOutputDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public string CreatedAt { get; set; }
        public int PostOriginId { get; set; }
    }
}
