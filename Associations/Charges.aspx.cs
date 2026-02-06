using System;
using System.Globalization;
using PropertyOps.App;

namespace PropertyOpsWebForms.Associations
{
    public partial class Charges : PropertyOpsWebForms.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlAssoc.DataSource = Db.Query(@"SELECT ba.AssociationId, p.Name AS Txt
                                                FROM dbo.BuildingAssociations ba
                                                JOIN dbo.Properties p ON p.PropertyId=ba.PropertyId
                                                WHERE ba.IsActive=1 ORDER BY p.Name");
                ddlAssoc.DataTextField = "Txt";
                ddlAssoc.DataValueField = "AssociationId";
                ddlAssoc.DataBind();

                DateTime now = DateTime.Today;
                txtY.Text = now.Year.ToString();
                txtM.Text = now.Month.ToString();
                txtAmt.Text = "0";

                Bind();
            }
        }

        protected void btnGen_Click(object sender, EventArgs e)
        {
            int y;
            int m;
            decimal amt;

            if (!int.TryParse(txtY.Text.Trim(), out y) || y < 2000 || y > 2100)
            {
                lblMsg.Text = "<div class='msg err'>Godina nije ispravna.</div>";
                return;
            }

            if (!int.TryParse(txtM.Text.Trim(), out m) || m < 1 || m > 12)
            {
                lblMsg.Text = "<div class='msg err'>Mesec nije ispravan.</div>";
                return;
            }

            if (!decimal.TryParse(txtAmt.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out amt) || amt <= 0)
            {
                lblMsg.Text = "<div class='msg err'>Iznos nije ispravan.</div>";
                return;
            }

            try
            {
                int assocId = int.Parse(ddlAssoc.SelectedValue);

                // Create or update charge header
                int exists = Convert.ToInt32(Db.Scalar(
                    @"SELECT COUNT(*) FROM dbo.AssociationCharges 
                      WHERE AssociationId=@a AND PeriodYear=@y AND PeriodMonth=@m",
                    Db.P("@a", assocId), Db.P("@y", y), Db.P("@m", m)));

                if (exists == 0)
                {
                    Db.Exec(@"INSERT INTO dbo.AssociationCharges(AssociationId,PeriodYear,PeriodMonth,ChargeMethod,MonthlyAmountPerUnit)
                              VALUES (@a,@y,@m,'PerUnit',@amt)",
                        Db.P("@a", assocId), Db.P("@y", y), Db.P("@m", m), Db.P("@amt", amt));
                }
                else
                {
                    Db.Exec(@"UPDATE dbo.AssociationCharges 
                              SET MonthlyAmountPerUnit=@amt 
                              WHERE AssociationId=@a AND PeriodYear=@y AND PeriodMonth=@m",
                        Db.P("@a", assocId), Db.P("@y", y), Db.P("@m", m), Db.P("@amt", amt));
                }

                int chargeId = Convert.ToInt32(Db.Scalar(
                    @"SELECT ChargeId FROM dbo.AssociationCharges 
                      WHERE AssociationId=@a AND PeriodYear=@y AND PeriodMonth=@m",
                    Db.P("@a", assocId), Db.P("@y", y), Db.P("@m", m)));

                // Generate charge items for all active units in association
                Db.Exec(@"
INSERT INTO dbo.AssociationChargeItems(ChargeId,UnitId,Amount)
SELECT @c, au.UnitId, @amt
FROM dbo.AssociationUnits au
WHERE au.AssociationId=@a
AND au.ActiveTo IS NULL
AND NOT EXISTS (SELECT 1 FROM dbo.AssociationChargeItems i WHERE i.ChargeId=@c AND i.UnitId=au.UnitId)",
                    Db.P("@c", chargeId), Db.P("@a", assocId), Db.P("@amt", amt));

                lblMsg.Text = "<div class='msg ok'>Zaduženje kreirano i stavke generisane.</div>";
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
SELECT ch.ChargeId, p.Name AS Property, ch.PeriodYear, ch.PeriodMonth, ch.MonthlyAmountPerUnit,
       (SELECT COUNT(*) FROM dbo.AssociationChargeItems i WHERE i.ChargeId=ch.ChargeId) AS Items
FROM dbo.AssociationCharges ch
JOIN dbo.BuildingAssociations ba ON ba.AssociationId=ch.AssociationId
JOIN dbo.Properties p ON p.PropertyId=ba.PropertyId
ORDER BY ch.ChargeId DESC");
            gv.DataBind();
        }
    }
}
