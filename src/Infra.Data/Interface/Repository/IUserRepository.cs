using Domain.Model;
using System.Threading.Tasks;

namespace Infra.Data.Interface.Repository
{
    public interface IUserRepository
    {
        Task<User> GetUserProfileAsync(string userName);
        Task<User> GetUserByUserNameAsync(string userName);
        Task<User> AddAsync(User model);
        Task<User> GetUserByIdAsync(int id);
    }
}
