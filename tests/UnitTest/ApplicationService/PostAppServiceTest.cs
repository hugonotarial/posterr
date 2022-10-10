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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.ApplicationService
{
    public class PostAppServiceTest
    {
        private readonly IPostAppService _postAppService;
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        private readonly Fixture _fixture;
        public PostAppServiceTest()
        {
            _postRepository = Substitute.For<IPostRepository>();
            _userRepository = Substitute.For<IUserRepository>();
            _mapper = Substitute.For<IMapper>();
            _postAppService = new PostAppService(_postRepository, _userRepository, _mapper);

            _fixture = new Fixture();
        }

        [Fact]
        public async Task WhenAdd_WithCorrectParameters_ShouldReturnSuccess()
        {
            // arrange
            var username = _fixture.Create<string>();
            var input = _fixture.Build<PostInputDto>().Without(x => x.PostOriginId).Create();
            var model = _fixture.Create<Post>();
            var output = _fixture.Create<PostOutputDto>();
            var user = _fixture.Create<User>();

            _userRepository.GetUserByUserNameAsync(username).Returns(user);
            _mapper.Map<PostInputDto, Post>(input).Returns(model);
            _postRepository.AddAsync(model).Returns(model);
            _postRepository.GetPostsByIdFilterAsync(model.Id, username).Returns(model);
            _mapper.Map<Post, PostOutputDto>(model).Returns(output);

            // act
            var response = await _postAppService.AddAsync(username, input).ConfigureAwait(false);

            // assert
            response.HasError.Should().BeFalse();
        }

        [Fact]
        public async Task WhenAdd_WithMoreThan5PostsADay_ShouldReturnError()
        {
            // arrange
            var username = _fixture.Create<string>();
            var input = _fixture.Build<PostInputDto>().Without(x => x.PostOriginId).Create();
            var user = _fixture.Create<User>();

            _userRepository.GetUserByUserNameAsync(username).Returns(user);
            _postRepository.GetPostsByPostDateAsync(username, DateTime.Now.ToString("yyyy-MM-dd")).Returns(6);

            // act
            var response = await _postAppService.AddAsync(username, input).ConfigureAwait(false);

            // assert
            response.HasError.Should().BeTrue();
            response.Errors.Count().Should().Be(1);
            response.Errors.FirstOrDefault().Key.Should().Be("limit_exceeded");
        }

        [Fact]
        public async Task WhenAdd_WithInvalidPostOriginId_ShouldReturnError()
        {
            // arrange
            var username = _fixture.Create<string>();
            var input = _fixture.Build<PostInputDto>().Create();
            var user = _fixture.Create<User>();

            _userRepository.GetUserByUserNameAsync(username).Returns(user);

            // act
            var response = await _postAppService.AddAsync(username, input).ConfigureAwait(false);

            // assert
            response.HasError.Should().BeTrue();
            response.Errors.Count().Should().Be(1);
            response.Errors.FirstOrDefault().Key.Should().Be("origin_not_found");
        }


        [Fact]
        public async Task WhenAdd_WithQuoteTypeNotAllowed_ShouldReturnError()
        {
            // arrange
            var username = _fixture.Create<string>();
            var input = _fixture.Build<PostInputDto>().With(x => x.Type, "quote_post").Create();
            var user = _fixture.Create<User>();
            var post =
                _fixture.Build<Post>()
                    .With(x => x.UserName, username)
                    .With(x => x.Type, "quote_post")
                    .Create();

            _userRepository.GetUserByUserNameAsync(username).Returns(user);
            _postRepository.GetPostsByIdFilterAsync((int)input.PostOriginId, username).Returns(post);

            // act
            var response = await _postAppService.AddAsync(username, input).ConfigureAwait(false);

            // assert
            response.HasError.Should().BeTrue();
            response.Errors.Count().Should().Be(1);
            response.Errors.FirstOrDefault().Key.Should().Be("type_not_allowed");
        }

        [Fact]
        public async Task WhenAdd_WithRepostTypeNotAllowed_ShouldReturnError()
        {
            // arrange
            var username = _fixture.Create<string>();
            var input = _fixture.Build<PostInputDto>().With(x => x.Type, "repost").Create();
            var user = _fixture.Create<User>();
            var post =
                _fixture.Build<Post>()
                    .With(x => x.UserName, _fixture.Create<string>())
                    .With(x => x.Type, "repost")
                    .Create();

            _userRepository.GetUserByUserNameAsync(username).Returns(user);
            _postRepository.GetPostsByIdFilterAsync((int)input.PostOriginId, username).Returns(post);

            // act
            var response = await _postAppService.AddAsync(username, input).ConfigureAwait(false);

            // assert
            response.HasError.Should().BeTrue();
            response.Errors.Count().Should().Be(1);
            response.Errors.FirstOrDefault().Key.Should().Be("type_not_allowed");
        }

        [Fact]
        private async Task WhenGetPostsByFilter_WithCorrectParameters_ShouldReturnSuccess()
        {
            // arrange
            string userName = _fixture.Create<string>();
            int fetchNext = _fixture.Create<int>();
            int offset = _fixture.Create<int>();
            bool allPosts = true;

            var posts = _fixture.Build<Post>().Without(x => x.PostOriginId).CreateMany();
            var outputs = _fixture.Build<PostGetOutputDto>().Without(x => x.PostOriginId).Without(x => x.PostOrigin).CreateMany();
            var post = _fixture.Build<Post>().Without(x => x.PostOriginId).Create();

            _postRepository.GetPostsByFilterAsync(userName, fetchNext, offset, allPosts).Returns(posts);
            _mapper.Map<IEnumerable<Post>, IEnumerable<PostGetOutputDto>>(posts).Returns(outputs);
            _postRepository.GetPostsByIdFilterAsync(_fixture.Create<int>(), userName).ReturnsForAnyArgs(post);

            // act
            var response = await _postAppService.GetPostsByFilterAsync(userName, fetchNext, offset, allPosts).ConfigureAwait(false);

            // assert
            response.HasError.Should().BeFalse();
        }
    }
}
