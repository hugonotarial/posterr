using System;

namespace ApplicationService.Dto.Output
{
    public class UserInsertedOutputDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
