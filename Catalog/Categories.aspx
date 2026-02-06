<%@ Page Language="C#" MasterPageFile="~/Master/Site.master" AutoEventWireup="true" CodeFile="Categories.aspx.cs" Inherits="PropertyOpsWebForms.Catalog.Categories" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="card">
  <h1>Kategorije</h1>
  <div class="inline">
    <div><label>Naziv</label><asp:TextBox ID='txtName' runat='server'/></div><div><label>Tip</label><asp:DropDownList ID='ddlType' runat='server'/></div><div><label>Linija</label><asp:DropDownList ID='ddlBL' runat='server'/></div>
    <div><asp:Button CssClass="btn" ID="btnAdd" runat="server" Text="Add" OnClick="btnAdd_Click"/></div>
  </div>
  <asp:Label ID="lblMsg" runat="server"/>
  <asp:GridView ID="gv" runat="server" CssClass="grid" AutoGenerateColumns="true"/>
</div>
</asp:Content>
