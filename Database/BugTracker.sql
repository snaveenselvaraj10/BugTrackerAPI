-- =============================================
-- Bug Tracker Database Full Setup Script
-- =============================================

-- Create Database
IF DB_ID('BugTrackerDB') IS NULL
BEGIN
    CREATE DATABASE BugTrackerDB;
END
GO

USE BugTrackerDB;
GO

-- =============================================
-- TABLES
-- =============================================

IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;
CREATE TABLE Users (
    UserId INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100),
    Email NVARCHAR(100)
);

IF OBJECT_ID('Bugs', 'U') IS NOT NULL DROP TABLE Bugs;
CREATE TABLE Bugs (
    BugId INT IDENTITY PRIMARY KEY,
    Title NVARCHAR(100),
    Description NVARCHAR(500),
    Severity NVARCHAR(20),
    Status NVARCHAR(20),
    CreatedBy INT,
    AssignedTo INT,
    CreatedDate DATETIME DEFAULT GETDATE()
);

IF OBJECT_ID('Comments', 'U') IS NOT NULL DROP TABLE Comments;
CREATE TABLE Comments (
    CommentId INT IDENTITY PRIMARY KEY,
    BugId INT,
    CommentText NVARCHAR(500),
    CreatedBy INT,
    CreatedDate DATETIME DEFAULT GETDATE()
);

IF OBJECT_ID('AuditLog', 'U') IS NOT NULL DROP TABLE AuditLog;
CREATE TABLE AuditLog (
    LogId INT IDENTITY PRIMARY KEY,
    BugId INT,
    OldStatus NVARCHAR(20),
    NewStatus NVARCHAR(20),
    ChangedDate DATETIME
);

-- =============================================
-- SAMPLE DATA
-- =============================================

INSERT INTO Users (Name, Email)
VALUES 
('Naveen', 'naveen@test.com'),
('Tester', 'tester@test.com');

-- =============================================
-- STORED PROCEDURE
-- =============================================

IF OBJECT_ID('sp_CreateBug', 'P') IS NOT NULL DROP PROCEDURE sp_CreateBug;
GO

CREATE PROCEDURE sp_CreateBug
    @Title NVARCHAR(100),
    @Description NVARCHAR(500),
    @Severity NVARCHAR(20),
    @CreatedBy INT
AS
BEGIN
    INSERT INTO Bugs (Title, Description, Severity, Status, CreatedBy, CreatedDate)
    VALUES (@Title, @Description, @Severity, 'Open', @CreatedBy, GETDATE());
END
GO

-- =============================================
-- FUNCTION
-- =============================================

IF OBJECT_ID('fn_OpenBugsByUser', 'FN') IS NOT NULL DROP FUNCTION fn_OpenBugsByUser;
GO

CREATE FUNCTION fn_OpenBugsByUser (@UserId INT)
RETURNS INT
AS
BEGIN
    DECLARE @Count INT;

    SELECT @Count = COUNT(*)
    FROM Bugs
    WHERE AssignedTo = @UserId AND Status != 'Closed';

    RETURN @Count;
END
GO

-- =============================================
-- TRIGGER
-- =============================================

IF OBJECT_ID('trg_BugStatusChange', 'TR') IS NOT NULL DROP TRIGGER trg_BugStatusChange;
GO

CREATE TRIGGER trg_BugStatusChange
ON Bugs
AFTER UPDATE
AS
BEGIN
    INSERT INTO AuditLog (BugId, OldStatus, NewStatus, ChangedDate)
    SELECT d.BugId, d.Status, i.Status, GETDATE()
    FROM deleted d
    INNER JOIN inserted i ON d.BugId = i.BugId
    WHERE d.Status <> i.Status;
END
GO

-- =============================================
-- TEST QUERY
-- =============================================

-- EXEC sp_CreateBug 'Test Bug', 'Sample issue', 'High', 1;
-- SELECT * FROM Bugs;
-- UPDATE Bugs SET Status = 'Closed' WHERE BugId = 1;
-- SELECT * FROM AuditLog;