# ADR 002 — Repository and Unit of Work Patterns

## Status
Accepted

## Context
Services needed to interact with the database without being coupled 
to EF Core directly. Multiple related operations needed to commit 
atomically.

## Decision
Implemented Repository pattern behind interfaces defined in Domain.
Wrapped repositories in a Unit of Work so multiple operations share 
one DbContext and commit together via a single SaveChangesAsync().

## Consequences
- Services are fully mockable in unit tests — no real DB needed
- EF Core is an implementation detail — services never reference it
- All related operations succeed or fail together