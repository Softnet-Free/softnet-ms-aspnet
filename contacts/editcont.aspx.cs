using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class contacts_editcont : System.Web.UI.Page
{
    ContactData m_contactData;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            long contactId;
            if (long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("cid"), out contactId) == false)
                throw new InvalidIdentifierSoftnetException();
            
            m_contactData = new ContactData();
            Atomic<string> partnerName = new Atomic<string>();
            SoftnetRegistry.GetContactData(this.Context.User.Identity.Name, contactId, m_contactData, partnerName);

            if (m_contactData.status != 2)
            {
                L_Partner.Text = string.Format("partner <span style='color: black'>{0}</span>", partnerName.Value);
            }
            else
            {
                L_Partner.Text = "<span class='disabled_status'>Your partner has been deleted from the network.</span>";
            }

            if (this.IsPostBack == false)
            {
                TB_ContactName.Text = m_contactData.contactName;
                TB_AssigningName.Text = m_contactData.assigningName;
            }
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void Back_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/contacts/default.aspx?edit=1");
    }

    protected void Save_Click(object sender, EventArgs e)
    {        
        if (m_contactData.status == 2)
        {
            Response.Redirect("~/contacts/default.aspx?edit=1");
            return;
        }

        try
        {
            SoftnetRegistry.UpdateContact(m_contactData.contactId, TB_ContactName.Text, TB_AssigningName.Text);
            Response.Redirect("~/contacts/default.aspx?edit=1");
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }
}