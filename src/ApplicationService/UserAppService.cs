using ApplicationService.Dto.Input;
using ApplicationService.Dto.Output;
using ApplicationService.Interface;
using AutoMapper;
using Domain.Model;
using Domain.ValueObject;
using Infra.Data.Interface.Repository;
using System.Threading.Tasks;

namespace ApplicationService
{
    public class UserAppService : IUserAppService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserAppService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Result<UserInsertedOutputDto>> AddAsync(UserInputDto input)
        {
            var result = new Result<UserInsertedOutputDto>();

            var user = await _userRepository.GetUserByUserNameAsync(input.UserName).ConfigureAwait(false);
            if (user is not null)
            {
                result.AddError("user_already_exists", "The user already exists.");
                return result;
            }
            
            var model = _mapper.Map<UserInputDto, User>(input);
            var response = await _userRepository.AddAsync(model).ConfigureAwait(false);
            var userDetail = await _userRepository.GetUserByIdAsync(response.Id).ConfigureAwait(false);
            
            result.Content = _mapper.Map<User, UserInsertedOutputDto>(userDetail);
            return result;
        }

        public async Task<Result<UserOutputDto>> GetUserProfileAsync(string userName)
        {
            var response = await _userRepository.GetUserProfileAsync(userName).ConfigureAwait(false);
            var result = _mapper.Map<User, UserOutputDto>(response);
            return new Result<UserOutputDto>(result);
        }
    }
}
