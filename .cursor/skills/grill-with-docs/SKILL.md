---
name: grill-with-docs
description: >-
  Интервью по ещё не записанному плану или дизайну, пока ветки решений не закрыты.
  Согласованный термин дописывает в glossary.md навыка svt-architecture.
  Используй при «погриль», «grill-with-docs», «grill me» и разборе плана до постановки.
---

# grill-with-docs

Интервью по дереву решений. Вопрос задаётся раундом: все развилки, которые уже можно спросить, и ни одной, которая ждёт ещё не данный ответ. Факты из репозитория и канона агент смотрит сам.

Постановка этим скилом не пишется. Код не начинается. Канон и глоссарий меняются только после явного «да» на конкретную формулировку.

## Когда применять

- «Погриль», «grill-with-docs», «grill me», «проведи интервью», «спроси меня»

Разработка справочника, рефакторинг и оценка архитектуры остаются у `svt-architecture` и `add-reference-book`. Запись файла постановки — у `to-spec`.

## Перед вопросами

Прочитай skill `svt-architecture` и `.cursor/skills/svt-architecture/glossary.md`. Термины в вопросах и рекомендациях бери оттуда. При первом упоминании нового термина — «русский перевод (English)», дальше короткий английский термин из левой колонки.

Факт, который есть в коде, каноне или глоссарии, не спрашивай. Поиск не держит остальные вопросы раунда: ждёт только тот вопрос, которому этот факт нужен.

## Раунды

Дерево: у каждого решения есть решения, которые на нём висят. Фронтир — решения, чьи предпосылки уже закрыты.

Раунд — весь текущий фронтир. Вопрос, который зависит от ещё открытого вопроса этого же раунда, уходит в следующий. После ответов фронтир пересчитывается заново.

Формат:

```
❓ **В1** — **<заголовок>**: <текст, можно с вариантами>

➡️ <рекомендуемый ответ>

---

❓ **В2** — **<заголовок>**: <текст>

➡️ <рекомендуемый ответ>
```

Формулируй так, чтобы «да» принимало рекомендацию. Ответ пользователя жди до следующего раунда.

Слово в глоссарии значит другое, чем в реплике пользователя, — скажи об этом в вопросе и спроси, какое значение берём. Реплика расходится с кодом — покажи расхождение в том же вопросе.

## Термины

Единственный файл терминов — `.cursor/skills/svt-architecture/glossary.md`. Корневой `GLOSSARY.md` и `GLOSSARY-MAP.md` не создавать.

Новая строка дописывается в существующую таблицу «Термин | Пояснение». Вступление файла не переписывать. Что попадает в таблицу, решает это вступление: узкий английский термин и русское пояснение. Отдельный фильтр «только уникальные понятия предметной области» не применять.

Глоссарий — словарь. Постановка, план, чеклист, схема и пути к коду в него не пишутся.

Строку пиши только после «да» на эту формулировку. До «да» термин остаётся в разговоре.

## Решения канона

Предлагай дописать файл канона, только когда верны все три условия:

1. Откатить дорого.
2. Без контекста решение удивит.
3. Был реальный выбор из альтернатив.

Файл бери из таблицы «Канон» в `svt-architecture/SKILL.md` — тот, который эту тему уже ведёт. Новый вид протокола и каталог `docs/adr/` не создавать. Правку пиши после «да» на конкретный абзац. До «да» решение остаётся в разговоре. В постановку его переносит `to-spec`; постановка — снимок, не процедура.

Уже записанный принцип архитектуры в файл не копируй.

## Конец

Фронтир пуст, когда каждая ветка либо закрыта ответом, либо явно оставлена пробелом. Спроси, общее ли понимание. После «да» постановку не пиши и код не начинай: постановка — отдельная команда `to-spec`.

## Источник

Адаптация `grilling` и `grill-with-docs` из [mattpocock/skills](https://github.com/mattpocock/skills).

MIT License

Copyright (c) 2026 Matt Pocock

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
