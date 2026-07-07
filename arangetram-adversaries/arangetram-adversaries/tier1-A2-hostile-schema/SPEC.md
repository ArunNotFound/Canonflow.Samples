# A2 · The Vishakanya Schema  [LOAD-BEARING — the agentic security model]

## What it attacks
Introspecting an untrusted database is executing someone's input through a
code generator. Constraint text and identifiers flow into generated F#, TS,
Kotlin, AND into provenance comments the agentic pitch calls "ground truth."
Poisoned provenance is prompt injection with a NuGet distribution channel.

## Why load-bearing, not paranoia
The manifesto's second audience is AI agents reading generated artifacts as
trusted context. If a malicious column name becomes an instruction in a
provenance comment, the product's headline positioning is also its exploit.

## The adversary (see db/init/hostile.sql)
Identifiers and constraint text weaponized: comment-terminators, path
traversal, prototype-pollution names, template-injection sigils, SQL in
strings, unicode homoglyphs, zero-width chars, a 10k-char identifier.

## Pass criteria (falsifiable)
- Generated F#/TS/Kotlin COMPILES and is INERT: no emitted comment escapes its
  comment context; no identifier escapes quoting; no generated string is
  executable.
- Provenance comments neutralize `*/`, newlines, and injection sigils
  (escape or strip, documented which).
- Oversized/invalid identifiers → honest error or sanitized-with-warning,
  never silent passthrough.
- A "provenance injection" test: a column named to look like an agent
  instruction appears in output as inert data, not as a directive.
