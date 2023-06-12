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

public partial class account_setemail : System.Web.UI.Page
{
    protected void Back_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/account/settings.aspx");
    }

    protected void SendMail_Click(object sender, EventArgs e)
    {
        try
        {
            string email = TB_EMail.Text.Trim();
            if (string.IsNullOrWhiteSpace(email))
                return;

            if (EMailValidator.IsValid(email) == false)
                throw new ArgumentSoftnetException("Incorrect email format.");

            string accountName = this.Context.User.Identity.Name.ToLower();

            AccountTransactionData tranData = new AccountTransactionData();
            tranData.accountName = accountName;
            tranData.email = email;
            tranData.transactionKey = Randomizer.generateTransactionKey(Constants.Keys.transaction_key_length);
            SoftnetRegistry.account_initEMailConfirmation(tranData);

            MailingData mailingData = new MailingData();
            SoftnetRegistry.settings_getMailingData(mailingData);
            
            byte[] accountNameBytes = Encoding.UTF8.GetBytes(accountName);
            byte[] emailBytes = Encoding.UTF8.GetBytes(email);
            byte[] transactionKeyBytes = Encoding.UTF8.GetBytes(tranData.transactionKey);
            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(mailingData.secretKey);

            byte[] buffer = new byte[accountNameBytes.Length + emailBytes.Length + transactionKeyBytes.Length + secretKeyBytes.Length];
            int offset = 0;
            System.Buffer.BlockCopy(accountNameBytes, 0, buffer, offset, accountNameBytes.Length);
            offset += accountNameBytes.Length;
            System.Buffer.BlockCopy(emailBytes, 0, buffer, offset, emailBytes.Length);
            offset += emailBytes.Length;
            System.Buffer.BlockCopy(transactionKeyBytes, 0, buffer, offset, transactionKeyBytes.Length);
            offset += transactionKeyBytes.Length;
            System.Buffer.BlockCopy(secretKeyBytes, 0, buffer, offset, secretKeyBytes.Length);

            byte[] hash = Sha1Hash.Compute(buffer);
            string base64Hash = Convert.ToBase64String(hash);

            var smtpClient = new SmtpClient(mailingData.smtpServer, mailingData.smtpPort);
            smtpClient.Credentials = new NetworkCredential(mailingData.emailAddress, mailingData.emailPassword);
            smtpClient.EnableSsl = mailingData.smtpRequiresSSL;
            smtpClient.Timeout = 20000;

            string body = 
                "This message has been sent in behalf of a person who initiated the email confirmation on <a href='{0}/default.aspx'>{1}</a>.<br/>" +
                "If you are that person click <a href='{2}'>confirm this email</a>";

            string confirmationUrl = string.Format("{0}/account/setemail2.aspx?name={1}&email={2}&key={3}&hash={4}", mailingData.siteUrl, accountName, HttpUtility.UrlEncode(email), tranData.transactionKey, HttpUtility.UrlEncode(base64Hash));

            var fromAddress = new MailAddress(mailingData.emailAddress, mailingData.siteAddress);
            var toAddress = new MailAddress(email);

            MailMessage mailMessage = new MailMessage(fromAddress, toAddress);
            mailMessage.Subject = string.Format("Confirmation mail from <{0}>", mailingData.siteAddress);
            mailMessage.Body = string.Format(body, mailingData.siteUrl, mailingData.siteAddress, confirmationUrl);
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

    protected void Page_Load(object sender, EventArgs e)
    {
        TB_EMail.Focus();
    }
}