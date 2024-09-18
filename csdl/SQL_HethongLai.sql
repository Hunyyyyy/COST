--Create database testchay;
--use  testchay
--Hệ thống lại Database
Create database TestNhiemVu;
Use TestNhiemVu;
--drop database TestNhiemVu
--use master;
--drop database testchay
Go;
--1
CREATE TABLE Users (
UserId INT PRIMARY KEY IDENTITY(1,1),
FullName NVARCHAR(100) NOT NULL,
Email NVARCHAR(100) NOT NULL,
PasswordHash NVARCHAR(100) NOT NULL,
PasswordSalt NVARCHAR(100) NOT NULL,
Role NVARCHAR(50) NOT NULL, -- Ví dụ: 'Manager', 'TeamLeader', 'Member'
CreatedAt DATETIME DEFAULT GETDATE()
);

--2
CREATE TABLE UserLogin (
UserId INT PRIMARY KEY FOREIGN KEY REFERENCES Users(UserId),
PasswordHash NVARCHAR(256) NOT NULL,
PasswordSalt NVARCHAR(256), -- Nếu sử dụng salt
LastLoginDate DATETIME,
FailedLoginAttempts INT DEFAULT 0
);

--3
CREATE TABLE Project (
ProjectId INT PRIMARY KEY IDENTITY(1,1),
ProjectName NVARCHAR(200) NOT NULL,
Description NVARCHAR(MAX),
StartDate DATETIME NOT NULL,
EndDate DATETIME,
Status NVARCHAR(50) DEFAULT 'Chưa bắt đầu', -- Ví dụ: 'Chưa bắt đầu', 'Đang thực hiện', 'Hoàn thành'
ManagerId INT FOREIGN KEY REFERENCES Users(UserId), -- Người quản lý dự án
CreatedAt DATETIME DEFAULT GETDATE(),
CreatedBy nvarchar(50),
Progress INT,
);

    --4
