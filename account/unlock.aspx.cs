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
using System.Text;

public partial class account_unlock : System.Web.UI.Page
{
    protected void SendMail_Click(object sender, EventArgs e)
    {
        string accountName = TB_TargetName.Text.Trim();
        if (string.IsNullOrWhiteSpace(accountName) || accountName.Length > 256)
            return;

        try
        {
            AccountTransactionData tranData = new AccountTransactionData();
            tranData.accountName = accountName;
            tranData.transactionKey = Randomizer.generateTransactionKey(Constants.Keys.transaction_key_length);
            SoftnetRegistry.account_initRecoveryOnName(tranData);
            
            MailingData mailingData = new MailingData();
            SoftnetRegistry.settings_getMailingData(mailingData);

            byte[] accountNameBytes = Encoding.UTF8.GetBytes(tranData.accountName);
            byte[] transactionKeyBytes = Encoding.UTF8.GetBytes(tranData.transactionKey);
            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(mailingData.secretKey);

            byte[] buffer = new byte[accountNameBytes.Length + transactionKeyBytes.Length + secretKeyBytes.Length];
            int offset = 0;
            System.Buffer.BlockCopy(accountNameBytes, 0, buffer, offset, accountNameBytes.Length);
            offset += accountNameBytes.Length;
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
                "This message contains a url generated to unlock the account '{0}' on <a href='{1}/default.aspx'>{2}</a>.<br/>" +
                "If you are the person who requested this url click <a href='{3}'>unlock the account</a>.";

            string confirmationUrl = string.Format("{0}/account/unlock2.aspx?name={1}&key={2}&hash={3}", mailingData.siteUrl, tranData.accountName, tranData.transactionKey, HttpUtility.UrlEncode(base64Hash));

            var fromAddress = new MailAddress(mailingData.emailAddress, mailingData.siteAddress);
            var toAddress = new MailAddress(tranData.email);

            MailMessage mailMessage = new MailMessage(fromAddress, toAddress);
            mailMessage.Subject = string.Format("Recovery mail from <{0}>", mailingData.siteAddress);
            mailMessage.Body = string.Format(body, tranData.accountName, mailingData.siteUrl, mailingData.siteAddress, confirmationUrl);
            mailMessage.IsBodyHtml = true;

            smtpClient.Send(mailMessage);

            P_SendMailButton.Visible = false;
            P_Success.Visible = true;
            L_Success.Text = string.Format("The recovery mail has been sent to <span style='color:#FF7400'>{0}</span>", tranData.email);
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

    }
}