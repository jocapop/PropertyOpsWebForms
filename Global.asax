<%@ Application Language="C#" %>
<script runat="server">
    void Application_Start(object sender, EventArgs e) 
    {
        // Seed admin user if DB is empty
        try { PropertyOps.App.Security.EnsureSeedAdmin(); } catch { /* ignore */ }
    }
</script>
