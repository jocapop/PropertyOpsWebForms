<%@ Page Language="C#" MasterPageFile="~/Master/Site.master" AutoEventWireup="true" CodeFile="Charges.aspx.cs" Inherits="PropertyOpsWebForms.Associations.Charges" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="card">
  <h1>Zaduženja po zgradi (mesečno po stanu)</h1>
  <div class="inline">
    <div><label>Zgrada</label><asp:DropDownList ID="ddlAssoc" runat="server"/></div>
    <div><label>Godina</label><asp:TextBox ID="txtY" runat="server"/></div>
    <div><label>Mesec (1-12)</label><asp:TextBox ID="txtM" runat="server"/></div>
    <div><label>Iznos po stanu (RSD)</label><asp:TextBox ID="txtAmt" runat="server"/></div>
    <div><asp:Button CssClass="btn" ID="btnGen" runat="server" Text="Create charge + generate items" OnClick="btnGen_Click"/></div>
  </div>
  <asp:Label ID="lblMsg" runat="server"/>
  <asp:GridView ID="gv" runat="server" CssClass="grid" AutoGenerateColumns="true"/>
</div>
</asp:Content>
