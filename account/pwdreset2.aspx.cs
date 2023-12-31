﻿/*
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

public partial class account_pwdreset2 : System.Web.UI.Page
{
    AccountTransactionData2 m_tranState;
    string m_receivedAccountName;

    protected void Save_Click(object sender, EventArgs e)
    { 
        try
        {
            string password_text = TB_Password.Text.Trim();
            if (password_text.Length != TB_Password.Text.Length)
            {
                L_ErrorMessage.Visible = true;
                L_ErrorMessage.Text = "The password must not contain leading or trailing whitespace characters.";
                return;
            }

            if (password_text.Length > Constants.MaxLength.owner_password)
            {
                L_ErrorMessage.Visible = true;
                L_ErrorMessage.Text = string.Format("The password length must not be more than {0} characters.", Constants.MaxLength.owner_password);
                return;
            }

            int passwordMinLength = SoftnetRegistry.settings_getUserPasswordMinLength();
            if (password_text.Length < passwordMinLength)
            {
                L_ErrorMessage.Visible = true;
                L_ErrorMessage.Text = string.Format("The password length must not be less than {0} characters", passwordMinLength);
                return;
            }

            TB_Password.Visible = false;

            byte[] password = Encoding.Unicode.GetBytes(password_text);
            byte[] salt = Randomizer.generateOctetString(16);
            byte[] salt_and_password = new byte[password.Length + salt.Length];
            System.Buffer.BlockCopy(salt, 0, salt_and_password, 0, salt.Length);
            System.Buffer.BlockCopy(password, 0, salt_and_password, salt.Length, password.Length);

            var sha1CSP = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            byte[] salted_password = sha1CSP.ComputeHash(salt_and_password);

            string salt_b64 = Convert.ToBase64String(salt);
            string salted_password_b64 = Convert.ToBase64String(salted_password);

            SoftnetRegistry.account_resetPassword(m_tranState, salted_password_b64, salt_b64);
            Response.Redirect("~/account/login.aspx");
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
            m_receivedAccountName = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("name");
            if (string.IsNullOrWhiteSpace(m_receivedAccountName) || m_receivedAccountName.Length > 256)
                throw new ArgumentSoftnetException("The recovery url has an invalid format.");
            
            string receivedTransactionKey = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("key");
            if (string.IsNullOrWhiteSpace(receivedTransactionKey) || receivedTransactionKey.Length > 64)
                throw new ArgumentSoftnetException("The recovery url has an invalid format.");

            string base64ReceivedHash = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("hash");
            if (string.IsNullOrWhiteSpace(base64ReceivedHash) || base64ReceivedHash.Length > 64)
                throw new ArgumentSoftnetException("The recovery url has an invalid format.");

            m_tranState = new AccountTransactionData2();
            m_tranState.accountName = m_receivedAccountName;
            SoftnetRegistry.account_getTransactionData(m_tranState);

            if (receivedTransactionKey.Equals(m_tranState.transactionKey) == false)
                throw new ArgumentSoftnetException("The recovery url is not valid.");

            if (m_tranState.currentTime - m_tranState.createdTime > 60480)
                throw new ArgumentSoftnetException("The recovery url has expired.");

            byte[] accountNameBytes = Encoding.UTF8.GetBytes(m_receivedAccountName);
            byte[] transactionKeyBytes = Encoding.UTF8.GetBytes(receivedTransactionKey);
            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(m_tranState.secretKey);

            byte[] buffer = new byte[accountNameBytes.Length + transactionKeyBytes.Length + secretKeyBytes.Length];
            int offset = 0;
            System.Buffer.BlockCopy(accountNameBytes, 0, buffer, offset, accountNameBytes.Length);
            offset += accountNameBytes.Length;
            System.Buffer.BlockCopy(transactionKeyBytes, 0, buffer, offset, transactionKeyBytes.Length);
            offset += transactionKeyBytes.Length;
            System.Buffer.BlockCopy(secretKeyBytes, 0, buffer, offset, secretKeyBytes.Length);

            byte[] hash = Sha1Hash.Compute(buffer);
            string base64Hash = Convert.ToBase64String(hash);

            if (base64ReceivedHash.Equals(base64Hash) == false)
                throw new ArgumentSoftnetException("The recovery url is not valid.");

            string message = "Type a new password of the account <span style='color:#D2691E;'>{0}</span> and click <span style='font-weight:bold'>save</span>.";
            L_Action.Text = string.Format(message, m_receivedAccountName);
            
            TB_Password.Focus();
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }
}