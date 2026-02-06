using System;
using System.Globalization;
using PropertyOps.App;

namespace PropertyOpsWebForms.Associations
{
    public partial class Meetings : PropertyOpsWebForms.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlAssoc.DataSource = Db.Query(@"SELECT ba.AssociationId, p.Name AS Txt
                                                FROM dbo.BuildingAssociations ba
                                                JOIN dbo.Properties p ON p.PropertyId=ba.PropertyId
                                                WHERE ba.IsActive=1 ORDER BY p.Name");
                ddlAssoc.DataTextField="Txt"; ddlAssoc.DataValueField="AssociationId"; ddlAssoc.DataBind();

                txtDT.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                Bind();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (!DateTime.TryParseExact(txtDT.Text.Trim(), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            { lblMsg.Text="<div class='msg err'>Datum/vreme format nije ispravan.</div>"; return; }

            try
            {
                int userId = CurrentUserId ?? 0;
                Db.Exec(@"INSERT INTO dbo.AssociationMeetings(AssociationId,MeetingDate,Title,Agenda,Minutes,Decisions,CreatedByUserId)
                          VALUES (@a,@d,@t,@ag,@m,@dec,@u)",
                    Db.P("@a", int.Parse(ddlAssoc.SelectedValue)),
                    Db.P("@d", dt),
                    Db.P("@t", txtTitle.Text.Trim()),
                    Db.P("@ag", txtAgenda.Text),
                    Db.P("@m", txtMinutes.Text),
                    Db.P("@dec", txtDec.Text),
                    Db.P("@u", userId));
                lblMsg.Text="<div class='msg ok'>Snimljeno.</div>";
                Bind();
            }
            catch (Exception ex)
            {
                lblMsg.Text="<div class='msg err'>Greška: " + Server.HtmlEncode(ex.Message) + "</div>";
            }
        }

        private void Bind()
        {
            gv.DataSource = Db.Query(@"
SELECT m.MeetingId, p.Name AS Property, m.MeetingDate, m.Title
FROM dbo.AssociationMeetings m
JOIN dbo.BuildingAssociations ba ON ba.AssociationId=m.AssociationId
JOIN dbo.Properties p ON p.PropertyId=ba.PropertyId
ORDER BY m.MeetingId DESC");
            gv.DataBind();
        }
    }
}