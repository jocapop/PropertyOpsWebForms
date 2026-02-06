<%@ Page Language="C#" MasterPageFile="~/Master/Site.master" AutoEventWireup="true" CodeFile="Units.aspx.cs" Inherits="PropertyOpsWebForms.Catalog.Units" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="card">
  <h1>Jedinice (stan/soba)</h1>
  <div class="inline">
    <div><label>Objekat</label><asp:DropDownList ID='ddlProp' runat='server'/></div><div><label>Oznaka</label><asp:TextBox ID='txtCode' runat='server'/></div><div><label>Namena</label><asp:DropDownList ID='ddlUsage' runat='server'/></div><div><label>m2</label><asp:TextBox ID='txtArea' runat='server'/></div>
    <div><asp:Button CssClass="btn" ID="btnAdd" runat="server" Text="Add" OnClick="btnAdd_Click"/></div>
  </div>
  <asp:Label ID="lblMsg" runat="server"/>
  <asp:GridView ID="gv" runat="server" CssClass="grid" AutoGenerateColumns="true"/>
</div>
</asp:Content>
