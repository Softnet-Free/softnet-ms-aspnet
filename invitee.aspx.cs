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
using System.Web.Security;
using System.Text;

public partial class invitee : System.Web.UI.Page
{
    protected void CreateAccount_Click(object sender, EventArgs e)
    {
        try
        {
            string ikey = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("key");
            if (string.IsNullOrEmpty(ikey))
                return;

            string userFullName = TB_InviteeName.Text.Trim();
            if (userFullName.Length > Constants.MaxLength.owner_name)
            {
                L_ErrorMessage.Visible = true;
                L_ErrorMessage.Text = string.Format("The length of your name must not be more than {0} characters.", Constants.MaxLength.owner_name);
                return;
            }

            if (userFullName.Length < Constants.MinLength.owner_name)
            {
                L_ErrorMessage.Visible = true;
                L_ErrorMessage.Text = string.Format("The length of your name must not be less than {0} characters.", Constants.MinLength.owner_name);
                return;
            }

            if (userFullName.Length != TB_InviteeName.Text.Length)
            {
                L_ErrorMessage.Visible = true;
                L_ErrorMessage.Text = "Your name must not contain leading or trailing whitespace characters.";
                return;
            }

            string accountName = TB_AccountName.Text.Trim();
            if (accountName.Length != TB_AccountName.Text.Length)
            {
                L_ErrorMessage.Visible = true;
                L_ErrorMessage.Text = "The account name must not contain leading or trailing whitespace characters.";
                return;
            }

            if (accountName.Length > Constants.MaxLength.account_name)
            {
                L_ErrorMessage.Visible = true;
                L_ErrorMessage.Text = string.Format("The length of the account name must not be more than {0} characters.", Constants.MaxLength.account_name);
                return;
            }

            if (accountName.Length < Constants.MinLength.account_name)
            {
                L_ErrorMessage.Visible = true;
                L_ErrorMessage.Text = string.Format("The length of the account name must not be less than {0} characters.", Constants.MinLength.account_name);
                return;
            }

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

            byte[] password = Encoding.Unicode.GetBytes(password_text);
            byte[] salt = Randomizer.generateOctetString(16);
            byte[] salt_and_password = new byte[password.Length + salt.Length];
            System.Buffer.BlockCopy(salt, 0, salt_and_password, 0, salt.Length);
            System.Buffer.BlockCopy(password, 0, salt_and_password, salt.Length, password.Length);

            var sha1CSP = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            byte[] salted_password = sha1CSP.ComputeHash(salt_and_password);

            string salt_b64 = Convert.ToBase64String(salt);
            string salted_password_b64 = Convert.ToBase64String(salted_password);

            SoftnetRegistry.account_signupByInvitation(ikey, userFullName, accountName, salted_password_b64, salt_b64);
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
            string ikey = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("key");
            if (string.IsNullOrEmpty(ikey))
                throw new ArgumentSoftnetException("The invitation key has not been specified.");

            InvitationData data = new InvitationData();
            data.ikey = ikey;
            SoftnetRegistry.getInvitationData(data);
            if (data.status == 1)
                throw new ArgumentSoftnetException("The invitation has already been applied.");
            if (data.status == 2)
                throw new ArgumentSoftnetException("Sorry, the invitation has expired.");

            if (string.IsNullOrEmpty(data.email) == false)
            {
                TD_EMail.Visible = true;
                TB_EMail.Text = data.email;
            }
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }
}