CREATE TABLE [activity_types] (
    [activity_type_id] int NOT NULL IDENTITY,
    [name] varchar(100) NOT NULL,
    [description] text NULL,
    CONSTRAINT [PK__activity__D2470C8792B1F20E] PRIMARY KEY ([activity_type_id])
);
GO


CREATE TABLE [users] (
    [user_id] int NOT NULL IDENTITY,
    [name] varchar(100) NOT NULL,
    [email] varchar(100) NOT NULL,
    [password_hash] varchar(128) NOT NULL,
    [role] int NOT NULL,
    [created_at] datetime NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__users__B9BE370FBC272336] PRIMARY KEY ([user_id])
);
GO


CREATE TABLE [labs] (
    [lab_id] int NOT NULL IDENTITY,
    [name] varchar(100) NOT NULL,
    [manager_id] int NOT NULL,
    [location] varchar(255) NULL,
    [description] text NULL,
    CONSTRAINT [PK__labs__66DE64DB381C94E8] PRIMARY KEY ([lab_id]),
    CONSTRAINT [FK__labs__manager_id__6FB49575] FOREIGN KEY ([manager_id]) REFERENCES [users] ([user_id])
);
GO


CREATE TABLE [equipment] (
    [equipment_id] int NOT NULL IDENTITY,
    [lab_id] int NOT NULL,
    [name] varchar(100) NOT NULL,
    [code] varchar(50) NOT NULL,
    [description] text NULL,
    [status] decimal(2,0) NOT NULL,
    CONSTRAINT [PK__equipmen__197068AFC616B45E] PRIMARY KEY ([equipment_id]),
    CONSTRAINT [FK__equipment__lab_i__7849DB76] FOREIGN KEY ([lab_id]) REFERENCES [labs] ([lab_id])
);
GO


CREATE TABLE [lab_zones] (
    [zone_id] int NOT NULL IDENTITY,
    [lab_id] int NOT NULL,
    [name] varchar(100) NOT NULL,
    [description] text NULL,
    CONSTRAINT [PK__lab_zone__80B401DFE18A342C] PRIMARY KEY ([zone_id]),
    CONSTRAINT [FK__lab_zones__lab_i__72910220] FOREIGN KEY ([lab_id]) REFERENCES [labs] ([lab_id])
);
GO


CREATE TABLE [bookings] (
    [booking_id] int NOT NULL IDENTITY,
    [user_id] int NOT NULL,
    [lab_id] int NOT NULL,
    [zone_id] int NOT NULL,
    [start_time] datetime NOT NULL,
    [end_time] datetime NOT NULL,
    [status] decimal(2,0) NOT NULL,
    [created_at] datetime NOT NULL DEFAULT ((getdate())),
    [notes] text NULL,
    CONSTRAINT [PK__bookings__5DE3A5B17836BDD3] PRIMARY KEY ([booking_id]),
    CONSTRAINT [FK__bookings__lab_id__7D0E9093] FOREIGN KEY ([lab_id]) REFERENCES [labs] ([lab_id]),
    CONSTRAINT [FK__bookings__user_i__7C1A6C5A] FOREIGN KEY ([user_id]) REFERENCES [users] ([user_id]),
    CONSTRAINT [FK__bookings__zone_i__7E02B4CC] FOREIGN KEY ([zone_id]) REFERENCES [lab_zones] ([zone_id])
);
GO


CREATE TABLE [lab_events] (
    [event_id] int NOT NULL IDENTITY,
    [lab_id] int NOT NULL,
    [zone_id] int NOT NULL,
    [activity_type_id] int NOT NULL,
    [organizer_id] int NOT NULL,
    [title] varchar(200) NOT NULL,
    [description] text NULL,
    [start_time] datetime NOT NULL,
    [end_time] datetime NOT NULL,
    [status] decimal(2,0) NOT NULL,
    [created_at] datetime NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__lab_even__2370F7275CD0E23F] PRIMARY KEY ([event_id]),
    CONSTRAINT [FK__lab_event__activ__03BB8E22] FOREIGN KEY ([activity_type_id]) REFERENCES [activity_types] ([activity_type_id]),
    CONSTRAINT [FK__lab_event__lab_i__01D345B0] FOREIGN KEY ([lab_id]) REFERENCES [labs] ([lab_id]),
    CONSTRAINT [FK__lab_event__organ__04AFB25B] FOREIGN KEY ([organizer_id]) REFERENCES [users] ([user_id]),
    CONSTRAINT [FK__lab_event__zone___02C769E9] FOREIGN KEY ([zone_id]) REFERENCES [lab_zones] ([zone_id])
);
GO


