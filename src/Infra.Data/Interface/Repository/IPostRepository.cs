using Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infra.Data.Interface.Repository
{
    public interface IPostRepository
    {
        Task<IEnumerable<Post>> GetPostsByFilterAsync(string userName, int fetchNext, int offset, bool allPosts);
        Task<Post> GetPostsByIdFilterAsync(int id, string userName);
        Task<Post> AddAsync(Post model);
        Task<int> GetPostsByPostDateAsync(string userName, string postDate);
    }
}
