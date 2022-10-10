using ApplicationService.Dto.Input;
using ApplicationService.Interface.Validation;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationService.Validation
{
    public class PostValidator : AbstractValidator<PostInputDto>, IPostValidator
    {
        private const string _requiredMessage = "The field {0} is required";
        private const string _maxLengthMessage = "The field {0} length should be at maximun {1} characters";
        private const string _notAllowedMessage = "The field {0} value is not allowed";

        private readonly string[] _allowedTypes = { "regular_post", "repost", "quote_post" };

        public PostValidator()
        {
            RuleFor(x => x.Message).NotEmpty().WithMessage(string.Format(_requiredMessage, nameof(PostInputDto.Message)));
            When(x => x.Message is not null, () =>
            {
                RuleFor(x => x.Message).MaximumLength(777).WithMessage(string.Format(_maxLengthMessage, nameof(PostInputDto.Message), "777"));
            });

            RuleFor(x => x.Type).NotEmpty().WithMessage(string.Format(_requiredMessage, nameof(PostInputDto.Type)));
            When(x => x.Type is not null, () =>
            {
                ValidateType();
                ValidatePostOrigin();
            });
        }

        private void ValidateType()
        {
            RuleFor(model => model).Must((model, cancellation) =>
            {
                return _allowedTypes.Contains(model.Type);
            }).WithMessage(string.Format(_notAllowedMessage, nameof(PostInputDto.Type))).OverridePropertyName(nameof(PostInputDto.Type));
        }

        private void ValidatePostOrigin()
        {
            RuleFor(model => model).Must((model, cancellation) =>
            {
                if (model.Type != "quote_post" && model.Type != "repost")
                {
                    return true;
                }

                if ((model.Type == "quote_post" || model.Type == "repost") && model.PostOriginId is not null)
                {
                    return true;
                }
                return false;

            }).WithMessage(string.Format(_requiredMessage, nameof(PostInputDto.PostOriginId))).OverridePropertyName(nameof(PostInputDto.PostOriginId));

            RuleFor(model => model).Must((model, cancellation) =>
            {
                if (model.Type == "regular_post" && model.PostOriginId is not null)
                {
                    return false;
                }
                return true;

            }).WithMessage($"The field {nameof(PostInputDto.PostOriginId)} must be empty for regular posts.").OverridePropertyName(nameof(PostInputDto.PostOriginId));
        }

        public IDictionary<string, IList<string>> IsValid(PostInputDto input)
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
