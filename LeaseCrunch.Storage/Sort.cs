namespace LeaseCrunch.Storage;

public record Sort
{
    public SortDirection Direction { get; init; } = SortDirection.Descending;
    public SortableProperties Property { get; init; } = SortableProperties.StartDate;
}