# ADR 003 — Result Pattern over Exceptions

## Status
Accepted

## Context
Services needed a way to communicate expected failures (not found, 
validation failed, conflict) back to controllers without using 
exceptions for flow control.

## Decision
Implemented a Result<T> type with Success/Failure factory methods. 
Services return Result<T> — they never throw for expected outcomes. 
Controllers map Result<T> to appropriate HTTP status codes.

## Consequences
- Service method signatures honestly communicate what can go wrong
- Unit tests can assert on Result without try/catch blocks
- Exception middleware only handles truly unexpected failures
- Controllers stay thin — no business logic leaks up