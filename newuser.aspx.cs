/*
*	Copyright 2023 Robert Koifman
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
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.Net.Mail;

public partial class newuser : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {            
            if (SoftnetRegistry.settings_getAnyoneCanRegister() == false)
                this.Response.Redirect("~/default.aspx");
        }
        catch (SoftnetException ex) 
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void SendMail_Click(object sender, EventArgs e)
    {
        try
        {
            string email = TB_EMail.Text.Trim();
            if (string.IsNullOrWhiteSpace(email) || email.Length > 256)
                return;

            if (EMailValidator.IsValid(email) == false)
                throw new ArgumentSoftnetException("Incorrect email format.");

            if (SoftnetRegistry.isEmailInUse(email))
                throw new ArgumentSoftnetException("Sorry, the email is already in use.");

            MailingData data = new MailingData();
            SoftnetRegistry.settings_getMailingData(data);

            byte[] emailBytes = Encoding.UTF8.GetBytes(email);
            string time = data.currentTime.ToString();
            byte[] timeBytes = Encoding.UTF8.GetBytes(time);
            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(data.secretKey);

            byte[] buffer = new byte[emailBytes.Length + timeBytes.Length + secretKeyBytes.Length];
            int offset = 0;
            System.Buffer.BlockCopy(emailBytes, 0, buffer, offset, emailBytes.Length);
            offset += emailBytes.Length;
            System.Buffer.BlockCopy(timeBytes, 0, buffer, offset, timeBytes.Length);
            offset += timeBytes.Length;
            System.Buffer.BlockCopy(secretKeyBytes, 0, buffer, offset, secretKeyBytes.Length);

            byte[] hash = Sha1Hash.Compute(buffer);
            string hashBase64 = Convert.ToBase64String(hash);

            var smtpClient = new SmtpClient(data.smtpServer, data.smtpPort);
            smtpClient.Credentials = new NetworkCredential(data.emailAddress, data.emailPassword);
            smtpClient.EnableSsl = data.smtpRequiresSSL;
            smtpClient.Timeout = 20000;

            string body =
                "This message has been sent in behalf of a person who initiated signing up with <span style='font-weight:bold'>{0}</span>.<br/>" +
                "If you are that person click <a href='{1}'>sign up with <span style='font-weight:bold'>{2}</span></a>.";

            string confirmationUrl = string.Format("{0}/signup.aspx?email={1}&time={2}&hash={3}", data.msUrl, HttpUtility.UrlEncode(email), time, HttpUtility.UrlEncode(hashBase64));

            var fromAddress = new MailAddress(data.emailAddress, data.siteAddress);
            var toAddress = new MailAddress(email);

            MailMessage mailMessage = new MailMessage(fromAddress, toAddress);
            mailMessage.Subject = string.Format("Confirmation mail to sign up with <{0}>", data.siteAddress);
            mailMessage.Body = string.Format(body, data.siteAddress, confirmationUrl, data.siteAddress);
            mailMessage.IsBodyHtml = true;

            smtpClient.Send(mailMessage);

            P_SendMailButton.Visible = false;
            P_Success.Visible = true;
        }
        catch (ArgumentSoftnetException ex)
        {
            P_SendMailButton.Visible = false;
            P_Error.Visible = true;
            L_Error.Text = ex.Message;
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
}