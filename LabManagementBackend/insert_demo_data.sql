-- Script to insert demo data for testing
-- Run this AFTER running migrations

USE LabManagementDB;
GO

-- 1. Insert Departments
SET IDENTITY_INSERT [departments] ON;
INSERT INTO [departments] ([department_id], [name], [description], [is_public])
VALUES 
    (1, 'Computer Science', 'Faculty of Computer Science and Information Technology', 1),
    (2, 'Electronics Engineering', 'Faculty of Electronics and Telecommunications', 1),
    (3, 'Mechanical Engineering', 'Faculty of Mechanical Engineering', 0),
    (4, 'Chemical Engineering', 'Faculty of Chemical and Food Technology', 0),
    (5, 'Physics', 'Faculty of Applied Sciences - Physics Department', 1);
SET IDENTITY_INSERT [departments] OFF;
GO

-- 2. Insert Users (password for all: "Password123!")
-- Note: User model has fields: UserId, Name, Email, PasswordHash, Role, CreatedAt
SET IDENTITY_INSERT [users] ON;
INSERT INTO [users] ([user_id], [name], [email], [password_hash], [role], [created_at])
VALUES 
    -- Admin (Role 0)
    (1, 'System Administrator', 'admin@university.edu', '$2a$11$xvK9Z8VqXQYxN1jYqYqYqO3YqYqYqYqYqYqYqYqYqYqYqYqYqYqYq', 0, SYSDATETIME()),
    
    -- School Managers (Role 1)
    (2, 'John Smith', 'manager1@university.edu', '$2a$11$xvK9Z8VqXQYxN1jYqYqYqO3YqYqYqYqYqYqYqYqYqYqYqYqYqYqYq', 1, SYSDATETIME()),
    (3, 'Emily Johnson', 'manager2@university.edu', '$2a$11$xvK9Z8VqXQYxN1jYqYqYqO3YqYqYqYqYqYqYqYqYqYqYqYqYqYqYq', 1, SYSDATETIME()),
    
    -- Lab Managers (Role 2)
    (4, 'Michael Chen', 'labmanager1@university.edu', '$2a$11$xvK9Z8VqXQYxN1jYqYqYqO3YqYqYqYqYqYqYqYqYqYqYqYqYqYqYq', 2, SYSDATETIME()),
    (5, 'Sarah Williams', 'labmanager2@university.edu', '$2a$11$xvK9Z8VqXQYxN1jYqYqYqO3YqYqYqYqYqYqYqYqYqYqYqYqYqYqYq', 2, SYSDATETIME()),
    (6, 'David Lee', 'labmanager3@university.edu', '$2a$11$xvK9Z8VqXQYxN1jYqYqYqO3YqYqYqYqYqYqYqYqYqYqYqYqYqYqYq', 2, SYSDATETIME()),
    
    -- Members/Students (Role 3)
    (7, 'Alice Brown', 'student1@university.edu', '$2a$11$xvK9Z8VqXQYxN1jYqYqYqO3YqYqYqYqYqYqYqYqYqYqYqYqYqYqYq', 3, SYSDATETIME()),
    (8, 'Bob Wilson', 'student2@university.edu', '$2a$11$xvK9Z8VqXQYxN1jYqYqYqO3YqYqYqYqYqYqYqYqYqYqYqYqYqYqYq', 3, SYSDATETIME()),
    (9, 'Carol Martinez', 'student3@university.edu', '$2a$11$xvK9Z8VqXQYxN1jYqYqYqO3YqYqYqYqYqYqYqYqYqYqYqYqYqYqYq', 3, SYSDATETIME()),
    (10, 'Daniel Garcia', 'student4@university.edu', '$2a$11$xvK9Z8VqXQYxN1jYqYqYqO3YqYqYqYqYqYqYqYqYqYqYqYqYqYqYq', 3, SYSDATETIME()),
    (11, 'Eva Anderson', 'researcher1@university.edu', '$2a$11$xvK9Z8VqXQYxN1jYqYqYqO3YqYqYqYqYqYqYqYqYqYqYqYqYqYqYq', 3, SYSDATETIME()),
    (12, 'Frank Taylor', 'researcher2@university.edu', '$2a$11$xvK9Z8VqXQYxN1jYqYqYqO3YqYqYqYqYqYqYqYqYqYqYqYqYqYqYq', 3, SYSDATETIME());
