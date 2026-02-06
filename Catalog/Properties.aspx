<%@ Page Language="C#" MasterPageFile="~/Master/Site.master" AutoEventWireup="true" CodeFile="Properties.aspx.cs" Inherits="PropertyOpsWebForms.Catalog.Properties" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="card">
  <h1>Objekti (zgrade)</h1>
  <div class="inline">
    <div><label>Naziv</label><asp:TextBox ID='txtName' runat='server'/></div><div><label>Adresa</label><asp:TextBox ID='txtAddr' runat='server'/></div>
    <div><asp:Button CssClass="btn" ID="btnAdd" runat="server" Text="Add" OnClick="btnAdd_Click"/></div>
  </div>
  <asp:Label ID="lblMsg" runat="server"/>
  <asp:GridView ID="gv" runat="server" CssClass="grid" AutoGenerateColumns="true"/>
</div>
</asp:Content>
