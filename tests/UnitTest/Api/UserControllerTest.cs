using Api.Controllers;
using ApplicationService.Dto.Input;
using ApplicationService.Dto.Output;
using ApplicationService.Interface;
using ApplicationService.Interface.Validation;
using AutoFixture;
using Domain.ValueObject;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Api
{
    public class UserControllerTest
    {
        private readonly UserController _userController;

        private readonly IUserAppService _userAppService;
        private readonly IPostAppService _postAppService;
        private readonly IFollowerAppService _followerAppService;
        private readonly IPostValidator _postValidator;
        private readonly IUserValidator _userValidator;
        private readonly Fixture _fixture;

        public UserControllerTest()
        {
            _userAppService = Substitute.For<IUserAppService>();
            _postAppService = Substitute.For<IPostAppService>();
            _followerAppService = Substitute.For<IFollowerAppService>();
            _postValidator = Substitute.For<IPostValidator>();
            _userValidator = Substitute.For<IUserValidator>();

            _fixture = new Fixture();

            _userController = new UserController(_userAppService, _postAppService, _followerAppService, _postValidator, _userValidator);
        }

        [Fact]
        public async Task WhenAddUser_WithCorrectParameter_ShouldReturnSuccess()
        {
            // arrange
            var input = _fixture.Create<UserInputDto>();
            var output = new Result<UserInsertedOutputDto>(_fixture.Create<UserInsertedOutputDto>());
            var errors = new Dictionary<string, IList<string>>();

            _userValidator.IsValid(input).Returns(errors);
            _userAppService.AddAsync(input).Returns(output);

            // act
            var response = await _userController.AddUserAsync(input).ConfigureAwait(false);

            // assert
            var statusCode = GetHttpStatusCode(response);
            statusCode.Should().Be(StatusCodes.Status201Created);
        }


        [Fact]
        public async Task WhenAddUser_WithInvalidValidationParameter_ShouldReturnError()
        {
            // arrange
            var input = _fixture.Create<UserInputDto>();
            var errors = new Dictionary<string, IList<string>>
            {
                { _fixture.Create<string>(), _fixture.CreateMany<string>().ToList() }
            };
            _userValidator.IsValid(input).Returns(errors);

            // act
            var response = await _userController.AddUserAsync(input).ConfigureAwait(false);

            // assert
            var statusCode = GetHttpStatusCode(response);
            statusCode.Should().Be(StatusCodes.Status400BadRequest);
        }


        [Fact]
        public async Task WhenAddUser_WithInvalidParameter_ShouldReturnError()
        {
            // arrange
            var input = _fixture.Create<UserInputDto>();
            var output = new Result<UserInsertedOutputDto>(_fixture.Create<UserInsertedOutputDto>());
            output.AddError(_fixture.Create<string>(), _fixture.Create<string>());

            var errors = new Dictionary<string, IList<string>>();
            _userValidator.IsValid(input).Returns(errors);
            _userAppService.AddAsync(input).Returns(output);

            // act
            var response = await _userController.AddUserAsync(input).ConfigureAwait(false);

            // assert
            var statusCode = GetHttpStatusCode(response);
            statusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task WhenGetUserProfile_WithCorrectParameter_ShouldReturnSuccess()
        {
            // arrange
            var username = _fixture.Create<string>();
            var output = new Result<UserOutputDto>(_fixture.Create<UserOutputDto>());

            _userAppService.GetUserProfileAsync(username).Returns(output);

            // act
            var response = await _userController.GetUserProfileAsync(username).ConfigureAwait(false);

            // assert
            var statusCode = GetHttpStatusCode(response);
            statusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task WhenAddFollower_WithCorrectParameter_ShouldReturnSuccess()
        {
            // arrange
            var username = _fixture.Create<string>();
            var followUsername = _fixture.Create<string>();
            var output = new Result<FollowerOutputDto>(_fixture.Create<FollowerOutputDto>());

            _followerAppService.AddAsync(username, followUsername).Returns(output);

            // act
            var response = await _userController.AddFollowerAsync(username, followUsername).ConfigureAwait(false);

            // assert
            var statusCode = GetHttpStatusCode(response);
            statusCode.Should().Be(StatusCodes.Status202Accepted);
        }

        [Fact]
        public async Task WhenAddFollower_WithInvalidParameter_ShouldReturnError()
        {
            // arrange
            var username = _fixture.Create<string>();
            var followUsername = _fixture.Create<string>();
            var output = new Result<FollowerOutputDto>(_fixture.Create<FollowerOutputDto>());
            output.AddError(_fixture.Create<string>(), _fixture.Create<string>());

            _followerAppService.AddAsync(username, followUsername).Returns(output);

            // act
            var response = await _userController.AddFollowerAsync(username, followUsername).ConfigureAwait(false);

            // assert
            var statusCode = GetHttpStatusCode(response);
            statusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task WhenDeleteFollower_WithCorrectParameter_ShouldReturnSuccess()
        {
            // arrange
            var username = _fixture.Create<string>();
            var followUsername = _fixture.Create<string>();
            var output = new Result<bool>(true);

            _followerAppService.DeleteAsync(username, followUsername).Returns(output);


            // act
            var response = await _userController.DeleteFollowerAsync(username, followUsername).ConfigureAwait(false);

            // assert
            var statusCode = GetHttpStatusCode(response);
            statusCode.Should().Be(StatusCodes.Status204NoContent);
        }

        [Fact]
        public async Task WhenDeleteFollower_WithInvalidParameter_ShouldReturnError()
        {
            // arrange
            var username = _fixture.Create<string>();
            var followUsername = _fixture.Create<string>();
            var output = new Result<bool>(true);
            output.AddError(_fixture.Create<string>(), _fixture.Create<string>());

            _followerAppService.DeleteAsync(username, followUsername).Returns(output);


            // act
            var response = await _userController.DeleteFollowerAsync(username, followUsername).ConfigureAwait(false);

            // assert
            var statusCode = GetHttpStatusCode(response);
            statusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task WhenGetPostsByFilter_WithCorrectParameter_ShouldReturnSuccess()
        {
            // arrange
            var username = _fixture.Create<string>();
            var fetch_next = _fixture.Create<int>();
            var offset = _fixture.Create<int>();
            var all_posts = _fixture.Create<bool>();

            var output = new Result<IEnumerable<PostGetOutputDto>>(_fixture.Build<PostGetOutputDto>().Without(x => x.PostOrigin).CreateMany());

            _postAppService.GetPostsByFilterAsync(username, fetch_next, offset, all_posts).Returns(output);

            // act
            var response = await _userController.GetPostsByFilterAsync(username, fetch_next, offset, all_posts).ConfigureAwait(false);

            // assert
            var statusCode = GetHttpStatusCode(response);
            statusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task WhenAddPost_WithCorrectParameter_ShouldReturnSuccess()
        {
            // arrange
            var username = _fixture.Create<string>();
            var input = _fixture.Create<PostInputDto>();
            var errors = new Dictionary<string, IList<string>>();
            var output = new Result<PostOutputDto>(_fixture.Create<PostOutputDto>());

            _postValidator.IsValid(input).Returns(errors);
            _postAppService.AddAsync(username, input).Returns(output);

            // act
            var response = await _userController.AddPostAsync(username, input).ConfigureAwait(false);

            // assert
            var statusCode = GetHttpStatusCode(response);
            statusCode.Should().Be(StatusCodes.Status201Created);
        }

        [Fact]
        public async Task WhenAddPost_WithInvalidValidationParameter_ShouldReturnError()
        {
            // arrange
            var username = _fixture.Create<string>();
            var input = _fixture.Create<PostInputDto>();
            var errors = new Dictionary<string, IList<string>>
            {
                { _fixture.Create<string>(), _fixture.CreateMany<string>().ToList() }
            };

            _postValidator.IsValid(input).Returns(errors);

            // act
            var response = await _userController.AddPostAsync(username, input).ConfigureAwait(false);

            // assert
            var statusCode = GetHttpStatusCode(response);
            statusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task WhenAddPost_WithInvalidParameter_ShouldReturnError()
        {
            // arrange
            var username = _fixture.Create<string>();
            var input = _fixture.Create<PostInputDto>();
            var errors = new Dictionary<string, IList<string>>();
            var output = new Result<PostOutputDto>(_fixture.Create<PostOutputDto>());
            output.AddError(_fixture.Create<string>(), _fixture.Create<string>());

            _postValidator.IsValid(input).Returns(errors);
            _postAppService.AddAsync(username, input).Returns(output);

            // act
            var response = await _userController.AddPostAsync(username, input).ConfigureAwait(false);

            // assert
            var statusCode = GetHttpStatusCode(response);
            statusCode.Should().Be(StatusCodes.Status400BadRequest);
        }


        private static HttpStatusCode GetHttpStatusCode(IActionResult functionResult)
        {
            try
            {
                return (HttpStatusCode)functionResult
                    .GetType()
                    .GetProperty("StatusCode")
                    .GetValue(functionResult, null);
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}
