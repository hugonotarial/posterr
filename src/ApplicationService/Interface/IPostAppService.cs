using ApplicationService.Dto.Input;
using ApplicationService.Dto.Output;
using Domain.ValueObject;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationService.Interface
{
    public interface IPostAppService
    {
        Task<Result<IEnumerable<PostGetOutputDto>>> GetPostsByFilterAsync(string userName, int fetchNext, int offset, bool allPosts);
        Task<Result<PostOutputDto>> AddAsync(string username, PostInputDto model);
    }
}
