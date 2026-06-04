# Backend Architecture — TaskFlow LMS

## Overview

The backend of TaskFlow LMS is built using **.NET 9 Web API** following **Clean Architecture principles** with strong separation of concerns.

The goal is to ensure:
- Maintainability
- Testability
- Scalability
- Framework independence (Domain-first design)

---

## Architecture Style

We are using:

- Clean Architecture
- Onion Architecture principles
- CQRS (Command Query Responsibility Segregation)
- Mediator Pattern (via MediatR)

---

## Project Structure
TaskFlow.Api
TaskFlow.Application
TaskFlow.Domain
TaskFlow.Infrastructure
TaskFlow.Persistence
TaskFlow.Contracts
TaskFlow.Tests


---

## Layer Responsibilities

### 1. API Layer (TaskFlow.Api)
- Entry point of the application
- Contains controllers only
- No business logic
- Handles HTTP concerns

### 2. Application Layer
- Business logic (Use Cases)
- CQRS Handlers (Commands / Queries)
- DTO mapping
- Interfaces for services

### 3. Domain Layer
- Core business entities
- Business rules
- No external dependencies

### 4. Infrastructure Layer
- External services (Email, File storage, etc.)
- Third-party integrations

### 5. Persistence Layer
- EF Core DbContext
- Repositories implementation
- Database configuration (SQLite for dev)

### 6. Contracts Layer
- Shared DTOs
- Request/Response models

---

## Key Design Decisions

- MediatR used for CQRS implementation
- EF Core used for ORM
- SQLite used for local development (no SQL Server dependency constraints)
- Dependency Injection used across all layers

---

## Authentication

- JWT-based authentication
- Stateless API design
- Role-based access control planned

---

## Future Enhancements

- SignalR for real-time updates
- Redis caching layer
- Microservice decomposition (future phase)