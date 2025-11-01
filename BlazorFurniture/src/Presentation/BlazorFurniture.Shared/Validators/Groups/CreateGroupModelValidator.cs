using BlazorFurniture.Shared.Models.Groups;
using BlazorFurniture.Shared.Resources.Validations;
using FluentValidation;

namespace BlazorFurniture.Shared.Validators.Groups;

public sealed class CreateGroupModelValidator : AbstractValidator<CreateGroupModel>
{
    public CreateGroupModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ValidationsResource.group_name_notEmpty)
            .MaximumLength(100)
            .WithMessage(ValidationsResource.group_name_maxLength);
    }
}
