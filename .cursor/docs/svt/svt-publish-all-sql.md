# svt-publish-all-sql

## Что делает

`svt-publish-all-sql` копирует SQL-скрипты деплоя из `Migrations/` и SoT в `C:\publish\v2` (или в `-TargetPath`). Исходники в репозитории остаются как есть. Перед копированием старые `*.sql` в целевой папке режима удаляются. На базу скрипт не накатывает: локальный накат — [`svt-create-programmability`](svt-create-programmability.md).

## Когда брать

Человек вызывает `/publish_all_sql`. Агент берёт скил сам, когда просят опубликовать SQL или обновить `C:\publish\v2`. Оба входа ведут в один [`SKILL.md`](../../skills/svt-publish-all-sql/SKILL.md).

| Ситуация | Куда |
|---|---|
| Опубликовать SQL для деплоя, обновить `C:\publish\v2` | Этот скил |
| Накатить SP на локальную `mdm` | [`svt-create-programmability`](svt-create-programmability.md) |
| Написать справочник и его SQL | [`svt-migrate-catalog`](svt-migrate-catalog.md); публикация — после того, как SoT и релиз собраны |

## Что нужно до старта

Запуск из корня репозитория `c:\Work\BlazorSVT`. Конвенция релизов — `BlazorSvt/SqlScripts/Migrations/README.md`. Имя папки в `Migrations/` совпадает с `VersionPrefix` в `BlazorSvt/BlazorSvt.csproj`. После выката папку версии не правят.

## Каталог деплоя

Четыре режима — `All` (по умолчанию), `Release`, `Programmability`, `FromSource`. Источник, приёмник и что в папку режима не копируется — в `SKILL.md`. Туда же шаг нового справочника: SoT, DDL-дельта релиза и строки манифеста в `Publish-AllSql.ps1`.

## Частые вопросы

**Скрипт накатил базу.**

Скрипт копирует файлы в `TargetPath`. Накат Programmability на localhost — [`svt-create-programmability`](svt-create-programmability.md).

**Какой режим брать.**

Без явного режима — `All`: все версии `Migrations/` в `C:\publish\v2\{version}\`. Остальные три режима и их источники — в `SKILL.md`.

## Сработало, если

- В ответе есть число файлов, `Mode=…` и путь `TargetPath`.
- При отсутствии версии или исходного файла показано сообщение, и выполнение остановлено.
- Файлы в `SqlScripts/` не изменились.

## Куда встаёт

Шаг после [`svt-migrate-catalog`](svt-migrate-catalog.md), когда SQL уже лежит в SoT и в папке релиза. Рядом [`svt-create-programmability`](svt-create-programmability.md) выполняет Programmability на localhost. Цепочка — [`docs/README.md`](../README.md).
