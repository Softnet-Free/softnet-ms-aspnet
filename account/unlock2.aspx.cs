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

public partial class account_unlock2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            string receivedAccountName = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("name");
            if (string.IsNullOrWhiteSpace(receivedAccountName) || receivedAccountName.Length > 256)
                throw new ArgumentSoftnetException("The recovery url has an invalid format.");

            string receivedTransactionKey = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("key");
            if (string.IsNullOrWhiteSpace(receivedTransactionKey) || receivedTransactionKey.Length > 64)
                throw new ArgumentSoftnetException("The recovery url has an invalid format.");

            string base64ReceivedHash = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("hash");
            if (string.IsNullOrWhiteSpace(base64ReceivedHash) || base64ReceivedHash.Length > 64)
                throw new ArgumentSoftnetException("The recovery url has an invalid format.");

            AccountTransactionData2 tranState = new AccountTransactionData2();
            tranState.accountName = receivedAccountName;
            SoftnetRegistry.account_getTransactionData(tranState);

            if (receivedTransactionKey.Equals(tranState.transactionKey) == false)
                throw new ArgumentSoftnetException("The recovery url is not valid.");

            if (tranState.currentTime - tranState.createdTime > 60480)
                throw new ArgumentSoftnetException("The recovery url has expired.");

            byte[] accountNameBytes = Encoding.UTF8.GetBytes(receivedAccountName);
            byte[] transactionKeyBytes = Encoding.UTF8.GetBytes(receivedTransactionKey);
            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(tranState.secretKey);

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

            SoftnetRegistry.account_unlock(tranState);

            L_Action.Text = string.Format("The account <span style='color:#D2691E;'>{0}</span> has been unlocked!.", receivedAccountName);           
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }
}