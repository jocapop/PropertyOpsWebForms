using System;
using PropertyOps.App;

namespace PropertyOpsWebForms.Reports
{
    public partial class Dashboard : PropertyOpsWebForms.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Bind();
            }
        }

        private void Bind()
        {
            gv12m.DataSource = Db.Query(@"
SELECT 
  CONVERT(varchar(7), TxDate, 120) AS [Mesec],
  SUM(CASE WHEN Direction='Income' THEN AmountTotal ELSE 0 END) AS Prihodi,
  SUM(CASE WHEN Direction='Expense' THEN AmountTotal ELSE 0 END) AS Rashodi,
  SUM(CASE WHEN Direction='Income' THEN AmountTotal ELSE -AmountTotal END) AS Neto
FROM dbo.Transactions
WHERE TxDate >= DATEADD(month,-11,DATEFROMPARTS(YEAR(GETDATE()),MONTH(GETDATE()),1))
GROUP BY CONVERT(varchar(7), TxDate, 120)
ORDER BY [Mesec];");
            gv12m.DataBind();

            gvEntity.DataSource = Db.Query(@"
SELECT 
  e.Name AS Entitet,
  SUM(CASE WHEN t.Direction='Income' THEN t.AmountTotal ELSE 0 END) AS Prihodi,
  SUM(CASE WHEN t.Direction='Expense' THEN t.AmountTotal ELSE 0 END) AS Rashodi,
  SUM(CASE WHEN t.Direction='Income' THEN t.AmountTotal ELSE -t.AmountTotal END) AS Neto
FROM dbo.Transactions t
LEFT JOIN dbo.Entities e ON e.EntityId = t.PaidByEntityId
WHERE YEAR(t.TxDate)=YEAR(GETDATE())
GROUP BY e.Name
ORDER BY Neto DESC;");
            gvEntity.DataBind();
        }
    }
}
