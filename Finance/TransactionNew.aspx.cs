using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using PropertyOps.App;

namespace PropertyOpsWebForms.Finance
{
    public partial class TransactionNew : PropertyOpsWebForms.PageBase
    {
        private DataTable AllocTable
        {
            get
            {
                if (ViewState["Alloc"] == null)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("EntityId", typeof(int));
                    dt.Columns.Add("EntityName", typeof(string));
                    dt.Columns.Add("BusinessLineId", typeof(int));
                    dt.Columns.Add("BusinessLine", typeof(string));
                    dt.Columns.Add("Amount", typeof(decimal));
                    dt.Columns.Add("Notes", typeof(string));
                    ViewState["Alloc"] = dt;
                }
                return (DataTable)ViewState["Alloc"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
                BindLookups();
                BindAlloc();
            }
        }

        private void BindLookups()
        {
            ddlCat.DataSource = Db.Query("SELECT CategoryId, Name FROM dbo.Categories ORDER BY Name");
            ddlCat.DataTextField = "Name"; ddlCat.DataValueField = "CategoryId"; ddlCat.DataBind();

            ddlPaidBy.DataSource = Db.Query("SELECT EntityId, Name FROM dbo.Entities WHERE IsActive=1 ORDER BY Name");
            ddlPaidBy.DataTextField = "Name"; ddlPaidBy.DataValueField = "EntityId"; ddlPaidBy.DataBind();

            ddlBL.DataSource = Db.Query("SELECT BusinessLineId, Name FROM dbo.BusinessLines ORDER BY Name");
            ddlBL.DataTextField = "Name"; ddlBL.DataValueField = "BusinessLineId"; ddlBL.DataBind();

            ddlCp.DataSource = Db.Query("SELECT CounterpartyId, FullName FROM dbo.Counterparties WHERE IsActive=1 ORDER BY FullName");
            ddlCp.DataTextField = "FullName"; ddlCp.DataValueField = "CounterpartyId"; ddlCp.DataBind();
            ddlCp.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- none --", ""));

            ddlUnit.DataSource = Db.Query(@"SELECT u.UnitId, (p.Name + ' / ' + u.UnitCode) AS Txt
                                            FROM dbo.Units u JOIN dbo.Properties p ON p.PropertyId=u.PropertyId
                                            WHERE u.IsActive=1 ORDER BY p.Name,u.UnitCode");
            ddlUnit.DataTextField = "Txt"; ddlUnit.DataValueField = "UnitId"; ddlUnit.DataBind();
            ddlUnit.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- none --", ""));

            ddlAssoc.DataSource = Db.Query(@"SELECT ba.AssociationId, (p.Name + ' (SZ)') AS Txt
                                            FROM dbo.BuildingAssociations ba 
                                            JOIN dbo.Properties p ON p.PropertyId=ba.PropertyId
                                            WHERE ba.IsActive=1 ORDER BY p.Name");
            ddlAssoc.DataTextField = "Txt"; ddlAssoc.DataValueField = "AssociationId"; ddlAssoc.DataBind();
            ddlAssoc.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- none --", ""));

            ddlAllocEntity.DataSource = Db.Query("SELECT EntityId, Name FROM dbo.Entities WHERE IsActive=1 ORDER BY Name");
            ddlAllocEntity.DataTextField = "Name"; ddlAllocEntity.DataValueField = "EntityId"; ddlAllocEntity.DataBind();

            ddlAllocBL.DataSource = Db.Query("SELECT BusinessLineId, Name FROM dbo.BusinessLines ORDER BY Name");
            ddlAllocBL.DataTextField = "Name"; ddlAllocBL.DataValueField = "BusinessLineId"; ddlAllocBL.DataBind();
        }

        protected void btnAddAlloc_Click(object sender, EventArgs e)
        {
            decimal amt;
            if (!decimal.TryParse(txtAllocAmt.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out amt) || amt <= 0)
            {
                lblMsg.Text = "<div class='msg err'>Alloc amount nije ispravan.</div>";
                return;
            }

            DataRow row = AllocTable.NewRow();
            row["EntityId"] = int.Parse(ddlAllocEntity.SelectedValue);
            row["EntityName"] = ddlAllocEntity.SelectedItem.Text;
            row["BusinessLineId"] = int.Parse(ddlAllocBL.SelectedValue);
            row["BusinessLine"] = ddlAllocBL.SelectedItem.Text;
            row["Amount"] = amt;
            row["Notes"] = txtAllocNote.Text.Trim();
            AllocTable.Rows.Add(row);

            txtAllocAmt.Text = "";
            txtAllocNote.Text = "";
            BindAlloc();
        }

        protected void gvAlloc_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Del")
            {
                int idx = Convert.ToInt32(e.CommandArgument);
                if (idx >= 0 && idx < AllocTable.Rows.Count)
                {
                    AllocTable.Rows.RemoveAt(idx);
                }
                BindAlloc();
            }
        }

        private void BindAlloc()
        {
            gvAlloc.DataSource = AllocTable;
            gvAlloc.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (AllocTable.Rows.Count == 0)
                {
                    lblMsg.Text = "<div class='msg err'>Dodaj bar jednu raspodelu.</div>";
                    return;
                }

                DateTime d;
                if (!DateTime.TryParseExact(txtDate.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                {
                    lblMsg.Text = "<div class='msg err'>Datum format mora biti YYYY-MM-DD.</div>";
                    return;
                }

                decimal total;
                if (!decimal.TryParse(txtAmt.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out total) || total <= 0)
                {
                    lblMsg.Text = "<div class='msg err'>Iznos nije ispravan.</div>";
                    return;
                }

                decimal sumAlloc = 0;
                foreach (DataRow r in AllocTable.Rows)
                {
                    sumAlloc += (decimal)r["Amount"];
                }

                if (Math.Round(sumAlloc, 2) != Math.Round(total, 2))
                {
                    lblMsg.Text = string.Format(
                        "<div class='msg err'>Zbir raspodele ({0:0.00}) mora biti jednak totalu ({1:0.00}).</div>",
                        sumAlloc, total
                    );
                    return;
                }

                int? unitId = string.IsNullOrWhiteSpace(ddlUnit.SelectedValue) ? (int?)null : int.Parse(ddlUnit.SelectedValue);
                int? cpId = string.IsNullOrWhiteSpace(ddlCp.SelectedValue) ? (int?)null : int.Parse(ddlCp.SelectedValue);

                int? assocId = string.IsNullOrWhiteSpace(ddlAssoc.SelectedValue) ? (int?)null : int.Parse(ddlAssoc.SelectedValue);
                int businessLineId = int.Parse(ddlBL.SelectedValue);

                int userId = CurrentUserId ?? 0;

                using (SqlConnection cn = Db.Open())
                using (SqlTransaction tx = cn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand(@"
INSERT INTO dbo.Transactions(TxDate,Direction,CategoryId,CounterpartyId,PaidByEntityId,PrimaryBusinessLineId,AmountTotal,Description,UnitId,CreatedByUserId)
OUTPUT INSERTED.TransactionId
VALUES (@d,@dir,@cat,@cp,@paid,@bl,@amt,@desc,@unit,@uid)", cn, tx);

                    cmd.Parameters.AddWithValue("@d", d);
                    cmd.Parameters.AddWithValue("@dir", ddlDir.SelectedValue);
                    cmd.Parameters.AddWithValue("@cat", int.Parse(ddlCat.SelectedValue));
                    cmd.Parameters.AddWithValue("@cp", (object)cpId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@paid", int.Parse(ddlPaidBy.SelectedValue));
                    cmd.Parameters.AddWithValue("@bl", businessLineId);
                    cmd.Parameters.AddWithValue("@amt", total);

                    string desc = (txtDesc.Text == null) ? "" : txtDesc.Text.Trim();
                    cmd.Parameters.AddWithValue("@desc", desc.Length == 0 ? (object)DBNull.Value : (object)desc);

                    cmd.Parameters.AddWithValue("@unit", (object)unitId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@uid", userId);

                    int txId = (int)cmd.ExecuteScalar();

                    foreach (DataRow r in AllocTable.Rows)
                    {
                        SqlCommand cmdA = new SqlCommand(@"
INSERT INTO dbo.TransactionAllocations(TransactionId,BeneficiaryEntityId,BusinessLineId,Amount,Notes)
VALUES (@t,@e,@b,@a,@n)", cn, tx);

                        cmdA.Parameters.AddWithValue("@t", txId);
                        cmdA.Parameters.AddWithValue("@e", (int)r["EntityId"]);
                        cmdA.Parameters.AddWithValue("@b", (int)r["BusinessLineId"]);
                        cmdA.Parameters.AddWithValue("@a", (decimal)r["Amount"]);

                        object notesObj = r["Notes"];
                        if (notesObj == null || notesObj == DBNull.Value)
                        {
                            cmdA.Parameters.AddWithValue("@n", DBNull.Value);
                        }
                        else
                        {
                            string notes = notesObj.ToString();
                            cmdA.Parameters.AddWithValue("@n", notes.Length == 0 ? (object)DBNull.Value : (object)notes);
                        }

                        cmdA.ExecuteNonQuery();
                    }

                    tx.Commit();
                }

                ViewState["Alloc"] = null;
                BindAlloc();

                lblMsg.Text = "<div class='msg ok'>Transakcija snimljena.</div>";
                txtAmt.Text = "";
                txtDesc.Text = "";
            }
            catch (Exception ex)
            {
                lblMsg.Text = "<div class='msg err'>Greška: " + Server.HtmlEncode(ex.Message) + "</div>";
            }
        }
    }
}
