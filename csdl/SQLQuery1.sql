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

CREATE TABLE Tasks (
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
CREATE TABLE Subtasks (
    SubtaskId INT PRIMARY KEY IDENTITY(1,1),
    TaskId INT FOREIGN KEY REFERENCES Tasks(TaskId) ON DELETE CASCADE,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    Status NVARCHAR(50) DEFAULT 'Chưa nhận', -- Ví dụ: 'Chưa nhận', 'Đang thực hiện', 'Hoàn thành'
    AssignedTo INT FOREIGN KEY REFERENCES Users(UserId), -- Người được giao công việc con (thành viên)
    CreatedAt DATETIME DEFAULT GETDATE()
);
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
ALTER TABLE Tasks
ADD Note NVARCHAR(256);
    -- Tuỳ chọn, nếu bạn sử dụng salt
	DELETE FROM Subtasks ;
	select * from Subtasks
		select * from Tasks