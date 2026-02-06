using System;
using System.Globalization;
using PropertyOps.App;

namespace PropertyOpsWebForms.Associations
{
    public partial class Associations : PropertyOpsWebForms.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlProp.DataSource = Db.Query("SELECT PropertyId, Name FROM dbo.Properties ORDER BY Name");
                ddlProp.DataTextField = "Name";
                ddlProp.DataValueField = "PropertyId";
                ddlProp.DataBind();

                //ddlEnt.DataSource = Db.Query("SELECT EntityId, Name FROM dbo.Entities WHERE EntityType='HOA' OR EntityType='Association' OR EntityType='Other' ORDER BY Name");
                ddlEnt.DataSource = Db.Query("SELECT EntityId, Name FROM dbo.Entities ORDER BY Name");
                ddlEnt.DataTextField = "Name";
                ddlEnt.DataValueField = "EntityId";
                ddlEnt.DataBind();

                Bind();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            decimal amt;

            if (!decimal.TryParse(txtAmt.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out amt) || amt <= 0)
            {
                lblMsg.Text = "<div class='msg err'>Iznos nije ispravan.</div>";
                return;
            }

            try
            {
                int propId = int.Parse(ddlProp.SelectedValue);
                int entId = int.Parse(ddlEnt.SelectedValue);

                // Upsert by property
                int exists = Convert.ToInt32(
                    Db.Scalar("SELECT COUNT(*) FROM dbo.BuildingAssociations WHERE PropertyId=@p",
                    Db.P("@p", propId))
                );

                if (exists == 0)
                {
                    Db.Exec(@"INSERT INTO dbo.BuildingAssociations
                              (PropertyId,EntityId,DefaultChargeMethod,DefaultMonthlyAmount,IsActive)
                              VALUES (@p,@e,'PerUnit',@a,1)",
                        Db.P("@p", propId),
                        Db.P("@e", entId),
                        Db.P("@a", amt));
                }
                else
                {
                    Db.Exec(@"UPDATE dbo.BuildingAssociations
                              SET EntityId=@e, DefaultMonthlyAmount=@a, IsActive=1
                              WHERE PropertyId=@p",
                        Db.P("@p", propId),
                        Db.P("@e", entId),
                        Db.P("@a", amt));
                }

                // Ensure membership list
                int assocId = Convert.ToInt32(
                    Db.Scalar("SELECT AssociationId FROM dbo.BuildingAssociations WHERE PropertyId=@p",
                    Db.P("@p", propId))
                );

                Db.Exec(@"
INSERT INTO dbo.AssociationUnits(AssociationId,UnitId)
SELECT @aid, u.UnitId
FROM dbo.Units u
WHERE u.PropertyId=@p
AND NOT EXISTS (
    SELECT 1 FROM dbo.AssociationUnits au
    WHERE au.AssociationId=@aid AND au.UnitId=u.UnitId
)",
                    Db.P("@aid", assocId),
                    Db.P("@p", propId)
                );

                lblMsg.Text = "<div class='msg ok'>Snimljeno (i dopunjen spisak stanova za zgradu).</div>";
                Bind();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "<div class='msg err'>Greška: " + Server.HtmlEncode(ex.Message) + "</div>";
            }
        }

        private void Bind()
        {
            gv.DataSource = Db.Query(@"
SELECT ba.AssociationId,
       p.Name AS Property,
       e.Name AS Entity,
       ba.DefaultMonthlyAmount,
       ba.IsActive
FROM dbo.BuildingAssociations ba
JOIN dbo.Properties p ON p.PropertyId = ba.PropertyId
JOIN dbo.Entities e ON e.EntityId = ba.EntityId
ORDER BY ba.AssociationId DESC");

            gv.DataBind();
        }
    }
}
