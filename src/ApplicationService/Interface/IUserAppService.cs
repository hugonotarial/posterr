using ApplicationService.Dto.Input;
using ApplicationService.Dto.Output;
using Domain.ValueObject;
using System.Threading.Tasks;

namespace ApplicationService.Interface
{
    public interface IUserAppService
    {
        Task<Result<UserOutputDto>> GetUserProfileAsync(string userName);
        Task<Result<UserInsertedOutputDto>> AddAsync(UserInputDto input);
    }
}
