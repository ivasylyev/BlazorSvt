#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""Generate module resx from MDM Dictionary (pei/{Entity}/ai). LocaleId 1=en, 2=ru."""

from __future__ import annotations

import argparse
import re
import subprocess
import sys
import xml.sax.saxutils as xml
from pathlib import Path

# MDM attribute name -> resx key suffix (LocationsNodesDetailDto.*)
DETAIL_ATTR_TO_RESX_SUFFIX: dict[str, str] = {
    "Name_ru": "NameRu",
    "Name_en": "NameEn",
    "IsArchive": "CannotDeliver",
    "LocationType": "LocationTypeName",
    "TypeNode": "TypeNodeName",
    "Region": "RegionName",
    "Country": "CountryName",
    "4_level_CityRU": "Level4CityRU",
    "4_level_CityEN": "Level4CityEN",
    "4_level_City_FIAS": "Level4CityFias",
    "СoordinateW": "CoordinateW",
    "СoordinateL": "CoordinateL",
}

GRID_ATTR_TO_DTO_SUFFIX: dict[str, str] = {
    "Name_ru": "Name",
    "Name_en": "Name",
    "LocationType": "LocationTypeName",
    "TypeNode": "TypeNodeName",
    "Region": "RegionName",
    "Country": "CountryName",
    "RegionRU": "RegionRU",
    "Code": "Code",
}

SQL_TEMPLATE = """
DECLARE @ContextID INT;
SELECT @ContextID = [Id]
FROM [dbo].[Context]
WHERE [Name] = 'pei/{entity}/ai';

SELECT
    ai.[Name] AS AttributeSystemName,
    dEn.[Value] AS TranslationEn,
    dRu.[Value] AS TranslationRu,
    ai.[Description] AS DescriptionRu
FROM AttributeInfo ai
LEFT JOIN Dictionary dEn
    ON dEn.[key] = ai.[Name] AND dEn.ContextId = @ContextID AND dEn.LocaleId = 1
LEFT JOIN Dictionary dRu
    ON dRu.[key] = ai.[Name] AND dRu.ContextId = @ContextID AND dRu.LocaleId = 2
WHERE ai.PrimitiveEntityInfoId = (
    SELECT TOP 1 Id FROM PrimitiveEntityInfo WHERE [name] = '{entity}'
)
ORDER BY ai.[Name];
"""

TITLE_RU_SQL = """
SELECT TOP 1 [Description]
FROM PrimitiveEntityInfo
WHERE [Name] = '{entity}';
"""

TITLE_EN_SQL = """
SELECT TOP 1 DEn.[Value]
FROM Dictionary DEn
INNER JOIN Dictionary DRu
    ON DEn.ContextId = DRu.ContextId AND DEn.[Key] = DRu.[Key]
WHERE DRu.[Value] = (
    SELECT TOP 1 [Description]
    FROM PrimitiveEntityInfo
    WHERE [Name] = '{entity}'
)
  AND DEn.LocaleId = 1
  AND DRu.LocaleId = 2;
"""


def run_scalar_sql(
    sql: str,
    server: str,
    database: str,
    user: str,
    password: str,
) -> str | None:
    import tempfile

    with tempfile.NamedTemporaryFile(mode="w", suffix=".txt", delete=False, encoding="utf-8") as tmp:
        out_path = tmp.name

    subprocess.run(
        [
            "sqlcmd",
            "-S", server,
            "-d", database,
            "-U", user,
            "-P", password,
            "-C",
            "-f", "o:65001",
            "-Q", sql,
            "-W",
            "-h", "-1",
            "-o", out_path,
        ],
        check=True,
    )

    raw = Path(out_path).read_text(encoding="utf-8", errors="replace")
    Path(out_path).unlink(missing_ok=True)

    for line in raw.splitlines():
        line = line.strip().lstrip("\ufeff")
        if not line or line.startswith("-") or line.endswith("rows affected)"):
            continue
        if line.startswith("("):
            continue
        return line
    return None


def fetch_entity_titles(
    entity: str,
    server: str,
    database: str,
    user: str,
    password: str,
    title_en_fallback: str | None,
) -> tuple[str, str]:
    title_ru = run_scalar_sql(
        TITLE_RU_SQL.format(entity=entity),
        server,
        database,
        user,
        password,
    )
    if not title_ru:
        raise SystemExit(
            f"PrimitiveEntityInfo.Description is empty for '{entity}'. "
            "Stop and ask the user for the Russian title."
        )

    title_en = run_scalar_sql(
        TITLE_EN_SQL.format(entity=entity),
        server,
        database,
        user,
        password,
    )
    if not title_en:
        if title_en_fallback:
            title_en = title_en_fallback
        else:
            raise SystemExit(
                f"No English Dictionary translation for Description={title_ru!r}. "
                f"Provide --title-en with agent translation."
            )

    return title_ru, title_en


