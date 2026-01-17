SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

------------------------------------------------------------
-- MPHMS PHASE 1 DATABASE SCHEMA (FINAL v1.2)
------------------------------------------------------------

------------------------------------------------------------
-- USER TYPE (Profession Classification)
------------------------------------------------------------

CREATE TABLE dbo.UserTypes (
    UserTypeId INT IDENTITY(1,1)
        CONSTRAINT PK_UserTypes PRIMARY KEY,

    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(200) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

CREATE UNIQUE INDEX UX_UserTypes_Name
ON dbo.UserTypes (Name);
GO

------------------------------------------------------------
-- ROLES & RBAC FOUNDATION
------------------------------------------------------------

CREATE TABLE dbo.Roles (
    RoleId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_Roles PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    Name NVARCHAR(50) NOT NULL,
    Description NVARCHAR(200) NULL,

    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsDeleted BIT NOT NULL DEFAULT 0
);

CREATE UNIQUE INDEX UX_Roles_Name
ON dbo.Roles (Name);
GO

CREATE TABLE dbo.UserRoles (
    UserRoleId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_UserRoles PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    UserId UNIQUEIDENTIFIER NOT NULL,
    RoleId UNIQUEIDENTIFIER NOT NULL,

    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_UserRoles_Roles
        FOREIGN KEY (RoleId)
        REFERENCES dbo.Roles (RoleId)
);
GO

------------------------------------------------------------
-- USER PROFILE
------------------------------------------------------------

CREATE TABLE dbo.UserProfiles (
    UserId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_UserProfiles PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    Email NVARCHAR(255) NOT NULL,
    UserTypeId INT NULL,

    IsActive BIT NOT NULL DEFAULT 1,
    TimeZone NVARCHAR(50) NULL,

    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_UserProfiles_UserTypes
        FOREIGN KEY (UserTypeId)
        REFERENCES dbo.UserTypes (UserTypeId)
);

CREATE UNIQUE INDEX UX_UserProfiles_Email
ON dbo.UserProfiles (Email);
GO

------------------------------------------------------------
-- LOOKUP TABLES
------------------------------------------------------------

CREATE TABLE dbo.HabitStatusLookup (StatusId INT PRIMARY KEY, Name NVARCHAR(50));
CREATE TABLE dbo.GoalStatusLookup (StatusId INT PRIMARY KEY, Name NVARCHAR(50));
CREATE TABLE dbo.NotificationTypeLookup (TypeId INT PRIMARY KEY, Name NVARCHAR(50));
CREATE TABLE dbo.HolidayTypeLookup (TypeId INT PRIMARY KEY, Name NVARCHAR(50));
CREATE TABLE dbo.DifficultyLevels (LevelId INT PRIMARY KEY, Name NVARCHAR(50));
GO

------------------------------------------------------------
-- CATEGORY MANAGEMENT
------------------------------------------------------------

CREATE TABLE dbo.HabitCategories (
    CategoryId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_HabitCategories PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    Name NVARCHAR(100) NOT NULL,
    DisplayOrder INT NOT NULL,

    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsDeleted BIT NOT NULL DEFAULT 0
);

CREATE UNIQUE INDEX UX_HabitCategories_Name
ON dbo.HabitCategories (Name);
GO

CREATE TABLE dbo.GoalCategories (
    CategoryId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_GoalCategories PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    Name NVARCHAR(100) NOT NULL,
    DisplayOrder INT NOT NULL,

    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsDeleted BIT NOT NULL DEFAULT 0
);
GO

------------------------------------------------------------
-- HABITS
------------------------------------------------------------

CREATE TABLE dbo.Habits (
    HabitId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_Habits PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    UserId UNIQUEIDENTIFIER NOT NULL,
    CategoryId UNIQUEIDENTIFIER NULL,

    Name NVARCHAR(200) NOT NULL,
    Difficulty INT NOT NULL,
    Status INT NOT NULL,

    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_Habits_UserProfiles
        FOREIGN KEY (UserId) REFERENCES dbo.UserProfiles (UserId),

    CONSTRAINT FK_Habits_Category
        FOREIGN KEY (CategoryId) REFERENCES dbo.HabitCategories (CategoryId)
);
GO

------------------------------------------------------------
-- HABIT SCHEDULE HISTORY (NEW - OPTION A)
------------------------------------------------------------

CREATE TABLE dbo.HabitScheduleHistory (
    HistoryId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_HabitScheduleHistory PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    HabitId UNIQUEIDENTIFIER NOT NULL,
    OldSchedule NVARCHAR(200) NOT NULL,
    NewSchedule NVARCHAR(200) NOT NULL,

    ChangedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_HabitScheduleHistory_Habits
        FOREIGN KEY (HabitId)
        REFERENCES dbo.Habits (HabitId)
);
GO

------------------------------------------------------------
-- HABIT LOGS
------------------------------------------------------------

CREATE TABLE dbo.HabitLogs (
    LogId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_HabitLogs PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    HabitId UNIQUEIDENTIFIER NOT NULL,
    LogDate DATE NOT NULL,
    Status INT NOT NULL,
    Notes NVARCHAR(1000) NULL,

    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_HabitLogs_Habits
        FOREIGN KEY (HabitId)
        REFERENCES dbo.Habits (HabitId),

    CONSTRAINT UQ_HabitLogs_Habit_Date
        UNIQUE (HabitId, LogDate)
);
GO

------------------------------------------------------------
-- SKIP REASONS
------------------------------------------------------------

CREATE TABLE dbo.SkipReasons (
    ReasonId INT IDENTITY(1,1)
        CONSTRAINT PK_SkipReasons PRIMARY KEY,

    Code NVARCHAR(50) NOT NULL,
    Description NVARCHAR(200) NOT NULL,

    IsSystemDefined BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0
);

CREATE UNIQUE INDEX UX_SkipReasons_Code
ON dbo.SkipReasons (Code);
GO

CREATE TABLE dbo.HabitSkipLogs (
    SkipLogId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_HabitSkipLogs PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    HabitLogId UNIQUEIDENTIFIER NOT NULL,
    ReasonId INT NOT NULL,
    Comment NVARCHAR(MAX) NULL,

    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_HabitSkipLogs_HabitLogs
        FOREIGN KEY (HabitLogId) REFERENCES dbo.HabitLogs (LogId),

    CONSTRAINT FK_HabitSkipLogs_SkipReasons
        FOREIGN KEY (ReasonId) REFERENCES dbo.SkipReasons (ReasonId)
);

CREATE UNIQUE INDEX UX_HabitSkipLogs_OnePerLog
ON dbo.HabitSkipLogs (HabitLogId);
GO

------------------------------------------------------------
-- GOALS
------------------------------------------------------------

CREATE TABLE dbo.Goals (
    GoalId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_Goals PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    UserId UNIQUEIDENTIFIER NOT NULL,
    CategoryId UNIQUEIDENTIFIER NULL,

    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000) NULL,

    Priority INT NOT NULL,
    Deadline DATE NULL,
    Status INT NOT NULL,

    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_Goals_UserProfiles
        FOREIGN KEY (UserId) REFERENCES dbo.UserProfiles (UserId),

    CONSTRAINT FK_Goals_Category
        FOREIGN KEY (CategoryId) REFERENCES dbo.GoalCategories (CategoryId)
);
GO

