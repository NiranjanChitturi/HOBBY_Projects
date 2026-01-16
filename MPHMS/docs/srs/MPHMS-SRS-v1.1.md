📄 Software Requirement Specification (SRS)
________________________________________
Project Name
Modular Productivity & Habit Management System (MPHMS)
________________________________________
1. Introduction
________________________________________
1.1 Purpose
This Software Requirement Specification (SRS) defines the complete, precise, measurable, and verifiable software requirements for the Modular Productivity & Habit Management System (MPHMS).
This document serves as the formal contract between business stakeholders and the technical implementation team and will be used for:
•	System architecture and design
•	Database schema modeling
•	API contract definition
•	Development and testing
•	Acceptance validation
________________________________________
1.2 Scope
MPHMS is a self-hosted, modular, web-based productivity system that enables users to:
•	Track habits with flexible schedules
•	Track goals with milestones and deadlines
•	Record rest days and planned breaks
•	Capture structured reasons for missed activities
•	Analyze productivity trends through reports and dashboards
•	Receive local notifications
The system shall be implemented using:
•	ASP.NET Core (C#)
•	SQL Server
•	RESTful APIs
•	JWT-based authentication
The system shall operate without any cloud services or third-party SaaS dependencies.
________________________________________
1.3 Definitions, Acronyms & Terminology
Term	Definition
Habit	A recurring activity tracked on a schedule
Habit Log	A dated record of habit completion or skip
Goal	A measurable objective with defined outcomes
Milestone	A checkpoint within a goal
Streak	Consecutive successful habit completions
Skip	Non-completion of a scheduled habit
Skip Reason	Categorized explanation for a skip
Rest Day	User-defined non-penalty tracking day
Holiday	Planned exclusion period
JWT	JSON Web Token
RBAC	Role-Based Access Control
API	Application Programming Interface
________________________________________
1.4 References
•	Business Requirement Document (BRD) – MPHMS v1.1
________________________________________
2. Overall Description
________________________________________
2.1 Product Perspective
MPHMS is a standalone modular platform designed using layered architecture principles:
Presentation Layer (Web UI)
API Layer (REST)
Application Layer (Business Logic)
Domain Layer (Core Rules)
Infrastructure Layer (SQL Server, SMTP)
Each module shall be independently extensible without breaking existing functionality.
________________________________________
2.2 User Classes and Characteristics
User Class	Characteristics
End User	Tracks habits and goals, views analytics
Admin	Configures system and manages users
System Process	Executes background jobs and notifications
________________________________________
2.3 Operating Environment
Component	Requirement
Server OS	Windows / Linux
Web Server	IIS / Kestrel
Database	SQL Server 2019+
Client	Modern web browsers
Network	Internet or intranet supported
________________________________________
2.4 Design & Implementation Constraints
The system must:
•	Use ASP.NET Core (C#)
•	Use SQL Server as the primary datastore
•	Be self-hosted
•	Operate without cloud dependencies
•	Function in intranet and offline environments
•	Avoid third-party SaaS platforms
________________________________________
2.5 Assumptions & Dependencies
•	SMTP server is available if email notifications are enabled
•	Users have access to modern browsers
•	Phase 1 is backend-first implementation
•	No mobile applications in Phase 1
________________________________________
3. Functional Requirements
________________________________________
3.1 User Management Module
________________________________________
3.1.1 Registration
FR-UM-01: The system shall allow users to register using a unique email address.
FR-UM-02: The system shall enforce password complexity rules.
FR-UM-03: The system shall store passwords using salted hashing.
________________________________________
3.1.2 Authentication & Authorization
FR-UM-04: The system shall authenticate users using JWT tokens.
FR-UM-05: The system shall invalidate expired tokens.
FR-UM-06: The system shall enforce role-based access to APIs.
________________________________________
3.1.3 Profile Management
FR-UM-07: Users shall view and update profile information.
FR-UM-08: Users shall securely change passwords.
FR-UM-09: Users shall deactivate their account.
FR-UM-10: Users shall configure notification preferences.
________________________________________
3.2 Habit Management Module
________________________________________
3.2.1 Habit Definition
FR-HB-01: Users shall create habits with name, category, and difficulty.
FR-HB-02: Habits shall support Active, Paused, and Archived states.
FR-HB-03: Users shall edit habit definitions.
________________________________________
3.2.2 Scheduling Rules
FR-HB-04: Habits shall support daily schedules.
FR-HB-05: Habits shall support weekly schedules with selected days.
FR-HB-06: Habits shall support custom recurrence patterns.
________________________________________
3.2.3 Habit Tracking
FR-HB-07: Users shall mark habits as completed for a given date.
FR-HB-08: The system shall allow marking a habit as skipped.
FR-HB-09: A skip shall require a skip reason selection.
FR-HB-10: The system shall calculate streaks automatically.
FR-HB-11: Rest days and holidays shall not break streaks.
________________________________________
3.2.4 Notes & Reflections
FR-HB-12: Users may add notes per habit log entry.
FR-HB-13: Notes shall be timestamped and immutable after submission.
________________________________________
3.3 Goal Management Module
________________________________________
3.3.1 Goal Definition
FR-GL-01: Users shall create goals with title, description, and priority.
FR-GL-02: Goals shall support short-term and long-term classification.
FR-GL-03: Goals shall support deadline tracking.
________________________________________
3.3.2 Milestones
FR-GL-04: Users shall define milestones per goal.
FR-GL-05: Milestones shall have target dates.
FR-GL-06: Milestone completion shall update overall goal progress.
________________________________________
3.3.3 Goal Tracking
FR-GL-07: The system shall calculate progress percentage automatically.
FR-GL-08: Users shall view goal progress history.
FR-GL-09: Goal status changes shall be audit logged.
________________________________________
3.4 Holiday & Rest Day Management
________________________________________
FR-HD-01: Users shall define vacation periods.
FR-HD-02: Users shall define sick days.
FR-HD-03: Users shall define recurring rest days.
FR-HD-04: Admins shall define global holidays.
FR-HD-05: Holidays shall exclude streak penalties.
FR-HD-06: Notifications shall pause during holidays.
________________________________________
3.5 Skip Reason Tracking
________________________________________
FR-SK-01: The system shall maintain predefined skip reason categories.
FR-SK-02: Users shall select exactly one skip reason per skip event.
FR-SK-03: Users may add descriptive notes.
FR-SK-04: Skip reason data shall be available for analytics and reporting.
________________________________________
3.6 Notification Module
________________________________________
FR-NT-01: The system shall generate daily habit reminders.
FR-NT-02: The system shall generate goal deadline alerts.
FR-NT-03: The system shall generate weekly productivity summaries.
FR-NT-04: Notifications shall support in-app delivery.
FR-NT-05: Email notification delivery shall be configurable.
________________________________________
3.7 Analytics & Reporting Module
________________________________________
FR-AR-01: The system shall calculate habit completion rates.
FR-AR-02: The system shall calculate streak trends.
FR-AR-03: The system shall generate goal progress reports.
FR-AR-04: The system shall generate monthly productivity summaries.
FR-AR-05: The system shall report skip reason distribution.
FR-AR-06: The system shall support historical trend analysis.
________________________________________
3.8 Admin Management Module
________________________________________
FR-AD-01: Admins shall view and manage users.
FR-AD-02: Admins shall enable or disable feature access.
FR-AD-03: Admins shall manage global holidays.
FR-AD-04: Admins shall configure system settings.
FR-AD-05: Admins shall view system activity logs.
________________________________________
4. External Interface Requirements
________________________________________
4.1 User Interface
•	Web-based responsive UI
•	Dashboard-centric navigation
•	Role-based access views
________________________________________
4.2 API Interface
•	RESTful endpoints
•	JSON request/response format
•	Secured via JWT
•	Versioned APIs supported
________________________________________
4.3 Database Interface
•	SQL Server relational schema
•	Indexed transactional tables
•	Aggregation tables for analytics
•	Referential integrity enforced
________________________________________
5. Non-Functional Requirements
________________________________________
5.1 Performance
NFR-01: API response time < 300ms (P95)
NFR-02: Report generation time < 5 seconds
________________________________________
5.2 Security
NFR-03: JWT authentication mandatory
NFR-04: Encrypted password storage
NFR-05: Role-based authorization
NFR-06: HTTPS communication enforced
________________________________________
5.3 Reliability & Availability
NFR-07: Habit tracking accuracy ≥ 99%
NFR-08: Graceful failure handling and recovery
________________________________________
5.4 Maintainability
NFR-09: Modular architecture enforced
NFR-10: Clean separation of concerns
NFR-11: Code documentation mandatory
________________________________________
5.5 Scalability
NFR-12: Support future modules without core refactoring
NFR-13: Support analytical data growth efficiently
________________________________________
6. Data Requirements (High-Level)
Core entities:
•	User
•	Role
•	Habit
•	HabitSchedule
•	HabitLog
•	Goal
•	Milestone
•	SkipReason
•	Holiday
•	Notification
•	AnalyticsSummary
•	AuditLog
________________________________________
7. Traceability Matrix (Excerpt)
BRD Objective	SRS Section
Habit Tracking	3.2
Goal Management	3.3
Behavioral Analytics	3.5, 3.7
Holiday System	3.4
Notifications	3.6
Admin Controls	3.8
________________________________________
✅ 8. Approval & Sign-Off
This Software Requirement Specification (SRS) document has been reviewed and approved by the relevant stakeholders.
Upon approval, this document becomes the official baseline for system design, development, testing, and implementation activities.
Any future changes to the requirements must follow a formal change management and version control process.
________________________________________
Approval Authority
 Role	Name	Signature	Date
Product Owner	__________________	__________________	__________
Technical Architect	__________________	__________________	__________
Development Lead	__________________	__________________	__________
QA / Validation Lead	__________________	__________________	__________

________________________________________
Change Control Policy
•	All requirement changes must be documented as SRS revisions
•	Each revision must include:
o	Version number
o	Change summary
o	Approval confirmation
•	Unapproved changes shall not be included in production releases
________________________________________
Baseline Status
Attribute	Value
Document Version	SRS v1.1
Status	Approved Baseline (Pending Sign-off)
Effective Date	Upon Stakeholder Approval
Applicable Release	MPHMS Phase 1


✅ Final Architect Review Status
Your SRS is now:
✔ IEEE-style structured
✔ Traceable to BRD
✔ Implementation-ready
✔ Test-case friendly
✔ Portfolio and interview ready

