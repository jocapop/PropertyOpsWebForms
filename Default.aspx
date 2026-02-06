<%@ Page Language="C#" MasterPageFile="~/Master/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="PropertyOpsWebForms.Default" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
  <div class="card">
    <h1>Home</h1>
    <p>Ovo je V1 aplikacije za evidenciju prihoda/rashoda, stambenih zajednica i osnovnih registara.</p>
    <ul>
      <li><b>Najbitnije:</b> Finance → New Tx (unos transakcije + raspodela)</li>
      <li>Associations → Charges/Payments (zaduženja i uplate po zgradi)</li>
      <li>Reports → Dashboard (sumarno po entitetu/liniji)</li>
    </ul>
    <p class="small">Seed admin: username <b>admin</b>, password <b>admin123!</b> (prvi start ako nema korisnika).</p>
  </div>
</asp:Content>