SET IDENTITY_INSERT [users] OFF;
GO

-- 3. Insert User-Department relationships
INSERT INTO [user_departments] ([user_id], [department_id], [created_at])
VALUES 
    -- Computer Science department members
    (4, 1, SYSDATETIME()), -- Lab Manager 1
    (7, 1, SYSDATETIME()), -- Student 1
    (8, 1, SYSDATETIME()), -- Student 2
    (11, 1, SYSDATETIME()), -- Researcher 1
    
    -- Electronics Engineering department members
    (5, 2, SYSDATETIME()), -- Lab Manager 2
    (9, 2, SYSDATETIME()), -- Student 3
    (12, 2, SYSDATETIME()), -- Researcher 2
    
    -- Mechanical Engineering department members
    (6, 3, SYSDATETIME()), -- Lab Manager 3
    (10, 3, SYSDATETIME()); -- Student 4
GO

-- 4. Insert Labs
-- Lab fields: LabId, Name, ManagerId, Location, Description, DepartmentId, IsOpen, Status
SET IDENTITY_INSERT [labs] ON;
INSERT INTO [labs] ([lab_id], [name], [manager_id], [location], [description], [department_id], [is_open], [status])
VALUES 
    (1, 'Computer Lab A', 4, 'Building A - Floor 3 - Room 301', 'General purpose computer lab with 40 workstations', 1, 1, 1),
    (2, 'Computer Lab B', 4, 'Building A - Floor 3 - Room 305', 'Advanced programming and software development lab', 1, 1, 1),
    (3, 'Network Lab', 4, 'Building A - Floor 4 - Room 402', 'Networking equipment and server room', 1, 1, 1),
    (4, 'AI & ML Lab', 4, 'Building A - Floor 5 - Room 501', 'Artificial Intelligence and Machine Learning research lab', 1, 0, 2),
    (5, 'Electronics Lab', 5, 'Building B - Floor 2 - Room 201', 'Basic electronics and circuit design lab', 2, 1, 1),
    (6, 'Embedded Systems Lab', 5, 'Building B - Floor 2 - Room 205', 'Microcontrollers and embedded programming', 2, 1, 1),
    (7, 'Robotics Lab', 5, 'Building B - Floor 3 - Room 301', 'Robotics research and development', 2, 1, 3),
    (8, 'CAD/CAM Lab', 6, 'Building C - Floor 1 - Room 105', 'Computer-Aided Design and Manufacturing', 3, 1, 1),
    (9, 'Materials Testing Lab', 6, 'Building C - Floor 2 - Room 201', 'Material strength and durability testing', 3, 0, 4);
SET IDENTITY_INSERT [labs] OFF;
GO

-- 5. Insert Lab Zones
SET IDENTITY_INSERT [lab_zones] ON;
INSERT INTO [lab_zones] ([zone_id], [lab_id], [name], [description])
VALUES 
    -- Computer Lab A zones
    (1, 1, 'Zone A1', 'Workstations 1-20'),
    (2, 1, 'Zone A2', 'Workstations 21-40'),
    
    -- Computer Lab B zones
    (3, 2, 'Zone B1', 'Development workstations'),
    (4, 2, 'Zone B2', 'Testing and debugging area'),
    
    -- Network Lab zones
    (5, 3, 'Server Room', 'Main server rack area'),
    (6, 3, 'Testing Area', 'Network testing workstations'),
    
    -- AI & ML Lab zones
    (7, 4, 'GPU Cluster', 'High-performance computing area'),
    (8, 4, 'Research Stations', 'Individual research workstations'),
    
    -- Electronics Lab zones
    (9, 5, 'Workbench Area', 'Electronics workbenches 1-15'),
    (10, 5, 'Equipment Storage', 'Tools and components storage'),
    
    -- Embedded Systems Lab zones
    (11, 6, 'Development Zone', 'Embedded development kits'),
    (12, 6, 'Testing Zone', 'Hardware testing equipment'),
    
    -- Robotics Lab zones
    (13, 7, 'Assembly Area', 'Robot assembly and construction'),
    (14, 7, 'Testing Arena', 'Robot testing and demonstration area'),
    
    -- CAD/CAM Lab zones
    (15, 8, 'CAD Workstations', 'Design workstations'),
    (16, 8, '3D Printing Area', '3D printers and CNC machines');