CREATE TABLE [reports] (
    [report_id] int NOT NULL IDENTITY,
    [zone_id] int NULL,
    [lab_id] int NULL,
    [generated_by] int NOT NULL,
    [report_type] varchar(100) NOT NULL,
    [content] text NULL,
    [generated_at] datetime NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__reports__779B7C581E1EA1F5] PRIMARY KEY ([report_id]),
    CONSTRAINT [FK__reports__generat__16CE6296] FOREIGN KEY ([generated_by]) REFERENCES [users] ([user_id]),
    CONSTRAINT [FK__reports__lab_id__17C286CF] FOREIGN KEY ([lab_id]) REFERENCES [labs] ([lab_id]),
    CONSTRAINT [FK__reports__zone_id__18B6AB08] FOREIGN KEY ([zone_id]) REFERENCES [lab_zones] ([zone_id])
);
GO


CREATE TABLE [event_participants] (
    [event_id] int NOT NULL,
    [user_id] int NOT NULL,
    [role] decimal(2,0) NOT NULL,
    CONSTRAINT [PK__event_pa__C8EB1457D657AA72] PRIMARY KEY ([event_id], [user_id]),
    CONSTRAINT [FK__event_par__event__078C1F06] FOREIGN KEY ([event_id]) REFERENCES [lab_events] ([event_id]),
    CONSTRAINT [FK__event_par__user___0880433F] FOREIGN KEY ([user_id]) REFERENCES [users] ([user_id])
);
GO


CREATE TABLE [notifications] (
    [notification_id] int NOT NULL IDENTITY,
    [recipient_id] int NOT NULL,
    [event_id] int NOT NULL,
    [message] text NOT NULL,
    [sent_at] datetime NOT NULL DEFAULT ((getdate())),
    [is_read] bit NOT NULL,
    CONSTRAINT [PK__notifica__E059842F312DB8D7] PRIMARY KEY ([notification_id]),
    CONSTRAINT [FK__notificat__event__12FDD1B2] FOREIGN KEY ([event_id]) REFERENCES [lab_events] ([event_id]),
    CONSTRAINT [FK__notificat__recip__1209AD79] FOREIGN KEY ([recipient_id]) REFERENCES [users] ([user_id])
);
GO


CREATE TABLE [security_logs] (
    [log_id] int NOT NULL IDENTITY,
    [event_id] int NOT NULL,
    [security_id] int NOT NULL,
    [action] decimal(2,0) NOT NULL,
    [timestamp] datetime NOT NULL DEFAULT ((getdate())),
    [photo_url] varchar(255) NULL,
    [notes] text NULL,
    CONSTRAINT [PK__security__9E2397E0366827BF] PRIMARY KEY ([log_id]),
    CONSTRAINT [FK__security___event__0C50D423] FOREIGN KEY ([event_id]) REFERENCES [lab_events] ([event_id]),
    CONSTRAINT [FK__security___secur__0D44F85C] FOREIGN KEY ([security_id]) REFERENCES [users] ([user_id])
);
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'activity_type_id', N'description', N'name') AND [object_id] = OBJECT_ID(N'[activity_types]'))
    SET IDENTITY_INSERT [activity_types] ON;
INSERT INTO [activity_types] ([activity_type_id], [description], [name])
VALUES (1, 'Hands-on training session', 'Workshop'),
(2, 'Educational seminar or lecture', 'Seminar'),
(3, 'Research activity', 'Research'),
(4, 'Laboratory experiment', 'Experiment'),
(5, 'Group meeting or discussion', 'Meeting');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'activity_type_id', N'description', N'name') AND [object_id] = OBJECT_ID(N'[activity_types]'))
    SET IDENTITY_INSERT [activity_types] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'user_id', N'created_at', N'email', N'name', N'password_hash', N'role') AND [object_id] = OBJECT_ID(N'[users]'))
    SET IDENTITY_INSERT [users] ON;
INSERT INTO [users] ([user_id], [created_at], [email], [name], [password_hash], [role])
VALUES (1, '2025-01-01T00:00:00.000', 'admin@lab.com', 'Admin User', '$2y$10$hHPvRxU0fb3iOGs2z2VeEuEZ0UTfBS/L6LINEkQ5uElUYJXpbSFsC', 0),
(2, '2025-01-01T00:00:00.000', 'schoolmanager@lab.com', 'School Manager', '$2y$10$hHPvRxU0fb3iOGs2z2VeEuEZ0UTfBS/L6LINEkQ5uElUYJXpbSFsC', 1),
(3, '2025-01-01T00:00:00.000', 'manager@lab.com', 'Lab Manager', '$2y$10$hHPvRxU0fb3iOGs2z2VeEuEZ0UTfBS/L6LINEkQ5uElUYJXpbSFsC', 2),
(4, '2025-01-01T00:00:00.000', 'security@lab.com', 'Security Staff', '$2y$10$hHPvRxU0fb3iOGs2z2VeEuEZ0UTfBS/L6LINEkQ5uElUYJXpbSFsC', 3),
(5, '2025-01-01T00:00:00.000', 'member@lab.com', 'Member', '$2y$10$hHPvRxU0fb3iOGs2z2VeEuEZ0UTfBS/L6LINEkQ5uElUYJXpbSFsC', 4);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'user_id', N'created_at', N'email', N'name', N'password_hash', N'role') AND [object_id] = OBJECT_ID(N'[users]'))
    SET IDENTITY_INSERT [users] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'lab_id', N'description', N'location', N'manager_id', N'name') AND [object_id] = OBJECT_ID(N'[labs]'))
    SET IDENTITY_INSERT [labs] ON;
