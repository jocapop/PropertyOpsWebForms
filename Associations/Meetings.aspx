<%@ Page Language="C#" MasterPageFile="~/Master/Site.master" AutoEventWireup="true" CodeFile="Meetings.aspx.cs" Inherits="PropertyOpsWebForms.Associations.Meetings" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="card">
  <h1>Zapisnici sa sastanaka (Stambena zajednica)</h1>
  <div class="inline">
    <div><label>Zgrada</label><asp:DropDownList ID="ddlAssoc" runat="server"/></div>
    <div><label>Datum i vreme (YYYY-MM-DD HH:MM)</label><asp:TextBox ID="txtDT" runat="server"/></div>
    <div><label>Naslov</label><asp:TextBox ID="txtTitle" runat="server"/></div>
  </div>

  <label>Dnevni red (Agenda)</label>
  <asp:TextBox ID="txtAgenda" runat="server" TextMode="MultiLine" Rows="3" />

  <label>Zapisnik (Minutes)</label>
  <asp:TextBox ID="txtMinutes" runat="server" TextMode="MultiLine" Rows="6" />

  <label>Odluke (Decisions)</label>
  <asp:TextBox ID="txtDec" runat="server" TextMode="MultiLine" Rows="3" />

  <div style="margin-top:10px;">
    <asp:Button CssClass="btn" ID="btnAdd" runat="server" Text="Save meeting" OnClick="btnAdd_Click"/>
  </div>

  <asp:Label ID="lblMsg" runat="server"/>
  <asp:GridView ID="gv" runat="server" CssClass="grid" AutoGenerateColumns="true"/>
</div>
</asp:Content>
