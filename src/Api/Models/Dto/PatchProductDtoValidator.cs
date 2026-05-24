using FluentValidation;

namespace WatchStore.Api.Models.Dto;

public sealed class PatchProductDtoValidator : AbstractValidator<PatchProductDto>
{
    public PatchProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .Must(nameField =>
            {
                if (nameField.IsSpecified)
                {
                    return !string.IsNullOrWhiteSpace(nameField.Value);
                }

                return true;
            })
            .WithMessage("Name cannot be null or whitespace")
            .Must(nameField =>
            {
                if (nameField is { IsSpecified: true, Value: not null })
                {
                    return nameField.Value.Length <= 255;
                }

                return true;
            })
            .WithMessage("Name cannot exceed 255 characters");

        RuleFor(x => x.Description)
            .Must(descField =>
            {
                if (descField is { IsSpecified: true, Value: not null })
                {
                    return descField.Value.Length <= 4000;
                }

                return true;
            })
            .WithMessage("Description cannot exceed 4000 characters");

        RuleFor(x => x.Price)
            .Must(priceField =>
            {
                if (priceField.IsSpecified)
                {
                    return priceField.Value.HasValue;
                }

                return true;
            })
            .WithMessage("Price cannot be null")
            .Must(priceField =>
            {
                if (priceField is { IsSpecified: true, Value: not null })
                {
                    return priceField.Value >= 0;
                }

                return true;
            })
            .WithMessage("Price cannot be negative");
    }
}
