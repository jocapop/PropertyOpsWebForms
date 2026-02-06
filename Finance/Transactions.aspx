<%@ Page Language="C#" MasterPageFile="~/Master/Site.master" AutoEventWireup="true" CodeFile="Transactions.aspx.cs" Inherits="PropertyOpsWebForms.Finance.Transactions" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="card">
  <h1>Transactions</h1>
  <div class="inline">
    <div><label>Od (YYYY-MM-DD)</label><asp:TextBox ID="txtFrom" runat="server"/></div>
    <div><label>Do (YYYY-MM-DD)</label><asp:TextBox ID="txtTo" runat="server"/></div>
    <div><label>Entitet (paid by)</label><asp:DropDownList ID="ddlEnt" runat="server"/></div>
    <div><asp:Button CssClass="btn secondary" ID="btnFilter" runat="server" Text="Filter" OnClick="btnFilter_Click"/></div>
    <div><asp:Button CssClass="btn" ID="btnCsv" runat="server" Text="Export CSV" OnClick="btnCsv_Click"/></div>
  </div>
  <asp:GridView ID="gv" runat="server" CssClass="grid" AutoGenerateColumns="true"/>
  <asp:Label ID="lblMsg" runat="server"/>
</div>
</asp:Content>
