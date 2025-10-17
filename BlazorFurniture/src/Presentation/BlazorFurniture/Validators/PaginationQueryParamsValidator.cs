using BlazorFurniture.Application.Common.Requests.QueryParams;
using FastEndpoints;
using FluentValidation;

namespace BlazorFurniture.Validators;

public class PaginationQueryParamsValidator<T> : Validator<T> where T : IPaginationQueryParams
{
    public PaginationQueryParamsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Page must be at least 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("PageSize cannot exceed 100.");
    }
}
