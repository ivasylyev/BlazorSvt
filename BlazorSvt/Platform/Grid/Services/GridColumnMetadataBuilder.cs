using System.Collections.Concurrent;
using System.Reflection;
using BlazorSvt.Platform.Grid.Models;
using BlazorSvt.Platform.Infrastructure.Config;
using Newtonsoft.Json;

namespace BlazorSvt.Platform.Grid.Services;

public static class GridColumnMetadataBuilder
{
    private static readonly ConcurrentDictionary<Type, GridSnapshotMetadata> Cache = new();

    public static GridSnapshotMetadata GetMetadata<T>() => GetMetadata(typeof(T));

    public static GridSnapshotMetadata GetMetadata(Type dtoType)
    {
        return Cache.GetOrAdd(dtoType, BuildMetadata);
    }

    public static string BuildAllowedColumnsJson(Type dtoType)
    {
        var metadata = GetMetadata(dtoType);
        var entries = metadata.Columns
            .Where(c => c.Filterable)
            .Select(c => new
            {
                ColumnName = c.PropertyName,
                SqlColumnName = c.SqlColumnName,
                ColumnType = ToSqlColumnType(c.ColumnType)
            });

        return JsonConvert.SerializeObject(entries);
    }

    public static string BuildSelectList(Type dtoType, bool keysOnly = false)
    {
        var metadata = GetMetadata(dtoType);

        if (keysOnly)
        {
            var entityKey = metadata.Columns.Single(c => c.IsEntityKey);
            return $"\n        SELECT\n            {entityKey.SqlColumnName}\n  ";
        }

        var selectParts = metadata.Columns
            .Where(c => c.IncludeInSelect)
            .Select(c => c.SelectExpression);

        return "\n        SELECT\n            " + string.Join(",\n            ", selectParts) + "\n  ";
    }

    private static GridSnapshotMetadata BuildMetadata(Type dtoType)
    {
        var snapshotAttr = dtoType.GetCustomAttribute<GridSnapshotAttribute>()
            ?? throw new InvalidOperationException(
                $"DTO {dtoType.Name} does not have GridSnapshotAttribute");

        var columns = dtoType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(TryBuildColumn)
            .Where(column => column is not null)
            .Cast<GridColumnMetadata>()
            .ToList();

        if (columns.Count == 0)
        {
            throw new InvalidOperationException(
                $"DTO {dtoType.Name} has no properties with GridColumnAttribute");
        }

        var entityKeyCount = columns.Count(c => c.IsEntityKey);
        if (entityKeyCount != 1)
        {
            throw new InvalidOperationException(
                $"DTO {dtoType.Name} must have exactly one GridColumn with IsEntityKey = true, found {entityKeyCount}");
        }

        return new GridSnapshotMetadata
        {
            TableName = snapshotAttr.TableName,
            Columns = columns,
            EntityKeyPropertyName = columns.Single(c => c.IsEntityKey).PropertyName
        };
    }

    private static GridColumnMetadata? TryBuildColumn(PropertyInfo property)
    {
        var attr = property.GetCustomAttribute<GridColumnAttribute>();
        if (attr is null)
        {
            return null;
        }

        var propertyName = property.Name;
        var sqlColumn = attr.SqlColumn ?? propertyName;
        var columnType = ResolveColumnType(property, attr);

        return new GridColumnMetadata
        {
            PropertyName = propertyName,
            SqlColumnName = sqlColumn,
            ColumnType = columnType,
            SelectExpression = BuildSelectExpression(property, attr, propertyName, sqlColumn),
            Filterable = attr.Filterable,
            IncludeInSelect = attr.IncludeInSelect,
            IsEntityKey = attr.IsEntityKey
        };
    }

    private static GridColumnType ResolveColumnType(PropertyInfo property, GridColumnAttribute attr)
    {
        if (attr.ColumnType.HasValue)
        {
            return attr.ColumnType.Value;
        }

        var inferred = TryInferColumnType(property.PropertyType);
        if (inferred.HasValue)
        {
            return inferred.Value;
        }

        if (!attr.Filterable)
        {
            return GridColumnType.Id;
        }

        throw new InvalidOperationException(
            $"Property {property.DeclaringType!.Name}.{property.Name} requires explicit GridColumnType");
    }

    private static GridColumnType? TryInferColumnType(Type propertyType)
    {
        var type = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        if (type == typeof(string))
        {
            return GridColumnType.Nvarchar;
        }

        if (type == typeof(bool))
        {
            return GridColumnType.Bit;
        }

        if (type.IsEnum
            || type == typeof(long)
            || type == typeof(int)
            || type == typeof(short)
            || type == typeof(byte))
        {
            return GridColumnType.Id;
        }

        if (type == typeof(DateTime) || type == typeof(DateOnly))
        {
            return GridColumnType.Date;
        }

        return null;
    }

    private static string BuildSelectExpression(
        PropertyInfo property,
        GridColumnAttribute attr,
        string propertyName,
        string sqlColumn)
    {
        if (!string.IsNullOrWhiteSpace(attr.SelectExpression))
        {
            return attr.SelectExpression;
        }

        var transform = attr.SelectTransform;
        if (transform == GridSelectTransform.Auto)
        {
            transform = IsDateOnly(property.PropertyType)
                ? GridSelectTransform.CastAsDate
                : GridSelectTransform.None;
        }

        return transform switch
        {
            GridSelectTransform.CastAsDate => $"CAST({sqlColumn} AS DATE) AS {propertyName}",
            GridSelectTransform.None when sqlColumn == propertyName => sqlColumn,
            GridSelectTransform.None => $"{sqlColumn} AS {propertyName}",
            _ => sqlColumn
        };
    }

    private static bool IsDateOnly(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type) ?? type;
        return underlying == typeof(DateOnly);
    }

    private static string ToSqlColumnType(GridColumnType columnType) =>
        columnType switch
        {
            GridColumnType.Id => "ID",
            GridColumnType.Nvarchar => "NVARCHAR",
            GridColumnType.Date => "DATE",
            GridColumnType.Bit => "BIT",
            _ => throw new ArgumentOutOfRangeException(nameof(columnType), columnType, null)
        };
}
