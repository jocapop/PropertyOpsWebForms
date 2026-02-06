using System;
using PropertyOps.App;

namespace PropertyOpsWebForms.Catalog
{
    public partial class Units : PropertyOpsWebForms.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                var dt_ddlProp = Db.Query(@"SELECT PropertyId, Name FROM dbo.Properties ORDER BY Name");
                ddlProp.DataSource = dt_ddlProp;
                ddlProp.DataTextField = "Name";
                ddlProp.DataValueField = "PropertyId";
                ddlProp.DataBind();

                var dt_ddlUsage = Db.Query(@"SELECT 'RentLong' Val,'RentLong' Txt UNION ALL SELECT 'Booking','Booking' UNION ALL SELECT 'Sale','Sale' UNION ALL SELECT 'NursingHome','NursingHome'");
                ddlUsage.DataSource = dt_ddlUsage;
                ddlUsage.DataTextField = "Txt";
                ddlUsage.DataValueField = "Val";
                ddlUsage.DataBind();

                Bind();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                Db.Exec(@"INSERT INTO dbo.Units(PropertyId,UnitCode,UnitType,AreaM2,Usage) VALUES (@ddlProp,@txtCode,'Apartment',TRY_CONVERT(decimal(10,2),@txtArea),@ddlUsage)", Db.P("@ddlProp", ddlProp.SelectedValue), Db.P("@txtCode", txtCode.Text.Trim()), Db.P("@ddlUsage", ddlUsage.SelectedValue), Db.P("@txtArea", txtArea.Text.Trim()));
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
            gv.DataSource = Db.Query(@"SELECT u.UnitId, p.Name AS Property, u.UnitCode, u.Usage, u.AreaM2, u.IsActive FROM dbo.Units u JOIN dbo.Properties p ON p.PropertyId=u.PropertyId ORDER BY u.UnitId DESC");
            gv.DataBind();
        }
    }
}