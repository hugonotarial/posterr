using ApplicationService.Dto.Output;
using Domain.ValueObject;
using System.Threading.Tasks;

namespace ApplicationService.Interface
{
    public interface IFollowerAppService
    {
        Task<Result<FollowerOutputDto>> AddAsync(string username, string followusername);
        Task<Result<bool>> DeleteAsync(string username, string followusername);
    }
}
