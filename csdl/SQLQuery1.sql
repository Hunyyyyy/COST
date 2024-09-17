CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Role NVARCHAR(50) NOT NULL, -- Ví dụ: 'Manager', 'TeamLeader', 'Member'
    CreatedAt DATETIME DEFAULT GETDATE()
);
--bo UserLogin
CREATE TABLE UserLogin (
    UserId INT PRIMARY KEY FOREIGN KEY REFERENCES Users(UserId),
    PasswordHash NVARCHAR(256) NOT NULL,
    PasswordSalt NVARCHAR(256), -- Nếu sử dụng salt
    LastLoginDate DATETIME,
    FailedLoginAttempts INT DEFAULT 0
);
--danh cho các thành viên xem nhiệm vụ trong (Chức năng danh sách nhiệm vụ)
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
	ProjectId INT FOREIGN KEY REFERENCES Projects(ProjectId);
);
--danh cho người tạo dự án xem nhiệm vụ trong (Chức năng Tọa và quản lý nhiệm vụ)
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
--các công việc con trong Chức năng danh sách nhiệm vụ ->bấm vào từng nhiệm vụ
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


--chưa sửa dụng
CREATE TABLE TaskNotifications (
    NotificationId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(UserId), -- Người nhận thông báo
    TaskId INT FOREIGN KEY REFERENCES Tasks(TaskId) ON DELETE CASCADE,
    NotificationMessage NVARCHAR(MAX),
    IsRead BIT DEFAULT 0, -- 0: chưa đọc, 1: đã đọc
    CreatedAt DATETIME DEFAULT GETDATE()
);
--chưa sửa dụng
CREATE TABLE Groups (
    GroupId INT PRIMARY KEY IDENTITY(1,1),
    GroupName NVARCHAR(100) NOT NULL,
    CreatedBy INT FOREIGN KEY REFERENCES Users(UserId),
    CreatedAt DATETIME DEFAULT GETDATE()
);
--chưa sửa dụng
CREATE TABLE GroupMembers (
    GroupMemberId INT PRIMARY KEY IDENTITY(1,1),
    GroupId INT FOREIGN KEY REFERENCES Groups(GroupId) ON DELETE CASCADE,
    UserId INT FOREIGN KEY REFERENCES Users(UserId)
);
--danh sách dự án trong tạo và quản lý dự án
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
--danh sách thành viên trong dự án trong tạo và quản lý dự án ->chọn dự án->cài đặt dự án
CREATE TABLE ProjectMembers (
    ProjectMemberId INT PRIMARY KEY IDENTITY(1,1),
    ProjectId INT FOREIGN KEY REFERENCES Projects(ProjectId) ON DELETE CASCADE,
    UserId INT FOREIGN KEY REFERENCES Users(UserId)
);
--tạo nhiệm vụ trong dự án trong tạo và quản lý dự án ->chọn dự án->tạo nhiệm vụ
CREATE TABLE ProjectTasks (
    ProjectTaskId INT PRIMARY KEY IDENTITY(1,1),
    ProjectId INT FOREIGN KEY REFERENCES Projects(ProjectId) ON DELETE CASCADE,
    TaskId INT FOREIGN KEY REFERENCES SaveTasks(TaskId) ON DELETE CASCADE
);
--chưa
CREATE TABLE ProjectNotifications (
    NotificationId INT PRIMARY KEY IDENTITY(1,1),
    ProjectId INT FOREIGN KEY REFERENCES Projects(ProjectId) ON DELETE CASCADE,
    UserId INT FOREIGN KEY REFERENCES Users(UserId), -- Người nhận thông báo
    NotificationMessage NVARCHAR(MAX),
    IsRead BIT DEFAULT 0, -- 0: chưa đọc, 1: đã đọc
    CreatedAt DATETIME DEFAULT GETDATE()
);
--bang nối giữa bảng Users và bảng project để xét role
CREATE TABLE ProjectUsers (
    ProjectUserId INT IDENTITY(1,1) PRIMARY KEY,
    ProjectId INT NOT NULL,
    UserId INT NOT NULL,
    Role NVARCHAR(50) NOT NULL,
    FOREIGN KEY (ProjectId) REFERENCES Projects(ProjectId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
--bảng các thành viên nhận nhiệm vụ trong danh sách nhiệm vụ->bấm vào nhiệm vụ->nhận nhiệm vụ
CREATE TABLE AssignedSubtasks (
    AssignedSubtaskId INT PRIMARY KEY IDENTITY(1,1),
    SubtaskId INT FOREIGN KEY REFERENCES Subtasks(SubtaskId) ON DELETE CASCADE, -- Liên kết với bảng Subtasks
    AssignedTo INT FOREIGN KEY REFERENCES Users(UserId), -- Người nhận công việc phụ
    Status NVARCHAR(50) DEFAULT 'Chưa nhận', -- Trạng thái công việc phụ (ví dụ: 'Chưa nhận', 'Đang thực hiện', 'Hoàn thành')
    AssignedAt DATETIME DEFAULT GETDATE(), -- Thời điểm giao công việc
    UpdatedAt DATETIME DEFAULT GETDATE() -- Thời điểm cập nhật công việc
);
CREATE TABLE SubtaskProgress (
    SubtaskProgressId INT PRIMARY KEY IDENTITY(1,1),
    SubtaskId INT FOREIGN KEY REFERENCES Subtasks(SubtaskId) ON DELETE CASCADE,
    AssignedTo INT FOREIGN KEY REFERENCES Users(UserId),
    Progress DECIMAL(5, 2) NOT NULL DEFAULT 0, -- Tiến độ của công việc con (0.00 đến 100.00)
    LastUpdatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE TaskProgress (
    TaskProgressId INT PRIMARY KEY IDENTITY(1,1),
    TaskId INT FOREIGN KEY REFERENCES SaveTasks(TaskId) ON DELETE CASCADE,
    Progress DECIMAL(5, 2) NOT NULL DEFAULT 0, -- Tiến độ của nhiệm vụ chính
    LastUpdatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE ProjectProgress (
    ProjectProgressId INT PRIMARY KEY IDENTITY(1,1),
    ProjectId INT FOREIGN KEY REFERENCES Projects(ProjectId) ON DELETE CASCADE,
    Progress DECIMAL(5, 2) NOT NULL DEFAULT 0, -- Tiến độ của dự án
    LastUpdatedAt DATETIME DEFAULT GETDATE()
);
--chờ quản lý phê duyệt
CREATE TABLE SubmittedSubtasks (
    SubmissionId INT PRIMARY KEY IDENTITY(1,1),
    SubtaskId INT NOT NULL,
    TaskId INT NOT NULL, -- ID của nhiệm vụ chứa công việc con
    ProjectId INT NOT NULL, -- ID của dự án chứa nhiệm vụ
    UserId INT NOT NULL,
    SubmittedAt DATETIME NOT NULL DEFAULT GETDATE(),
    Status NVARCHAR(50) DEFAULT 'Đang xem xét',
    Notes NVARCHAR(MAX),
	FilePath NVARCHAR(MAX),
    FOREIGN KEY (SubtaskId) REFERENCES Subtasks(SubtaskId) ON DELETE CASCADE,
    FOREIGN KEY (TaskId) REFERENCES SaveTasks(TaskId) ON DELETE CASCADE,
    FOREIGN KEY (ProjectId) REFERENCES Projects(ProjectId) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
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
    FOREIGN KEY (SubtaskId) REFERENCES Subtasks(SubtaskId) ON DELETE CASCADE,
    FOREIGN KEY (TaskId) REFERENCES SaveTasks(TaskId) ON DELETE CASCADE,
    FOREIGN KEY (ProjectId) REFERENCES Projects(ProjectId) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
ALTER TABLE Subtasks
ADD ProjectID INT FOREIGN KEY REFERENCES Projects(ProjectID);
ADD ProjectID INT;
ALTER TABLE Subtasks
DROP CONSTRAINT IF EXISTS FK_Subtasks_Tasks; -- Xóa ràng buộc khóa ngoại cũ nếu tồn tại
ALTER TABLE SubmittedSubtasks
ADD FilePath NVARCHAR(MAX)

ALTER TABLE AssignedSubtasks
ADD  TaskId INT FOREIGN KEY REFERENCES SaveTasks(TaskId),
FOREIGN KEY (TaskId) REFERENCES SaveTasks(TaskId) ON DELETE CASCADE;
ALTER TABLE SaveTasks
ADD Progress INT DEFAULT 0;
ALTER TABLE Projects
ADD Progress INT DEFAULT 0;
ALTER TABLE SentTasksList
ADD Note NVARCHAR(256);
Delete Tasks
DROP TABLE TaskProgress;

    -- Tuỳ chọn, nếu bạn sử dụng salt
	DELETE FROM SubmittedSubtasks ;
	DELETE FROM Subtasks ;
	DELETE FROM Subtasks ;
	DELETE FROM SubmittedSubtasks ;
		DELETE FROM AssignedSubtasks ;
			DELETE FROM SentTasksList ;
	select * from SubtaskProgress

	select * from ProjectUsers
	select * from TaskProgress
		select * from SubmittedSubtasks
		select * from ProjectProgress
		select * from SaveTasks
				select * from SubtaskProgress
EXEC sp_rename 'Tasks', 'SaveTasks';


