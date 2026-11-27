#Requires -Version 5.1
<#
.SYNOPSIS
    Discovery fact-sheet справочника МДМ для миграции в BlazorSVT.

.DESCRIPTION
    Выполняет все запросы discovery-queries.md одним проходом и пишет
    единый markdown-факт-лист: идентификация, атрибуты, заполненность,
    типы колонок view, Code, переводы, заголовки, дети, sync-сигналы.
    Агент интерпретирует факт-лист по discovery-rules.md; ручные запросы
    из discovery-queries.md остаются для полей, которых в листе нет.

.PARAMETER RegistryName
    Имя реестра (EntityInfo.Name).

.PARAMETER OutFile
    Путь markdown-файла результата.

.PARAMETER RepoRoot
    Корень репозитория (для appsettings.json и проверки Modules/{Entity}).
    По умолчанию выводится из расположения скрипта.
#>
param(
    [Parameter(Mandatory = $true)]
    [string]$RegistryName,

    [Parameter(Mandatory = $true)]
    [string]$OutFile,

    [string]$RepoRoot = $null
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if (-not $RepoRoot) {
    $RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..\..')).Path
}

# ---------- подключение ----------
$appsettings = Get-Content -LiteralPath (Join-Path $RepoRoot 'BlazorSvt\appsettings.json') -Raw
$line = ($appsettings -split "`r?`n" | Where-Object { $_ -match '"MdmDb"' -and $_ -notmatch '^\s*//' } | Select-Object -First 1)
if (-not $line) { throw "Database:MdmDb not found in appsettings.json" }
$connStr = [regex]::Match($line, '"MdmDb"\s*:\s*"([^"]+)"').Groups[1].Value
if ($connStr -match 'User ID=([^;]+)') {
    $authArgs = @('-U', $Matches[1], '-P', [regex]::Match($connStr, 'Password=([^;]+)').Groups[1].Value)
}
else {
    $authArgs = @('-E')
}

# ---------- helpers ----------
$script:queryCount = 0
$tmpFiles = New-Object System.Collections.Generic.List[string]

function Invoke-Sql([string]$sql) {
    $script:queryCount++
    $sqlFile = [IO.Path]::GetTempFileName() + '.sql'
    $outFile = [IO.Path]::GetTempFileName()
    $tmpFiles.Add($sqlFile); $tmpFiles.Add($outFile)
    $sql = "SET NOCOUNT ON;`r`n" + $sql
    [IO.File]::WriteAllText($sqlFile, $sql, (New-Object Text.UTF8Encoding $true))
    & sqlcmd -S localhost -d mdm @authArgs -b -f o:65001 -h -1 -y 8000 -s "`t" -i $sqlFile -o $outFile 2>&1 | Out-Null
    if ($LASTEXITCODE -ne 0) {
        $err = Get-Content -LiteralPath $outFile -Raw -ErrorAction SilentlyContinue
        throw "sqlcmd failed (exit $LASTEXITCODE): $err"
    }
    $content = [IO.File]::ReadAllText($outFile, [Text.Encoding]::UTF8)
    return @($content -split "`r?`n" | ForEach-Object { $_.TrimStart([char]0xFEFF).Trim() } | Where-Object { $_ -ne '' })
}

function ConvertFrom-Row([string]$line) { return @($line.Split("`t") | ForEach-Object { $_.Trim() }) }

function Add-Section([string]$title, [string[]]$headers, [string[]]$rows) {
    [void]$sb.AppendLine("`n## $title")
    if (-not $rows -or $rows.Count -eq 0) {
        [void]$sb.AppendLine('(нет данных)')
        return
    }
    [void]$sb.AppendLine('| ' + ($headers -join ' | ') + ' |')
    [void]$sb.AppendLine('|' + ('---|' * $headers.Count))
    foreach ($r in $rows) {
        $cells = (ConvertFrom-Row $r) | ForEach-Object { $_ -replace '\|', '/' }
        [void]$sb.AppendLine('| ' + ($cells -join ' | ') + ' |')
    }
}

# ---------- 0. идентификация ----------
$sb = New-Object Text.StringBuilder
[void]$sb.AppendLine("# Факт-лист discovery: $RegistryName")
[void]$sb.AppendLine("Сгенерировано: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss'); БД: localhost/mdm")
[void]$sb.AppendLine('Интерпретация — discovery-rules.md; пробелы — discovery-queries.md.')

$ident = @(Invoke-Sql @"
SELECT e.Id, e.Name, es.Id, es.ParentId, p.Id, p.Name,
       es.RelationshipType, es.RelationshipAttributeInfoId, es.IsManyToOne
FROM dbo.EntityInfo e
JOIN dbo.EntityStructure es ON es.EntityInfoId = e.Id
JOIN dbo.PrimitiveEntityInfo p ON p.Id = es.PrimitiveEntityInfoId
WHERE e.Name = N'$RegistryName'
ORDER BY es.Id;
"@)

if ($ident.Count -eq 0) {
    [void]$sb.AppendLine("`n## ОШИБКА`nНет строки в EntityInfo для '$RegistryName'.")
    [IO.File]::WriteAllText($OutFile, $sb.ToString(), (New-Object Text.UTF8Encoding $false))
    throw "Registry name not found: $RegistryName"
}

Add-Section 'Идентификация (шаг 0 SKILL.md)' `
    @('EntityInfoId', 'EntityName', 'StructureId', 'ParentId', 'PEI', 'PrimitiveName', 'RelationshipType', 'RelationshipAttributeInfoId', 'IsManyToOne') `
    $ident

$main = $null
$children = New-Object System.Collections.Generic.List[object]
foreach ($row in $ident) {
    $c = ConvertFrom-Row $row
    if ($c[3] -eq 'NULL') { $main = @{ PEI = $c[4]; Name = $c[5] } }
    else {
        $children.Add(@{ StructureId = $c[2]; PEI = $c[4]; Name = $c[5]; RelType = $c[6]; RelAttrId = $c[7] })
    }
}
if (-not $main) { throw "No main primitive (ParentId IS NULL) for $RegistryName" }
$LegacyName = $main.Name
$MainPEI = $main.PEI

# Имя модуля по правилу «_» шага 0
$Entity = ($RegistryName -split '_' | ForEach-Object { $_.Substring(0, 1).ToUpper() + $_.Substring(1) }) -join ''
$renameMap = @{ Rates = 'TransportRate'; TransportLegs = 'TransportLeg'; LegalType = 'Contractor' }
if ($renameMap.ContainsKey($LegacyName)) { $Entity = $renameMap[$LegacyName] }

$viewName = "vw_$LegacyName"
$viewExists = (Invoke-Sql "SELECT CASE WHEN OBJECT_ID('dbo.$viewName') IS NULL THEN 0 ELSE 1 END")[0]
$pedTable = "dbo.PrimitiveEntityData_$MainPEI"

[void]$sb.AppendLine("`n**Выводы:** ``{LegacyName}`` = ``$LegacyName``; ``{Entity}`` = ``$Entity``; ``{LegacyName}Id`` = ``$($LegacyName)Id``; PED = ``$pedTable``; view ``dbo.$viewName``: $(if ($viewExists -eq '1') { 'есть' } else { '**НЕТ — найти проецирующее view вручную**' })")

$moduleDir = Join-Path $RepoRoot "BlazorSvt\Modules\$Entity"
$snapExists = (Invoke-Sql "SELECT CASE WHEN OBJECT_ID('v2.${Entity}_Snapshot') IS NULL THEN 0 ELSE 1 END")[0]
[void]$sb.AppendLine("``Modules/$Entity``: $(if (Test-Path $moduleDir) { '**СУЩЕСТВУЕТ — уже мигрирован/доработка**' } else { 'нет' }); ``v2.${Entity}_Snapshot``: $(if ($snapExists -eq '1') { 'есть' } else { 'нет' })")

# ---------- 1. объём и свежесть (класс, каскад) ----------
if ($viewExists -eq '1') {
    $counts = @(Invoke-Sql @"
SELECT COUNT(*),
   SUM(CASE WHEN PrimitiveEntityDataStateId = 1 THEN 1 ELSE 0 END),
   CONVERT(varchar(19), MAX(LastChangeDate), 120),
   SUM(CASE WHEN LastChangeDate >= DATEADD(year,-1,GETDATE()) THEN 1 ELSE 0 END),
   SUM(CASE WHEN LastChangeDate >= DATEADD(year,-3,GETDATE()) THEN 1 ELSE 0 END)
FROM dbo.$viewName;
"@)
    Add-Section 'Объём и свежесть (класс / каскад)' @('Total', 'Active', 'MAX(LastChangeDate)', 'Changed1y', 'Changed3y') $counts
}

$rowVer = (Invoke-Sql "SELECT CASE WHEN COL_LENGTH('$pedTable', 'RowVer') IS NULL THEN 0 ELSE 1 END")[0]
$syncRows = Invoke-Sql "SELECT Entity, SourceName, LastRowVersion, LastRunUtc FROM v2.SyncState WHERE Entity LIKE '%$Entity%' OR SourceName LIKE '%PrimitiveEntityData_$MainPEI%';"
Add-Section "Sync-сигналы (RowVer на $pedTable = $rowVer)" @('Entity', 'SourceName', 'LastRowVersion', 'LastRunUtc') $syncRows

# ---------- 2. атрибуты (объединённый §1 + §4) ----------
$attrQuery = @"
SELECT ai.[Name], ai.[Description], s.IsMain, s.MainRank,
       ISNULL(ag.[Rank], 0), s.[Rank],
       ISNULL(xx.ValueEn, 'Default'), ISNULL(xx.ValueRu, N'По умолчанию')
FROM AttributeInfo ai
JOIN AttributeSettings s ON ai.Id = s.AttributeInfoId
LEFT JOIN AttributesGroup ag ON ag.Id = s.GroupId
OUTER APPLY (
    SELECT LOWER([Key]) AS [Key],
           MAX(CASE WHEN LocaleId = 1 THEN Value END) AS ValueEn,
           MAX(CASE WHEN LocaleId = 2 THEN Value END) AS ValueRu
    FROM Dictionary d
    WHERE LOWER(d.[Key]) = LOWER(ag.[Name]) AND ContextId = 258
    GROUP BY LOWER([Key])
) xx
WHERE ai.PrimitiveEntityInfoId = {0}
ORDER BY ISNULL(ag.[Rank], 0), s.[Rank];
"@

function Get-Attributes([int]$pei) { return Invoke-Sql ($attrQuery -f $pei) }

$attrs = Get-Attributes $MainPEI
Add-Section "Атрибуты примитива $LegacyName (PEI $MainPEI): IsMain=1 — короткий список (порядок MainRank); остальное — карточка (GroupRank, AttributeRank)" `
    @('Атрибут', 'Description RU', 'IsMain', 'MainRank', 'GroupRank', 'AttributeRank', 'GroupNameEn', 'GroupNameRu') $attrs

# ---------- 3. колонки view ----------
function Get-ViewColumns([string]$view) {
    return Invoke-Sql @"
SELECT c.name, t.name, c.max_length, c.precision, c.scale, c.is_nullable
FROM sys.columns c
JOIN sys.views v ON v.object_id = c.object_id
JOIN sys.types t ON t.user_type_id = c.user_type_id
WHERE v.object_id = OBJECT_ID('dbo.$view')
ORDER BY c.column_id;
"@
}

$viewCols = @()
if ($viewExists -eq '1') { $viewCols = Get-ViewColumns $viewName }
Add-Section "Колонки dbo.$viewName (типы по sys.columns)" `
    @('Колонка', 'Тип', 'max_length', 'precision', 'scale', 'nullable') $viewCols

$colSet = @{}
foreach ($r in $viewCols) { $colSet[(ConvertFrom-Row $r)[0].ToLower()] = $true }

# ---------- 4. заполненность §3 (один агрегат, транспонированный) ----------
function Get-AttrCounts([string]$view, [hashtable]$colSet, [string[]]$names) {
    $present = @($names | Where-Object { $colSet[$_.ToLower()] })
    $missing = @($names | Where-Object { -not $colSet[$_.ToLower()] })
    $result = New-Object System.Collections.Generic.List[string]
    if ($present.Count -gt 0) {
        $sums = ($present | ForEach-Object { "SUM(CASE WHEN [$_] IS NOT NULL AND CONVERT(NVARCHAR(4000), [$_]) <> N'' THEN 1 ELSE 0 END) AS [$_]" }) -join ', '
        $vals = ($present | ForEach-Object { "(N'$_', s.[$_])" }) -join ', '
        $rows = Invoke-Sql "SELECT v.attr, v.cnt FROM (SELECT $sums FROM dbo.$view WHERE PrimitiveEntityDataStateId = 1) s CROSS APPLY (VALUES $vals) v(attr, cnt);"
        $cntMap = @{}
        foreach ($r in $rows) { $c = ConvertFrom-Row $r; $cntMap[$c[0]] = $c[1] }
        foreach ($n in $names) {
            if ($cntMap.ContainsKey($n)) { $result.Add("$n`t$($cntMap[$n])") }
            elseif ($missing -contains $n) { $result.Add("$n`t<нет колонки во view>") }
        }
    }
    else {
        foreach ($n in $missing) { $result.Add("$n`t<нет колонки во view>") }
    }
    return $result.ToArray()
}

$attrNames = @($attrs | ForEach-Object { (ConvertFrom-Row $_)[0] })
if ($viewExists -eq '1' -and $attrNames.Count -gt 0) {
    $table = Get-AttrCounts $viewName $colSet $attrNames
    Add-Section 'Заполненность атрибутов §3 (неархивные, IS NOT NULL AND <> '''')' @('Атрибут', 'Заполнено') $table
}

# ---------- 5. тип Code (§2a) ----------
$codeRes = Invoke-Sql @"
SELECT a.ValueGeneratorId, g.Name, COUNT(*),
       SUM(CASE WHEN TRY_CONVERT(INT, d.Code) IS NULL OR LTRIM(d.Code) = N'' THEN 1 ELSE 0 END)
FROM $pedTable d
JOIN dbo.AttributeInfo a
    ON a.PrimitiveEntityInfoId = $MainPEI AND a.Name = N'Code'
LEFT JOIN dbo.ValueGenerator g ON g.Id = a.ValueGeneratorId
GROUP BY a.ValueGeneratorId, g.Name;
"@
Add-Section 'Тип Code (§2a): NotInt=0 → вопрос с рекомендацией INT' `
    @('ValueGeneratorId', 'ValueGenerator', 'Total', 'NotInt') $codeRes

# ---------- 6. переводы (§5 пакетно) ----------
$transRes = Invoke-Sql @"
DECLARE @ContextID INT;
SELECT @ContextID = [Id] FROM [dbo].[Context] WHERE [Name] = 'pei/$LegacyName/ai';
SELECT ai.[Name], dEn.[Value], dRu.[Value], ai.[Description]
FROM AttributeInfo ai
LEFT JOIN Dictionary dEn ON dEn.[key] = ai.[Name] AND dEn.ContextId = @ContextID AND dEn.LocaleId = 1
LEFT JOIN Dictionary dRu ON dRu.[key] = ai.[Name] AND dRu.ContextId = @ContextID AND dRu.LocaleId = 2
WHERE ai.PrimitiveEntityInfoId = $MainPEI
ORDER BY ai.[Name];
"@
Add-Section 'Переводы (§5): En → .resx, Ru → .ru-RU.resx' `
    @('Атрибут', 'TranslationEn', 'TranslationRu', 'DescriptionRu') $transRes

# ---------- 7. заголовки ----------
$titleRu = Invoke-Sql "SELECT TOP 1 [Description] FROM PrimitiveEntityInfo WHERE [Name] = '$LegacyName';"
$titleEn = Invoke-Sql @"
SELECT TOP 1 DEn.[Value]
FROM Dictionary DEn
INNER JOIN Dictionary DRu ON DEn.ContextId = DRu.ContextId AND DEn.[Key] = DRu.[Key]
WHERE DRu.[Value] = (SELECT TOP 1 [Description] FROM PrimitiveEntityInfo WHERE [Name] = '$LegacyName')
  AND DEn.LocaleId = 1 AND DRu.LocaleId = 2;
"@
Add-Section 'Заголовок справочника' @('RU (Description)', 'EN (Dictionary)') @("$($titleRu -join ' ')`t$($titleEn -join ' ')")

# ---------- 8. ссылочные кандидаты ----------
$numCols = @($viewCols | ForEach-Object {
    $c = ConvertFrom-Row $_
    if ($c[1] -in @('int', 'bigint', 'smallint', 'tinyint') -and
        $c[0] -notin @('Id', "${LegacyName}Id", 'PrimitiveEntityDataStateId', 'CreationUserId', 'LastChangeUserId', 'IsActive', 'Code')) { $c[0] }
})
if ($numCols.Count -gt 0) {
    $inList = ($numCols | ForEach-Object { "N'vw_$_'" }) -join ', '
    $views = Invoke-Sql "SELECT name FROM sys.views WHERE name IN ($inList) ORDER BY name;"
    $rows = foreach ($n in $numCols) {
        $hit = if ($views -contains "vw_$n") { "vw_$n" } else { '' }
        "$n`t$hit"
    }
    Add-Section 'Числовые колонки view — кандидаты на ссылки (§6 discovery-rules; совпадение view — подсказка, не решение)' `
        @('Колонка', 'Совпадающее view') $rows
}

# ---------- 8а. distinct-сводка числовых колонок (идентификация ссылок) ----------
if ($numCols.Count -gt 0 -and $viewExists -eq '1') {
    $dcParts = ($numCols | ForEach-Object { "SELECT N'$_' AS c, COUNT(DISTINCT [$_]) AS n FROM dbo.$viewName WHERE PrimitiveEntityDataStateId = 1 AND [$_] IS NOT NULL" })
    $dcRows = Invoke-Sql ($dcParts -join ' UNION ALL ')
    Add-Section 'Distinct-значений в числовых колонках (неархивные)' @('Колонка', 'Distinct') $dcRows

    $small = @($dcRows | ForEach-Object { $c = ConvertFrom-Row $_; if ([int]$c[1] -gt 0 -and [int]$c[1] -le 30) { $c[0] } })
    if ($small.Count -gt 0) {
        $sParts = ($small | ForEach-Object { "SELECT N'$_' AS c, CONVERT(NVARCHAR(100), v) FROM (SELECT DISTINCT [$_] AS v FROM dbo.$viewName WHERE PrimitiveEntityDataStateId = 1 AND [$_] IS NOT NULL) t" })
        $sRows = Invoke-Sql (($sParts -join ' UNION ALL ') + ' ORDER BY 1, 2')
        Add-Section 'Значения колонок с <=30 distinct (для сверки со словарями — enum-кандидаты)' @('Колонка', 'Значение') $sRows
    }
}

# ---------- 8б. объекты БД, ссылающиеся на PED ----------
$refObjs = Invoke-Sql @"
SELECT o.name, o.type_desc
FROM sys.objects o
WHERE o.type IN ('P','FN','IF','TF','V')
  AND o.is_ms_shipped = 0
  AND OBJECT_DEFINITION(o.object_id) LIKE '%PrimitiveEntityData_$MainPEI%'
ORDER BY o.type, o.name;
"@
Add-Section "Объекты БД со ссылкой на $pedTable (PopulateAffectedKeys, Load*, views — сверить состав с SoT SqlScripts)" `
    @('Объект', 'Тип') $refObjs

# ---------- 9. дамп активных строк для малого словаря ----------
if ($viewExists -eq '1' -and $counts) {
    $active = [int](ConvertFrom-Row $counts[0])[1]
    if ($active -gt 0 -and $active -le 150) {
        $dump = Invoke-Sql "SELECT * FROM dbo.$viewName WHERE PrimitiveEntityDataStateId = 1 ORDER BY Id;"
        $colNames = ($viewCols | ForEach-Object { (ConvertFrom-Row $_)[0] })
        [void]$sb.AppendLine("`n## Дамп активных строк ($active <= 150 — кандидат в enum по §7)")
        [void]$sb.AppendLine('| ' + ($colNames -join ' | ') + ' |')
        [void]$sb.AppendLine('|' + ('---|' * $colNames.Count))
        foreach ($r in $dump) {
            $cells = (ConvertFrom-Row $r) | ForEach-Object { $_ -replace '\|', '/' -replace '  +', ' ' }
            [void]$sb.AppendLine('| ' + ($cells -join ' | ') + ' |')
        }
    }
}

# ---------- 10. дети (§6) ----------
[void]$sb.AppendLine("`n## Дочерние примитивы (§6)")
if ($children.Count -eq 0) { [void]$sb.AppendLine('Нет.') }
foreach ($ch in $children) {
    $chName = $ch.Name
    $chView = "vw_$chName"
    $chViewExists = (Invoke-Sql "SELECT CASE WHEN OBJECT_ID('dbo.$chView') IS NULL THEN 0 ELSE 1 END")[0]
    $mainElsewhere = @(Invoke-Sql @"
SELECT e.Id, e.Name
FROM dbo.EntityStructure es
JOIN dbo.EntityInfo e ON e.Id = es.EntityInfoId
WHERE es.PrimitiveEntityInfoId = $($ch.PEI) AND es.ParentId IS NULL;
"@)
    [void]$sb.AppendLine("`n### Ребёнок ``$chName`` (PEI $($ch.PEI), StructureId $($ch.StructureId), RelationshipType $($ch.RelType), RelAttr $($ch.RelAttrId))")
    [void]$sb.AppendLine("- view ``dbo.$chView``: $(if ($chViewExists -eq '1') { 'есть' } else { '**НЕТ**' }); основной примитив другой сущности: $(if ($mainElsewhere -and $mainElsewhere.Count -gt 0) { ($mainElsewhere | ForEach-Object { (ConvertFrom-Row $_)[1] }) -join ', ' } else { 'нет' })")
    if ($chViewExists -ne '1') { continue }

    $parentKey = "${LegacyName}Id"
    $chCounts = @(Invoke-Sql @"
SELECT COUNT(*), SUM(CASE WHEN PrimitiveEntityDataStateId = 1 THEN 1 ELSE 0 END) FROM dbo.$chView;
SELECT MAX(Total), MAX(Active) FROM (
    SELECT [$parentKey], COUNT(*) AS Total,
           SUM(CASE WHEN PrimitiveEntityDataStateId = 1 THEN 1 ELSE 0 END) AS Active
    FROM dbo.$chView GROUP BY [$parentKey]) t;
"@)
    # два результата идут подряд: строка 0 — объёмы, строка 1 — максимумы
    $cc = ConvertFrom-Row $chCounts[0]
    $mx = if ($chCounts.Count -gt 1) { ConvertFrom-Row $chCounts[1] } else { @('NULL', 'NULL') }
    [void]$sb.AppendLine("- Строк всего: $($cc[0]); неархивных: $($cc[1]); макс. на родителя всего: $($mx[0]); неархивных: $($mx[1]) (колонка связи ``$parentKey`` — если NULL, проверить имя колонки во view ребёнка)")

    $chCols = Get-ViewColumns $chView
    Add-Section "Колонки dbo.$chView" @('Колонка', 'Тип', 'max_length', 'precision', 'scale', 'nullable') $chCols

    $chColSet = @{}
    foreach ($r in $chCols) { $chColSet[(ConvertFrom-Row $r)[0].ToLower()] = $true }
    $chAttrs = Get-Attributes ([int]$ch.PEI)
    Add-Section "Атрибуты ребёнка $chName" `
        @('Атрибут', 'Description RU', 'IsMain', 'MainRank', 'GroupRank', 'AttributeRank', 'GroupNameEn', 'GroupNameRu') $chAttrs

    $chAttrNames = @($chAttrs | ForEach-Object { (ConvertFrom-Row $_)[0] })
    if ($chAttrNames.Count -gt 0) {
        $table = Get-AttrCounts $chView $chColSet $chAttrNames
        Add-Section "Заполненность атрибутов ребёнка $chName (неархивные)" @('Атрибут', 'Заполнено') $table
    }

    $chTrans = Invoke-Sql @"
DECLARE @ContextID INT;
SELECT @ContextID = [Id] FROM [dbo].[Context] WHERE [Name] = 'pei/$chName/ai';
SELECT ai.[Name], dEn.[Value], dRu.[Value], ai.[Description]
FROM AttributeInfo ai
LEFT JOIN Dictionary dEn ON dEn.[key] = ai.[Name] AND dEn.ContextId = @ContextID AND dEn.LocaleId = 1
LEFT JOIN Dictionary dRu ON dRu.[key] = ai.[Name] AND dRu.ContextId = @ContextID AND dRu.LocaleId = 2
WHERE ai.PrimitiveEntityInfoId = $($ch.PEI)
ORDER BY ai.[Name];
"@
    Add-Section "Переводы ребёнка $chName" @('Атрибут', 'TranslationEn', 'TranslationRu', 'DescriptionRu') $chTrans

    $chTitle = Invoke-Sql @"
SELECT TOP 1 DEn.[Value]
FROM Dictionary DEn
INNER JOIN Dictionary DRu ON DEn.ContextId = DRu.ContextId AND DEn.[Key] = DRu.[Key]
WHERE DRu.[Value] = (SELECT TOP 1 [Description] FROM PrimitiveEntityInfo WHERE [Name] = '$chName')
  AND DEn.LocaleId = 1 AND DRu.LocaleId = 2;
"@
    [void]$sb.AppendLine("`nЗаголовок группы ребёнка: EN = ``$($chTitle -join ' ')``")
}

[void]$sb.AppendLine("`n---`nЗапросов sqlcmd выполнено скриптом: $queryCount")

# ---------- запись ----------
$dir = Split-Path -Parent $OutFile
if ($dir -and -not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
[IO.File]::WriteAllText($OutFile, $sb.ToString(), (New-Object Text.UTF8Encoding $false))

foreach ($f in $tmpFiles) { Remove-Item -LiteralPath $f -ErrorAction SilentlyContinue }
Write-Host "FactSheet: $OutFile"
Write-Host "Entity=$Entity; LegacyName=$LegacyName; PEI=$MainPEI; children=$($children.Count); queries=$queryCount"
