# Роль: документация (на весь батч)

Сначала прочитай `{AgentsDir}\common.md` и `{BatchDir}\plan.md`, затем разделы 1 и 8 каждого `{BatchDir}\{Entity}\spec.md` строк плана со статусом «в работе».

Задача — документы, которые меняются вместе с новыми справочниками (как в чеклисте `add-reference-book`: руководство, Postman, developers.md). Три файла правишь один раз на всех. Параллельно работают csharp-агенты — код не трогай.

## Твои файлы

- `docs/user-guide/svt-2-user-guide.ru.confluence.md` и `.en.confluence.md`
- `docs/api/svt-api-v1.vasilevivv-local-tests.postman.json`
- `docs/api/svt-api-v1-developers.md`
- `{BatchDir}\docs-report.md`

Ты единственный, кто пишет в эти файлы. Образец строк — соседние справочники в тех же файлах; код API и заголовки — из раздела 1 spec.

## Шаги (по каждому справочнику)

1. Руководство RU и EN: перечень раздела 1 и строка таблицы меню (домен, пункт, заголовок страницы из раздела 1).
2. Postman: папка `{код API}` по образцу соседней (Schema, Search first page с сохранением id, Get item, Export short, Export full) и переменная camelCase от `{Entity}Id` (`transportRouteId`, не `transportrouteId`) = `"0"`.
3. `svt-api-v1-developers.md`: строка каталога; фразу «ещё не перенесён» поправить, если есть.

## После всех

4. Postman — `ConvertFrom-Json` один раз.
5. Кодировка как у файлов.
6. `docs-report.md`: по каждому справочнику что добавлено.
