using ApplicationService.Dto.Input;
using ApplicationService.Interface.Validation;
using ApplicationService.Validation;
using AutoFixture;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace UnitTest.ApplicationService.Validation
{
    public class UserValidatorTest
    {
        private readonly IUserValidator _userValidator;
        private readonly Fixture _fixture;

        public UserValidatorTest()
        {
            _userValidator = new UserValidator();
            _fixture = new Fixture();
        }

        [Fact]
        public void WhenValidate_WithCorrectParameters_ShouldReturnSuccess()
        {
            // arrange
            var input = _fixture.Build<UserInputDto>()
                .With(x => x.UserName, "usertest")
                .Create();

            // act
            var response = _userValidator.IsValid(input);

            // assert
            response.Should().HaveCount(0);
        }

        [Fact]
        public void WhenValidate_WithEmptyUsername_ShouldReturnError()
        {
            // arrange
            var input = _fixture.Build<UserInputDto>()
                .With(x => x.UserName, string.Empty)
                .Create();

            // act
            var response = _userValidator.IsValid(input);

            // assert
            response.Should().HaveCount(1);
            response.FirstOrDefault().Value.FirstOrDefault().Should().Be("The field UserName is required");
        }

        [Fact]
        public void WhenValidate_WithNonAlfanumericUsername_ShouldReturnError()
        {
            // arrange
            var input = _fixture.Build<UserInputDto>()
                .With(x => x.UserName, "test user")
                .Create();

            // act
            var response = _userValidator.IsValid(input);

            // assert
            response.Should().HaveCount(1);
            response.FirstOrDefault().Value.FirstOrDefault().Should().Be("The field UserName must contains only alphanumeric characters");
        }

        [Fact]
        public void WhenValidate_WithLongUsername_ShouldReturnError()
        {
            // arrange
            var input = _fixture.Build<UserInputDto>()
                .With(x => x.UserName, "usertest123456789")
                .Create();

            // act
            var response = _userValidator.IsValid(input);

            // assert
            response.Should().HaveCount(1);
            response.FirstOrDefault().Value.FirstOrDefault().Should().Be("The field UserName length should be at maximun 14 characters");
        }
    }
}
