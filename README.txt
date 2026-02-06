PROPERTYOPS - WebForms (.NET Framework 4.8) - V1

1) SQL baza
   - Otvori /db/01_create_db_and_core.sql i izvrši u SSMS
   - Zatim izvrši /db/02_associations.sql

2) Podešavanje konekcije
   - U Web.config promeni connectionString:
     Data Source=YOUR_SQL_SERVER;Initial Catalog=PropertyOps_v1;Integrated Security=True;...

3) Start iz Visual Studio 2022 (ili 2017)
   - Otvori PropertyOpsWebForms.sln
   - Build + Run (IIS Express)
   - Prvi start, ako nema korisnika, aplikacija kreira admin nalog:
     username: admin
     password: admin123!

4) Moduli koji su "ready" u V1
   - Finance: New Tx + Transactions + CSV export
   - Associations: definicija SZ po zgradi + zaduženja + uplate + 'ko nije platio' + zapisnici
   - Catalog: Entities, Counterparties, Properties, Units, Categories
   - Reports: Dashboard (po entitetu i po liniji posla, na osnovu allocations)

5) Moduli koji su skeleton (V2)
   - Rent / Booking / Heating / Construction / NursingHome

Napomena:
- Raspodela (allocations) je OBAVEZNA za svaku transakciju (da bi dashboard radio).
- Za stambene zajednice: napravi prvo Entity (Catalog -> Entiteti) sa EntityType = HOA i PIB.
  Zatim Catalog -> Objekti (zgrada), pa Units (stanovi), pa Associations -> Stambene zajednice (vezivanje).
