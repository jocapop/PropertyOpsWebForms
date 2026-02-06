using System;
using PropertyOps.App;

namespace PropertyOpsWebForms.Catalog
{
    public partial class Properties : PropertyOpsWebForms.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                Bind();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                Db.Exec(@"INSERT INTO dbo.Properties(Name,Address) VALUES (@txtName,@txtAddr)", Db.P("@txtName", txtName.Text.Trim()), Db.P("@txtAddr", txtAddr.Text.Trim()));
                lblMsg.Text = "<div class='msg ok'>Snimljeno.</div>";
                Bind();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "<div class='msg err'>Greška: " + Server.HtmlEncode(ex.Message) + "</div>";
            }
        }

        private void Bind()
        {
            gv.DataSource = Db.Query(@"SELECT PropertyId, Name, Address FROM dbo.Properties ORDER BY PropertyId DESC");
            gv.DataBind();
        }
    }
}