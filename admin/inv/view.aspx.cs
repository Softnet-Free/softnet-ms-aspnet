/*
*   Copyright 2023 Robert Koifman
*   
*   Licensed under the Apache License, Version 2.0 (the "License");
*   you may not use this file except in compliance with the License.
*   You may obtain a copy of the License at
*
*   http://www.apache.org/licenses/LICENSE-2.0
*
*   Unless required by applicable law or agreed to in writing, software
*   distributed under the License is distributed on an "AS IS" BASIS,
*   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*   See the License for the specific language governing permissions and
*   limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Net.Mail;

public partial class admin_inv_view : System.Web.UI.Page
{
    InvitationData m_data;

    protected void Back_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/admin/inv/default.aspx");
    }   

    protected void Edit_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Format("~/admin/inv/edit.aspx?inv={0}", m_data.invitationId));
    }

    protected void Close_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/admin/inv/default.aspx");
    }

    protected void Send_Click(object sender, EventArgs e)
    {
        try
        {
            MailingData mailingData = new MailingData();
            SoftnetRegistry.settings_getMailingData(mailingData);

            var smtpClient = new SmtpClient(mailingData.smtpServer, mailingData.smtpPort);
            smtpClient.Credentials = new NetworkCredential(mailingData.emailAddress, mailingData.emailPassword);
            smtpClient.EnableSsl = mailingData.smtpRequiresSSL;
            smtpClient.Timeout = 20000;

            string body = "<span style='font-weight:bold;'>{0}</span> invites you to sign up with <a href='{1}'>{2}</a>.<br/>" +
                "If you like to accept the invitation click <a href='{3}'>sign up</a>";

            string invitationUrl = string.Format("{0}/invitee.aspx?key={1}", mailingData.msUrl, m_data.ikey);

            var fromAddress = new MailAddress(mailingData.emailAddress, mailingData.siteAddress);
            var toAddress = new MailAddress(m_data.email);

            MailMessage mailMessage = new MailMessage(fromAddress, toAddress);
            mailMessage.Subject = string.Format("Invitation to sign up with <{0}>", mailingData.siteAddress);
            mailMessage.Body = string.Format(body, SoftnetRegistry.getOwnerName(this.Context.User.Identity.Name), mailingData.msUrl, mailingData.siteAddress, invitationUrl);
            mailMessage.IsBodyHtml = true;

            smtpClient.Send(mailMessage);
            
            P_Success.Visible = true;
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
        catch (SmtpException ex)
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

            L_Url.Text = string.Format("{0}/invitee.aspx?key={1}", SoftnetRegistry.settings_getManagementSystemUrl(), m_data.ikey);

            if (string.IsNullOrEmpty(m_data.email))
            {
                P_SendButton.Visible = false;
                TD_SendButton.Attributes["style"] = "";
                L_EMail.Text = "&nbsp;";
            }
            else
                L_EMail.Text = m_data.email;

            if (string.IsNullOrEmpty(m_data.description))
                L_Description.Text = "&nbsp;";
            else
                L_Description.Text = m_data.description;

            if (m_data.assignProviderRole)
                L_AssignProviderRole.Text = "YES";
            else
                L_AssignProviderRole.Text = "NO";
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }
}