SET IDENTITY_INSERT [lab_zones] OFF;
GO

-- 6. Insert Equipment
-- Equipment fields: EquipmentId, LabId, Name, Code, Description, Status
SET IDENTITY_INSERT [equipment] ON;
INSERT INTO [equipment] ([equipment_id], [lab_id], [name], [code], [description], [status])
VALUES 
    -- Computer Lab A equipment
    (1, 1, 'Dell OptiPlex 7090', 'PC-A-001', 'Desktop computer - i7 processor, 16GB RAM', 1),
    (2, 1, 'Dell OptiPlex 7090', 'PC-A-002', 'Desktop computer - i7 processor, 16GB RAM', 1),
    (3, 1, 'Dell OptiPlex 7090', 'PC-A-003', 'Desktop computer - i7 processor, 16GB RAM', 2),
    (4, 1, 'Epson EB-2250U', 'PROJ-A-001', 'LCD Projector 5000 lumens', 1),
    
    -- Network Lab equipment
    (5, 3, 'Cisco Catalyst 2960', 'SW-N-001', '48-port managed switch', 1),
    (6, 3, 'Cisco ISR 4331', 'RT-N-001', 'Integrated services router', 1),
    (7, 3, 'Dell PowerEdge R740', 'SRV-N-001', 'Rack server - Xeon processor, 128GB RAM', 1),
    (8, 3, 'Dell PowerEdge R740', 'SRV-N-002', 'Rack server - Xeon processor, 128GB RAM', 3),
    
    -- AI & ML Lab equipment
    (9, 4, 'Custom Build Workstation', 'WS-AI-001', 'RTX 4090 GPU, 128GB RAM, 2TB NVMe', 1),
    (10, 4, 'Custom Build Workstation', 'WS-AI-002', 'RTX 4090 GPU, 128GB RAM, 2TB NVMe', 1),
    (11, 4, 'NVIDIA DGX A100', 'GPU-CL-001', 'AI computing cluster - 8x A100 GPUs', 1),
    
    -- Electronics Lab equipment
    (12, 5, 'Tektronix TDS2024C', 'OSC-E-001', 'Digital oscilloscope 200MHz 4-channel', 1),
    (13, 5, 'Tektronix TDS2024C', 'OSC-E-002', 'Digital oscilloscope 200MHz 4-channel', 1),
    (14, 5, 'Keysight E3631A', 'PSU-E-001', 'Triple output DC power supply', 1),
    (15, 5, 'Fluke 87V', 'MM-E-001', 'Digital multimeter', 1),
    
    -- Robotics Lab equipment
    (16, 7, 'Arduino Mega 2560', 'ROB-R-001', 'Microcontroller board for robotics', 1),
    (17, 7, 'Raspberry Pi 4', 'ROB-R-002', 'Single-board computer 8GB RAM', 1),
    (18, 7, 'RPLidar A2M8', 'SEN-R-001', '360° laser scanner', 2);
SET IDENTITY_INSERT [equipment] OFF;
GO

-- 7. Insert Activity Types
SET IDENTITY_INSERT [activity_types] ON;
INSERT INTO [activity_types] ([activity_type_id], [name], [description])
VALUES 
    (1, 'Lecture', 'Regular class lecture or tutorial session'),
    (2, 'Workshop', 'Hands-on workshop or training session'),
    (3, 'Seminar', 'Academic seminar or presentation'),
    (4, 'Research', 'Research project or experiment'),
    (5, 'Competition', 'Hackathon, coding competition, or contest'),
    (6, 'Meeting', 'Team meeting or project discussion'),
    (7, 'Exam', 'Practical examination or assessment'),
    (8, 'Maintenance', 'Lab equipment maintenance or setup');
