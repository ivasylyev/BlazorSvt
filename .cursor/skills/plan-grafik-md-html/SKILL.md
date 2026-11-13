---
name: plan-grafik-md-html
description: >-
  Конвертация и синхронизация разделов 0 / 1а / 1б план-графика Атлас между
  Markdown (текст без скриптов) и интерактивным HTML. Используй когда пользователь
  просит выгрузить разделы 0, 1а, 1б в md, обновить HTML из md, plan-grafik-md-html,
  разделы_0_1а_1б.md, пайплайн стримов, V12_SPEC или презентацию руководства.
---

# План-график: MD ↔ HTML (разделы 0, 1а, 1б)

## Файлы

| Файл | Назначение |
|------|------------|
| `docs/docs/План-график Атлас/Логистика_—_эталонный_процесс_и_Атлас_план_график_v24_+_чат_v2_2.html` | SoT интерактива (CSS/JS + данные) |
| `docs/docs/План-график Атлас/разделы_0_1а_1б.md` | Редактируемый снимок **только текста/таблиц** разделов 0, 1а, 1б |
| [format.md](format.md) | Схема MD и что куда пишется |
| [scripts/extract.ps1](scripts/extract.ps1) | HTML → MD |
| [scripts/apply.ps1](scripts/apply.ps1) | MD → HTML (скрипты HTML не трогает) |

## Когда что запускать

1. **Выгрузить / обновить MD из HTML** (после правок в браузере или чужих патчей HTML):

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File ".cursor/skills/plan-grafik-md-html/scripts/extract.ps1"
```

2. **Записать правки MD обратно в HTML** (после редактирования `разделы_0_1а_1б.md`):

```powershell
# сначала сухой прогон
powershell -NoProfile -ExecutionPolicy Bypass -File ".cursor/skills/plan-grafik-md-html/scripts/apply.ps1" -DryRun

# запись (делает backup `*_backup_before_md_apply_*.html`)
powershell -NoProfile -ExecutionPolicy Bypass -File ".cursor/skills/plan-grafik-md-html/scripts/apply.ps1" -What all
```

Частичная запись:

- `-What intros` — вводные абзацы секций `#pres0-sec`, `#streams-sec`, `#pipe6-sec`
- `-What v12` — таблица вех раздела **1а** → поля `V12_SPEC.streams[].rels` (`t`, `d`, `own`, `type`, `metric`); от этого же питается UI **1б**
- `-What p0` — блоки а/б/в/д/е стримов раздела **0** (`id="p0-…"`); фильтр `-StreamId p0-СВТ`

## Правила агента

1. Сначала прочитай [format.md](format.md), если правишь структуру MD.
2. **Не** вставляй в MD CSS, `<script>`, iframe и шаблон `v12-tpl`.
3. Перед `apply.ps1` без `-DryRun` — сообщи пользователю, что будет backup HTML.
4. После успешного apply при необходимости снова запусти `extract.ps1`, чтобы MD совпал с HTML.
5. Новые вехи / новые стримы / правки `steps` и `TASKS` этим skill **не создаются**: только обновление существующих полей. Новые сущности — правка HTML/`V12_SPEC` отдельно (как в прежних `_patch_*.ps1`).
6. Не выдумывай сроки и метрики: нет в MD/HTML — `пробел`.
7. Раздел **1б** в MD — описание и список стримов; интерактив пересобирается скриптами HTML из `V12_SPEC` + `TASKS` при открытии файла.

## Типовой цикл правки

```
extract.ps1 → правка разделы_0_1а_1б.md → apply.ps1 -DryRun → apply.ps1 → (опц.) extract.ps1
```
