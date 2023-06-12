using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_inv_new : System.Web.UI.Page
{
    int c_invitationKeyLength = 32;

    protected void Back_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/admin/inv/default.aspx");
    }

    protected void Cancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/admin/inv/default.aspx");
    }    

    protected void CreateInvitation_Click(object sender, EventArgs e)
    {        
        try
        {
            bool assignProviderRole = CB_AssignProviderRole.Checked;
            string ikey = Randomizer.generateInvitationKey(Constants.Keys.invitation_key_length);

            string email = TB_EMail.Text.Trim();
            string description = TB_Description.Text.Trim();
            if (description.Length == 0)
                description = AutoNaming.addSuffix("Inv");

            long invitationId = SoftnetRegistry.admin_addNewInvitation(this.Context.User.Identity.Name, ikey, email, description, assignProviderRole);
            Response.Redirect(string.Format("~/admin/inv/view.aspx?inv={0}", invitationId));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {        
        if(this.IsPostBack == false)
        {
            try
            {
                CB_AssignProviderRole.Checked = SoftnetRegistry.settings_getAutoAssignProviderRole();
            }
            catch (SoftnetException ex)
            {
                ExceptionHandler.exec(this, ex);
            }
        }
    }
}