SET IDENTITY_INSERT [activity_types] OFF;
GO

-- 8. Insert Lab Events
-- LabEvent fields: EventId, LabId, ZoneId, ActivityTypeId, OrganizerId, Title, Description, StartTime, EndTime, Status, CreatedAt, RowVersion
SET IDENTITY_INSERT [lab_events] ON;
INSERT INTO [lab_events] ([event_id], [lab_id], [zone_id], [activity_type_id], [organizer_id], [title], [description], [start_time], [end_time], [status], [created_at])
VALUES 
    -- Past events (Status 3 = Completed)
    (1, 1, 1, 2, 4, 'Python Programming Workshop', 'Introduction to Python Programming for beginners', DATEADD(DAY, -10, SYSDATETIME()), DATEADD(DAY, -10, DATEADD(HOUR, 4, SYSDATETIME())), 3, DATEADD(DAY, -15, SYSDATETIME())),
    (2, 2, 3, 3, 4, 'Software Architecture Seminar', 'Best Practices in Software Architecture Design', DATEADD(DAY, -7, SYSDATETIME()), DATEADD(DAY, -7, DATEADD(HOUR, 2, SYSDATETIME())), 3, DATEADD(DAY, -10, SYSDATETIME())),
    
    -- Upcoming events (Status 1 = Scheduled)
    (3, 1, 1, 5, 4, 'Annual Coding Competition 2025', 'University-wide coding competition with prizes', DATEADD(DAY, 7, SYSDATETIME()), DATEADD(DAY, 7, DATEADD(HOUR, 8, SYSDATETIME())), 1, SYSDATETIME()),
    (4, 5, 9, 2, 5, 'PCB Design Workshop', 'Learn PCB design from scratch using KiCad', DATEADD(DAY, 10, SYSDATETIME()), DATEADD(DAY, 10, DATEADD(HOUR, 3, SYSDATETIME())), 1, SYSDATETIME()),
    (5, 7, 14, 3, 5, 'Autonomous Robotics Seminar', 'Latest trends in autonomous robotics and AI', DATEADD(DAY, 14, SYSDATETIME()), DATEADD(DAY, 14, DATEADD(HOUR, 2, SYSDATETIME())), 1, SYSDATETIME()),
    (6, 3, 5, 8, 4, 'Network Lab Maintenance', 'Scheduled maintenance for network equipment', DATEADD(DAY, 20, SYSDATETIME()), DATEADD(DAY, 20, DATEADD(HOUR, 6, SYSDATETIME())), 1, SYSDATETIME());
SET IDENTITY_INSERT [lab_events] OFF;
GO

