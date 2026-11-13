# -*- coding: utf-8 -*-
import re
from pathlib import Path

p = Path(r"d:\SVT\docs\docs\План-график Атлас")
html = next(p.glob("Логистика*v2_2.html"))
t = html.read_text(encoding="utf-8")

def slice_sec(start_id, end_id):
    a = t.find(f'id="{start_id}"')
    b = t.find(f'id="{end_id}"')
    return a, b, (b - a) if a >= 0 and b > a else -1

for a, b in [("pres0-sec", "goal-sec"), ("streams-sec", "pipe6-sec"), ("pipe6-sec", "next-sec")]:
    print(a, "->", b, slice_sec(a, b))

m = re.search(r"const V12_SPEC = (\{.*?\});\r?\nconst V12_EXTRA", t, re.S)
print("V12_SPEC", bool(m), "len", len(m.group(1)) if m else 0)
if m:
    import json
    spec = json.loads(m.group(1))
    print("streams", len(spec["streams"]))
    for s in spec["streams"]:
        print(f"  {s['code']}: rels={len(s.get('rels',[]))} n={s.get('n')}")

# count p0 streams
ids = re.findall(r'id="(p0-[^"]+)"', t)
print("p0 ids", ids)
