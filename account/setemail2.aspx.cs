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
using System.Text;

public partial class account_setemail2 : System.Web.UI.Page
{
    AccountTransactionData2 m_tranData;
    string m_receivedEMail;
    
    protected void Apply_Click(object sender, EventArgs e)
    {
        try
        {
            P_ApplyButton.Visible = false;
            L_Message.Visible = true;
            SoftnetRegistry.account_setEMail(m_tranData, m_receivedEMail);
            L_Message.Text = "The email has been successfully set!";
        }
        catch (ArgumentSoftnetException ex)
        {
            L_Message.Text = ex.Message;
            L_Message.CssClass = "failureNotification";
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
            string receivedAccountName = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("name");
            if (string.IsNullOrWhiteSpace(receivedAccountName) || receivedAccountName.Length > 256)
                throw new ArgumentSoftnetException("The confirmation url has an invalid format.");

            m_receivedEMail = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("email");
            if (string.IsNullOrWhiteSpace(m_receivedEMail) || m_receivedEMail.Length > 256)
                throw new ArgumentSoftnetException("The confirmation url has an invalid format.");

            string receivedTransactionKey = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("key");
            if (string.IsNullOrWhiteSpace(receivedTransactionKey) || receivedTransactionKey.Length > 64)
                throw new ArgumentSoftnetException("The confirmation url has an invalid format.");

            string base64ReceivedHash = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("hash");
            if (string.IsNullOrWhiteSpace(base64ReceivedHash) || base64ReceivedHash.Length > 64)
                throw new ArgumentSoftnetException("The confirmation url has an invalid format.");

            m_tranData = new AccountTransactionData2();
            m_tranData.accountName = receivedAccountName;
            SoftnetRegistry.account_getTransactionData(m_tranData);

            if (receivedTransactionKey.Equals(m_tranData.transactionKey) == false)
                throw new ArgumentSoftnetException("The confirmation url is not valid.");

            if (m_tranData.currentTime - m_tranData.createdTime > 60480)
                throw new ArgumentSoftnetException("The confirmation url has expired.");

            byte[] accountNameBytes = Encoding.UTF8.GetBytes(receivedAccountName);
            byte[] emailBytes = Encoding.UTF8.GetBytes(m_receivedEMail);
            byte[] transactionKeyBytes = Encoding.UTF8.GetBytes(receivedTransactionKey);
            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(m_tranData.secretKey);

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

            if(base64ReceivedHash.Equals(base64Hash) == false)
                throw new ArgumentSoftnetException("The confirmation url is not valid.");

            if (string.IsNullOrEmpty(m_tranData.email) == false)
            {
                L_Action.Text = string.Format("Currently, the recovery email of the account <span style='color:#D2691E'>{0}</span> is <span style='color:blue'>{1}</span>. If you like to change it to <span style='color:blue'>{2}</span> click the <span style='font-weight:bold'>apply</span> button.", receivedAccountName, m_tranData.email, m_receivedEMail);
            }
            else
            {
                L_Action.Text = string.Format("Currently, the account <span style='color:#D2691E'>{0}</span> has no recovery email. If you want to set the email <span style='color:blue'>{1}</span> click the <span style='font-weight:bold'>apply</span> button.", receivedAccountName, m_receivedEMail);            
            }
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }
}