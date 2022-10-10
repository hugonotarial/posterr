using ApplicationService.Dto.Input;
using System.Collections.Generic;

namespace ApplicationService.Interface.Validation
{
    public interface IPostValidator
    {
        IDictionary<string, IList<string>> IsValid(PostInputDto input);
    }
}
