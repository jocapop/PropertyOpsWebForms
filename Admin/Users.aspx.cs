using System;
using System.Data;
using System.Data.SqlClient;
using PropertyOps.App;

namespace PropertyOpsWebForms.Admin
{
    public partial class Users : PropertyOpsWebForms.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsAdmin()) { Response.Redirect("~/Default.aspx"); return; }
            if (!IsPostBack) Bind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var u = txtU.Text.Trim();
                var p = txtP.Text;
                if (u.Length < 3 || p.Length < 6)
                {
                    lblMsg.Text = "<div class='msg err'>Username min 3, password min 6 karaktera.</div>";
                    return;
                }

                var salt = Security.NewSalt();
                var hash = Security.HashPassword(p, salt);

                Db.Exec("INSERT INTO dbo.Users(Username,PasswordHash,PasswordSalt,IsActive) VALUES (@u,@h,@s,1)",
                    Db.P("@u", u), Db.P("@h", hash), Db.P("@s", salt));

                var userId = Convert.ToInt32(Db.Scalar("SELECT UserId FROM dbo.Users WHERE Username=@u", Db.P("@u", u)));
                var roleId = Convert.ToInt32(Db.Scalar("SELECT RoleId FROM dbo.Roles WHERE Name=@r", Db.P("@r", ddlRole.SelectedValue)));
                Db.Exec("INSERT INTO dbo.UserRoles(UserId,RoleId) VALUES (@u,@r)", Db.P("@u", userId), Db.P("@r", roleId));

                lblMsg.Text = "<div class='msg ok'>Korisnik dodat.</div>";
                txtU.Text = ""; txtP.Text = "";
                Bind();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "<div class='msg err'>Greška: " + Server.HtmlEncode(ex.Message) + "</div>";
            }
        }

        private void Bind()
        {
            var dt = Db.Query(@"
SELECT u.UserId, u.Username, u.IsActive,
       STUFF((SELECT ',' + r.Name FROM dbo.UserRoles ur JOIN dbo.Roles r ON r.RoleId=ur.RoleId WHERE ur.UserId=u.UserId FOR XML PATH('')),1,1,'') AS Roles
FROM dbo.Users u
ORDER BY u.UserId DESC");
            gv.DataSource = dt;
            gv.DataBind();
        }
    }
}