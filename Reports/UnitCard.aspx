<%@ Page Language="C#" MasterPageFile="~/Master/Site.master" AutoEventWireup="true" CodeFile="UnitCard.aspx.cs" Inherits="PropertyOpsWebForms.Reports.UnitCard" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
  <h2>Kartica stana / jedinice</h2>
  <div class="card">
    <asp:Label ID="lblMsg" runat="server" />
    <div>
      Jedinica:
      <asp:DropDownList ID="ddlUnit" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlUnit_SelectedIndexChanged"></asp:DropDownList>
    </div>
  </div>

  <div class="card">
    <h3>Prihodi/Rashodi</h3>
    <asp:GridView ID="gvTx" runat="server" AutoGenerateColumns="true" CssClass="grid"></asp:GridView>
  </div>

  <div class="card">
    <h3>Sažetak</h3>
    <asp:GridView ID="gvSum" runat="server" AutoGenerateColumns="true" CssClass="grid"></asp:GridView>
  </div>
</asp:Content>