CREATE TABLE SaveTask(
TaskId INT PRIMARY KEY IDENTITY(1,1),
Title NVARCHAR(200) NOT NULL,
Description NVARCHAR(MAX),
DueDate DATETIME NOT NULL,
Priority NVARCHAR(50), -- Ví dụ: 'Cao', 'Trung bình', 'Thấp'
Status NVARCHAR(50) DEFAULT 'Đang chờ', -- Ví dụ: 'Đang chờ', 'Đang thực hiện', 'Hoàn thành'
AssignedTo INT FOREIGN KEY REFERENCES Users(UserId), -- Người được giao nhiệm vụ (nhóm trưởng)
CreatedBy INT FOREIGN KEY REFERENCES Users(UserId), -- Người tạo nhiệm vụ (quản lý)
CreatedAt DATETIME DEFAULT GETDATE(),
Note nvarchar(100),
ProjectId int foreign key REFERENCES  Project(ProjectId),
Progress INT
);
--5
CREATE TABLE SentTasksList (
TaskId INT PRIMARY KEY IDENTITY(1,1),
Title NVARCHAR(200) NOT NULL,
Description NVARCHAR(MAX),
DueDate DATETIME NOT NULL,
Priority NVARCHAR(50), -- Ví dụ: 'Cao', 'Trung bình', 'Thấp'
Status NVARCHAR(50) DEFAULT 'Đang chờ', -- Ví dụ: 'Đang chờ', 'Đang thực hiện', 'Hoàn thành'
CreatedBy INT FOREIGN KEY REFERENCES Users(UserId), -- Người tạo nhiệm vụ (quản lý)
CreatedAt DATETIME DEFAULT GETDATE(),
Note nvarchar(100),
ProjectId int foreign key REFERENCES  Project(ProjectId)
);
--6
CREATE TABLE Subtask (
SubtaskId INT PRIMARY KEY IDENTITY(1,1),
TaskId INT FOREIGN KEY REFERENCES SaveTask(TaskId) ON DELETE CASCADE,
Title NVARCHAR(200) NOT NULL,
Description NVARCHAR(MAX),
Status NVARCHAR(50) DEFAULT 'Chưa nhận', -- Ví dụ: 'Chưa nhận', 'Đang thực hiện', 'Hoàn thành'
AssignedTo INT FOREIGN KEY REFERENCES Users(UserId), -- Người được giao công việc con (thành viên)
CreatedAt DATETIME DEFAULT GETDATE(),
ProjectId int foreign key REFERENCES  Project(ProjectId)
);
--7
CREATE TABLE AssignedSubtask (
AssignedSubtaskId INT PRIMARY KEY IDENTITY(1,1),
SubtaskId INT FOREIGN KEY REFERENCES Subtask(SubtaskId) ON DELETE CASCADE, -- Liên kết với bảng Subtasks
AssignedTo INT FOREIGN KEY REFERENCES Users(UserId), -- Người nhận công việc phụ
Status NVARCHAR(50) DEFAULT 'Chưa nhận', -- Trạng thái công việc phụ (ví dụ: 'Chưa nhận', 'Đang thực hiện', 'Hoàn thành')
AssignedAt DATETIME DEFAULT GETDATE(), -- Thời điểm giao công việc
UpdatedAt DATETIME DEFAULT GETDATE(), -- Thời điểm cập nhật công việc
ProjectId int foreign key REFERENCES  Project(ProjectId),
MemberId int foreign key REFERENCES Users(UserId),
TaskId int foreign key REFERENCES  SaveTask(TaskId)
);
--8
CREATE TABLE SubtaskProgress (
SubtaskProgressId INT PRIMARY KEY IDENTITY(1,1),
SubtaskId INT FOREIGN KEY REFERENCES Subtask(SubtaskId) ON DELETE CASCADE,
AssignedTo INT FOREIGN KEY REFERENCES Users(UserId),
Progress DECIMAL(5, 2) NOT NULL DEFAULT 0, -- Tiến độ của công việc con (0.00 đến 100.00)
LastUpdatedAt DATETIME DEFAULT GETDATE()
);
--9
CREATE TABLE SubmittedSubtask (
SubmissionId INT PRIMARY KEY IDENTITY(1,1),
SubtaskId INT NOT NULL,
TaskId INT NOT NULL, -- ID của nhiệm vụ chứa công việc con
ProjectId INT NOT NULL, -- ID của dự án chứa nhiệm vụ
UserId INT NOT NULL,
SubmittedAt DATETIME NOT NULL DEFAULT GETDATE(),
Status NVARCHAR(50) DEFAULT 'Đang xem xét',
Notes NVARCHAR(MAX),
FilePath NVARCHAR(MAX)
FOREIGN KEY (SubtaskId) REFERENCES Subtask(SubtaskId) ,
FOREIGN KEY (TaskId) REFERENCES SaveTask(TaskId) ,
FOREIGN KEY (ProjectId) REFERENCES Project(ProjectId) ,
FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
--10
CREATE TABLE TaskProgress (
    TaskProgressId INT PRIMARY KEY IDENTITY(1,1),
    TaskId INT FOREIGN KEY REFERENCES SaveTask(TaskId) ON DELETE CASCADE,
    Progress DECIMAL(5, 2) NOT NULL DEFAULT 0, -- Tiến độ của nhiệm vụ chính
    LastUpdatedAt DATETIME DEFAULT GETDATE()
);
--11
CREATE TABLE TaskNotification (
NotificationId INT PRIMARY KEY IDENTITY(1,1),
UserId INT FOREIGN KEY REFERENCES Users(UserId), -- Người nhận thông báo
TaskId INT FOREIGN KEY REFERENCES SaveTask(TaskId) ON DELETE CASCADE,
NotificationMessage NVARCHAR(MAX),
IsRead BIT DEFAULT 0, -- 0: chưa đọc, 1: đã đọc
CreatedAt DATETIME DEFAULT GETDATE()
);
--12
CREATE TABLE Groups (
GroupId INT PRIMARY KEY IDENTITY(1,1),
GroupName NVARCHAR(100) NOT NULL,
CreatedBy INT FOREIGN KEY REFERENCES Users(UserId),
CreatedAt DATETIME DEFAULT GETDATE()
);
--13
CREATE TABLE GroupMember (
GroupMemberId INT PRIMARY KEY IDENTITY(1,1),
GroupId INT FOREIGN KEY REFERENCES Groups(GroupId) ON DELETE CASCADE,
UserId INT FOREIGN KEY REFERENCES Users(UserId)
);
   --14
CREATE TABLE ProjectMember (
ProjectMemberId INT PRIMARY KEY IDENTITY(1,1),
ProjectId INT FOREIGN KEY REFERENCES Project(ProjectId) ON DELETE CASCADE,
UserId INT FOREIGN KEY REFERENCES Users(UserId)
);
--15
CREATE TABLE ProjectTask (
ProjectTaskId INT PRIMARY KEY IDENTITY(1,1),
ProjectId INT FOREIGN KEY REFERENCES Project(ProjectId) ON DELETE CASCADE,
TaskId INT FOREIGN KEY REFERENCES SaveTask(TaskId) ON DELETE CASCADE
);

CREATE TABLE ProjectProgress (
    ProjectProgressId INT PRIMARY KEY IDENTITY(1,1),
    ProjectId INT FOREIGN KEY REFERENCES Project(ProjectId) ON DELETE CASCADE,
    Progress DECIMAL(5, 2) NOT NULL DEFAULT 0, -- Tiến độ của dự án
    LastUpdatedAt DATETIME DEFAULT GETDATE()
);
--16
CREATE TABLE ProjectNotification (
NotificationId INT PRIMARY KEY IDENTITY(1,1),
ProjectId INT FOREIGN KEY REFERENCES Project(ProjectId) ON DELETE CASCADE,
UserId INT FOREIGN KEY REFERENCES Users(UserId), -- Người nhận thông báo
NotificationMessage NVARCHAR(MAX),
IsRead BIT DEFAULT 0, -- 0: chưa đọc, 1: đã đọc
CreatedAt DATETIME DEFAULT GETDATE()
);
--17
CREATE TABLE ProjectUser (
ProjectUserId INT IDENTITY(1,1) PRIMARY KEY,
ProjectId INT NOT NULL,
UserId INT NOT NULL,
Role NVARCHAR(50) NOT NULL,
FOREIGN KEY (ProjectId) REFERENCES Project(ProjectId),
FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
--18
CREATE TABLE Reminders (
ReminderId INT PRIMARY KEY IDENTITY(1,1),
ProjectId INT NOT NULL,
UserId INT NOT NULL,
ReminderContent NVARCHAR(MAX) NOT NULL,
ReminderDate DATETIME NOT NULL,
IsAcknowledged BIT DEFAULT 0, -- 0 = chưa được xác nhận, 1 = đã xác nhận
CreatedAt DATETIME DEFAULT GETDATE(),
FOREIGN KEY (ProjectId) REFERENCES Project(ProjectId),
FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
--19
CREATE TABLE SubmittedSubtasksByManager (
SubmissionId INT PRIMARY KEY IDENTITY(1,1),
SubtaskId INT NOT NULL,
TaskId INT NOT NULL, -- ID của nhiệm vụ chứa công việc con
ProjectId INT NOT NULL, -- ID của dự án chứa nhiệm vụ
UserId INT NOT NULL,
SubmittedAt DATETIME NOT NULL DEFAULT GETDATE(),
Status NVARCHAR(50) DEFAULT 'Đã duyệt',
Notes NVARCHAR(MAX),
FilePath NVARCHAR(MAX),
    FOREIGN KEY (SubtaskId) REFERENCES Subtask(SubtaskId) ,
    FOREIGN KEY (TaskId) REFERENCES SaveTask(TaskId) ,
    FOREIGN KEY (ProjectId) REFERENCES Project(ProjectId) ,
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
--bảng project
--select*from Project
--alter table Project 
--drop column CreateBy 
--add CreatedBy nvarchar(50);

--bảng sentaskslist
--select *from SentTasksList
--alter table SentTasksList
--add Note nvarchar(100)
--alter table SentTasksList
--add ProjectId int foreign key REFERENCES  Project(ProjectId)

--select *from SaveTask
--alter table SaveTask
--add Note nvarchar(100)
--add Progress INT

--select *from Subtasks
--alter table Subtasks
--add ProjectId int foreign key REFERENCES  Project(ProjectId)
--select *from SaveTask
--alter table SaveTask
--add ProjectId int foreign key REFERENCES  Project(ProjectId)

--delete Project;
--delete SentTasksList
--select *from Project
--alter table Project
--add  Progress INT

--Alter table AssignedSubtask
--add ProjectId int foreign key REFERENCES  Project(ProjectId)
--add TaskId int foreign key REFERENCES  SaveTask(TaskId)

