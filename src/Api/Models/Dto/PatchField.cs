namespace WatchStore.Api.Models.Dto;

public readonly record struct PatchField<T>
{
    public bool IsSpecified { get; init; }

    public T Value { get; init; }

    public static PatchField<T> Unspecified => new() { IsSpecified = false, Value = default! };

    public static PatchField<T> Specified(T value) => new() { IsSpecified = true, Value = value };
}
