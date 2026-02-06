<%@ Page Language="C#" MasterPageFile="~/Master/Site.master" AutoEventWireup="true" CodeFile="TransactionNew.aspx.cs" Inherits="PropertyOpsWebForms.Finance.TransactionNew" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="card">
  <h1>New Transaction (Prihod/Rashod/Transfer)</h1>
  <div class="inline">
    <div><label>Datum (YYYY-MM-DD)</label><asp:TextBox ID="txtDate" runat="server"/></div>
    <div><label>Direction</label>
      <asp:DropDownList ID="ddlDir" runat="server">
        <asp:ListItem>Income</asp:ListItem>
        <asp:ListItem>Expense</asp:ListItem>
        <asp:ListItem>Transfer</asp:ListItem>
      </asp:DropDownList>
    </div>
    <div><label>Kategorija</label><asp:DropDownList ID="ddlCat" runat="server"/></div>
    <div><label>Paid by (ko je platio)</label><asp:DropDownList ID="ddlPaidBy" runat="server"/></div>
    <div><label>Linija posla</label><asp:DropDownList ID="ddlBL" runat="server"/></div>
    <div><label>Iznos (RSD)</label><asp:TextBox ID="txtAmt" runat="server"/></div>
  </div>

  <div class="inline">
    <div><label>Komitent (opciono)</label><asp:DropDownList ID="ddlCp" runat="server"/></div>
    <div><label>Stan/Jedinica (opciono)</label><asp:DropDownList ID="ddlUnit" runat="server"/></div>
    <div><label>Stambena zajednica (opciono)</label><asp:DropDownList ID="ddlAssoc" runat="server"/></div>
  </div>

  <label>Opis</label>
  <asp:TextBox ID="txtDesc" runat="server" TextMode="MultiLine" Rows="2" />

  <hr />
  <h2>Raspodela (Allocations)</h2>
  <p class="small">Unesi jednu ili više raspodela (npr. Top Stan 70%, Aqua Stan 30%). Iznos raspodele mora da bude jednak totalu.</p>

  <div class="inline">
    <div><label>Entitet</label><asp:DropDownList ID="ddlAllocEntity" runat="server"/></div>
    <div><label>Linija posla</label><asp:DropDownList ID="ddlAllocBL" runat="server"/></div>
    <div><label>Iznos</label><asp:TextBox ID="txtAllocAmt" runat="server"/></div>
    <div><label>Napomena</label><asp:TextBox ID="txtAllocNote" runat="server"/></div>
    <div><asp:Button CssClass="btn secondary" ID="btnAddAlloc" runat="server" Text="Add alloc" OnClick="btnAddAlloc_Click"/></div>
  </div>

  <asp:GridView ID="gvAlloc" runat="server" CssClass="grid" AutoGenerateColumns="false" OnRowCommand="gvAlloc_RowCommand">
    <Columns>
      <asp:BoundField DataField="EntityName" HeaderText="Entity" />
      <asp:BoundField DataField="BusinessLine" HeaderText="BusinessLine" />
      <asp:BoundField DataField="Amount" HeaderText="Amount" />
      <asp:BoundField DataField="Notes" HeaderText="Notes" />
      <asp:ButtonField CommandName="Del" Text="Delete" ButtonType="Button" />
    </Columns>
  </asp:GridView>

  <div style="margin-top:10px;">
    <asp:Button CssClass="btn" ID="btnSave" runat="server" Text="Save transaction" OnClick="btnSave_Click"/>
  </div>

  <asp:Label ID="lblMsg" runat="server"/>
</div>
</asp:Content>
