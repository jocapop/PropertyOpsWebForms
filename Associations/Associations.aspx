<%@ Page Language="C#" MasterPageFile="~/Master/Site.master" AutoEventWireup="true" CodeFile="Associations.aspx.cs" Inherits="PropertyOpsWebForms.Associations.Associations" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="card">
  <h1>Stambene zajednice (po zgradi)</h1>
  <p class="small">Svaka zgrada ima svoj PIB (Entity tip HOA). Ovde vezuješ zgradu (Property) na Entity stambene zajednice i tarifni iznos po stanu.</p>

  <div class="inline">
    <div><label>Zgrada (Property)</label><asp:DropDownList ID="ddlProp" runat="server"/></div>
    <div><label>Entity (HOA)</label><asp:DropDownList ID="ddlEnt" runat="server"/></div>
    <div><label>Mesečno po stanu (RSD)</label><asp:TextBox ID="txtAmt" runat="server"/></div>
    <div><asp:Button CssClass="btn" ID="btnAdd" runat="server" Text="Add/Update" OnClick="btnAdd_Click"/></div>
  </div>

  <asp:Label ID="lblMsg" runat="server"/>
  <asp:GridView ID="gv" runat="server" CssClass="grid" AutoGenerateColumns="true"/>
</div>
</asp:Content>
