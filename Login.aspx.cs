using System;
using System.Web.Security;
using PropertyOps.App;

namespace PropertyOpsWebForms
{
    public partial class Login : System.Web.UI.Page
    {
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            var userId = Security.ValidateUser(txtUser.Text.Trim(), txtPass.Text);
            if (!userId.HasValue)
            {
                lblMsg.Text = "<div class='msg err'>Pogrešan username ili lozinka.</div>";
                return;
            }

            // Create auth ticket with userId as Name
            FormsAuthentication.SetAuthCookie(userId.Value.ToString(), false);
            Response.Redirect("~/Default.aspx");
        }
    }
}