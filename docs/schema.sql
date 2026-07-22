IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    CREATE TABLE [Ingredients] (
        [IngredientId] int NOT NULL IDENTITY,
        [IngredientName] nvarchar(100) NOT NULL,
        [QuantityInStock] decimal(10,3) NOT NULL,
        [Unit] nvarchar(20) NOT NULL,
        [LowStockThreshold] decimal(10,3) NOT NULL,
        CONSTRAINT [PK_Ingredients] PRIMARY KEY ([IngredientId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    CREATE TABLE [MenuCategories] (
        [MenuCategoryId] int NOT NULL IDENTITY,
        [CategoryName] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_MenuCategories] PRIMARY KEY ([MenuCategoryId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    CREATE TABLE [RestaurantTables] (
        [TableId] int NOT NULL IDENTITY,
        [TableName] nvarchar(20) NOT NULL,
        [Capacity] int NOT NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_RestaurantTables] PRIMARY KEY ([TableId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [UserId] int NOT NULL IDENTITY,
        [Username] nvarchar(50) NOT NULL,
        [PasswordHash] varbinary(max) NOT NULL,
        [PasswordSalt] varbinary(max) NOT NULL,
        [FullName] nvarchar(100) NOT NULL,
        [Role] int NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    CREATE TABLE [MenuItems] (
        [MenuItemId] int NOT NULL IDENTITY,
        [ItemName] nvarchar(150) NOT NULL,
        [Price] money NOT NULL,
        [IsAvailable] bit NOT NULL,
        [MenuCategoryId] int NOT NULL,
        CONSTRAINT [PK_MenuItems] PRIMARY KEY ([MenuItemId]),
        CONSTRAINT [FK_MenuItems_MenuCategories_MenuCategoryId] FOREIGN KEY ([MenuCategoryId]) REFERENCES [MenuCategories] ([MenuCategoryId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    CREATE TABLE [Orders] (
        [OrderId] int NOT NULL IDENTITY,
        [OpenedAt] datetime2 NOT NULL,
        [Status] int NOT NULL,
        [RowVersion] rowversion NOT NULL,
        [TableId] int NOT NULL,
        [UserId] int NULL,
        CONSTRAINT [PK_Orders] PRIMARY KEY ([OrderId]),
        CONSTRAINT [FK_Orders_RestaurantTables_TableId] FOREIGN KEY ([TableId]) REFERENCES [RestaurantTables] ([TableId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Orders_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    CREATE TABLE [Shifts] (
        [ShiftId] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [OpenedAt] datetime2 NOT NULL,
        [ClosedAt] datetime2 NULL,
        [OpeningCash] money NOT NULL,
        [ClosingCash] money NULL,
        CONSTRAINT [PK_Shifts] PRIMARY KEY ([ShiftId]),
        CONSTRAINT [FK_Shifts_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    CREATE TABLE [MenuItemIngredients] (
        [MenuItemId] int NOT NULL,
        [IngredientId] int NOT NULL,
        [QuantityRequired] decimal(10,3) NOT NULL,
        CONSTRAINT [PK_MenuItemIngredients] PRIMARY KEY ([MenuItemId], [IngredientId]),
        CONSTRAINT [FK_MenuItemIngredients_Ingredients_IngredientId] FOREIGN KEY ([IngredientId]) REFERENCES [Ingredients] ([IngredientId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MenuItemIngredients_MenuItems_MenuItemId] FOREIGN KEY ([MenuItemId]) REFERENCES [MenuItems] ([MenuItemId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    CREATE TABLE [OrderItems] (
        [OrderItemId] int NOT NULL IDENTITY,
        [OrderId] int NOT NULL,
        [MenuItemId] int NOT NULL,
        [Quantity] int NOT NULL,
        [UnitPrice] money NOT NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_OrderItems] PRIMARY KEY ([OrderItemId]),
        CONSTRAINT [FK_OrderItems_MenuItems_MenuItemId] FOREIGN KEY ([MenuItemId]) REFERENCES [MenuItems] ([MenuItemId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([OrderId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    CREATE TABLE [Payments] (
        [PaymentId] int NOT NULL IDENTITY,
        [OrderId] int NOT NULL,
        [CashierUserId] int NOT NULL,
        [AmountPaid] money NOT NULL,
        [Method] int NOT NULL,
        [PaidAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Payments] PRIMARY KEY ([PaymentId]),
        CONSTRAINT [FK_Payments_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([OrderId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Payments_Users_CashierUserId] FOREIGN KEY ([CashierUserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IngredientId', N'IngredientName', N'LowStockThreshold', N'QuantityInStock', N'Unit') AND [object_id] = OBJECT_ID(N'[Ingredients]'))
        SET IDENTITY_INSERT [Ingredients] ON;
    EXEC(N'INSERT INTO [Ingredients] ([IngredientId], [IngredientName], [LowStockThreshold], [QuantityInStock], [Unit])
    VALUES (1, N''Chicken'', 3.0, 20.0, N''kg''),
    (2, N''Rice'', 5.0, 30.0, N''kg''),
    (3, N''Beef'', 3.0, 15.0, N''kg''),
    (4, N''Rice Noodle'', 4.0, 20.0, N''kg''),
    (5, N''Tea Leaves'', 1.0, 5.0, N''kg'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IngredientId', N'IngredientName', N'LowStockThreshold', N'QuantityInStock', N'Unit') AND [object_id] = OBJECT_ID(N'[Ingredients]'))
        SET IDENTITY_INSERT [Ingredients] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'MenuCategoryId', N'CategoryName') AND [object_id] = OBJECT_ID(N'[MenuCategories]'))
        SET IDENTITY_INSERT [MenuCategories] ON;
    EXEC(N'INSERT INTO [MenuCategories] ([MenuCategoryId], [CategoryName])
    VALUES (1, N''Appetizers''),
    (2, N''Main Course''),
    (3, N''Beverages'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'MenuCategoryId', N'CategoryName') AND [object_id] = OBJECT_ID(N'[MenuCategories]'))
        SET IDENTITY_INSERT [MenuCategories] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'TableId', N'Capacity', N'Status', N'TableName') AND [object_id] = OBJECT_ID(N'[RestaurantTables]'))
        SET IDENTITY_INSERT [RestaurantTables] ON;
    EXEC(N'INSERT INTO [RestaurantTables] ([TableId], [Capacity], [Status], [TableName])
    VALUES (1, 2, 0, N''T1''),
    (2, 2, 0, N''T2''),
    (3, 4, 0, N''T3''),
    (4, 4, 0, N''T4''),
    (5, 6, 0, N''T5''),
    (6, 6, 0, N''T6'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'TableId', N'Capacity', N'Status', N'TableName') AND [object_id] = OBJECT_ID(N'[RestaurantTables]'))
        SET IDENTITY_INSERT [RestaurantTables] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'FullName', N'PasswordHash', N'PasswordSalt', N'Role', N'Username') AND [object_id] = OBJECT_ID(N'[Users]'))
        SET IDENTITY_INSERT [Users] ON;
    EXEC(N'INSERT INTO [Users] ([UserId], [FullName], [PasswordHash], [PasswordSalt], [Role], [Username])
    VALUES (1, N''Administrator'', 0x89180B75BA85A62CB4F1469578E24DD9048E7B9504AD15180DDB656F3F46A230, 0x4BB4178A8C536507AB136708A9B56A27, 0, N''admin'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'FullName', N'PasswordHash', N'PasswordSalt', N'Role', N'Username') AND [object_id] = OBJECT_ID(N'[Users]'))
        SET IDENTITY_INSERT [Users] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'MenuItemId', N'IsAvailable', N'ItemName', N'MenuCategoryId', N'Price') AND [object_id] = OBJECT_ID(N'[MenuItems]'))
        SET IDENTITY_INSERT [MenuItems] ON;
    EXEC(N'INSERT INTO [MenuItems] ([MenuItemId], [IsAvailable], [ItemName], [MenuCategoryId], [Price])
    VALUES (1, CAST(1 AS bit), N''Spring Rolls'', 1, 45000.0),
    (2, CAST(1 AS bit), N''Chicken Rice'', 2, 55000.0),
    (3, CAST(1 AS bit), N''Beef Noodle Soup'', 2, 65000.0),
    (4, CAST(1 AS bit), N''Iced Tea'', 3, 15000.0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'MenuItemId', N'IsAvailable', N'ItemName', N'MenuCategoryId', N'Price') AND [object_id] = OBJECT_ID(N'[MenuItems]'))
        SET IDENTITY_INSERT [MenuItems] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IngredientId', N'MenuItemId', N'QuantityRequired') AND [object_id] = OBJECT_ID(N'[MenuItemIngredients]'))
        SET IDENTITY_INSERT [MenuItemIngredients] ON;
    EXEC(N'INSERT INTO [MenuItemIngredients] ([IngredientId], [MenuItemId], [QuantityRequired])
    VALUES (1, 2, 0.3),
    (2, 2, 0.2),
    (3, 3, 0.25),
    (4, 3, 0.2),
    (5, 4, 0.02)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IngredientId', N'MenuItemId', N'QuantityRequired') AND [object_id] = OBJECT_ID(N'[MenuItemIngredients]'))
        SET IDENTITY_INSERT [MenuItemIngredients] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_MenuItemIngredients_IngredientId] ON [MenuItemIngredients] ([IngredientId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_MenuItems_MenuCategoryId] ON [MenuItems] ([MenuCategoryId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_OrderItems_MenuItemId] ON [OrderItems] ([MenuItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_OrderItems_OrderId] ON [OrderItems] ([OrderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Orders_TableId] ON [Orders] ([TableId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Orders_UserId] ON [Orders] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Payments_CashierUserId] ON [Payments] ([CashierUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Payments_OrderId] ON [Payments] ([OrderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Shifts_UserId] ON [Shifts] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708131002_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260708131002_InitialCreate', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708135338_AddShiftIdToPayment'
)
BEGIN
    ALTER TABLE [Payments] ADD [ShiftId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708135338_AddShiftIdToPayment'
)
BEGIN
    CREATE INDEX [IX_Payments_ShiftId] ON [Payments] ([ShiftId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708135338_AddShiftIdToPayment'
)
BEGIN
    ALTER TABLE [Payments] ADD CONSTRAINT [FK_Payments_Shifts_ShiftId] FOREIGN KEY ([ShiftId]) REFERENCES [Shifts] ([ShiftId]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708135338_AddShiftIdToPayment'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260708135338_AddShiftIdToPayment', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708145022_AddOpenedByUserToOrder'
)
BEGIN
    ALTER TABLE [Orders] DROP CONSTRAINT [FK_Orders_Users_UserId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708145022_AddOpenedByUserToOrder'
)
BEGIN
    DROP INDEX [IX_Orders_UserId] ON [Orders];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708145022_AddOpenedByUserToOrder'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'UserId');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Orders] DROP COLUMN [UserId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708145022_AddOpenedByUserToOrder'
)
BEGIN
    ALTER TABLE [Orders] ADD [OpenedByUserId] int NOT NULL DEFAULT 1;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708145022_AddOpenedByUserToOrder'
)
BEGIN
    CREATE INDEX [IX_Orders_OpenedByUserId] ON [Orders] ([OpenedByUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708145022_AddOpenedByUserToOrder'
)
BEGIN
    ALTER TABLE [Orders] ADD CONSTRAINT [FK_Orders_Users_OpenedByUserId] FOREIGN KEY ([OpenedByUserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708145022_AddOpenedByUserToOrder'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260708145022_AddOpenedByUserToOrder', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260709043914_HardenShiftAndStockConcurrency'
)
BEGIN
    DROP INDEX [IX_Shifts_UserId] ON [Shifts];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260709043914_HardenShiftAndStockConcurrency'
)
BEGIN
    ALTER TABLE [Ingredients] ADD [RowVersion] rowversion NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260709043914_HardenShiftAndStockConcurrency'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Shifts_UserId] ON [Shifts] ([UserId]) WHERE [ClosedAt] IS NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260709043914_HardenShiftAndStockConcurrency'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260709043914_HardenShiftAndStockConcurrency', N'8.0.11');
END;
GO

COMMIT;
GO

