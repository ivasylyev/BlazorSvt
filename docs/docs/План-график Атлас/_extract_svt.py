# -*- coding: utf-8 -*-
p = r"d:\SVT\docs\docs\План-график Атлас\Логистика_—_эталонный_процесс_и_Атлас_план_график_v24_+_чат_v2_2.html"
c = open(p, encoding="utf-8").read()
needle = '"code":"СВТ"'
i = c.find(needle)
print("index", i)
out = r"d:\SVT\docs\docs\План-график Атлас\_svt_v12_chunk.txt"
if i < 0:
    open(out, "w", encoding="utf-8").write("NOT FOUND")
else:
    # find enclosing stream object start
    start = c.rfind("{", 0, i)
    # crude brace match forward from start
    depth = 0
    end = start
    for j in range(start, len(c)):
        if c[j] == "{":
            depth += 1
        elif c[j] == "}":
            depth -= 1
            if depth == 0:
                end = j + 1
                break
    chunk = c[start:end]
    open(out, "w", encoding="utf-8").write(chunk)
    print("wrote", len(chunk), "chars")
