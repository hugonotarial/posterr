using Domain.Model;
using System.Threading.Tasks;

namespace Infra.Data.Interface.Repository
{
    public interface IFollowerRepository
    {
        Task<Follower> AddAsync(Follower model);
        Task DeleteAync(int id);
        Task<Follower> GetFollowerByUserIdAsync(string username, string followusername);
    }
}
