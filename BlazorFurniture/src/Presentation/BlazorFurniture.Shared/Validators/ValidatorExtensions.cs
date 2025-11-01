using FluentValidation;

namespace BlazorFurniture.Shared.Validators;

public static class ValidatorExtensions
{
    public static Func<object, string, Task<IEnumerable<string>>> AsPropertyValidator<TModel>(
        this AbstractValidator<TModel> validator )
        where TModel : class
    {
        return async ( model, propertyName ) =>
        {
            if (model is not TModel || string.IsNullOrWhiteSpace(propertyName))
                return [];

            var result = await validator.ValidateAsync(ValidationContext<TModel>.CreateWithOptions((TModel)model, x => x.IncludeProperties(propertyName)));

            return result.IsValid
                ? []
                : result.Errors
                    .Where(e => string.Equals(e.PropertyName, propertyName, StringComparison.Ordinal))
                    .Select(e => e.ErrorMessage);
        };
    }
}
