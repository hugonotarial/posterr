using Dapper;
using Domain.Model;
using Infra.Data.Interface;
using Infra.Data.Interface.Repository;
using Infra.Data.Repository.Base;
using System.Threading.Tasks;

namespace Infra.Data.Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        protected override string AddScript
        {
            get =>
                $@"INSERT INTO [dbo].[Users]
                            ([UserName]
                            ,[Name]
                            ,[PhoneNumber]
                            ,[Address])
                      OUTPUT Inserted.ID
                      VALUES
                            (@UserName
                            ,@Name
                            ,@PhoneNumber
                            ,@Address)";

        }

        private readonly string _getUserProfile =
            @"SELECT U.UserName,
                     U.CreatedAt,

	                (SELECT COUNT(FollowingUserId)
 	                 FROM Followers
 	                 WHERE UserId = U.Id) AS NumberFollowing,

	                (SELECT COUNT(UserId)
	                 FROM Followers
	                 WHERE FollowingUserId = U.Id) AS NumberOfFollowers,

	                (SELECT COUNT(Id)
	                    FROM Posts
	                    WHERE UserId = U.Id) AS NumberOfPosts

                FROM Users U
               WHERE U.UserName = @userName";

        private readonly string _getUserById =
            @"SELECT U.Id,
                     U.UserName,
                     U.CreatedAt,
                     U.Name,
                     U.PhoneNumber,
                     U.Address
                FROM Users U
               WHERE U.Id = @id";


        private readonly string _getUserByUserName =
            @"SELECT U.Id,
                     U.UserName,
                     U.CreatedAt
                FROM Users U
               WHERE U.UserName = @userName";


        public UserRepository(IDbContext context) : base(context)
        {
        }

        public Task<User> GetUserByIdAsync(int id)
        {
            return
            _dbContext.Connection.QueryFirstOrDefaultAsync<User>(
                _getUserById,
                new { Id = id },
                _dbContext.Transaction);
        }

        public Task<User> GetUserProfileAsync(string userName)
        {
            return
            _dbContext.Connection.QueryFirstOrDefaultAsync<User>(
                _getUserProfile,
                new { userName },
                _dbContext.Transaction);
        }

        public Task<User> GetUserByUserNameAsync(string userName)
        {
            return
            _dbContext.Connection.QueryFirstOrDefaultAsync<User>(
                _getUserByUserName,
                new { userName },
                _dbContext.Transaction);
        }
    }
}
