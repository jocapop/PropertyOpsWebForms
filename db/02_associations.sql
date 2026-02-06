USE PropertyOps_v1;
GO

IF OBJECT_ID('dbo.Attachments','U') IS NOT NULL DROP TABLE dbo.Attachments;
IF OBJECT_ID('dbo.AssociationMeetings','U') IS NOT NULL DROP TABLE dbo.AssociationMeetings;
IF OBJECT_ID('dbo.AssociationPayments','U') IS NOT NULL DROP TABLE dbo.AssociationPayments;
IF OBJECT_ID('dbo.AssociationChargeItems','U') IS NOT NULL DROP TABLE dbo.AssociationChargeItems;
IF OBJECT_ID('dbo.AssociationCharges','U') IS NOT NULL DROP TABLE dbo.AssociationCharges;
IF OBJECT_ID('dbo.AssociationUnits','U') IS NOT NULL DROP TABLE dbo.AssociationUnits;
IF OBJECT_ID('dbo.BuildingAssociations','U') IS NOT NULL DROP TABLE dbo.BuildingAssociations;

CREATE TABLE dbo.BuildingAssociations (
    AssociationId INT IDENTITY(1,1) PRIMARY KEY,
    PropertyId INT NOT NULL,
    EntityId INT NOT NULL, -- Entity type HOA (one per building)
    DefaultChargeMethod NVARCHAR(20) NOT NULL DEFAULT 'PerUnit',
    DefaultMonthlyAmount DECIMAL(18,2) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Assoc_Property FOREIGN KEY (PropertyId) REFERENCES dbo.Properties(PropertyId),
    CONSTRAINT FK_Assoc_Entity FOREIGN KEY (EntityId) REFERENCES dbo.Entities(EntityId)
);

CREATE TABLE dbo.AssociationUnits (
    AssociationUnitId INT IDENTITY(1,1) PRIMARY KEY,
    AssociationId INT NOT NULL,
    UnitId INT NOT NULL,
    ActiveFrom DATE NOT NULL DEFAULT GETDATE(),
    ActiveTo DATE NULL,
    CONSTRAINT UQ_AssocUnit UNIQUE(AssociationId, UnitId),
    CONSTRAINT FK_AssocUnits_Assoc FOREIGN KEY (AssociationId) REFERENCES dbo.BuildingAssociations(AssociationId),
    CONSTRAINT FK_AssocUnits_Unit FOREIGN KEY (UnitId) REFERENCES dbo.Units(UnitId)
);

CREATE TABLE dbo.AssociationCharges (
    ChargeId INT IDENTITY(1,1) PRIMARY KEY,
    AssociationId INT NOT NULL,
    PeriodYear INT NOT NULL,
    PeriodMonth INT NOT NULL,
    ChargeMethod NVARCHAR(20) NOT NULL, -- PerUnit
    MonthlyAmountPerUnit DECIMAL(18,2) NOT NULL,
    Notes NVARCHAR(300) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT UQ_AssocCharges UNIQUE(AssociationId, PeriodYear, PeriodMonth),
    CONSTRAINT FK_AssocCharges_Assoc FOREIGN KEY (AssociationId) REFERENCES dbo.BuildingAssociations(AssociationId)
);

CREATE TABLE dbo.AssociationChargeItems (
    ChargeItemId INT IDENTITY(1,1) PRIMARY KEY,
    ChargeId INT NOT NULL,
    UnitId INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    DueDate DATE NULL,
    CONSTRAINT UQ_ChargeItem UNIQUE(ChargeId, UnitId),
    CONSTRAINT FK_ChargeItems_Charge FOREIGN KEY (ChargeId) REFERENCES dbo.AssociationCharges(ChargeId),
    CONSTRAINT FK_ChargeItems_Unit FOREIGN KEY (UnitId) REFERENCES dbo.Units(UnitId)
);

CREATE TABLE dbo.AssociationPayments (
    PaymentId INT IDENTITY(1,1) PRIMARY KEY,
    AssociationId INT NOT NULL,
    UnitId INT NOT NULL,
    CounterpartyId INT NULL,
    PaymentDate DATE NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    PeriodYear INT NULL,
    PeriodMonth INT NULL,
    ReferenceNo NVARCHAR(50) NULL,
    Notes NVARCHAR(300) NULL,
    CreatedByUserId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_AssocPay_Assoc FOREIGN KEY (AssociationId) REFERENCES dbo.BuildingAssociations(AssociationId),
    CONSTRAINT FK_AssocPay_Unit FOREIGN KEY (UnitId) REFERENCES dbo.Units(UnitId),
    CONSTRAINT FK_AssocPay_Counterparty FOREIGN KEY (CounterpartyId) REFERENCES dbo.Counterparties(CounterpartyId),
    CONSTRAINT FK_AssocPay_User FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users(UserId)
);

CREATE TABLE dbo.AssociationMeetings (
    MeetingId INT IDENTITY(1,1) PRIMARY KEY,
    AssociationId INT NOT NULL,
    MeetingDate DATETIME2 NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Agenda NVARCHAR(MAX) NULL,
    Minutes NVARCHAR(MAX) NULL,
    Decisions NVARCHAR(MAX) NULL,
    CreatedByUserId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Meetings_Assoc FOREIGN KEY (AssociationId) REFERENCES dbo.BuildingAssociations(AssociationId),
    CONSTRAINT FK_Meetings_User FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users(UserId)
);

CREATE TABLE dbo.Attachments (
    AttachmentId INT IDENTITY(1,1) PRIMARY KEY,
    OwnerType NVARCHAR(50) NOT NULL,
    OwnerId INT NOT NULL,
    FileName NVARCHAR(255) NOT NULL,
    FilePath NVARCHAR(500) NOT NULL,
    UploadedByUserId INT NOT NULL,
    UploadedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Att_User FOREIGN KEY (UploadedByUserId) REFERENCES dbo.Users(UserId)
);
GO

CREATE INDEX IX_AssocPayments_Period ON dbo.AssociationPayments(AssociationId, PeriodYear, PeriodMonth);
CREATE INDEX IX_AssocChargeItems_Unit ON dbo.AssociationChargeItems(UnitId);
GO
