import os
root = r'c:\Work\BlazorSVT'
files = []
for dp, _, fns in os.walk(root):
    for fn in fns:
        if fn.lower().endswith('.sql'):
            files.append(os.path.join(dp, fn))
files.sort()
utf8_bom = []
not_bom = []
for p in files:
    with open(p, 'rb') as f:
        b = f.read(4)
    rel = os.path.relpath(p, root)
    if len(b) >= 3 and b[:3] == b'\xef\xbb\xbf':
        utf8_bom.append(rel)
    else:
        if len(b) >= 2 and b[:2] == b'\xff\xfe':
            enc = 'UTF-16 LE'
        elif len(b) >= 2 and b[:2] == b'\xfe\xff':
            enc = 'UTF-16 BE'
        else:
            enc = 'UTF-8 without BOM (or ANSI/other)'
        not_bom.append((rel, enc, b[:3].hex() if len(b)>=3 else b.hex()))
print(f'TOTAL: {len(files)}')
print(f'UTF-8 BOM: {len(utf8_bom)}')
print('NOT UTF-8 BOM:')
for rel, enc, hx in not_bom:
    print(f'  [{enc}] {rel} (first bytes: {hx})')
if not not_bom:
    print('  (none - all files have UTF-8 BOM)')
