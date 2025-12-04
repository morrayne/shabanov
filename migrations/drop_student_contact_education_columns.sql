-- Drop ContactInfoId and EducationId columns from dbo.Students if they exist
-- IMPORTANT: make a database backup before running this script.
SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- Drop FK constraint(s) on ContactInfoId in Students (if any)
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Students') AND name = 'ContactInfoId')
BEGIN
    DECLARE @fkName nvarchar(200);
    SELECT TOP (1) @fkName = fk.name
    FROM sys.foreign_keys fk
    JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
    JOIN sys.columns c ON fkc.parent_object_id = c.object_id AND fkc.parent_column_id = c.column_id
    WHERE fk.parent_object_id = OBJECT_ID('dbo.Students') AND c.name = 'ContactInfoId';

    IF @fkName IS NOT NULL
    BEGIN
        PRINT 'Dropping FK constraint ' + @fkName + ' on dbo.Students(ContactInfoId)';
        EXEC(N'ALTER TABLE dbo.Students DROP CONSTRAINT [' + @fkName + ']');
    END

    PRINT 'Dropping column dbo.Students.ContactInfoId';
    ALTER TABLE dbo.Students DROP COLUMN ContactInfoId;
END
ELSE
    PRINT 'Column ContactInfoId does not exist on dbo.Students - skipping.';

-- Drop FK constraint(s) on EducationId in Students (if any)
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Students') AND name = 'EducationId')
BEGIN
    DECLARE @fkName2 nvarchar(200);
    SELECT TOP (1) @fkName2 = fk.name
    FROM sys.foreign_keys fk
    JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
    JOIN sys.columns c ON fkc.parent_object_id = c.object_id AND fkc.parent_column_id = c.column_id
    WHERE fk.parent_object_id = OBJECT_ID('dbo.Students') AND c.name = 'EducationId';

    IF @fkName2 IS NOT NULL
    BEGIN
        PRINT 'Dropping FK constraint ' + @fkName2 + ' on dbo.Students(EducationId)';
        EXEC(N'ALTER TABLE dbo.Students DROP CONSTRAINT [' + @fkName2 + ']');
    END

    PRINT 'Dropping column dbo.Students.EducationId';
    ALTER TABLE dbo.Students DROP COLUMN EducationId;
END
ELSE
    PRINT 'Column EducationId does not exist on dbo.Students - skipping.';

COMMIT TRANSACTION;
PRINT 'Done.';
