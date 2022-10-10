using ApplicationService;
using ApplicationService.Dto.Input;
using ApplicationService.Dto.Output;
using ApplicationService.Interface;
using AutoFixture;
using AutoMapper;
using Domain.Model;
using FluentAssertions;
using Infra.Data.Interface.Repository;
using NSubstitute;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.ApplicationService
{
    public class UserAppServiceTest
    {
        private readonly IUserAppService _userAppService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        private readonly Fixture _fixture;
        public UserAppServiceTest()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _mapper = Substitute.For<IMapper>();
            _userAppService = new UserAppService(_userRepository, _mapper);

            _fixture = new Fixture();
        }

        [Fact]
        public async Task WhenAdd_WithCorrectParameters_ShouldReturnSuccess()
        {
            // arrange
            var user = _fixture.Create<User>();
            var input = _fixture.Build<UserInputDto>().Create();
            var output = _fixture.Create<UserInsertedOutputDto>();

            _mapper.Map<UserInputDto, User>(input).Returns(user);
            _userRepository.AddAsync(user).Returns(user);
            _mapper.Map<User, UserInsertedOutputDto>(user).Returns(output);

            // act
            var response = await _userAppService.AddAsync(input).ConfigureAwait(false);

            // assert
            response.HasError.Should().BeFalse();
        }

        [Fact]
        public async Task WhenAdd_AAlreadyExistentUser_ShouldReturnError()
        {
            // arrange
            var user = _fixture.Create<User>();
            var input = _fixture.Build<UserInputDto>().Create();

            _userRepository.GetUserByUserNameAsync(input.UserName).Returns(user);

            // act
            var response = await _userAppService.AddAsync(input).ConfigureAwait(false);

            // assert
            response.HasError.Should().BeTrue();
            response.Errors.Count().Should().Be(1);
            response.Errors.FirstOrDefault().Key.Should().Be("user_already_exists");
        }

        [Fact]
        public async Task WhenGetUserProfile_WithCorrectParameters_ShouldReturnSuccess()
        {
            // arrange
            var username = _fixture.Create<string>();
            var user = _fixture.Create<User>();
            var output = _fixture.Create<UserOutputDto>();

            _userRepository.GetUserProfileAsync(username).Returns(user);
            _mapper.Map<User, UserOutputDto>(user).Returns(output);

            // act
            var response = await _userAppService.GetUserProfileAsync(username).ConfigureAwait(false);

            // assert
            response.HasError.Should().BeFalse();
        }
    }
}
