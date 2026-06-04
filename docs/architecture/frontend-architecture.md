# Frontend Architecture — Angular LMS

## Overview

The frontend is built using **Angular 20+ standalone components architecture**.

It follows a modular feature-driven structure designed for scalability and maintainability.

---

## Architecture Style

- Feature-based modular architecture
- Standalone components (no NgModules for features)
- Lazy loading via routes
- Reactive programming (RxJS)

---

## Folder Structure

core/
shared/
features/
layouts/
models/
services/


---

## Folder Responsibilities

### Core
- Singleton services
- Guards (AuthGuard, RoleGuard)
- Interceptors (JWT, Error handling)

### Shared
- Reusable UI components
- Pipes
- Directives

### Features
Each business module:
- auth/
- dashboard/
- courses/
- users/
- assignments/

Each feature contains:
- components
- services
- routes

---

## State Management

- RxJS BehaviorSubjects (initial phase)
- Future: NgRx (if complexity increases)

---

## API Communication

- Angular HttpClient
- Central API service layer
- Interceptors for:
  - JWT injection
  - Error handling

---

## Authentication Flow

- JWT stored in localStorage (initial phase)
- Route guards protect private routes
- Token validation handled via API middleware

---

## UI Strategy

- Component-driven UI
- PrimeNG used for UI components
- Responsive layout system

---

## Future Improvements

- NgRx state management
- Micro frontend support (optional future)
- SignalR integration for live updates