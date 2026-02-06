using System;
using PropertyOps.App;

namespace PropertyOpsWebForms.Reports
{
    public partial class UnitCard : PropertyOpsWebForms.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlUnit.DataSource = Db.Query(@"
SELECT u.UnitId, (p.Name + ' / ' + u.UnitCode) AS Txt
FROM dbo.Units u
JOIN dbo.Properties p ON p.PropertyId=u.PropertyId
WHERE u.IsActive=1
ORDER BY p.Name, u.UnitCode");
                ddlUnit.DataTextField = "Txt";
                ddlUnit.DataValueField = "UnitId";
                ddlUnit.DataBind();

                if (ddlUnit.Items.Count > 0)
                {
                    Bind();
                }
                else
                {
                    lblMsg.Text = "<div class='msg err'>Nema unetih jedinica.</div>";
                }
            }
        }

        protected void ddlUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            Bind();
        }

        private void Bind()
        {
            int unitId = int.Parse(ddlUnit.SelectedValue);

            gvTx.DataSource = Db.Query(@"
SELECT TxDate AS Datum, Direction AS Smer, AmountTotal AS Iznos, Description AS Opis
FROM dbo.Transactions
WHERE UnitId=@u
ORDER BY TxDate DESC, TransactionId DESC", Db.P("@u", unitId));
            gvTx.DataBind();

            gvSum.DataSource = Db.Query(@"
SELECT 
  SUM(CASE WHEN Direction='Income' THEN AmountTotal ELSE 0 END) AS Prihodi,
  SUM(CASE WHEN Direction='Expense' THEN AmountTotal ELSE 0 END) AS Rashodi,
  SUM(CASE WHEN Direction='Income' THEN AmountTotal ELSE -AmountTotal END) AS Neto
FROM dbo.Transactions
WHERE UnitId=@u", Db.P("@u", unitId));
            gvSum.DataBind();
        }
    }
}
