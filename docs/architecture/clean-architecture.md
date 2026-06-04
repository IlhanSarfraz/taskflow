# Clean Architecture in TaskFlow LMS

## Overview

TaskFlow LMS follows **Clean Architecture principles** to ensure separation of concerns and long-term maintainability.

---

## Core Principle

> The inner layers must not depend on outer layers.

Dependency flow:

Domain → Application → Infrastructure → API


---

## Layers Explained

### Domain Layer (Core)
- Entities
- Value Objects
- Business rules
- No external dependencies

Example:
- User
- Course
- Enrollment

---

### Application Layer
- Use cases (CQRS)
- Interfaces
- Business workflows
- DTO mapping

Uses:
- MediatR
- FluentValidation

---

### Infrastructure Layer
- Email services
- File storage
- External APIs

Implements interfaces defined in Application

---

### Persistence Layer
- EF Core DbContext
- Database mappings
- Repository implementations

---

### API Layer
- Controllers only
- Request routing
- Authentication middleware

---

## Why Clean Architecture

- Independent of frameworks
- Easy to test
- Easy to scale
- Supports long-term enterprise growth

---

## CQRS Pattern

We separate:
- Commands → write operations
- Queries → read operations

Benefits:
- Clear separation of responsibilities
- Better scalability
- Easier optimization of read/write models

---

## MediatR Usage

Used to:
- Decouple controllers from business logic
- Implement CQRS handlers cleanly

---

## Key Rule

> Controllers must NEVER contain business logic.

All logic flows:

Controller → MediatR → Handler → Domain