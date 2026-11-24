namespace BlazorSvt.Platform.Grid.Services;

/// <summary>
/// Порядок и стартовая свёртка групп карточки.
/// Группы-таблицы дочернего примитива идут перед группами полей.
/// Секция со строками свёрнута, секция с ошибкой загрузки и группы полей раскрыты.
/// </summary>
public static class DetailAccordionPlan
{
    public static IReadOnlyList<DetailAccordionSection> Build(
        IReadOnlyList<DetailChildAccordionInput> visibleChildSections,
        int visibleFieldGroupCount)
    {
        var sections = new List<DetailAccordionSection>(visibleChildSections.Count + visibleFieldGroupCount);
        for (var i = 0; i < visibleChildSections.Count; i++)
        {
            sections.Add(new DetailAccordionSection(
                DetailAccordionSectionKind.ChildTable,
                i,
                visibleChildSections[i].HasLoadError));
        }

        for (var i = 0; i < visibleFieldGroupCount; i++)
        {
            sections.Add(new DetailAccordionSection(
                DetailAccordionSectionKind.FieldGroup,
                i,
                true));
        }

        return sections;
    }

    public static bool AllExpanded(IReadOnlyList<DetailAccordionSection> sections) =>
        sections.All(section => section.StartsExpanded);
}

public enum DetailAccordionSectionKind
{
    ChildTable,
    FieldGroup
}

public readonly record struct DetailChildAccordionInput(bool HasLoadError);

public readonly record struct DetailAccordionSection(
    DetailAccordionSectionKind Kind,
    int SourceIndex,
    bool StartsExpanded);
