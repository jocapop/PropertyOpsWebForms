<%@ Page Language="C#" MasterPageFile="~/Master/Site.master" AutoEventWireup="true" CodeFile="Payments.aspx.cs" Inherits="PropertyOpsWebForms.Associations.Payments" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="card">
  <h1>Uplate po zgradi</h1>

  <div class="inline">
    <div><label>Zgrada</label><asp:DropDownList ID="ddlAssoc" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlAssoc_SelectedIndexChanged"/></div>
    <div><label>Stan</label><asp:DropDownList ID="ddlUnit" runat="server"/></div>
    <div><label>Datum (YYYY-MM-DD)</label><asp:TextBox ID="txtDate" runat="server"/></div>
    <div><label>Iznos</label><asp:TextBox ID="txtAmt" runat="server"/></div>
    <div><label>Godina</label><asp:TextBox ID="txtY" runat="server"/></div>
    <div><label>Mesec</label><asp:TextBox ID="txtM" runat="server"/></div>
    <div><asp:Button CssClass="btn" ID="btnAdd" runat="server" Text="Add payment" OnClick="btnAdd_Click"/></div>
  </div>

  <asp:Label ID="lblMsg" runat="server"/>

  <hr />
  <h2>Ko nije platio (za izabrani period)</h2>
  <div class="inline">
    <div><label>Godina</label><asp:TextBox ID="txtRY" runat="server"/></div>
    <div><label>Mesec</label><asp:TextBox ID="txtRM" runat="server"/></div>
    <div><asp:Button CssClass="btn secondary" ID="btnRun" runat="server" Text="Show unpaid" OnClick="btnRun_Click"/></div>
  </div>

  <asp:GridView ID="gv" runat="server" CssClass="grid" AutoGenerateColumns="true"/>
</div>
</asp:Content>
