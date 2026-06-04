# Database Schema — TaskFlow LMS

## Overview

Database is designed for simplicity using **SQLite (development)** and structured to support future migration to SQL Server or PostgreSQL.

---

## Core Tables (Planned)

### Users
- Id (GUID)
- Username
- Email
- PasswordHash
- Role
- CreatedAt

---

### Courses
- Id
- Title
- Description
- InstructorId
- CreatedAt

---

### Enrollments
- Id
- UserId
- CourseId
- EnrolledAt

---

### Tasks / Assignments
- Id
- CourseId
- Title
- Description
- DueDate

---

### Submissions
- Id
- AssignmentId
- UserId
- FileUrl
- SubmittedAt

---

## Relationships

- User → Enrollments (1:M)
- Course → Enrollments (1:M)
- Course → Assignments (1:M)
- Assignment → Submissions (1:M)

---

## EF Core Strategy

- Code First approach
- Migrations enabled
- DbContext in Persistence layer

---

## Future Enhancements

- Audit logs table
- Soft delete support
- Multi-tenant schema support