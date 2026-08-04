using BlazorSvt.Platform.Grid.Models;

namespace BlazorSvt.IntegrationTests.Infrastructure;

public static class FtsFilterTestSupport
{
    public static GridFilter IsArchiveFalse { get; } =
        new("IsArchive", "False", GridFilterOperators.EqualsOperator);

    public static GridFilter IdEquals(string propertyName, int idValue) =>
        new(propertyName, idValue.ToString(), GridFilterOperators.EqualsOperator);

    public static GridFilter Contains(string propertyName, string value) =>
        new(propertyName, value, GridFilterOperators.ContainsOperator);

    public static GridQuery CreateQuery(params GridFilter[] filters) =>
        new(
            1,
            10,
            new GridSort(null, "ASC"),
            [IsArchiveFalse, ..filters]);
}
