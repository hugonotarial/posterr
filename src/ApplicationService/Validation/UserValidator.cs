using ApplicationService.Dto.Input;
using ApplicationService.Interface.Validation;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationService.Validation
{
    public class UserValidator : AbstractValidator<UserInputDto>, IUserValidator
    {
        private const string _requiredMessage = "The field {0} is required";
        private const string _maxLengthMessage = "The field {0} length should be at maximun {1} characters";
        private const string _alfaNumericMessage = "The field {0} must contains only alphanumeric characters";

        public UserValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage(string.Format(_requiredMessage, nameof(UserInputDto.UserName)));

            When(x => x.UserName is not null, () =>
            {
                RuleFor(x => x.UserName).MaximumLength(14).WithMessage(string.Format(_maxLengthMessage, nameof(UserInputDto.UserName), "14"));

                ValidateUserName();
            });
        }

        private void ValidateUserName()
        {
            RuleFor(model => model).Must((model, cancellation) =>
            {
                return model.UserName.All(char.IsLetterOrDigit);
            }).WithMessage(string.Format(_alfaNumericMessage, nameof(UserInputDto.UserName))).OverridePropertyName(nameof(UserInputDto.UserName)); ;
        }

        public IDictionary<string, IList<string>> IsValid(UserInputDto input)
        {
            var results = this.Validate(input);
            var errors = new Dictionary<string, IList<string>>();

            if (results.IsValid == false)
            {
                foreach (var failure in results.Errors)
                {
                    var response = errors.TryGetValue(failure.PropertyName, out IList<string> listError);
                    if (response)
                    {
                        listError.Add(failure.ErrorMessage);
                    }
                    else
                    {
                        errors.Add(failure.PropertyName, new List<string> { failure.ErrorMessage });
                    }
                }
            }

            return errors;
        }
    }
}