INSERT INTO [labs] ([lab_id], [description], [location], [manager_id], [name])
VALUES (1, 'Laboratory for biology experiments and research', 'Building A - Floor 2', 2, 'Biology Lab'),
(2, 'Computer lab with 30 workstations', 'Building B - Floor 3', 2, 'Computer Lab'),
(3, 'Chemistry laboratory with safety equipment', 'Building A - Floor 1', 3, 'Chemistry Lab');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'lab_id', N'description', N'location', N'manager_id', N'name') AND [object_id] = OBJECT_ID(N'[labs]'))
    SET IDENTITY_INSERT [labs] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'equipment_id', N'code', N'description', N'lab_id', N'name', N'status') AND [object_id] = OBJECT_ID(N'[equipment]'))
    SET IDENTITY_INSERT [equipment] ON;
INSERT INTO [equipment] ([equipment_id], [code], [description], [lab_id], [name], [status])
VALUES (1, 'EQ-001', 'Digital microscope with 1000x magnification', 1, 'Microscope', 1.0),
(2, 'EQ-002', 'High-speed centrifuge', 1, 'Centrifuge', 1.0),
(3, 'EQ-003', 'Workstation with development tools', 2, 'Computer Station', 1.0),
(4, 'EQ-004', 'Network server infrastructure', 2, 'Server Rack', 1.0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'equipment_id', N'code', N'description', N'lab_id', N'name', N'status') AND [object_id] = OBJECT_ID(N'[equipment]'))
    SET IDENTITY_INSERT [equipment] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'zone_id', N'description', N'lab_id', N'name') AND [object_id] = OBJECT_ID(N'[lab_zones]'))
    SET IDENTITY_INSERT [lab_zones] ON;
INSERT INTO [lab_zones] ([zone_id], [description], [lab_id], [name])
VALUES (1, 'Main experiment area', 1, 'Zone A'),
(2, 'Storage and preparation area', 1, 'Zone B'),
(3, 'Development stations', 2, 'Zone A'),
(4, 'Server room', 2, 'Zone B'),
(5, 'Chemical storage', 3, 'Zone A');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'zone_id', N'description', N'lab_id', N'name') AND [object_id] = OBJECT_ID(N'[lab_zones]'))
    SET IDENTITY_INSERT [lab_zones] OFF;
GO


CREATE INDEX [IX_bookings_lab_id] ON [bookings] ([lab_id]);
GO


CREATE INDEX [IX_bookings_user_id] ON [bookings] ([user_id]);
GO


CREATE INDEX [IX_bookings_zone_id] ON [bookings] ([zone_id]);
GO


CREATE INDEX [IX_equipment_lab_id] ON [equipment] ([lab_id]);
GO


CREATE UNIQUE INDEX [UQ__equipmen__357D4CF958F5304E] ON [equipment] ([code]);
GO


CREATE INDEX [IX_event_participants_user_id] ON [event_participants] ([user_id]);
GO


CREATE INDEX [IX_lab_events_activity_type_id] ON [lab_events] ([activity_type_id]);
GO


CREATE INDEX [IX_lab_events_lab_id] ON [lab_events] ([lab_id]);
GO


CREATE INDEX [IX_lab_events_organizer_id] ON [lab_events] ([organizer_id]);
GO


CREATE INDEX [IX_lab_events_zone_id] ON [lab_events] ([zone_id]);
GO


CREATE INDEX [IX_lab_zones_lab_id] ON [lab_zones] ([lab_id]);
GO


CREATE INDEX [IX_labs_manager_id] ON [labs] ([manager_id]);
GO


CREATE INDEX [IX_notifications_event_id] ON [notifications] ([event_id]);
GO


CREATE INDEX [IX_notifications_recipient_id] ON [notifications] ([recipient_id]);
GO


CREATE INDEX [IX_reports_generated_by] ON [reports] ([generated_by]);
GO


CREATE INDEX [IX_reports_lab_id] ON [reports] ([lab_id]);
GO


CREATE INDEX [IX_reports_zone_id] ON [reports] ([zone_id]);
GO


CREATE INDEX [IX_security_logs_event_id] ON [security_logs] ([event_id]);
GO


CREATE INDEX [IX_security_logs_security_id] ON [security_logs] ([security_id]);
GO


CREATE UNIQUE INDEX [UQ__users__AB6E6164598EED90] ON [users] ([email]);
GO


