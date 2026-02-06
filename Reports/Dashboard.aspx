<%@ Page Language="C#" MasterPageFile="~/Master/Site.master" AutoEventWireup="true" CodeFile="Dashboard.aspx.cs" Inherits="PropertyOpsWebForms.Reports.Dashboard" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
  <h2>Dashboard</h2>
  <div class="card">
    <h3>Prihodi/Rashodi (poslednjih 12 meseci)</h3>
    <asp:GridView ID="gv12m" runat="server" AutoGenerateColumns="true" CssClass="grid"></asp:GridView>
  </div>

  <div class="card">
    <h3>Prihodi/Rashodi po entitetu (tekuća godina)</h3>
    <asp:GridView ID="gvEntity" runat="server" AutoGenerateColumns="true" CssClass="grid"></asp:GridView>
  </div>
</asp:Content>