def set_resx_data_value(path: Path, key: str, value: str) -> None:
    text = path.read_text(encoding="utf-8")
    escaped = xml.escape(value)
    pattern = rf'(<data name="{re.escape(key)}" xml:space="preserve">\s*<value>).*?(</value>)'
    new_text, count = re.subn(pattern, rf"\1{escaped}\2", text, count=1, flags=re.DOTALL)
    if count == 0:
        raise SystemExit(f"Key {key!r} not found in {path}")
    path.write_text(new_text, encoding="utf-8")


def update_platform_menu_titles(
    platform_dir: Path,
    entity: str,
    title_ru: str,
    title_en: str,
) -> None:
    menu_key = f"HeaderMenu.{entity}"
    set_resx_data_value(platform_dir / "Platform.ru-RU.resx", menu_key, title_ru)
    set_resx_data_value(platform_dir / "Platform.resx", menu_key, title_en)


def has_cyrillic(text: str) -> bool:
    return bool(re.search(r"[\u0400-\u04FF]", text))


def run_sql(entity: str, server: str, database: str, user: str, password: str) -> list[tuple[str, str | None, str | None, str | None]]:
    import tempfile

    sql = SQL_TEMPLATE.format(entity=entity)
    with tempfile.NamedTemporaryFile(mode="w", suffix=".txt", delete=False, encoding="utf-8") as tmp:
        out_path = tmp.name

    subprocess.run(
        [
            "sqlcmd",
            "-S", server,
            "-d", database,
            "-U", user,
            "-P", password,
            "-C",
            "-f", "o:65001",
            "-Q", sql,
            "-s", "|",
            "-W",
            "-h", "-1",
            "-o", out_path,
        ],
        check=True,
    )

    raw = Path(out_path).read_text(encoding="utf-8", errors="replace")
    Path(out_path).unlink(missing_ok=True)

    rows: list[tuple[str, str | None, str | None, str | None]] = []
    for line in raw.splitlines():
        if not line.strip() or line.startswith("-"):
            continue
        parts = line.split("|")
        if len(parts) < 4:
            continue
        attr = parts[0].strip().lstrip("\ufeff")
        en = parts[1].strip() if parts[1].strip() else None
        ru = parts[2].strip() if parts[2].strip() else None
        desc = parts[3].strip() if parts[3].strip() else None
        rows.append((attr, en, ru, desc))
    return rows


def detail_resx_suffix(attr: str) -> str:
    return DETAIL_ATTR_TO_RESX_SUFFIX.get(attr, attr)


def pick_en(attr: str, translation_en: str | None, description_ru: str | None) -> str:
    if translation_en:
        return translation_en
  # Description in MDM is Russian — never use for default resx
    return attr


def pick_ru(attr: str, translation_ru: str | None, description_ru: str | None) -> str:
    if translation_ru:
        return translation_ru
    if description_ru:
        return description_ru
    return attr


def write_resx(path: Path, values: dict[str, str]) -> None:
    lines = [
        '<?xml version="1.0" encoding="utf-8"?>',
        "<root>",
        '  <resheader name="resmimetype"><value>text/microsoft-resx</value></resheader>',
        '  <resheader name="version"><value>2.0</value></resheader>',
        '  <resheader name="reader"><value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value></resheader>',
        '  <resheader name="writer"><value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value></resheader>',
    ]
    for key in sorted(values.keys()):
        lines.append(f'  <data name="{xml.escape(key)}" xml:space="preserve">')
        lines.append(f"    <value>{xml.escape(values[key])}</value>")
        lines.append("  </data>")
    lines.append("</root>")
    path.write_text("\n".join(lines) + "\n", encoding="utf-8")


