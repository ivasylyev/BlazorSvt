# publish-all-sql

## Что делает

`publish-all-sql` копирует SQL-скрипты деплоя из `Migrations/` и SoT в `C:\publish\v2` (или в `-TargetPath`). Исходники в репозитории не изменяет. Перед копированием удаляет старые `*.sql` в целевой папке режима.

На базу скрипт не накатывает. Локальный накат — [`create-programmability`](create-programmability.md).

## Когда брать

| Ситуация | Куда |
|---|---|
| Опубликовать SQL для деплоя, обновить `C:\publish\v2` | Этот скил или `/publish_all_sql` |
| Накатить SP на локальную `mdm` | [`create-programmability`](create-programmability.md) |
| Написать справочник и его SQL | [`add-reference-book`](add-reference-book.md); публикация — после того, как SoT и релиз собраны |

## Что нужно до старта

Запуск из корня репозитория `c:\Work\BlazorSVT`. Конвенция релизов — `BlazorSvt/SqlScripts/Migrations/README.md`. Имя папки в `Migrations/` совпадает с `VersionPrefix` в `BlazorSvt/BlazorSvt.csproj`. После выката папку версии не правят.

## Как устроено

Процедура и команды — [`SKILL.md`](../../skills/publish-all-sql/SKILL.md).

| Mode | Источник | Куда |
|---|---|---|
| `All` (по умолчанию) | `SqlScripts/Migrations/{x.y.z}/` | `TargetPath\{x.y.z}\` |
| `Release` | `Migrations/{Version}/` | `TargetPath\{Version}\` |
| `Programmability` | SoT `**/Programmability/*.sql` | `TargetPath\programmability\` |
| `FromSource` | SoT Structure + Programmability | плоский `TargetPath\` с префиксами `001.`… |

Опционально: `-TargetPath`. В папку режима не копируются `Create-Programmability.ps1`, `Publish-AllSql.ps1`, `README.md`, `Translations_info.txt`, `Migrations/README.md`.

Новый справочник в публикации: SoT в `Modules/{Name}/`, DDL-дельта в `Migrations/X.Y.Z/`, строки манифеста `FromSource` и `Programmability` в `Publish-AllSql.ps1`. Подробности шага — в `SKILL.md`.

Человек вызывает `/publish_all_sql`. Агент берёт скил сам, когда просят опубликовать SQL. Оба входа ведут в один `SKILL.md`.

## Сработало, если

- В ответе есть число файлов, `Mode=…` и путь `TargetPath`.
- При отсутствии версии или исходного файла показано сообщение, и выполнение остановлено.
- Файлы в `SqlScripts/` не изменились.

## Куда встаёт

После [`add-reference-book`](add-reference-book.md), когда SQL уже лежит в SoT и в папке релиза. Рядом [`create-programmability`](create-programmability.md) выполняет Programmability на localhost и в `C:\publish\v2` ничего не кладёт.
