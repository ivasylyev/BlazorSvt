# Фаза 1 — Discovery

Прямой прогон `svt-migrate-catalog`, без оркестратора: сначала факт-лист, затем правила экрана. Запросы — только для пробелов факт-листа.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File ".cursor/skills/svt-migrate-catalog/scripts/generate-discovery-facts.ps1" -RegistryName {ИмяРеестра} -OutFile {WorkDir}\facts.md
```

`{ИмяРеестра}` — имя из колонки «Имя» `catalogs-scope.md`. Результат — facts.md: идентификация шага 0, атрибуты, заполненность, типы view, переводы, заголовки, дети, sync-сигналы, кандидаты ссылок, объекты на PED. Что факт-лист не закрывает (цели ссылок без совпадения имён, метрики каскада конкретной ссылки, `stg.Load*`, члены словаря-цели) — добрать точечно из [discovery-queries.md](discovery-queries.md), файл после выборки не держать.

| Когда | Файл |
|---|---|
| Состав экрана, порядок, подписи, ссылки, enum, каскад | [discovery-rules.md](discovery-rules.md) |
| Запрос к `mdm` для пробелов факт-листа, DDL | [discovery-queries.md](discovery-queries.md) |
| У справочника есть дочерний примитив | [child-primitives.md](child-primitives.md), раздел «Discovery» |

Оркестратор называет роли один из этих файлов. Второй роль не открывает, пока её файл этого не сказал.

Дальше по фазам — [sql.md](sql.md). Класс «стабильный словарь с enum» — [SKILL.md](SKILL.md).
