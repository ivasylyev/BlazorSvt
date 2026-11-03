using BlazorSvt.Platform.Domain.IdsEnum;
using FluentAssertions;

namespace BlazorSvt.UnitTests.Platform.Domain;

[Trait("Category", "Unit")]
public class IdsEnumSetsTests
{
    [Fact]
    public void TransportKindSets_RuAndEn_HaveSameIds()
        => Ids(TransportKindSets.MainEn).Should().Equal(Ids(TransportKindSets.MainRu));

    [Fact]
    public void TransportTypeLevel3Sets_RuAndEn_HaveSameIds()
        => Ids(TransportTypeLevel3Sets.MainEn).Should().Equal(Ids(TransportTypeLevel3Sets.MainRu));

    private static IEnumerable<long> Ids<TEnum>(IEnumerable<TEnum> values) where TEnum : Enum
        => values.Select(v => Convert.ToInt64(v));
}
