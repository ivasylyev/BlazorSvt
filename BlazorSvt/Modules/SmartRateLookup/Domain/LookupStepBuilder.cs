namespace BlazorSvt.Modules.SmartRateLookup.Domain;

/// <summary>Строит шаги поиска: уровни 1–8 для основного узла, затем те же уровни для каждого узла DPO.</summary>
public static class LookupStepBuilder
{
    public static IReadOnlyList<LookupStep> Build(ResolvedLookupInput input)
    {
        var baseSteps = new List<(int Level, LookupLifts Lifts, long EndNode, long? Product, long? Group, long? Type, long? Kind)>();

        foreach (var (level, lifts) in LookupLevels.All)
        {
            long endNode;
            if (lifts.HasFlag(LookupLifts.DestinationToRegion))
            {
                if (input.RegionNodeToId is not { } region)
                {
                    continue;
                }

                endNode = region;
            }
            else
            {
                endNode = input.NodeToId;
            }

            long? product = input.ProductId;
            long? group = null;
            if (lifts.HasFlag(LookupLifts.ProductToGroup))
            {
                if (input.ProductGroupId is null)
                {
                    continue;
                }

                product = null;
                group = input.ProductGroupId;
            }

            var byKind = lifts.HasFlag(LookupLifts.TransportTypeToKind);
            baseSteps.Add((
                level,
                lifts,
                endNode,
                product,
                group,
                byKind ? null : input.TransportTypeId,
                byKind ? input.TransportKindId : null));
        }

        var steps = new List<LookupStep>();
        var order = 0;

        foreach (var s in baseSteps)
        {
            steps.Add(new LookupStep(++order, s.Level, s.Lifts, input.NodeFromId, null, s.EndNode, s.Product, s.Group, s.Type, s.Kind));
        }

        foreach (var s in baseSteps)
        {
            foreach (var dpo in input.DpoNodes)
            {
                steps.Add(new LookupStep(++order, s.Level, s.Lifts, dpo.NodeId, dpo, s.EndNode, s.Product, s.Group, s.Type, s.Kind));
            }
        }

        return steps;
    }
}
