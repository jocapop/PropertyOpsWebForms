using System;
using PropertyOps.App;

namespace PropertyOpsWebForms.Catalog
{
    public partial class Entities : PropertyOpsWebForms.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                var dt_ddlType = Db.Query(@"SELECT 'Company' AS Val,'Company' AS Txt UNION ALL SELECT 'Owner','Owner' UNION ALL SELECT 'HOA','HOA' UNION ALL SELECT 'Other','Other'");
                ddlType.DataSource = dt_ddlType;
                ddlType.DataTextField = "Txt";
                ddlType.DataValueField = "Val";
                ddlType.DataBind();

                Bind();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                Db.Exec(@"INSERT INTO dbo.Entities(Name,EntityType,TaxId,RegNo) VALUES (@txtName,@ddlType,@txtTax,@txtReg)", Db.P("@txtName", txtName.Text.Trim()), Db.P("@ddlType", ddlType.SelectedValue), Db.P("@txtTax", txtTax.Text.Trim()), Db.P("@txtReg", txtReg.Text.Trim()));
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
            gv.DataSource = Db.Query(@"SELECT EntityId, Name, EntityType, TaxId, RegNo, IsActive FROM dbo.Entities ORDER BY EntityId DESC");
            gv.DataBind();
        }
    }
}