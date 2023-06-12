using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_inv_edit : System.Web.UI.Page
{
    InvitationData m_data;

    protected void Back_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/admin/inv/default.aspx");
    }

    protected void Cancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Format("~/admin/inv/view.aspx?inv={0}", m_data.invitationId));
    }

    protected void Save_Click(object sender, EventArgs e)
    {
        try
        {
            string email = TB_EMail.Text;
            string description = TB_Description.Text;
            bool assignProviderRole = CB_AssignProviderRole.Checked;

            SoftnetRegistry.admin_updateInvitationData(m_data.invitationId, email, description, assignProviderRole);
            Response.Redirect(string.Format("~/admin/inv/view.aspx?inv={0}", m_data.invitationId));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            long invitationId;
            if (long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("inv"), out invitationId) == false)
                throw new InvalidIdentifierSoftnetException();

            m_data = new InvitationData();
            m_data.invitationId = invitationId;
            SoftnetRegistry.admin_getInvitationData(this.Context.User.Identity.Name, m_data);

            L_Url.Text = string.Format("{0}/invitee.aspx?key={1}", SoftnetRegistry.settings_getSiteUrl(), m_data.ikey);

            if (this.IsPostBack == false)
            {
                TB_EMail.Text = m_data.email;
                TB_Description.Text = m_data.description;
                CB_AssignProviderRole.Checked = m_data.assignProviderRole;
            }
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }
}