using System.Threading.Tasks;

namespace Infra.Data.Interface.Repository.Base
{
    public interface IBaseRepository<T>
    {
        Task<T> AddAsync(T model);
        Task DeleteAync(int id);
    }
}
