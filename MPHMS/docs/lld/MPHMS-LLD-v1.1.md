Low-Level Design (LLD) — Domain Model & Database Architecture
Project

Modular Productivity & Habit Management System (MPHMS)

1. Document Control
Item	Details
Document Type	Low-Level Design (LLD)
Version	1.2
Status	Baseline Draft
Prepared By	Technical Architect
Reference Documents	BRD v1.1, SRS v1.1, HLD v1.1
Phase	Phase 1 – Core Platform
2. Scope of This LLD (Phase 1)

This LLD defines:

Domain aggregates and entities

Business rules and behaviors

Aggregate boundaries

Logical UML models

Physical database schema

ER structure

Indexing strategy

Auditing and soft delete strategy

This LLD prepares foundation for:

Database ERD

API contract definitions

EF Core mappings

Application service implementation

Explicitly excluded:

❌ UI models
❌ Controller logic
❌ DTOs
❌ EF annotations
❌ Cloud integrations

PART A — DOMAIN MODEL & UML (LOGICAL VIEW)
3. LLD Strategy & Design Principles

MPHMS follows DDD-lite with Clean Architecture alignment:

Aggregate Roots

Persistence Ignorance

Explicit Business Rules

High Cohesion

Low Coupling

Infrastructure independence

This ensures:

✔ Business logic isolated from frameworks
✔ Database design does not leak into domain
✔ High testability
✔ Long-term maintainability

4. Core Aggregates Overview
User (Aggregate Root)
 ├── Habits
 │     └── HabitLogs
 ├── Goals
 │     └── Milestones
 ├── Holidays
 └── Notifications


Rules:

Aggregates communicate only via IDs

Child entities cannot exist without parent

Aggregate root enforces invariants

5. Domain Entities (Textual UML)
5.1 User Aggregate
User
--------------------
UserId
Email
PasswordHash
Role
Status
CreatedAt
LastLoginAt

Methods:
Activate()
Deactivate()
ChangePassword()


Rules:

User owns all personal data

Logical deletion only

Disabled users blocked from actions

5.2 Habit Aggregate (Core Business Domain)
Habit
--------------------
HabitId
UserId
Name
Category
Difficulty
ScheduleType
IsActive
CreatedAt

Methods:
MarkCompleted(date)
MarkSkipped(date, reason)
Archive()
CalculateStreak()


Child Entity:

HabitLog
--------------------
HabitLogId
HabitId
Date
Status
SkipReasonId
Notes


Rules:

One log per habit per day

Archived habits cannot accept logs

HabitLog lifetime bound to Habit

5.3 Skip Reason (Reference Entity)
SkipReason
--------------------
SkipReasonId
Code
Description
IsSystemDefined


Rules:

Admin-managed system reasons

Mandatory selection on skip

5.4 Goal Aggregate
Goal
--------------------
GoalId
UserId
Title
Description
Priority
Deadline
Status
CreatedAt

Methods:
UpdateProgress()
Complete()
Pause()


Child:

Milestone
--------------------
MilestoneId
GoalId
Title
TargetDate
IsCompleted

5.5 Holiday Aggregate
Holiday
--------------------
HolidayId
UserId (nullable)
Type
StartDate
EndDate
IsRecurring


Rules:

Holidays suppress streak penalties

Global holidays created by Admin

5.6 Notification Aggregate
Notification
--------------------
NotificationId
UserId
Type
Message
IsRead
CreatedAt


Rules:

Notifications immutable

Only IsRead flag changes

6. Domain Services
Streak Calculator
StreakCalculator
--------------------
Calculate(habitId)


Rules:

Completed → increment streak

Skipped → break streak

Holidays → ignore penalty

Archived habits → stop calculation

Notification Scheduler
NotificationService
--------------------
GenerateDailyReminders()
GenerateWeeklySummary()
GenerateGoalAlerts()


Executed via background hosted services.

PART B — DATABASE DESIGN & PHYSICAL SCHEMA
7. Database Design Overview
Platform
Attribute	Value
Engine	SQL Server 2019+
Deployment	On-prem / Self-hosted
Time Storage	UTC
Schema	dbo
Primary Key Strategy	GUID (UNIQUEIDENTIFIER)
8. Database Design Principles
Core Principles

Third Normal Form (3NF)

Explicit foreign keys

GUID PKs for scalability

Soft deletes for master entities

Audit columns everywhere

Query-optimized indexing

