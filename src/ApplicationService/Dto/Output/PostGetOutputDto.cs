namespace ApplicationService.Dto.Output
{
    public class PostGetOutputDto
    {
        public int PostId { get; set; }
        public string PostUserName { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public string CreatedAt { get; set; }
        public bool Following { get; set; }
        public int? PostOriginId { get; set; }
        public PostGetOutputDto PostOrigin { get; set; }
    }
}
