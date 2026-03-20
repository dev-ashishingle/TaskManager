# ADR 001 — Clean Architecture

## Status
Accepted

## Context
Needed an architecture that would keep business logic testable and 
independent of frameworks, databases, and delivery mechanisms.

## Decision
Adopted Clean Architecture with four layers: Domain, Application, 
Infrastructure, and API. Dependencies point inward only — Domain 
has zero external dependencies.

## Consequences
- Domain and Application layers are fully unit testable without 
  spinning up a database or HTTP server
- Swapping SQL Server for another database only requires changes 
  in Infrastructure — nothing else changes
- Slightly more initial setup compared to a simple MVC project