USE PropertyOps_v1;
GO

IF COL_LENGTH('dbo.RentContracts', 'ReceivingEntityId') IS NULL
BEGIN
    ALTER TABLE dbo.RentContracts ADD ReceivingEntityId INT NULL;
    ALTER TABLE dbo.RentContracts ADD CONSTRAINT FK_RentContracts_ReceivingEntity
        FOREIGN KEY (ReceivingEntityId) REFERENCES dbo.Entities(EntityId);
END
GO

IF COL_LENGTH('dbo.Bookings', 'ReceivingEntityId') IS NULL
BEGIN
    ALTER TABLE dbo.Bookings ADD ReceivingEntityId INT NULL;
    ALTER TABLE dbo.Bookings ADD CONSTRAINT FK_Bookings_ReceivingEntity
        FOREIGN KEY (ReceivingEntityId) REFERENCES dbo.Entities(EntityId);
END
GO

IF OBJECT_ID('dbo.HeatingTariffs','U') IS NULL
BEGIN
    CREATE TABLE dbo.HeatingTariffs (
        TariffId INT IDENTITY(1,1) PRIMARY KEY,
        PeriodYear INT NOT NULL,
        PeriodMonth INT NOT NULL,
        PricePerM2 DECIMAL(18,4) NOT NULL DEFAULT 0,
        PricePerUnit DECIMAL(18,4) NOT NULL DEFAULT 0,
        Notes NVARCHAR(200) NULL,
        CONSTRAINT UQ_HeatingTariffs UNIQUE(PeriodYear, PeriodMonth)
    );
END
GO

IF OBJECT_ID('dbo.ConstructionProjects','U') IS NULL
BEGIN
    CREATE TABLE dbo.ConstructionProjects (
        ProjectId INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        StartDate DATE NULL,
        EndDate DATE NULL,
        Notes NVARCHAR(500) NULL,
        IsActive BIT NOT NULL DEFAULT 1
    );
END
GO

IF OBJECT_ID('dbo.ConstructionSales','U') IS NULL
BEGIN
    CREATE TABLE dbo.ConstructionSales (
        SaleId INT IDENTITY(1,1) PRIMARY KEY,
        ProjectId INT NOT NULL,
        UnitId INT NULL,
        BuyerCounterpartyId INT NOT NULL,
        SaleDate DATE NOT NULL,
        TotalPrice DECIMAL(18,2) NOT NULL,
        Notes NVARCHAR(300) NULL,
        ReceivingEntityId INT NULL,
        CONSTRAINT FK_Sales_Project FOREIGN KEY (ProjectId) REFERENCES dbo.ConstructionProjects(ProjectId),
        CONSTRAINT FK_Sales_Unit FOREIGN KEY (UnitId) REFERENCES dbo.Units(UnitId),
        CONSTRAINT FK_Sales_Buyer FOREIGN KEY (BuyerCounterpartyId) REFERENCES dbo.Counterparties(CounterpartyId),
        CONSTRAINT FK_Sales_Entity FOREIGN KEY (ReceivingEntityId) REFERENCES dbo.Entities(EntityId)
    );
END
GO

IF OBJECT_ID('dbo.NHResidents','U') IS NULL
BEGIN
    CREATE TABLE dbo.NHResidents (
        ResidentId INT IDENTITY(1,1) PRIMARY KEY,
        UnitId INT NOT NULL,
        CounterpartyId INT NOT NULL,
        StartDate DATE NOT NULL,
        EndDate DATE NULL,
        MonthlyFee DECIMAL(18,2) NOT NULL,
        ReceivingEntityId INT NULL,
        Notes NVARCHAR(300) NULL,
        Status NVARCHAR(20) NOT NULL DEFAULT 'Active',
        CONSTRAINT FK_NH_Unit FOREIGN KEY (UnitId) REFERENCES dbo.Units(UnitId),
        CONSTRAINT FK_NH_Cp FOREIGN KEY (CounterpartyId) REFERENCES dbo.Counterparties(CounterpartyId),
        CONSTRAINT FK_NH_Entity FOREIGN KEY (ReceivingEntityId) REFERENCES dbo.Entities(EntityId)
    );
END
GO

DECLARE @blAssoc INT = (SELECT BusinessLineId FROM dbo.BusinessLines WHERE Name='Association');
DECLARE @blConstr INT = (SELECT BusinessLineId FROM dbo.BusinessLines WHERE Name='Construction');
DECLARE @blNH INT = (SELECT BusinessLineId FROM dbo.BusinessLines WHERE Name='NursingHome');
DECLARE @blRent INT = (SELECT BusinessLineId FROM dbo.BusinessLines WHERE Name='RentLong');
DECLARE @blBooking INT = (SELECT BusinessLineId FROM dbo.BusinessLines WHERE Name='Booking');
DECLARE @blHeating INT = (SELECT BusinessLineId FROM dbo.BusinessLines WHERE Name='Heating');

IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Name=N'Prodaja stanova (prihod)')
    INSERT INTO dbo.Categories(Name,CategoryType,BusinessLineId) VALUES (N'Prodaja stanova (prihod)','Income',@blConstr);

IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Name=N'Troškovi izgradnje')
    INSERT INTO dbo.Categories(Name,CategoryType,BusinessLineId) VALUES (N'Troškovi izgradnje','Expense',@blConstr);

IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Name=N'Dom za stare (prihod)')
    INSERT INTO dbo.Categories(Name,CategoryType,BusinessLineId) VALUES (N'Dom za stare (prihod)','Income',@blNH);

IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Name=N'Dom za stare (trošak)')
    INSERT INTO dbo.Categories(Name,CategoryType,BusinessLineId) VALUES (N'Dom za stare (trošak)','Expense',@blNH);

IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Name=N'Zakupnina (prihod)')
    INSERT INTO dbo.Categories(Name,CategoryType,BusinessLineId) VALUES (N'Zakupnina (prihod)','Income',@blRent);

IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Name=N'Booking prihod')
    INSERT INTO dbo.Categories(Name,CategoryType,BusinessLineId) VALUES (N'Booking prihod','Income',@blBooking);

IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Name=N'Grejanje prihod')
    INSERT INTO dbo.Categories(Name,CategoryType,BusinessLineId) VALUES (N'Grejanje prihod','Income',@blHeating);
GO
