PROPERTYOPS V2 (WebForms, .NET Framework 4.8)

1) SQL (SSMS):
   - db/01_create_db_and_core.sql
   - db/02_associations.sql
   - db/03_v2_upgrades.sql

2) Web.config:
   - connectionStrings/AppDb (SQL Server + DB name PropertyOps_v1)

3) Login:
   - Seed admin (ako nema korisnika): admin / admin123!

NAPOMENA (zašto nema DataSet/DataSource connection stringa):
- Aplikacija koristi ADO.NET (SqlConnection/SqlCommand) kroz App_Code/Db.cs,
  pa ne koristi Visual Studio "DataSet" dizajnere niti "SqlDataSource" kontrole.
