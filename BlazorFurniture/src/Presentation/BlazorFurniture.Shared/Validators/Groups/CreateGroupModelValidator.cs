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
            .WithMessage(string.Format(ValidationsResource.group_name_maxLength, 100));

        RuleFor(x => x.Description)
            .MaximumLength(250)
            .WithMessage(string.Format(ValidationsResource.group_description_maxLength, 250));
    }
}
