<%@ Page Language="C#" MasterPageFile="~/Master/Site.master" AutoEventWireup="true" CodeFile="Users.aspx.cs" Inherits="PropertyOpsWebForms.Admin.Users" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="card">
  <h1>Admin - Users</h1>
  <div class="inline">
    <div><label>Username</label><asp:TextBox ID="txtU" runat="server"/></div>
    <div><label>Password</label><asp:TextBox ID="txtP" TextMode="Password" runat="server"/></div>
    <div><label>Role</label>
      <asp:DropDownList ID="ddlRole" runat="server">
        <asp:ListItem>Admin</asp:ListItem>
        <asp:ListItem>User</asp:ListItem>
      </asp:DropDownList>
    </div>
    <div><asp:Button CssClass="btn" ID="btnAdd" runat="server" Text="Add user" OnClick="btnAdd_Click"/></div>
  </div>
  <asp:Label ID="lblMsg" runat="server"/>
  <asp:GridView ID="gv" runat="server" CssClass="grid" AutoGenerateColumns="true"/>
</div>
</asp:Content>
