using System;
using System.Data;
using System.Globalization;
using System.Text;
using PropertyOps.App;

namespace PropertyOpsWebForms.Finance
{
    public partial class Transactions : PropertyOpsWebForms.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtFrom.Text = DateTime.Today.AddDays(-30).ToString("yyyy-MM-dd");
                txtTo.Text = DateTime.Today.ToString("yyyy-MM-dd");
                ddlEnt.DataSource = Db.Query("SELECT EntityId, Name FROM dbo.Entities ORDER BY Name");
                ddlEnt.DataTextField = "Name"; ddlEnt.DataValueField = "EntityId"; ddlEnt.DataBind();
                ddlEnt.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- all --", ""));
                Bind();
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            Bind();
        }

        private void Bind()
        {
            DateTime from = DateTime.Today.AddDays(-30), to = DateTime.Today;
            DateTime.TryParseExact(txtFrom.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out from);
            DateTime.TryParseExact(txtTo.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out to);

            string sql = @"
SELECT t.TransactionId, t.TxDate, t.Direction, c.Name AS Category, e.Name AS PaidBy, bl.Name AS BusinessLine,
       t.AmountTotal, t.Currency, t.Description
FROM dbo.Transactions t
JOIN dbo.Categories c ON c.CategoryId=t.CategoryId
JOIN dbo.Entities e ON e.EntityId=t.PaidByEntityId
JOIN dbo.BusinessLines bl ON bl.BusinessLineId=t.PrimaryBusinessLineId
WHERE t.TxDate BETWEEN @f AND @t";

            var ps = new System.Collections.Generic.List<System.Data.SqlClient.SqlParameter>
            {
                Db.P("@f", from.Date),
                Db.P("@t", to.Date)
            };
            if (!string.IsNullOrWhiteSpace(ddlEnt.SelectedValue))
            {
                sql += " AND t.PaidByEntityId=@e";
                ps.Add(Db.P("@e", int.Parse(ddlEnt.SelectedValue)));
            }
            sql += " ORDER BY t.TransactionId DESC";

            var dt = Db.Query(sql, ps.ToArray());
            gv.DataSource = dt;
            gv.DataBind();
            ViewState["Last"] = dt;
        }

        protected void btnCsv_Click(object sender, EventArgs e)
        {
            var dt = ViewState["Last"] as DataTable;
            if (dt == null || dt.Rows.Count == 0) { lblMsg.Text = "<div class='msg err'>Nema podataka.</div>"; return; }

            var sb = new StringBuilder();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (i > 0) sb.Append(";");
                sb.Append(dt.Columns[i].ColumnName);
            }
            sb.AppendLine();

            foreach (DataRow r in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (i > 0) sb.Append(";");
                    var v = (PropertyOps.App.Compat.SafeReplace(PropertyOps.App.Compat.SafeToString(r[i]), ";", ",") ?? "");
                    sb.Append(v);
                }
                sb.AppendLine();
            }

            Response.Clear();
            Response.ContentType = "text/csv";
            Response.AddHeader("Content-Disposition", "attachment;filename=transactions.csv");
            Response.ContentEncoding = Encoding.UTF8;
            Response.Write(sb.ToString());
            Response.End();
        }
    }
}