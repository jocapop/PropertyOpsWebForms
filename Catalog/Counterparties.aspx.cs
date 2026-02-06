using System;
using PropertyOps.App;

namespace PropertyOpsWebForms.Catalog
{
    public partial class Counterparties : PropertyOpsWebForms.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                var dt_ddlType = Db.Query(@"SELECT 'Customer' Val,'Customer' Txt UNION ALL SELECT 'Supplier','Supplier' UNION ALL SELECT 'Owner','Owner' UNION ALL SELECT 'HeatingCustomer','HeatingCustomer' UNION ALL SELECT 'Other','Other'");
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
                Db.Exec(@"INSERT INTO dbo.Counterparties(Type,FullName,Phone,PIB) VALUES (@ddlType,@txtFull,@txtPhone,@txtPIB)", Db.P("@ddlType", ddlType.SelectedValue), Db.P("@txtFull", txtFull.Text.Trim()), Db.P("@txtPhone", txtPhone.Text.Trim()), Db.P("@txtPIB", txtPIB.Text.Trim()));
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
            gv.DataSource = Db.Query(@"SELECT CounterpartyId, Type, FullName, Phone, PIB, IsActive FROM dbo.Counterparties ORDER BY CounterpartyId DESC");
            gv.DataBind();
        }
    }
}