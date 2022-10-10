using System;
using Domain.Model.Base;

namespace Domain.Model
{
    public class User : ModelBase
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public DateTime CreatedAt { get; set; }

        public int NumberOfFollowers { get; set; }
        public int NumberFollowing { get; set; }
        public int NumberOfPosts { get; set; }
    }
}
