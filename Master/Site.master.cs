using System;
using System.Web;
using System.Web.Security;
using PropertyOps.App;

namespace PropertyOpsWebForms.Master
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated) return;

            var userId = TryGetUserId();
            if (userId.HasValue)
            {
                lblUser.Text = "User: " + Security.GetUsername(userId.Value);
            }
            else lblUser.Text = "";
        }

        private int? TryGetUserId()
        {
            //// Stored in auth ticket UserData
            //var idStr = Context.User.Identity.Name;
            //int id;
            //int id;
            //if (int.TryParse) return id;
            //return null;
            var idStr = Context.User.Identity.Name;

            int id;
            if (int.TryParse(idStr, out id)) return id;

            return null;
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Response.Redirect("~/Login.aspx");
        }
    }
}