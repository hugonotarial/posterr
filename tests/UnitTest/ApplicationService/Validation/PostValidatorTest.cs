using ApplicationService.Dto.Input;
using ApplicationService.Interface.Validation;
using ApplicationService.Validation;
using AutoFixture;
using Bogus;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace UnitTest.ApplicationService.Validation
{
    public class PostValidatorTest
    {
        private readonly IPostValidator _postValidator;
        private readonly Fixture _fixture;
        private readonly Faker _faker;

        public PostValidatorTest()
        {
            _postValidator = new PostValidator();
            _fixture = new Fixture();
            _faker = new Faker();
        }

        [Fact]
        public void WhenValidate_WithRegularPostParameters_ShouldReturnSuccess()
        {
            // arrange
            var input = _fixture.Build<PostInputDto>()
                .With(x => x.Type, "regular_post")
                .Without(x => x.PostOriginId)
                .Create();

            // act
            var response = _postValidator.IsValid(input);

            // assert
            response.Should().HaveCount(0);
        }

        [Fact]
        public void WhenValidate_WithRepostParameters_ShouldReturnSuccess()
        {
            // arrange
            var input = _fixture.Build<PostInputDto>()
                .With(x => x.Type, "repost")
                .Create();

            // act
            var response = _postValidator.IsValid(input);

            // assert
            response.Should().HaveCount(0);
        }

        [Fact]
        public void WhenValidate_WithQuotePostParameters_ShouldReturnSuccess()
        {
            // arrange
            var input = _fixture.Build<PostInputDto>()
                .With(x => x.Type, "quote_post")
                .Create();

            // act
            var response = _postValidator.IsValid(input);

            // assert
            response.Should().HaveCount(0);
        }


        [Fact]
        public void WhenValidate_WithEmptyMessageParameter_ShouldReturnError()
        {
            // arrange
            var input = _fixture.Build<PostInputDto>()
                .With(x => x.Type, "regular_post")
                .Without(x => x.Message)
                .Without(x => x.PostOriginId)
                .Create();

            // act
            var response = _postValidator.IsValid(input);

            // assert
            response.Should().HaveCount(1);
            response.FirstOrDefault().Value.FirstOrDefault().Should().Be("The field Message is required");
        }

        [Fact]
        public void WhenValidate_WithEmptyTypeParameter_ShouldReturnError()
        {
            // arrange
            var input = _fixture.Create<PostInputDto>();
            input.Type = null;

            // act
            var response = _postValidator.IsValid(input);

            // assert
            response.Should().HaveCount(1);
            response.FirstOrDefault().Value.FirstOrDefault().Should().Be("The field Type is required");
        }


        [Fact]
        public void WhenValidate_WithLongMessageParameter_ShouldReturnError()
        {
            // arrange
            var input = _fixture.Build<PostInputDto>()
                .With(x => x.Message, _faker.Random.AlphaNumeric(778))
                .With(x => x.Type, "regular_post")
                .Without(x => x.PostOriginId)
                .Create();

            // act
            var response = _postValidator.IsValid(input);

            // assert
            response.Should().HaveCount(1);
            response.FirstOrDefault().Value.FirstOrDefault().Should().Be("The field Message length should be at maximun 777 characters");
        }

        [Fact]
        public void WhenValidate_WithInvalidTypeParameter_ShouldReturnError()
        {
            // arrange
            var input = _fixture.Build<PostInputDto>()
                .With(x => x.Type, _fixture.Create<string>())
                .Create();

            // act
            var response = _postValidator.IsValid(input);

            // assert
            response.Should().HaveCount(1);
            response.FirstOrDefault().Value.FirstOrDefault().Should().Be("The field Type value is not allowed");
        }

        [Fact]
        public void WhenValidate_QuoteTypeWithEmptyOrigin_ShouldReturnError()
        {
            // arrange
            var input = _fixture.Build<PostInputDto>()
                .With(x => x.Type, "quote_post")
                .Without(x => x.PostOriginId)
                .Create();

            // act
            var response = _postValidator.IsValid(input);

            // assert
            response.Should().HaveCount(1);
            response.FirstOrDefault().Value.FirstOrDefault().Should().Be("The field PostOriginId is required");
        }

        [Fact]
        public void WhenValidate_RepostTypeWithEmptyOrigin_ShouldReturnError()
        {
            // arrange
            var input = _fixture.Build<PostInputDto>()
                .With(x => x.Type, "repost")
                .Without(x => x.PostOriginId)
                .Create();

            // act
            var response = _postValidator.IsValid(input);

            // assert
            response.Should().HaveCount(1);
            response.FirstOrDefault().Value.FirstOrDefault().Should().Be("The field PostOriginId is required");
        }
    }
}