-- 9. Insert Bookings
-- Booking fields: BookingId, UserId, LabId, ZoneId, StartTime, EndTime, Status, CreatedAt, Notes, RowVersion
SET IDENTITY_INSERT [bookings] ON;
INSERT INTO [bookings] ([booking_id], [user_id], [lab_id], [zone_id], [start_time], [end_time], [status], [created_at], [notes])
VALUES 
    -- Past bookings (Status 3 = Completed)
    (1, 7, 1, 1, DATEADD(DAY, -5, SYSDATETIME()), DATEADD(DAY, -5, DATEADD(HOUR, 2, SYSDATETIME())), 3, DATEADD(DAY, -6, SYSDATETIME()), 'Data Structures lab session'),
    (2, 8, 2, 3, DATEADD(DAY, -3, SYSDATETIME()), DATEADD(DAY, -3, DATEADD(HOUR, 3, SYSDATETIME())), 3, DATEADD(DAY, -4, SYSDATETIME()), 'Software Engineering project work'),
    
    -- Cancelled booking (Status 2 = Cancelled)
    (3, 11, 4, 7, DATEADD(DAY, -2, SYSDATETIME()), DATEADD(DAY, -2, DATEADD(HOUR, 4, SYSDATETIME())), 2, DATEADD(DAY, -3, SYSDATETIME()), 'Deep Learning model training - Lab closed for maintenance'),
    
    -- Active bookings (Status 1 = Confirmed)
    (4, 7, 1, 2, DATEADD(HOUR, -1, SYSDATETIME()), DATEADD(HOUR, 1, SYSDATETIME()), 1, DATEADD(HOUR, -2, SYSDATETIME()), 'Algorithm practice session'),
    (5, 9, 5, 9, SYSDATETIME(), DATEADD(HOUR, 2, SYSDATETIME()), 1, DATEADD(HOUR, -1, SYSDATETIME()), 'Circuit design lab work'),
    
    -- Upcoming bookings (Status 1 = Confirmed)
    (6, 8, 1, 1, DATEADD(DAY, 1, SYSDATETIME()), DATEADD(DAY, 1, DATEADD(HOUR, 2, SYSDATETIME())), 1, SYSDATETIME(), 'Web Development practice'),
    (7, 11, 3, 5, DATEADD(DAY, 2, SYSDATETIME()), DATEADD(DAY, 2, DATEADD(HOUR, 3, SYSDATETIME())), 1, SYSDATETIME(), 'Network security research'),
    (8, 12, 6, 11, DATEADD(DAY, 2, SYSDATETIME()), DATEADD(DAY, 2, DATEADD(HOUR, 4, SYSDATETIME())), 1, SYSDATETIME(), 'IoT device development'),
    (9, 10, 8, 15, DATEADD(DAY, 3, SYSDATETIME()), DATEADD(DAY, 3, DATEADD(HOUR, 2, SYSDATETIME())), 1, SYSDATETIME(), 'CAD design project'),
    (10, 7, 2, 4, DATEADD(DAY, 5, SYSDATETIME()), DATEADD(DAY, 5, DATEADD(HOUR, 3, SYSDATETIME())), 1, SYSDATETIME(), 'Final project testing');
SET IDENTITY_INSERT [bookings] OFF;
GO

-- 10. Insert Event Participants
-- Role: 1 = Participant, 2 = Organizer/Helper
INSERT INTO [event_participants] ([event_id], [user_id], [role])
VALUES 
    -- Python Workshop participants
    (1, 7, 1), -- Student 1 - Participant
    (1, 8, 1), -- Student 2 - Participant
    (1, 11, 2), -- Researcher 1 - Helper
    
    -- Software Architecture Seminar participants
    (2, 8, 1),
    (2, 11, 1),
    (2, 12, 2),
    
    -- Coding Competition participants
    (3, 7, 1),
    (3, 8, 1),
    (3, 9, 1),
    (3, 4, 2), -- Lab Manager as organizer
    
    -- PCB Design Workshop participants
    (4, 9, 1),
    (4, 12, 1),
    (4, 5, 2),
    
    -- Robotics Seminar participants
    (5, 9, 1),
    (5, 10, 1),
    (5, 5, 2);
GO

-- 11. Insert Reports
-- Report fields: ReportId, ZoneId, LabId, ReportType, Content, GeneratedAt, UserId, PhotoUrl
SET IDENTITY_INSERT [reports] ON;
INSERT INTO [reports] ([report_id], [lab_id], [zone_id], [report_type], [content], [generated_at], [user_id], [photo_url])
VALUES 
    (1, 1, 1, 'Equipment Issue', 'Computer PC-A-003 has a faulty RAM module. Screen freezes randomly during operation. Needs replacement.', DATEADD(DAY, -3, SYSDATETIME()), 7, NULL),
    (2, 3, 6, 'Network Issue', 'Server SRV-N-002 experiencing network connectivity problems. Unable to ping gateway. Needs diagnostics.', DATEADD(DAY, -2, SYSDATETIME()), 11, NULL),
    (3, 5, 9, 'Equipment Damage', 'Oscilloscope OSC-E-002 has a cracked screen. Still functional but display is affected. Replacement recommended.', DATEADD(DAY, -1, SYSDATETIME()), 9, 'https://example.com/photos/osc-damage.jpg'),
    (4, 7, 13, 'Maintenance Request', 'LIDAR sensor SEN-R-001 needs calibration. Distance readings seem inaccurate by 10-15cm.', SYSDATETIME(), 12, NULL),
    (5, 1, NULL, 'Usage Report', 'Monthly usage statistics for Computer Lab A - November 2025. Total bookings: 45, Peak hours: 14:00-18:00', SYSDATETIME(), 4, NULL);
