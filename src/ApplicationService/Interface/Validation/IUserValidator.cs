using ApplicationService.Dto.Input;
using System.Collections.Generic;

namespace ApplicationService.Interface.Validation
{
    public interface IUserValidator
    {
        IDictionary<string, IList<string>> IsValid(UserInputDto input);
    }
}
