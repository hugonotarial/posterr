using ApplicationService.Dto.Output;
using ApplicationService.Interface;
using AutoMapper;
using Domain.Model;
using Domain.ValueObject;
using Infra.Data.Interface.Repository;
using System.Threading.Tasks;

namespace ApplicationService
{
    public class FollowerAppService : IFollowerAppService
    {
        private readonly IFollowerRepository _followerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public FollowerAppService(IFollowerRepository followerRepository, IUserRepository userRepository, IMapper mapper)
        {
            _followerRepository = followerRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Result<FollowerOutputDto>> AddAsync(string username, string followusername)
        {
            var result = new Result<FollowerOutputDto>();

            var user = await _userRepository.GetUserByUserNameAsync(username).ConfigureAwait(false);
            var followUser = await _userRepository.GetUserByUserNameAsync(followusername).ConfigureAwait(false);

            if (user is null)
            {
                result.AddError("user_not_found", "The record was not found");
            }

            if (followUser is null)
            {
                result.AddError("followuser_not_found", "The record was not found");
            }

            if (username == followusername)
            {
                result.AddError("invalid_request", "The username can't be the same as follow username");
            }

            var followRecord = await _followerRepository.GetFollowerByUserIdAsync(username, followusername).ConfigureAwait(false);
            if (followRecord is not null)
            {
                result.AddError("invalid_request", $"The user {username } is already following {followusername}");
            }

            if (result.HasError)
            {
                return result;
            }

            var model = new Follower(user.Id, followUser.Id);
            var response = await _followerRepository.AddAsync(model).ConfigureAwait(false);
            result.Content = _mapper.Map<Follower, FollowerOutputDto>(response);
            return result;
        }

        public async Task<Result<bool>> DeleteAsync(string username, string followusername)
        {
            var result = new Result<bool>(true);

            var response = await _followerRepository.GetFollowerByUserIdAsync(username, followusername).ConfigureAwait(false);
            if (response is null)
            {
                result.Content = false;
                result.AddError("record_not_found", "The record was not found");
                return result;
            }

            await _followerRepository.DeleteAync(response.Id).ConfigureAwait(false);
            return result;
        }
    }
}
