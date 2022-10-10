using Domain.Model.Base;

namespace Domain.Model
{
    public class Follower : ModelBase
    {
        public Follower()
        {

        }

        public Follower(int userId, int followingUserId)
        {
            UserId = userId;
            FollowingUserId = followingUserId;
        }

        public int UserId { get; set; }
        public int FollowingUserId { get; set; }
    }
}
