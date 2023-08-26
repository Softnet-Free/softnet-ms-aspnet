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
using System.Text.RegularExpressions;

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
                TB_UserDefaultName.Text = m_contactData.userDefaultName;
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
            string contactName = TB_ContactName.Text.Trim();
            if (contactName.Length > Constants.MaxLength.contact_name)
            {
                L_ContactNameError.Visible = true;
                L_ContactNameError.Text = string.Format("The contact name must not contain more than {0} characters.", Constants.MaxLength.contact_name);
                return;
            }

            string userDefaultName = TB_UserDefaultName.Text;
            if (userDefaultName.Length > 0)
            {
                if (userDefaultName.Length > Constants.MaxLength.user_name)
                {
                    L_UserDefaultNameError.Visible = true;
                    L_UserDefaultNameError.Text = string.Format("The user name must not contain more than {0} characters.", Constants.MaxLength.user_name);
                    return;
                }

                if (Regex.IsMatch(userDefaultName, @"[^\x20-\x7F]", RegexOptions.None))
                {
                    L_UserDefaultNameError.Visible = true;
                    L_UserDefaultNameError.Text = "Valid symbols in the user name are latin letters, numbers, spaces and the following characters: $ . * + # @ % & = ' : ^ ( ) [ ] - / !";
                    return;
                }

                if (Regex.IsMatch(userDefaultName, @"^[a-zA-Z]", RegexOptions.None) == false)
                {
                    L_UserDefaultNameError.Visible = true;
                    L_UserDefaultNameError.Text = "The leading character must be a latin letter.";
                    return;
                }

                if (Regex.IsMatch(userDefaultName, @"[\s]$", RegexOptions.None))
                {
                    L_UserDefaultNameError.Visible = true;
                    L_UserDefaultNameError.Text = "The trailing space is illegal.";
                    return;
                }

                if (Regex.IsMatch(userDefaultName, @"[^\w\s.$*+#@%&=':\^()\[\]\-/!]", RegexOptions.None))
                {
                    L_UserDefaultNameError.Visible = true;
                    L_UserDefaultNameError.Text = "Valid symbols in the user name are latin letters, numbers, spaces and the following characters: $ . * + # @ % & = ' : ^ ( ) [ ] - / !";
                    return;
                }

                if (Regex.IsMatch(userDefaultName, @"[\s]{2,}", RegexOptions.None))
                {
                    L_UserDefaultNameError.Visible = true;
                    L_UserDefaultNameError.Text = "Two or more consecutive spaces are not allowed.";
                    return;
                }
            }

            SoftnetRegistry.UpdateContact(m_contactData.contactId, contactName, userDefaultName);
            Response.Redirect("~/contacts/default.aspx?edit=1");
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }
}