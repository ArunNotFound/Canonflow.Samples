with open('list.md', 'r') as f:
    text = f.read()

text += """
### 11. Gatepass App (`gatepass-core`)
- [x] Generated `--fscheck` (FSA) in dogfood.sh
- [x] F# Tests (FsCheck property tests added for Resident/Visitor flows)
- [x] TypeScript / Jest tests added
- [x] Duplicate Zod imports removed
"""

with open('list.md', 'w') as f:
    f.write(text)
