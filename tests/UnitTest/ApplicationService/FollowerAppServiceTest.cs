using ApplicationService;
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

namespace UnitTest.ApplicationService.Validation
{
    public class FollowerAppServiceTest
    {
        private readonly IFollowerAppService _followerAppService;
        private readonly IFollowerRepository _followerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        private readonly Fixture _fixture;

        public FollowerAppServiceTest()
        {
            _followerRepository = Substitute.For<IFollowerRepository>();
            _userRepository = Substitute.For<IUserRepository>();
            _mapper = Substitute.For<IMapper>();
            _followerAppService = new FollowerAppService(_followerRepository, _userRepository, _mapper);

            _fixture = new Fixture();
        }

        [Fact]
        public async Task WhenAdd_WithCorrectParamters_ShouldReturnSuccess()
        {
            // arrange
            var user = _fixture.Create<User>();
            var username = _fixture.Create<string>();

            var followerUsername = _fixture.Create<string>();
            var followerUser = _fixture.Create<User>();

            var follower = _fixture.Create<Follower>();
            var followeOutput = _fixture.Create<FollowerOutputDto>();

            _userRepository.GetUserByUserNameAsync(username).Returns(user);
            _userRepository.GetUserByUserNameAsync(followerUsername).Returns(followerUser);
            _mapper.Map<Follower, FollowerOutputDto>(follower).ReturnsForAnyArgs(followeOutput);

            // act
            var response = await _followerAppService.AddAsync(username, followerUsername).ConfigureAwait(false);

            // assert
            response.HasError.Should().BeFalse();
        }

        [Fact]
        public async Task WhenAdd_WithInvalidUserParamter_ShouldReturnError()
        {
            // arrange
            User user = null;
            var username = _fixture.Create<string>();

            var followerUsername = _fixture.Create<string>();
            var followerUser = _fixture.Create<User>();

            var follower = _fixture.Create<Follower>();
            var followeOutput = _fixture.Create<FollowerOutputDto>();

            _userRepository.GetUserByUserNameAsync(username).Returns(user);
            _userRepository.GetUserByUserNameAsync(followerUsername).Returns(followerUser);
            _mapper.Map<Follower, FollowerOutputDto>(follower).ReturnsForAnyArgs(followeOutput);

            // act
            var response = await _followerAppService.AddAsync(username, followerUsername).ConfigureAwait(false);

            // assert
            response.HasError.Should().BeTrue();
            response.Errors.Count().Should().Be(1);
            response.Errors.FirstOrDefault().Key.Should().Be("user_not_found");
        }

        [Fact]
        public async Task WhenAdd_WithInvalidFollowerUserParamter_ShouldReturnError()
        {
            // arrange
            var user = _fixture.Create<User>();
            var username = _fixture.Create<string>();

            var followerUsername = _fixture.Create<string>();
            User followerUser = null;

            var follower = _fixture.Create<Follower>();
            var followeOutput = _fixture.Create<FollowerOutputDto>();

            _userRepository.GetUserByUserNameAsync(username).Returns(user);
            _userRepository.GetUserByUserNameAsync(followerUsername).Returns(followerUser);
            _mapper.Map<Follower, FollowerOutputDto>(follower).ReturnsForAnyArgs(followeOutput);

            // act
            var response = await _followerAppService.AddAsync(username, followerUsername).ConfigureAwait(false);

            // assert
            response.HasError.Should().BeTrue();
            response.Errors.Count().Should().Be(1);
            response.Errors.FirstOrDefault().Key.Should().Be("followuser_not_found");
        }

        [Fact]
        public async Task WhenAdd_WithEqualUsersParamters_ShouldReturnError()
        {
            // arrange
            var user = _fixture.Create<User>();
            var username = _fixture.Create<string>();

            var followerUsername = username;
            var followerUser = _fixture.Create<User>();

            var follower = _fixture.Create<Follower>();
            var followeOutput = _fixture.Create<FollowerOutputDto>();

            _userRepository.GetUserByUserNameAsync(username).Returns(user);
            _userRepository.GetUserByUserNameAsync(followerUsername).Returns(followerUser);
            _mapper.Map<Follower, FollowerOutputDto>(follower).ReturnsForAnyArgs(followeOutput);

            // act
            var response = await _followerAppService.AddAsync(username, followerUsername).ConfigureAwait(false);

            // assert
            response.HasError.Should().BeTrue();
            response.Errors.Count().Should().Be(1);
            response.Errors.FirstOrDefault().Key.Should().Be("invalid_request");
        }

        [Fact]
        public async Task WhenAdd_WithExistentRecordParamters_ShouldReturnError()
        {
            // arrange
            var user = _fixture.Create<User>();
            var username = _fixture.Create<string>();

            var followerUsername = username;
            var followerUser = _fixture.Create<User>();

            var follower = _fixture.Create<Follower>();
            var followeOutput = _fixture.Create<FollowerOutputDto>();

            _userRepository.GetUserByUserNameAsync(username).Returns(user);
            _userRepository.GetUserByUserNameAsync(followerUsername).Returns(followerUser);
            _mapper.Map<Follower, FollowerOutputDto>(follower).ReturnsForAnyArgs(followeOutput);
            _followerRepository.GetFollowerByUserIdAsync(username, followerUsername).Returns(follower);

            // act
            var response = await _followerAppService.AddAsync(username, followerUsername).ConfigureAwait(false);

            // assert
            response.HasError.Should().BeTrue();
            response.Errors.Count().Should().Be(1);
            response.Errors.FirstOrDefault().Key.Should().Be("invalid_request");
        }

        [Fact]
        public async Task WhenDelete_WithCorrectParamters_ShouldReturnSuccess()
        {
            // arrange
            var follower = _fixture.Create<Follower>();
            var username = _fixture.Create<string>();
            var followerUsername = _fixture.Create<string>();

            _followerRepository.GetFollowerByUserIdAsync(username, followerUsername).Returns(follower);

            // act
            var response = await _followerAppService.DeleteAsync(username, followerUsername).ConfigureAwait(false);

            // assert
            response.HasError.Should().BeFalse();
        }

        [Fact]
        public async Task WhenDelete_WithNotFoundRecord_ShouldReturnError()
        {
            // arrange
            Follower follower = null;
            var username = _fixture.Create<string>();
            var followerUsername = _fixture.Create<string>();

            _followerRepository.GetFollowerByUserIdAsync(username, followerUsername).Returns(follower);

            // act
            var response = await _followerAppService.DeleteAsync(username, followerUsername).ConfigureAwait(false);

            // assert
            response.HasError.Should().BeTrue();
            response.Errors.Count().Should().Be(1);
            response.Errors.FirstOrDefault().Key.Should().Be("record_not_found");
        }
    }
}
