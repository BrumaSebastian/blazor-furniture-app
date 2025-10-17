using BlazorFurniture.Application.Features.GroupManagement.Requests;
using FastEndpoints;

namespace BlazorFurniture.Validators.Groups;

public class GetGroupsRequestValidator : Validator<GetGroupsRequest>
{
    public GetGroupsRequestValidator()
    {
        RuleFor(x => x.Filters)
            .SetValidator(new GroupQueryFiltersValidator());
    }
}
