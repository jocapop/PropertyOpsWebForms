using System;
using PropertyOps.App;

namespace PropertyOpsWebForms.Catalog
{
    public partial class Categories : PropertyOpsWebForms.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                var dt_ddlType = Db.Query(@"SELECT 'Income' Val,'Income' Txt UNION ALL SELECT 'Expense','Expense' UNION ALL SELECT 'Transfer','Transfer'");
                ddlType.DataSource = dt_ddlType;
                ddlType.DataTextField = "Txt";
                ddlType.DataValueField = "Val";
                ddlType.DataBind();

                var dt_ddlBL = Db.Query(@"SELECT BusinessLineId, Name FROM dbo.BusinessLines ORDER BY Name");
                ddlBL.DataSource = dt_ddlBL;
                ddlBL.DataTextField = "Name";
                ddlBL.DataValueField = "BusinessLineId";
                ddlBL.DataBind();

                Bind();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                Db.Exec(@"INSERT INTO dbo.Categories(Name,CategoryType,BusinessLineId) VALUES (@txtName,@ddlType,@ddlBL)", Db.P("@txtName", txtName.Text.Trim()), Db.P("@ddlType", ddlType.SelectedValue), Db.P("@ddlBL", ddlBL.SelectedValue));
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
            gv.DataSource = Db.Query(@"SELECT c.CategoryId, c.Name, c.CategoryType, bl.Name AS BusinessLine FROM dbo.Categories c LEFT JOIN dbo.BusinessLines bl ON bl.BusinessLineId=c.BusinessLineId ORDER BY c.CategoryId DESC");
            gv.DataBind();
        }
    }
}