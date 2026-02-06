-- PropertyOps_v1 (Core + Finance + Heating/Rent/Booking skeleton)
IF DB_ID('PropertyOps_v1') IS NULL
    CREATE DATABASE PropertyOps_v1;
GO
USE PropertyOps_v1;
GO

-- Roles/Users
IF OBJECT_ID('dbo.UserRoles','U') IS NOT NULL DROP TABLE dbo.UserRoles;
IF OBJECT_ID('dbo.Users','U') IS NOT NULL DROP TABLE dbo.Users;
IF OBJECT_ID('dbo.Roles','U') IS NOT NULL DROP TABLE dbo.Roles;

CREATE TABLE dbo.Roles (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE dbo.Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARBINARY(64) NOT NULL,  -- SHA2_512
    PasswordSalt VARBINARY(32) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE TABLE dbo.UserRoles (
    UserId INT NOT NULL,
    RoleId INT NOT NULL,
    CONSTRAINT PK_UserRoles PRIMARY KEY (UserId, RoleId),
    CONSTRAINT FK_UserRoles_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_UserRoles_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles(RoleId)
);

-- Entities
IF OBJECT_ID('dbo.Entities','U') IS NOT NULL DROP TABLE dbo.Entities;
CREATE TABLE dbo.Entities (
    EntityId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    EntityType NVARCHAR(30) NOT NULL, -- Company | Owner | HOA | Other
    TaxId NVARCHAR(30) NULL,
    RegNo NVARCHAR(30) NULL,
    BankAccount NVARCHAR(50) NULL,
    Phone NVARCHAR(50) NULL,
    Email NVARCHAR(100) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

-- Counterparties
IF OBJECT_ID('dbo.Counterparties','U') IS NOT NULL DROP TABLE dbo.Counterparties;
CREATE TABLE dbo.Counterparties (
    CounterpartyId INT IDENTITY(1,1) PRIMARY KEY,
    Type NVARCHAR(30) NOT NULL, -- Customer | Supplier | Owner | HeatingCustomer | Other
    FullName NVARCHAR(200) NOT NULL,
    Phone NVARCHAR(50) NULL,
    Email NVARCHAR(100) NULL,
    Address NVARCHAR(250) NULL,
    JMBG NVARCHAR(13) NULL,
    PIB NVARCHAR(30) NULL,
    BankAccount NVARCHAR(50) NULL,
    Notes NVARCHAR(500) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

-- BusinessLines & Categories
IF OBJECT_ID('dbo.Categories','U') IS NOT NULL DROP TABLE dbo.Categories;
IF OBJECT_ID('dbo.BusinessLines','U') IS NOT NULL DROP TABLE dbo.BusinessLines;

CREATE TABLE dbo.BusinessLines (
    BusinessLineId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(80) NOT NULL UNIQUE
);

CREATE TABLE dbo.Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(120) NOT NULL,
    CategoryType NVARCHAR(20) NOT NULL, -- Income | Expense | Transfer
    BusinessLineId INT NULL,
    CONSTRAINT FK_Categories_BusinessLines FOREIGN KEY (BusinessLineId) REFERENCES dbo.BusinessLines(BusinessLineId)
);

-- Properties & Units
IF OBJECT_ID('dbo.UnitOwnership','U') IS NOT NULL DROP TABLE dbo.UnitOwnership;
IF OBJECT_ID('dbo.Units','U') IS NOT NULL DROP TABLE dbo.Units;
IF OBJECT_ID('dbo.Properties','U') IS NOT NULL DROP TABLE dbo.Properties;

CREATE TABLE dbo.Properties (
    PropertyId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Address NVARCHAR(250) NULL,
    Notes NVARCHAR(500) NULL
);

CREATE TABLE dbo.Units (
    UnitId INT IDENTITY(1,1) PRIMARY KEY,
    PropertyId INT NOT NULL,
    UnitCode NVARCHAR(50) NOT NULL,
    UnitType NVARCHAR(30) NOT NULL,  -- Apartment | Room | Commercial | Other
    AreaM2 DECIMAL(10,2) NULL,
    DefaultRentPrice DECIMAL(18,2) NULL,
    DefaultSalePrice DECIMAL(18,2) NULL,
    Usage NVARCHAR(30) NOT NULL, -- RentLong | Booking | Sale | NursingHome
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT UQ_Units UNIQUE(PropertyId, UnitCode),
    CONSTRAINT FK_Units_Properties FOREIGN KEY (PropertyId) REFERENCES dbo.Properties(PropertyId)
);

CREATE TABLE dbo.UnitOwnership (
    UnitOwnershipId INT IDENTITY(1,1) PRIMARY KEY,
    UnitId INT NOT NULL,
    EntityId INT NOT NULL,
    SharePct DECIMAL(5,2) NOT NULL DEFAULT 100.00,
    EffectiveFrom DATE NOT NULL DEFAULT GETDATE(),
    EffectiveTo DATE NULL,
    CONSTRAINT FK_UnitOwnership_Units FOREIGN KEY (UnitId) REFERENCES dbo.Units(UnitId),
    CONSTRAINT FK_UnitOwnership_Entities FOREIGN KEY (EntityId) REFERENCES dbo.Entities(EntityId)
);

-- Heating skeleton
IF OBJECT_ID('dbo.HeatingReadings','U') IS NOT NULL DROP TABLE dbo.HeatingReadings;
IF OBJECT_ID('dbo.HeatingCustomers','U') IS NOT NULL DROP TABLE dbo.HeatingCustomers;
IF OBJECT_ID('dbo.HeatingSystems','U') IS NOT NULL DROP TABLE dbo.HeatingSystems;

CREATE TABLE dbo.HeatingSystems (
    HeatingSystemId INT IDENTITY(1,1) PRIMARY KEY,
    PropertyId INT NOT NULL,
    Name NVARCHAR(150) NOT NULL,
    Location NVARCHAR(250) NULL,
    Notes NVARCHAR(500) NULL,
    Status NVARCHAR(30) NOT NULL DEFAULT 'Active',
    CONSTRAINT FK_HeatingSystems_Properties FOREIGN KEY (PropertyId) REFERENCES dbo.Properties(PropertyId)
);

CREATE TABLE dbo.HeatingCustomers (
    HeatingCustomerId INT IDENTITY(1,1) PRIMARY KEY,
    CounterpartyId INT NOT NULL,
    PropertyId INT NOT NULL,
    UnitId INT NULL,
    BillingMethod NVARCHAR(20) NOT NULL, -- PerM2 | Calorimeter
    AreaM2 DECIMAL(10,2) NULL,
    CalorimeterNo NVARCHAR(50) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_HeatingCustomers_Counterparties FOREIGN KEY (CounterpartyId) REFERENCES dbo.Counterparties(CounterpartyId),
    CONSTRAINT FK_HeatingCustomers_Properties FOREIGN KEY (PropertyId) REFERENCES dbo.Properties(PropertyId),
    CONSTRAINT FK_HeatingCustomers_Units FOREIGN KEY (UnitId) REFERENCES dbo.Units(UnitId)
);

CREATE TABLE dbo.HeatingReadings (
    ReadingId INT IDENTITY(1,1) PRIMARY KEY,
    HeatingCustomerId INT NOT NULL,
    ReadingDate DATE NOT NULL,
    Value DECIMAL(18,3) NOT NULL,
    EnteredByUserId INT NOT NULL,
    EnteredAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Readings_Customers FOREIGN KEY (HeatingCustomerId) REFERENCES dbo.HeatingCustomers(HeatingCustomerId),
    CONSTRAINT FK_Readings_Users FOREIGN KEY (EnteredByUserId) REFERENCES dbo.Users(UserId)
);

-- Rent & Booking skeleton
IF OBJECT_ID('dbo.Bookings','U') IS NOT NULL DROP TABLE dbo.Bookings;
IF OBJECT_ID('dbo.RentContracts','U') IS NOT NULL DROP TABLE dbo.RentContracts;

CREATE TABLE dbo.RentContracts (
    RentContractId INT IDENTITY(1,1) PRIMARY KEY,
    UnitId INT NOT NULL,
    TenantCounterpartyId INT NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NULL,
    MonthlyRent DECIMAL(18,2) NOT NULL,
    IncludesHeating BIT NOT NULL DEFAULT 0,
    Notes NVARCHAR(500) NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Active',
    CONSTRAINT FK_RentContracts_Units FOREIGN KEY (UnitId) REFERENCES dbo.Units(UnitId),
    CONSTRAINT FK_RentContracts_Tenants FOREIGN KEY (TenantCounterpartyId) REFERENCES dbo.Counterparties(CounterpartyId)
);

CREATE TABLE dbo.Bookings (
    BookingId INT IDENTITY(1,1) PRIMARY KEY,
    UnitId INT NOT NULL,
    GuestCounterpartyId INT NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    TotalPrice DECIMAL(18,2) NOT NULL,
    Channel NVARCHAR(50) NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Booked',
    Notes NVARCHAR(500) NULL,
    CONSTRAINT FK_Bookings_Units FOREIGN KEY (UnitId) REFERENCES dbo.Units(UnitId),
    CONSTRAINT FK_Bookings_Guests FOREIGN KEY (GuestCounterpartyId) REFERENCES dbo.Counterparties(CounterpartyId)
);

-- Finance ledger
IF OBJECT_ID('dbo.TransactionAllocations','U') IS NOT NULL DROP TABLE dbo.TransactionAllocations;
IF OBJECT_ID('dbo.Transactions','U') IS NOT NULL DROP TABLE dbo.Transactions;

CREATE TABLE dbo.Transactions (
    TransactionId INT IDENTITY(1,1) PRIMARY KEY,
    TxDate DATE NOT NULL,
    Direction NVARCHAR(10) NOT NULL, -- Income | Expense | Transfer
    CategoryId INT NOT NULL,
    CounterpartyId INT NULL,
    PaidByEntityId INT NOT NULL,
    PrimaryBusinessLineId INT NOT NULL,
    AmountTotal DECIMAL(18,2) NOT NULL,
    Currency NVARCHAR(10) NOT NULL DEFAULT 'RSD',
    Description NVARCHAR(300) NULL,
    UnitId INT NULL,
    RentContractId INT NULL,
    BookingId INT NULL,
    HeatingCustomerId INT NULL,
    CreatedByUserId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Tx_Category FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(CategoryId),
    CONSTRAINT FK_Tx_Counterparty FOREIGN KEY (CounterpartyId) REFERENCES dbo.Counterparties(CounterpartyId),
    CONSTRAINT FK_Tx_PaidByEntity FOREIGN KEY (PaidByEntityId) REFERENCES dbo.Entities(EntityId),
    CONSTRAINT FK_Tx_BusinessLine FOREIGN KEY (PrimaryBusinessLineId) REFERENCES dbo.BusinessLines(BusinessLineId),
    CONSTRAINT FK_Tx_Unit FOREIGN KEY (UnitId) REFERENCES dbo.Units(UnitId),
    CONSTRAINT FK_Tx_RentContract FOREIGN KEY (RentContractId) REFERENCES dbo.RentContracts(RentContractId),
    CONSTRAINT FK_Tx_Booking FOREIGN KEY (BookingId) REFERENCES dbo.Bookings(BookingId),
    CONSTRAINT FK_Tx_HeatingCustomer FOREIGN KEY (HeatingCustomerId) REFERENCES dbo.HeatingCustomers(HeatingCustomerId),
    CONSTRAINT FK_Tx_User FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users(UserId)
);

CREATE TABLE dbo.TransactionAllocations (
    AllocationId INT IDENTITY(1,1) PRIMARY KEY,
    TransactionId INT NOT NULL,
    BeneficiaryEntityId INT NOT NULL,
    BusinessLineId INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Notes NVARCHAR(200) NULL,
    CONSTRAINT FK_Alloc_Tx FOREIGN KEY (TransactionId) REFERENCES dbo.Transactions(TransactionId),
    CONSTRAINT FK_Alloc_Entity FOREIGN KEY (BeneficiaryEntityId) REFERENCES dbo.Entities(EntityId),
    CONSTRAINT FK_Alloc_BL FOREIGN KEY (BusinessLineId) REFERENCES dbo.BusinessLines(BusinessLineId)
);

CREATE INDEX IX_Tx_Date ON dbo.Transactions(TxDate);
CREATE INDEX IX_Tx_Unit ON dbo.Transactions(UnitId);
CREATE INDEX IX_Alloc_Tx ON dbo.TransactionAllocations(TransactionId);
GO

-- Seed
INSERT INTO dbo.Roles(Name) VALUES ('Admin'), ('User');

INSERT INTO dbo.BusinessLines(Name) VALUES
('RentLong'), ('Booking'), ('Heating'), ('Construction'), ('NursingHome'), ('Association'), ('General');

INSERT INTO dbo.Entities(Name, EntityType) VALUES
(N'Top Stan', 'Company'),
(N'Aqua Stan', 'Company'),
(N'Vlasnici (zbirno)', 'Owner');

INSERT INTO dbo.Categories(Name, CategoryType, BusinessLineId)
SELECT N'Zakupnina (prihod)', 'Income', bl.BusinessLineId FROM dbo.BusinessLines bl WHERE bl.Name='RentLong'
UNION ALL SELECT N'Booking prihod', 'Income', (SELECT BusinessLineId FROM dbo.BusinessLines WHERE Name='Booking')
UNION ALL SELECT N'Grejanje prihod', 'Income', (SELECT BusinessLineId FROM dbo.BusinessLines WHERE Name='Heating')
UNION ALL SELECT N'Odrzavanje zgrade (prihod)', 'Income', (SELECT BusinessLineId FROM dbo.BusinessLines WHERE Name='Association')
UNION ALL SELECT N'Čišćenje zgrade', 'Expense', (SELECT BusinessLineId FROM dbo.BusinessLines WHERE Name='Association')
UNION ALL SELECT N'Majstori i popravke', 'Expense', (SELECT BusinessLineId FROM dbo.BusinessLines WHERE Name='General')
UNION ALL SELECT N'Trošak grejanja (gorivo/servis)', 'Expense', (SELECT BusinessLineId FROM dbo.BusinessLines WHERE Name='Heating')
UNION ALL SELECT N'Interni transfer', 'Transfer', (SELECT BusinessLineId FROM dbo.BusinessLines WHERE Name='General');
GO
