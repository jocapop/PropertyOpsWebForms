<%@ Page Language="C#" MasterPageFile="~/Master/Site.master" AutoEventWireup="true" CodeFile="Entities.aspx.cs" Inherits="PropertyOpsWebForms.Catalog.Entities" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="card">
  <h1>Entiteti (Top/Aqua/Vlasnici/SZ)</h1>
  <div class="inline">
    <div><label>Naziv</label><asp:TextBox ID='txtName' runat='server'/></div><div><label>Tip</label><asp:DropDownList ID='ddlType' runat='server'/></div><div><label>PIB</label><asp:TextBox ID='txtTax' runat='server'/></div><div><label>Maticni</label><asp:TextBox ID='txtReg' runat='server'/></div>
    <div><asp:Button CssClass="btn" ID="btnAdd" runat="server" Text="Add" OnClick="btnAdd_Click"/></div>
  </div>
  <asp:Label ID="lblMsg" runat="server"/>
  <asp:GridView ID="gv" runat="server" CssClass="grid" AutoGenerateColumns="true"/>
</div>
</asp:Content>
