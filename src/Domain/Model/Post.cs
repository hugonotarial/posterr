using System;
using Domain.Model.Base;

namespace Domain.Model
{
    public class Post : ModelBase
    {
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; }
        public int? PostOriginId { get; set; }

        public bool? Following { get; set; }
        public string UserName { get; set; }
    }
}