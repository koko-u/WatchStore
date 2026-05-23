using System.Linq;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace WatchStore.Api.Extensions;

public static class ValidationErrorExtension
{
    public static ValidationProblemDetails IntoProblemDetails(this ValidationResult result)
    {
        var errors = result
            .Errors.GroupBy(x => x.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        return new ValidationProblemDetails(errors);
    }
}
