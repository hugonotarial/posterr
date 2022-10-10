using Dapper;
using Domain.Model;
using Infra.Data.Interface;
using Infra.Data.Interface.Repository;
using Infra.Data.Repository.Base;
using System.Threading.Tasks;

namespace Infra.Data.Repository
{
    public class FollowerRepository : BaseRepository<Follower>, IFollowerRepository
    {
        public FollowerRepository(IDbContext context) : base(context)
        {
        }

        protected override string AddScript
        {
            get =>
                $@"INSERT INTO [dbo].[Followers]
                       ([UserId]
                       ,[FollowingUserId])
                 OUTPUT Inserted.ID
                 VALUES
                       (@userId
                       ,@followingUserId)";
        }

        protected override string DeleteScript
        {
            get =>
                $@"DELETE FROM [dbo].[Followers]
                    WHERE Id = @Id";
        }

        private readonly string _getFollowerByUserIdFollowerId =
            @"SELECT [Id]
                    ,[UserId]
                    ,[FollowingUserId]
                FROM [dbo].[Followers]
               WHERE UserId = (SELECT Id FROM [dbo].Users WHERE UserName = @UserName)
                 AND FollowingUserId = (SELECT Id FROM [dbo].Users WHERE UserName = @FollowingUserId)";


        public Task<Follower> GetFollowerByUserIdAsync(string username, string followusername)
        {
            return
                _dbContext.Connection.QueryFirstOrDefaultAsync<Follower>(
                    _getFollowerByUserIdFollowerId,
                    new { UserName = username, FollowingUserId = followusername },
                    _dbContext.Transaction);
        }
    }
}
