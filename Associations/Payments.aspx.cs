using System;
using System.Globalization;
using PropertyOps.App;

namespace PropertyOpsWebForms.Associations
{
    public partial class Payments : PropertyOpsWebForms.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlAssoc.DataSource = Db.Query(@"
SELECT ba.AssociationId, p.Name AS Txt
FROM dbo.BuildingAssociations ba
JOIN dbo.Properties p ON p.PropertyId=ba.PropertyId
WHERE ba.IsActive=1
ORDER BY p.Name");
                ddlAssoc.DataTextField = "Txt";
                ddlAssoc.DataValueField = "AssociationId";
                ddlAssoc.DataBind();

                txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
                txtY.Text = DateTime.Today.Year.ToString();
                txtM.Text = DateTime.Today.Month.ToString();
                txtRY.Text = txtY.Text;
                txtRM.Text = txtM.Text;

                BindUnits();
            }
        }

        protected void ddlAssoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindUnits();
        }

        private void BindUnits()
        {
            int assocId;
            if (!int.TryParse(ddlAssoc.SelectedValue, out assocId))
                return;

            string sql =
                "SELECT u.UnitId, (p.Name + ' / ' + u.UnitCode) AS Txt " +
                "FROM dbo.AssociationUnits au " +
                "JOIN dbo.Units u ON u.UnitId=au.UnitId " +
                "JOIN dbo.Properties p ON p.PropertyId=u.PropertyId " +
                "WHERE au.AssociationId=@a AND au.ActiveTo IS NULL AND u.IsActive=1 " +
                "ORDER BY u.UnitCode";

            ddlUnit.DataSource = Db.Query(sql, Db.P("@a", assocId));
            ddlUnit.DataTextField = "Txt";
            ddlUnit.DataValueField = "UnitId";
            ddlUnit.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            DateTime d;
            decimal amt;
            int yy;
            int mm;

            if (!DateTime.TryParseExact(txtDate.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
            {
                lblMsg.Text = "<div class='msg err'>Datum nije ispravan.</div>";
                return;
            }

            if (!decimal.TryParse(txtAmt.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out amt) || amt <= 0)
            {
                lblMsg.Text = "<div class='msg err'>Iznos nije ispravan.</div>";
                return;
            }

            int? y = int.TryParse(txtY.Text.Trim(), out yy) ? (int?)yy : (int?)null;
            int? m = int.TryParse(txtM.Text.Trim(), out mm) ? (int?)mm : (int?)null;
            int userId = CurrentUserId ?? 0;

            try
            {
                int assocId = int.Parse(ddlAssoc.SelectedValue);
                int unitId = int.Parse(ddlUnit.SelectedValue);

                Db.Exec(@"INSERT INTO dbo.AssociationPayments(AssociationId,UnitId,PaymentDate,Amount,PeriodYear,PeriodMonth,CreatedByUserId)
                          VALUES (@a,@u,@d,@amt,@y,@m,@uid)",
                    Db.P("@a", assocId),
                    Db.P("@u", unitId),
                    Db.P("@d", d.Date),
                    Db.P("@amt", amt),
                    Db.P("@y", (object)y ?? DBNull.Value),
                    Db.P("@m", (object)m ?? DBNull.Value),
                    Db.P("@uid", userId));

                int hoaEntityId = Convert.ToInt32(Db.Scalar("SELECT EntityId FROM dbo.BuildingAssociations WHERE AssociationId=@a", Db.P("@a", assocId)));
                int catId = Lookup.GetCategoryIdByName("Odrzavanje zgrade (prihod)");
                int blId = Lookup.GetBusinessLineIdByName("Association");
                string unitCode = Convert.ToString(Db.Scalar("SELECT UnitCode FROM dbo.Units WHERE UnitId=@u", Db.P("@u", unitId)));

                // for description formatting when y/m are null
                int yDesc = y.HasValue ? y.Value : d.Year;
                int mDesc = m.HasValue ? m.Value : d.Month;

                Db.Exec(@"
DECLARE @txId INT;
INSERT INTO dbo.Transactions(TxDate,Direction,CategoryId,CounterpartyId,PaidByEntityId,PrimaryBusinessLineId,AmountTotal,Description,UnitId,CreatedByUserId)
VALUES (@dt,'Income',@cat,NULL,@ent,@bl,@amt,@desc,@unit,@uid);
SET @txId = SCOPE_IDENTITY();
INSERT INTO dbo.TransactionAllocations(TransactionId,BeneficiaryEntityId,BusinessLineId,Amount,Notes)
VALUES (@txId,@ent,@bl,@amt,'SZ uplata');",
                    Db.P("@dt", d.Date),
                    Db.P("@cat", catId),
                    Db.P("@ent", hoaEntityId),
                    Db.P("@bl", blId),
                    Db.P("@amt", amt),
                    Db.P("@desc", string.Format("SZ uplata {0}-{1:00} / {2}", yDesc, mDesc, unitCode)),
                    Db.P("@unit", unitId),
                    Db.P("@uid", userId));

                lblMsg.Text = "<div class='msg ok'>Uplata snimljena + knjiženje u finansije kreirano.</div>";
            }
            catch (Exception ex)
            {
                lblMsg.Text = "<div class='msg err'>Greška: " + Server.HtmlEncode(ex.Message) + "</div>";
            }
        }

        protected void btnRun_Click(object sender, EventArgs e)
        {
            int y;
            int m;

            if (!int.TryParse(txtRY.Text.Trim(), out y) || !int.TryParse(txtRM.Text.Trim(), out m))
            {
                lblMsg.Text = "<div class='msg err'>Godina/mesec nisu ispravni.</div>";
                return;
            }

            int assocId = int.Parse(ddlAssoc.SelectedValue);

            gv.DataSource = Db.Query(@"
WITH Charge AS (
    SELECT TOP 1 ch.ChargeId
    FROM dbo.AssociationCharges ch
    WHERE ch.AssociationId=@a AND ch.PeriodYear=@y AND ch.PeriodMonth=@m
    ORDER BY ch.ChargeId DESC
),
Items AS (
    SELECT i.UnitId, i.Amount
    FROM dbo.AssociationChargeItems i
    JOIN Charge c ON c.ChargeId=i.ChargeId
),
Pay AS (
    SELECT p.UnitId, SUM(p.Amount) AS Paid
    FROM dbo.AssociationPayments p
    WHERE p.AssociationId=@a AND p.PeriodYear=@y AND p.PeriodMonth=@m
    GROUP BY p.UnitId
)
SELECT (pr.Name + ' / ' + u.UnitCode) AS Unit,
       i.Amount AS Charged,
       ISNULL(p.Paid,0) AS Paid,
       (i.Amount - ISNULL(p.Paid,0)) AS Debt
FROM Items i
JOIN dbo.Units u ON u.UnitId=i.UnitId
JOIN dbo.Properties pr ON pr.PropertyId=u.PropertyId
LEFT JOIN Pay p ON p.UnitId=i.UnitId
WHERE (i.Amount - ISNULL(p.Paid,0)) > 0.01
ORDER BY Debt DESC",
                Db.P("@a", assocId), Db.P("@y", y), Db.P("@m", m));

            gv.DataBind();
        }
    }
}