def build_values(
    entity: str,
    rows: list[tuple[str, str | None, str | None, str | None]],
    title_ru: str,
    title_en: str,
) -> tuple[dict[str, str], dict[str, str]]:
    en: dict[str, str] = {}
    ru: dict[str, str] = {}

    detail_prefix = f"{entity}DetailDto."
    dto_prefix = f"{entity}Dto."

    for attr, translation_en, translation_ru, description_ru in rows:
        detail_key = detail_prefix + detail_resx_suffix(attr)
        en[detail_key] = pick_en(attr, translation_en, description_ru)
        ru[detail_key] = pick_ru(attr, translation_ru, description_ru)

        if attr in GRID_ATTR_TO_DTO_SUFFIX:
            grid_key = dto_prefix + GRID_ATTR_TO_DTO_SUFFIX[attr]
            if grid_key not in en:
                en[grid_key] = pick_en(attr, translation_en, description_ru)
                ru[grid_key] = pick_ru(attr, translation_ru, description_ru)

    # UI / grid helpers (not in AttributeInfo)
    ui_en = {
        f"{entity}Grid.Title": title_en,
        f"{dto_prefix}LocationTypeCode": "Location type code",
        f"{dto_prefix}TypeNodeCode": "Node type code",
        f"{dto_prefix}RegionCode": "Region code",
        f"{dto_prefix}CountryCode": "Country code",
        f"{dto_prefix}CreationDate": "Creation date",
        f"{dto_prefix}LastChangeDate": "Last change date",
        f"{dto_prefix}IsArchive": "Archive",
        f"{dto_prefix}Yes": "Yes",
        f"{dto_prefix}No": "No",
        f"{dto_prefix}Archive": "Archive",
        f"{dto_prefix}Active": "Active",
        f"{detail_prefix}Code": en.get(f"{detail_prefix}Code", "Code"),
        f"{detail_prefix}CreationDate": "Creation date",
        f"{detail_prefix}LastChangeDate": "Last change date",
        f"{detail_prefix}IsArchive": "Archive",
        f"{detail_prefix}Yes": "Yes",
        f"{detail_prefix}No": "No",
        f"{detail_prefix}Archive": "Archive",
        f"{detail_prefix}Active": "Active",
        f"{detail_prefix}CoordinateW": "Geographic latitude",
        f"{detail_prefix}CoordinateL": "Geographic longitude",
        f"{detail_prefix}Group.0.Default": "Default",
    }
    ui_ru = {
        f"{entity}Grid.Title": title_ru,
        f"{dto_prefix}LocationTypeCode": "Код типа местоположения",
        f"{dto_prefix}TypeNodeCode": "Код типа узла",
        f"{dto_prefix}RegionCode": "Код региона",
        f"{dto_prefix}CountryCode": "Код страны",
        f"{dto_prefix}CreationDate": "Дата создания",
        f"{dto_prefix}LastChangeDate": "Дата изменения",
        f"{dto_prefix}IsArchive": "Архив",
        f"{dto_prefix}Yes": "Да",
        f"{dto_prefix}No": "Нет",
        f"{dto_prefix}Archive": "Архив",
        f"{dto_prefix}Active": "Активный",
        f"{detail_prefix}Code": ru.get(f"{detail_prefix}Code", "Code"),
        f"{detail_prefix}CreationDate": "Дата создания",
        f"{detail_prefix}LastChangeDate": "Дата изменения",
        f"{detail_prefix}IsArchive": "Архив",
        f"{detail_prefix}Yes": "Да",
        f"{detail_prefix}No": "Нет",
        f"{detail_prefix}Archive": "Архив",
        f"{detail_prefix}Active": "Активный",
        f"{detail_prefix}CoordinateW": "Географическая широта",
        f"{detail_prefix}CoordinateL": "Географическая долгота",
        f"{detail_prefix}Group.0.Default": "По умолчанию",
    }

    en.update(ui_en)
    ru.update(ui_ru)

    # Remove erroneous Cyrillic coordinate keys if present from MDM
    for key in list(en.keys()):
        if "oordinate" in key and has_cyrillic(key):
            del en[key]
    for key in list(ru.keys()):
        if "oordinate" in key and has_cyrillic(key):
            del ru[key]

    return en, ru


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--entity", default="LocationsNodes")
    parser.add_argument("--output-dir", type=Path, required=True)
    parser.add_argument("--server", default="S001ITD-0084")
    parser.add_argument("--database", default="mdm")
    parser.add_argument("--user", default="SVT")
    parser.add_argument("--password", default="SVTsrv1!")
    parser.add_argument("--title-en", dest="title_en", default=None, help="Agent translation when Dictionary has no EN title")
    parser.add_argument("--platform-resources-dir", type=Path, default=None)
    args = parser.parse_args()

    title_ru, title_en = fetch_entity_titles(
        args.entity,
        args.server,
        args.database,
        args.user,
        args.password,
        args.title_en,
    )

    rows = run_sql(args.entity, args.server, args.database, args.user, args.password)
    en, ru = build_values(args.entity, rows, title_ru, title_en)

    args.output_dir.mkdir(parents=True, exist_ok=True)
    write_resx(args.output_dir / f"{args.entity}.resx", en)
    write_resx(args.output_dir / f"{args.entity}.ru-RU.resx", ru)

    if args.platform_resources_dir:
        update_platform_menu_titles(args.platform_resources_dir, args.entity, title_ru, title_en)

    print(f"Generated {len(en)} keys -> {args.output_dir}")
    print(f"  Title RU: {title_ru!r}")
    print(f"  Title EN: {title_en!r}")
    # Spot-check
    detail = f"{args.entity}DetailDto."
    for sample in ("Pobox", "City", "AddressCountryISO2"):
        k = detail + sample
        if k in en:
            print(f"  EN {sample}: {en[k]!r}")
        if k in ru:
            print(f"  RU {sample}: {ru[k]!r}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