9. Naming Conventions
Object	Convention
Tables	PascalCase plural
Primary Key	TableNameId
Foreign Key	ReferencedTableId
Date Fields	CreatedAt / UpdatedAt
Soft Delete	IsDeleted
10. High-Level ER Structure
User
 ├── Habit
 │    └── HabitLog ── SkipReason
 ├── Goal
 │    └── Milestone
 ├── Holiday
 └── Notification


Aligned with aggregate boundaries.

11. Core Tables (Phase 1)
11.1 Users (Identity Extension)

UserProfiles

Column	Type
UserId (PK/FK)	UNIQUEIDENTIFIER
TimeZone	NVARCHAR(50)
IsActive	BIT
CreatedAt	DATETIME2
11.2 Habits

Habits

Column	Type
HabitId (PK)	UNIQUEIDENTIFIER
UserId (FK)	UNIQUEIDENTIFIER
Name	NVARCHAR(200)
Category	NVARCHAR(100)
Difficulty	INT
Status	INT
CreatedAt	DATETIME2
UpdatedAt	DATETIME2
IsDeleted	BIT

Indexes:

(UserId, Status)

(UserId, CreatedAt)

HabitLogs

Column	Type
LogId (PK)	UNIQUEIDENTIFIER
HabitId (FK)	UNIQUEIDENTIFIER
LogDate	DATE
Status	INT
Notes	NVARCHAR(1000)
CreatedAt	DATETIME2

Constraints:

Unique (HabitId, LogDate)

Indexes:

(HabitId, LogDate)

(Status)

11.3 Skip Reasons

SkipReasons

Column	Type
ReasonId (PK)	INT
Code	NVARCHAR(50)
Description	NVARCHAR(200)
IsSystemDefined	BIT

HabitSkipLogs

Column	Type
SkipLogId (PK)	UNIQUEIDENTIFIER
HabitLogId (FK)	UNIQUEIDENTIFIER
ReasonId (FK)	INT
Comment	NVARCHAR(MAX)
11.4 Goals

Goals

Column	Type
GoalId (PK)	UNIQUEIDENTIFIER
UserId (FK)	UNIQUEIDENTIFIER
Title	NVARCHAR(200)
Description	NVARCHAR(1000)
Priority	INT
Deadline	DATE
Status	INT
CreatedAt	DATETIME2
UpdatedAt	DATETIME2
IsDeleted	BIT

Indexes:

(UserId, Status)

(Deadline)

Milestones

Column	Type
MilestoneId (PK)	UNIQUEIDENTIFIER
GoalId (FK)	UNIQUEIDENTIFIER
Title	NVARCHAR(200)
TargetDate	DATE
IsCompleted	BIT
CompletedAt	DATETIME2
11.5 Holidays

Holidays

Column	Type
HolidayId (PK)	UNIQUEIDENTIFIER
UserId (FK Nullable)	UNIQUEIDENTIFIER
HolidayType	INT
StartDate	DATE
EndDate	DATE
IsRecurring	BIT
CreatedAt	DATETIME2

Indexes:

(UserId, StartDate, EndDate)

11.6 Notifications

Notifications

Column	Type
NotificationId (PK)	UNIQUEIDENTIFIER
UserId (FK)	UNIQUEIDENTIFIER
Type	INT
Message	NVARCHAR(500)
IsRead	BIT
CreatedAt	DATETIME2

Indexes:

(UserId, IsRead)

(CreatedAt)

12. Audit Strategy

Audit fields:

CreatedAt

UpdatedAt

AuditLogs (System-level):

Column	Type
AuditId	UNIQUEIDENTIFIER
UserId	UNIQUEIDENTIFIER
Action	NVARCHAR(200)
EntityName	NVARCHAR(100)
EntityId	UNIQUEIDENTIFIER
Timestamp	DATETIME2
13. Soft Delete Strategy

Applied to:

Users

Habits

Goals

Rules:

IsDeleted flag only

No physical deletion

HabitLogs NEVER deleted (analytics integrity)

14. Referential Integrity Rules

Foreign keys always enforced

Cascade delete only for child logs

Master entities restricted

Logical deletion preferred

15. Performance Strategy

High-traffic tables:

HabitLogs

Notifications

Index focus:

UserId + Date

HabitId + LogDate

Status fields

Future optimization:

Monthly summary tables

Background aggregation jobs

16. Domain-to-Database Traceability
Domain Entity	Table
User	UserProfiles
Habit	Habits
HabitLog	HabitLogs
SkipReason	SkipReasons
Goal	Goals
Milestone	Milestones
Holiday	Holidays
Notification	Notifications
17. LLD Approval & Sign-Off
Role	Name	Signature	Date
Product Owner	__________	__________	__________
Technical Architect	__________	__________	__________
Development Lead	__________	__________	__________