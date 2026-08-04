namespace BlazorSvt.Platform.Grid.Models;

public sealed class GridSnapshotQuery
{
    public required string TableName { get; init; }
    public required string AllowedColumnsJson { get; init; }
    public required string SelectList { get; init; }
    public required string KeysOnlySelectList { get; init; }

    public static GridSnapshotQuery For<T>() => For(typeof(T));

    public static GridSnapshotQuery For(Type dtoType)
    {
        var metadata = GridColumnMetadataBuilder.GetMetadata(dtoType);
        return new GridSnapshotQuery
        {
            TableName = metadata.TableName,
            AllowedColumnsJson = GridColumnMetadataBuilder.BuildAllowedColumnsJson(dtoType),
            SelectList = GridColumnMetadataBuilder.BuildSelectList(dtoType, keysOnly: false),
            KeysOnlySelectList = GridColumnMetadataBuilder.BuildSelectList(dtoType, keysOnly: true)
        };
    }
}
