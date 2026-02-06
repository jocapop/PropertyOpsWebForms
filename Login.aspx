<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="PropertyOpsWebForms.Login" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Login - PropertyOps</title>
    <link href="<%= ResolveClientUrl("~/Content/site.css") %>" rel="stylesheet" />
</head>
<body>
<form runat="server">
  <div class="container">
    <div class="card" style="max-width:520px;margin:40px auto;">
      <h1>Login</h1>
      <label>Username</label>
      <asp:TextBox ID="txtUser" runat="server" />
      <label>Password</label>
      <asp:TextBox ID="txtPass" TextMode="Password" runat="server" />
      <div style="margin-top:10px;">
        <asp:Button CssClass="btn" ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" />
      </div>
      <asp:Label ID="lblMsg" runat="server" />
      <p class="small">Ako je prazna baza korisnika: admin / admin123!</p>
        <p class="small">v3 verzija</p>
    </div>
  </div>
</form>
</body>
</html>
