using System;
using System.Linq.Expressions;
using Dapper;
using WatchStore.Api.Models.Dto;

namespace WatchStore.Api.Extensions;

public static class SqlBuilderExtensions
{
    public static bool SetIfSpecified<T>(
        this SqlBuilder builder,
        string setClause,
        Expression<Func<PatchField<T>>> fieldExpression
    )
    {
        var memberExpression =
            fieldExpression.Body as MemberExpression
            ?? throw new InvalidOperationException("Expression must be member access.");

        var parameterName = memberExpression.Member.Name;

        var getter = fieldExpression.Compile();

        var field = getter();

        if (field.IsSpecified)
        {
            var parameters = new DynamicParameters();

            parameters.Add(parameterName, field.Value);

            builder.Set(setClause, parameters);

            return true;
        }

        return false;
    }
}