------------------------------------------------------------
-- PRODUCTIVITY SCORE SNAPSHOT (NEW - OPTION A)
------------------------------------------------------------

CREATE TABLE dbo.ProductivityScoreSnapshots (
    SnapshotId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_ProductivitySnapshots PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    UserId UNIQUEIDENTIFIER NOT NULL,
    Month INT NOT NULL,
    Year INT NOT NULL,

    Score DECIMAL(5,2) NOT NULL,
    CalculationVersion NVARCHAR(50) NOT NULL,

    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_ProductivitySnapshots_UserProfiles
        FOREIGN KEY (UserId)
        REFERENCES dbo.UserProfiles (UserId)
);

CREATE UNIQUE INDEX UX_Productivity_User_Period
ON dbo.ProductivityScoreSnapshots (UserId, Year, Month);
GO

------------------------------------------------------------
-- SYSTEM SETTINGS
------------------------------------------------------------

CREATE TABLE dbo.SystemSettings (
    SettingId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_SystemSettings PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    SettingKey NVARCHAR(100) NOT NULL,
    SettingValue NVARCHAR(MAX) NOT NULL,
    Description NVARCHAR(300) NULL,

    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsDeleted BIT NOT NULL DEFAULT 0
);

CREATE UNIQUE INDEX UX_SystemSettings_Key
ON dbo.SystemSettings (SettingKey);
GO

------------------------------------------------------------
-- EMAIL TEMPLATES
------------------------------------------------------------

CREATE TABLE dbo.EmailTemplates (
    TemplateId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_EmailTemplates PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    TemplateCode NVARCHAR(100) NOT NULL,
    Subject NVARCHAR(200) NOT NULL,
    BodyHtml NVARCHAR(MAX) NOT NULL,

    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsDeleted BIT NOT NULL DEFAULT 0
);

CREATE UNIQUE INDEX UX_EmailTemplates_Code
ON dbo.EmailTemplates (TemplateCode);
GO

------------------------------------------------------------
-- APPLICATION LOGGING
------------------------------------------------------------

CREATE TABLE dbo.ApplicationLogs (
    LogId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_ApplicationLogs PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    LogLevel NVARCHAR(20) NOT NULL,
    Source NVARCHAR(100) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,

    UserId UNIQUEIDENTIFIER NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsDeleted BIT NOT NULL DEFAULT 0
);
GO

------------------------------------------------------------
-- ERROR LOGGING
------------------------------------------------------------

CREATE TABLE dbo.ErrorLogs (
    ErrorId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_ErrorLogs PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    ExceptionType NVARCHAR(200) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    StackTrace NVARCHAR(MAX) NULL,

    Source NVARCHAR(200) NULL,
    UserId UNIQUEIDENTIFIER NULL,

    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsDeleted BIT NOT NULL DEFAULT 0
);
GO

------------------------------------------------------------
-- AUDIT LOGGING
------------------------------------------------------------

CREATE TABLE dbo.AuditLogs (
    AuditId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_AuditLogs PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    UserId UNIQUEIDENTIFIER NULL,
    Action NVARCHAR(200) NOT NULL,
    EntityName NVARCHAR(100) NOT NULL,
    EntityId UNIQUEIDENTIFIER NOT NULL,

    Timestamp DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsDeleted BIT NOT NULL DEFAULT 0
);
GO

------------------------------------------------------------
-- END MPHMS PHASE 1 FINAL SCHEMA
------------------------------------------------------------
