using BlazorBootstrap;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Platform.Grid.Services;

/// <summary>Подписи операторов фильтра — общие для грида справочника и таблиц дочерних примитивов.</summary>
public static class GridFilterOperatorTranslations
{
    public static IEnumerable<FilterOperatorInfo> Create(IStringLocalizer<PlatformResources> localizer) =>
    [
        // Текстовые фильтры
        new("*a*", localizer["FilterOperator.Contains"], FilterOperator.Contains),
        new("!*a*", localizer["FilterOperator.DoesNotContain"], FilterOperator.DoesNotContain),
        new("a**", localizer["FilterOperator.StartsWith"], FilterOperator.StartsWith),
        new("**a", localizer["FilterOperator.EndsWith"], FilterOperator.EndsWith),
        new("=", localizer["FilterOperator.Equals"], FilterOperator.Equals),
        new("!=", localizer["FilterOperator.NotEquals"], FilterOperator.NotEquals),
        // Числовые фильтры и даты
        new("<", localizer["FilterOperator.LessThan"], FilterOperator.LessThan),
        new("<=", localizer["FilterOperator.LessThanOrEquals"], FilterOperator.LessThanOrEquals),
        new(">", localizer["FilterOperator.GreaterThan"], FilterOperator.GreaterThan),
        new(">=", localizer["FilterOperator.GreaterThanOrEquals"], FilterOperator.GreaterThanOrEquals),
        // Общие
        new("x", localizer["FilterOperator.Clear"], FilterOperator.Clear)
    ];
}
