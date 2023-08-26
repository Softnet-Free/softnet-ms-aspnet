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
using System.Web.Security;

public partial class account_login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {        
        
    }

    protected void LoginUser_LoginError(object sender, EventArgs e)
    {
        MembershipUser userInfo = Membership.GetUser(LoginUser.UserName);
        if (userInfo == null)
        {
            LoginUser.FailureText = string.Format("There is no user in the database with the username <span style='color:black'>{0}</span>.", LoginUser.UserName);
        }
        else if (!userInfo.IsApproved)
        {
            LoginUser.FailureText = "Your account has been disabled. Please contact the site administrator...";
        }
        else if (userInfo.IsLockedOut)
        {
            string unlockUrl = HttpContext.Current.Request.Url.AbsoluteUri.Replace("login.aspx", "unlock.aspx");
            LoginUser.FailureText = "Your account has been locked out because of a maximum number of incorrect login attempts. " +
                string.Format("In order to unlock your account go to the section <a href='{0}'>account unlocking</a>.", unlockUrl);
        }
    }
}
