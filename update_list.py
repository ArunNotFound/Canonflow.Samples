with open('list.md', 'r') as f:
    content = f.read()
content += """
### 12. Wecar App (`wecar-core`)
- [x] SOTA Hybrid Approach (DB-First + FsAssay DDD)
- [x] Complex Chat DB Schema (Users, Groups, Messages)
- [x] CanonFlow FSA Extraction
- [x] FsAssay Property Tests for strict types
"""
with open('list.md', 'w') as f:
    f.write(content)
