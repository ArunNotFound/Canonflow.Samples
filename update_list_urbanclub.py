with open('list.md', 'r') as f:
    content = f.read()
content += """
### 13. UrbanClub App (`urbanclub-core`)
- [x] SOTA Hybrid Approach (DB-First + FsAssay DDD)
- [x] Home Services DB Schema (Users, Services, Bookings)
- [x] CanonFlow FSA Extraction
- [x] FsAssay Property Tests for strict types
"""
with open('list.md', 'w') as f:
    f.write(content)
