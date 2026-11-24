# Wrapper: HTML → MD для разделов 0 / 1а / 1б
# Канонический скрипт — .cursor/skills/atlas-plan-grafik-md-html/scripts/extract.ps1
# Путь считается от расположения этого файла: docs/docs/План-график Атлас → корень репозитория.
$extract = Join-Path $PSScriptRoot '..\..\..\.cursor\skills\atlas-plan-grafik-md-html\scripts\extract.ps1'
& $extract @args
