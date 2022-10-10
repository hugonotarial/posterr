using Dapper;
using Domain.Model;
using Infra.Data.Interface;
using Infra.Data.Interface.Repository;
using Infra.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infra.Data.Repository
{
    public class PostRepository : BaseRepository<Post>, IPostRepository
    {
        public PostRepository(IDbContext context) : base(context)
        {
        }

        protected override string AddScript
        {
            get =>
                $@"INSERT INTO [dbo].[Posts]
                            ([Message]
                            ,[UserId]
                            ,[Type]
                            ,[PostOriginId])
                      OUTPUT Inserted.ID
                      VALUES
                            (@Message
                            ,@UserId
                            ,@Type
                            ,@PostOriginId)";

        }

        private readonly string _getPostsByDate =
            @"SELECT COUNT(Id)
                FROM Posts
               WHERE UserId = (SELECT Id FROM Users WHERE UserName = @userName)
                 AND CONVERT(varchar, CreatedAt, 23) = @postDate";

        private readonly string _getPostsByIdFilter =
            @"SELECT P.Id,                        
                     U.UserName,
                     P.Message,
                     P.CreatedAt,
                     P.Type,
                     P.PostOriginId,
                     CASE
                         WHEN @UserName = U.UserName THEN NULL
                         WHEN
                              (SELECT F.id
                                 FROM Followers F
                                INNER JOIN Users MU ON MU.Id = F.UserId
                                INNER JOIN Users FU ON FU.Id = F.FollowingUserId
                                WHERE MU.Username = @UserName
                                  AND FU.Username = U.Username) IS NULL THEN 0
                        ELSE 1
                     END AS Following
                FROM Posts P
               INNER JOIN Users U ON U.Id = p.UserId 
               WHERE P.Id = @Id";

        private readonly string _getPostsByFilter =
            @"SELECT P.Id,                        
                     U.UserName,
                     P.Message,
                     P.CreatedAt,
                     P.Type,
                     P.PostOriginId,
                     CASE
                         WHEN @userName = U.UserName THEN NULL
                         WHEN
                              (SELECT F.id
                                 FROM Followers F
                                INNER JOIN Users MU ON MU.Id = F.UserId
                                INNER JOIN Users FU ON FU.Id = F.FollowingUserId
                                WHERE MU.Username = @userName
                                  AND FU.Username = U.Username) IS NULL THEN 0
                        ELSE 1
                     END AS Following
                FROM Posts P
               INNER JOIN Users U ON U.Id = p.UserId 
               WHERE (U.UserName = @userName OR @allPosts = 1)
               ORDER BY P.CreatedAt DESC
              OFFSET @offset ROWS FETCH NEXT @fetchNext ROWS ONLY";

        public Task<Post> GetPostsByIdFilterAsync(int id, string userName)
        {
            return
                _dbContext.Connection.QueryFirstOrDefaultAsync<Post>(
                    _getPostsByIdFilter,
                    new { Id = id, UserName = userName },
                    _dbContext.Transaction);
        }

        public Task<IEnumerable<Post>> GetPostsByFilterAsync(string userName, int fetchNext, int offset, bool allPosts)
        {
            return
                _dbContext.Connection.QueryAsync<Post>(
                    _getPostsByFilter,
                    new { fetchNext, offset, userName, allPosts },
                    _dbContext.Transaction);
        }

        public Task<int> GetPostsByPostDateAsync(string userName, string postDate)
        {
            return
                _dbContext.Connection.QueryFirstOrDefaultAsync<int>(
                    _getPostsByDate,
                    new { userName, postDate },
                    _dbContext.Transaction);
        }
    }
}
