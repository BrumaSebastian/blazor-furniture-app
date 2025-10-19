using BlazorFurniture.Application.Features.GroupManagement.Requests;
using FastEndpoints;
using FluentValidation;

namespace BlazorFurniture.Validators.Groups;

public class UpdateGroupRequestValidator : Validator<UpdateGroupRequest>
{
    public UpdateGroupRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Group name must not be empty.")
            .MaximumLength(100)
            .WithMessage("Group name must not exceed 100 characters.");
    }
}
