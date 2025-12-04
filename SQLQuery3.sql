-- Скрипт для создания тестовых данных: StudyProgram и StudentGroup
-- Выполнять на базе данных SQL Server

-- Сначала создаем тестовую программу обучения (если её ещё нет)
IF NOT EXISTS (SELECT 1 FROM StudyPrograms WHERE Institution = N'Тестовый Университет')
BEGIN
    INSERT INTO StudyPrograms (Institution, Faculty, Direction, Speciality)
    VALUES (
        N'Тестовый Университет',
        N'Факультет Информационных Технологий',
        N'09.03.01 Информатика и вычислительная техника',
        N'Программная инженерия'
    );
    PRINT 'Тестовая программа обучения создана';
END
ELSE
BEGIN
    PRINT 'Тестовая программа обучения уже существует';
END

-- Получаем Id созданной программы
DECLARE @StudyProgramId INT;
SELECT @StudyProgramId = Id FROM StudyPrograms WHERE Institution = N'Тестовый Университет';

-- Создаем тестовую группу
IF NOT EXISTS (SELECT 1 FROM StudentGroups WHERE Name = N'ИТ-101' AND StudyProgramId = @StudyProgramId)
BEGIN
    INSERT INTO StudentGroups (Name, Number, StudyProgramId)
    VALUES (
        N'ИТ-101',
        101,
        @StudyProgramId
    );
    PRINT 'Тестовая группа ИТ-101 создана';
END
ELSE
BEGIN
    PRINT 'Тестовая группа ИТ-101 уже существует';
END

-- Проверка результатов
SELECT 
    sg.Id AS GroupId,
    sg.Name AS GroupName,
    sg.Number AS GroupNumber,
    sp.Institution,
    sp.Faculty,
    sp.Direction,
    sp.Speciality
FROM StudentGroups sg
INNER JOIN StudyPrograms sp ON sg.StudyProgramId = sp.Id
WHERE sg.Name = N'ИТ-101';
