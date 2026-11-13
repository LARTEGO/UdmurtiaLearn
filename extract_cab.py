import olefile, os

ole = olefile.OleFileIO('/workspace/СЛОВАРЬ/GoldenDict-v2.msi')
os.makedirs('/tmp/msi', exist_ok=True)
for entry in ole.listdir():
    name = '/'.join(entry)
    nb = name.encode('utf-16-le', errors='ignore')
    idx = nb.find(b'\x0bCAB!')
    if idx >= 0:
        data = ole.openstream(entry).read()
        cabname = nb[idx + 1:].replace(b'\x00', b'').decode('ascii') + '.cab'
        out = f'/tmp/msi/{cabname}'
        off = data.find(b'MSCABINET')
        print(out, 'len', len(data), 'head', data[:8].hex(), 'mscaboff', off)
        open(out, 'wb').write(data[off:] if off >= 0 else data)
print("DONE")
