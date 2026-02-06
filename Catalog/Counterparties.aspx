<%@ Page Language="C#" MasterPageFile="~/Master/Site.master" AutoEventWireup="true" CodeFile="Counterparties.aspx.cs" Inherits="PropertyOpsWebForms.Catalog.Counterparties" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="card">
  <h1>Komitenti</h1>
  <div class="inline">
    <div><label>Tip</label><asp:DropDownList ID='ddlType' runat='server'/></div><div><label>Ime / Naziv</label><asp:TextBox ID='txtFull' runat='server'/></div><div><label>Telefon</label><asp:TextBox ID='txtPhone' runat='server'/></div><div><label>PIB</label><asp:TextBox ID='txtPIB' runat='server'/></div>
    <div><asp:Button CssClass="btn" ID="btnAdd" runat="server" Text="Add" OnClick="btnAdd_Click"/></div>
  </div>
  <asp:Label ID="lblMsg" runat="server"/>
  <asp:GridView ID="gv" runat="server" CssClass="grid" AutoGenerateColumns="true"/>
</div>
</asp:Content>
