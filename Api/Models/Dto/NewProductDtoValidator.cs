using FluentValidation;

namespace WatchStore.Api.Models.Dto;

public sealed class NewProductDtoValidator : AbstractValidator<NewProductDto>
{
    public NewProductDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);

        RuleFor(x => x.Description).MaximumLength(4000);

        RuleFor(x => x.Price).NotNull().GreaterThanOrEqualTo(0);
    }
}