SET IDENTITY_INSERT [reports] OFF;
GO

-- 12. Insert Security Logs
-- SecurityLog fields: LogId, EventId, SecurityId, ActionType, PhotoUrl, Notes, LoggedAt, RowVersion
SET IDENTITY_INSERT [security_logs] ON;
INSERT INTO [security_logs] ([log_id], [event_id], [security_id], [action_type], [photo_url], [notes], [logged_at])
VALUES 
    (1, 1, 7, 1, NULL, 'Checked in for Python Workshop', DATEADD(DAY, -10, SYSDATETIME())),
    (2, 1, 7, 2, NULL, 'Checked out after workshop completion', DATEADD(DAY, -10, DATEADD(HOUR, 4, SYSDATETIME()))),
    (3, 2, 8, 1, NULL, 'Checked in for Software Architecture Seminar', DATEADD(DAY, -7, SYSDATETIME())),
    (4, 2, 8, 2, NULL, 'Checked out after seminar', DATEADD(DAY, -7, DATEADD(HOUR, 2, SYSDATETIME()))),
    (5, 3, 11, 3, 'https://example.com/security/emergency-access.jpg', 'Emergency access approved for network maintenance', DATEADD(DAY, -1, SYSDATETIME()));
SET IDENTITY_INSERT [security_logs] OFF;
GO

-- 13. Insert Notifications
-- Notification fields: NotificationId, RecipientId, EventId, Message, SentAt, IsRead
SET IDENTITY_INSERT [notifications] ON;
INSERT INTO [notifications] ([notification_id], [recipient_id], [event_id], [message], [sent_at], [is_read])
VALUES 
    (1, 7, 3, 'You have been registered for Annual Coding Competition 2025', SYSDATETIME(), 0),
    (2, 8, 3, 'You have been registered for Annual Coding Competition 2025', SYSDATETIME(), 0),
    (3, 9, 4, 'Reminder: PCB Design Workshop starts in 3 days', SYSDATETIME(), 0),
    (4, 9, 5, 'You have been registered for Autonomous Robotics Seminar', DATEADD(HOUR, -2, SYSDATETIME()), 1),
    (5, 10, 5, 'You have been registered for Autonomous Robotics Seminar', DATEADD(HOUR, -2, SYSDATETIME()), 0),
    (6, 4, 6, 'Network Lab Maintenance scheduled for next month', DATEADD(DAY, -1, SYSDATETIME()), 1),
    (7, 11, 1, 'Python Programming Workshop completed successfully', DATEADD(DAY, -10, SYSDATETIME()), 1),
    (8, 12, 2, 'Software Architecture Seminar completed successfully', DATEADD(DAY, -7, SYSDATETIME()), 1);
SET IDENTITY_INSERT [notifications] OFF;
GO

-- Display summary
SELECT 'Demo data inserted successfully!' AS Result;
SELECT 
    (SELECT COUNT(*) FROM departments) AS Departments,
    (SELECT COUNT(*) FROM users) AS Users,
    (SELECT COUNT(*) FROM user_departments) AS UserDepartments,
    (SELECT COUNT(*) FROM labs) AS Labs,
    (SELECT COUNT(*) FROM lab_zones) AS LabZones,
    (SELECT COUNT(*) FROM equipment) AS Equipment,
    (SELECT COUNT(*) FROM activity_types) AS ActivityTypes,
    (SELECT COUNT(*) FROM lab_events) AS LabEvents,
    (SELECT COUNT(*) FROM event_participants) AS EventParticipants,
    (SELECT COUNT(*) FROM bookings) AS Bookings,
    (SELECT COUNT(*) FROM reports) AS Reports,
    (SELECT COUNT(*) FROM security_logs) AS SecurityLogs,
    (SELECT COUNT(*) FROM notifications) AS Notifications;
GO
