CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Role NVARCHAR(50) NOT NULL, -- Ví dụ: 'Manager', 'TeamLeader', 'Member'
    CreatedAt DATETIME DEFAULT GETDATE()
);
CREATE TABLE UserLogin (
    UserId INT PRIMARY KEY FOREIGN KEY REFERENCES Users(UserId),
    PasswordHash NVARCHAR(256) NOT NULL,
    PasswordSalt NVARCHAR(256), -- Nếu sử dụng salt
    LastLoginDate DATETIME,
    FailedLoginAttempts INT DEFAULT 0
);

CREATE TABLE SaveTasks (
    TaskId INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    DueDate DATETIME NOT NULL,
    Priority NVARCHAR(50), -- Ví dụ: 'Cao', 'Trung bình', 'Thấp'
    Status NVARCHAR(50) DEFAULT 'Đang chờ', -- Ví dụ: 'Đang chờ', 'Đang thực hiện', 'Hoàn thành'
    AssignedTo INT FOREIGN KEY REFERENCES Users(UserId), -- Người được giao nhiệm vụ (nhóm trưởng)
    CreatedBy INT FOREIGN KEY REFERENCES Users(UserId), -- Người tạo nhiệm vụ (quản lý)
    CreatedAt DATETIME DEFAULT GETDATE()
);
CREATE TABLE SentTasksList (
    TaskId INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    DueDate DATETIME NOT NULL,
    Priority NVARCHAR(50), -- Ví dụ: 'Cao', 'Trung bình', 'Thấp'
    Status NVARCHAR(50) DEFAULT 'Đang chờ', -- Ví dụ: 'Đang chờ', 'Đang thực hiện', 'Hoàn thành'
    CreatedBy INT FOREIGN KEY REFERENCES Users(UserId), -- Người tạo nhiệm vụ (quản lý)
    CreatedAt DATETIME DEFAULT GETDATE()
);
CREATE TABLE Subtasks (
    SubtaskId INT PRIMARY KEY IDENTITY(1,1),
    TaskId INT FOREIGN KEY REFERENCES Tasks(TaskId) ON DELETE CASCADE,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    Status NVARCHAR(50) DEFAULT 'Chưa nhận', -- Ví dụ: 'Chưa nhận', 'Đang thực hiện', 'Hoàn thành'
    AssignedTo INT FOREIGN KEY REFERENCES Users(UserId), -- Người được giao công việc con (thành viên)
    CreatedAt DATETIME DEFAULT GETDATE()
);

SELECT DISTINCT TaskId
FROM Subtasks
WHERE TaskId NOT IN (SELECT TaskId FROM SaveTasks);

-- Cập nhật bảng SaveTasks để thêm khóa ngoại liên kết với Projects
ALTER TABLE SaveTasks
ADD ProjectId INT FOREIGN KEY REFERENCES Projects(ProjectId);


CREATE TABLE TaskProgress (
    ProgressId INT PRIMARY KEY IDENTITY(1,1),
    SubtaskId INT FOREIGN KEY REFERENCES Subtasks(SubtaskId) ON DELETE CASCADE,
    UserId INT FOREIGN KEY REFERENCES Users(UserId), -- Người cập nhật tiến trình
    Status NVARCHAR(50), -- Ví dụ: 'Đang thực hiện', 'Hoàn thành'
    ProgressDate DATETIME DEFAULT GETDATE(),
    Notes NVARCHAR(MAX) -- Ghi chú, nếu có
);
CREATE TABLE TaskNotifications (
    NotificationId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(UserId), -- Người nhận thông báo
    TaskId INT FOREIGN KEY REFERENCES Tasks(TaskId) ON DELETE CASCADE,
    NotificationMessage NVARCHAR(MAX),
    IsRead BIT DEFAULT 0, -- 0: chưa đọc, 1: đã đọc
    CreatedAt DATETIME DEFAULT GETDATE()
);
CREATE TABLE Groups (
    GroupId INT PRIMARY KEY IDENTITY(1,1),
    GroupName NVARCHAR(100) NOT NULL,
    CreatedBy INT FOREIGN KEY REFERENCES Users(UserId),
    CreatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE GroupMembers (
    GroupMemberId INT PRIMARY KEY IDENTITY(1,1),
    GroupId INT FOREIGN KEY REFERENCES Groups(GroupId) ON DELETE CASCADE,
    UserId INT FOREIGN KEY REFERENCES Users(UserId)
);
CREATE TABLE Projects (
    ProjectId INT PRIMARY KEY IDENTITY(1,1),
    ProjectName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    StartDate DATETIME NOT NULL,
    EndDate DATETIME,
    Status NVARCHAR(50) DEFAULT 'Chưa bắt đầu', -- Ví dụ: 'Chưa bắt đầu', 'Đang thực hiện', 'Hoàn thành'
    ManagerId INT FOREIGN KEY REFERENCES Users(UserId), -- Người quản lý dự án
    CreatedAt DATETIME DEFAULT GETDATE()
);
CREATE TABLE ProjectMembers (
    ProjectMemberId INT PRIMARY KEY IDENTITY(1,1),
    ProjectId INT FOREIGN KEY REFERENCES Projects(ProjectId) ON DELETE CASCADE,
    UserId INT FOREIGN KEY REFERENCES Users(UserId)
);
CREATE TABLE ProjectTasks (
    ProjectTaskId INT PRIMARY KEY IDENTITY(1,1),
    ProjectId INT FOREIGN KEY REFERENCES Projects(ProjectId) ON DELETE CASCADE,
    TaskId INT FOREIGN KEY REFERENCES SaveTasks(TaskId) ON DELETE CASCADE
);
CREATE TABLE ProjectNotifications (
    NotificationId INT PRIMARY KEY IDENTITY(1,1),
    ProjectId INT FOREIGN KEY REFERENCES Projects(ProjectId) ON DELETE CASCADE,
    UserId INT FOREIGN KEY REFERENCES Users(UserId), -- Người nhận thông báo
    NotificationMessage NVARCHAR(MAX),
    IsRead BIT DEFAULT 0, -- 0: chưa đọc, 1: đã đọc
    CreatedAt DATETIME DEFAULT GETDATE()
);
CREATE TABLE ProjectUsers (
    ProjectUserId INT IDENTITY(1,1) PRIMARY KEY,
    ProjectId INT NOT NULL,
    UserId INT NOT NULL,
    Role NVARCHAR(50) NOT NULL,
    FOREIGN KEY (ProjectId) REFERENCES Projects(ProjectId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

ALTER TABLE SentTasksList
ADD ProjectID INT FOREIGN KEY REFERENCES Projects(ProjectID);
ADD ProjectID INT;


ALTER TABLE SentTasksList
ADD Note NVARCHAR(256);
Delete Tasks
DROP TABLE Projects;

    -- Tuỳ chọn, nếu bạn sử dụng salt
	DELETE FROM SaveTasks ;
	DELETE FROM Projects ;
		DELETE FROM ProjectUsers ;
			DELETE FROM SentTasksList ;
	select * from Projects

	select * from SaveTasks
		select * from Projects
		select * from ProjectUsers
		select * from SentTasksList
EXEC sp_rename 'Tasks', 'SaveTasks';

