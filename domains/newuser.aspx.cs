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

public partial class newuser : System.Web.UI.Page
{
    UserCreatingDataset m_userDataset;
    UrlBuider m_urlBuider;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        try
        {
            long domainId;
            if (long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("did"), out domainId) == false)
                throw new InvalidIdentifierSoftnetException();

            m_userDataset = new UserCreatingDataset();
            SoftnetRegistry.GetUserCreatingDataset(this.Context.User.Identity.Name, domainId, m_userDataset);

            string retString = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("ret");
            m_urlBuider = new UrlBuider(retString);

            this.Title = string.Format("New User - {0}", m_userDataset.domainName);

            HL_Domain.NavigateUrl = string.Format("~/domains/domain.aspx?did={0}", m_userDataset.domainId);
            HL_Domain.Text = m_userDataset.domainName;

            ListItem emptyItem = new ListItem("--------- select contact ---------", "");
            DDL_ContactList.Items.Add(emptyItem);
            foreach (ContactData cData in m_userDataset.contacts)
            {
                if (cData.status == 0 || cData.status == 1)
                {
                    ListItem item = new ListItem(cData.contactName, cData.contactId.ToString());
                    DDL_ContactList.Items.Add(item);
                }
            }

            int ownerIndex = m_userDataset.users.FindIndex(x => x.name.Equals("Owner"));
            if (ownerIndex >= 0)
            {
                UserData ownerData = m_userDataset.users[ownerIndex];
                m_userDataset.users.RemoveAt(ownerIndex);
                m_userDataset.users.Insert(0, ownerData);
            }

            int guestIndex = m_userDataset.users.FindIndex(x => x.name.Equals("Guest"));
            if (guestIndex >= 0)
            {
                UserData guestData = m_userDataset.users[guestIndex];
                m_userDataset.users.RemoveAt(guestIndex);
                m_userDataset.users.Add(guestData);
            }

            foreach (UserData userData in m_userDataset.users)
            {
                TableRow row = new TableRow();
                T_ExistingUsers.Rows.Add(row);

                if (userData.kind == 2)
                {
                    TableCell cellName = new TableCell();
                    row.Controls.Add(cellName);
                    cellName.Attributes["style"] = "border-bottom: 1px solid #A2C5D3; border-right: 1px solid #A2C5D3; padding: 4px 5px 4px 5px;";
                    Label labelUserName = new Label();
                    cellName.Controls.Add(labelUserName);
                    labelUserName.Text = userData.name;
                    labelUserName.CssClass = "user";

                    if (userData.dedicated)
                        labelUserName.CssClass = labelUserName.CssClass + " user_dedicated";

                    if (userData.enabled == false)
                        labelUserName.CssClass = labelUserName.CssClass + " disabled_status";

                    TableCell cellContact = new TableCell();
                    row.Controls.Add(cellContact);
                    cellContact.Attributes["style"] = "border-bottom: 1px solid #A2C5D3; padding: 4px 5px 4px 5px;";
                }
                else if (userData.kind == 3)
                {
                    userData.contactData = m_userDataset.contacts.Find(x => x.contactId == userData.contactId);
                    if (userData.contactData == null)
                        userData.contactData = ContactData.nullContact;

                    TableCell cellName = new TableCell();
                    row.Controls.Add(cellName);
                    cellName.Attributes["style"] = "border-bottom: 1px solid #A2C5D3; border-right: 1px solid #A2C5D3; padding: 4px 5px 4px 5px;";
                    Label labelUserName = new Label();
                    cellName.Controls.Add(labelUserName);
                    labelUserName.Text = userData.name;
                    labelUserName.CssClass = "user";

                    if (userData.dedicated)
                        labelUserName.CssClass = labelUserName.CssClass + " user_dedicated";

                    if (userData.contactData.status == 2)
                    {
                        labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                        labelUserName.ToolTip = "Your partner has been deleted from the network.";
                    }
                    else if(userData.contactData.status == 3)
                    {
                        labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                        labelUserName.ToolTip = "Your partner has been deleted from the network.";
                    }
                    else if (userData.enabled == false)
                    {
                        labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                        labelUserName.ToolTip = "The user is disabled.";
                    }

                    TableCell cellContact = new TableCell();
                    row.Controls.Add(cellContact);
                    cellContact.Attributes["style"] = "border-bottom: 1px solid #A2C5D3; padding: 4px 5px 4px 5px;";
                    Label labelContactName = new Label();
                    cellContact.Controls.Add(labelContactName);
                    labelContactName.Text = userData.contactData.contactName;
                    labelContactName.CssClass = "name";

                    if (userData.contactData.status == 1)
                    {
                        labelContactName.CssClass = "contact_in_status_1";
                        labelContactName.ToolTip = "Your partner deleted the contact. However it can be usable again if your partner restore it.";
                    }
                    else if (userData.contactData.status == 2)
                    {
                        labelContactName.CssClass = "contact_in_status_2";
                        labelContactName.ToolTip = "The contact is no longer usable as your partner has been deleted from the network.";
                    }
                    else if (userData.contactData.status == 3)
                    {
                        labelContactName.CssClass = "contact_in_status_3";
                        labelContactName.ToolTip = "The contact is unknown.";
                    }
                }
                else if (userData.kind == 1)
                {
                    TableCell cellName = new TableCell();
                    row.Controls.Add(cellName);
                    cellName.Attributes["style"] = "border-bottom: 1px solid #A2C5D3; border-right: 1px solid #A2C5D3; padding: 4px 5px 4px 5px;";
                    Label labelUserName = new Label();
                    cellName.Controls.Add(labelUserName);
                    labelUserName.Text = userData.name;
                    labelUserName.CssClass = "user_owner";

                    if (userData.enabled == false)
                        labelUserName.CssClass = labelUserName.CssClass + " disabled_status";

                    TableCell cellContact = new TableCell();
                    row.Controls.Add(cellContact);
                    cellContact.Attributes["style"] = "border-bottom: 1px solid #A2C5D3; padding: 4px 5px 4px 5px;";
                }
                else
                {
                    TableCell cellName = new TableCell();
                    row.Controls.Add(cellName);
                    cellName.Attributes["style"] = "border-bottom: 1px solid #A2C5D3; border-right: 1px solid #A2C5D3; padding: 4px 5px 4px 5px;";
                    Label labelUserName = new Label();
                    cellName.Controls.Add(labelUserName);
                    labelUserName.Text = userData.name;
                    labelUserName.CssClass = "user_guest";

                    if (userData.enabled == false)
                        labelUserName.CssClass = labelUserName.CssClass + " disabled_status";

                    TableCell cellContact = new TableCell();
                    row.Controls.Add(cellContact);
                    cellContact.Attributes["style"] = "border-bottom: 1px solid #A2C5D3; padding: 4px 5px 4px 5px;";
                }
            }            
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void Back_Click(object sender, EventArgs e)
    {
        string retUrl = m_urlBuider.getBackUrl();
        if (retUrl != null)
            Response.Redirect(retUrl);
        else
            Response.Redirect(string.Format("~/domains/domain.aspx?did={0}", m_userDataset.domainId));        
    }

    protected void CreatePrivateUser_Click(object sender, EventArgs e)
    {
        L_ContactUserError.Visible = false;

        string userName = TB_PrivateUserName.Text;
        if (string.IsNullOrWhiteSpace(userName))
        {
            L_PrivateUserError.Visible = true;
            L_PrivateUserError.Text = "An empty name is not allowed.";
            return;
        }

        if (userName.Length > Constants.MaxLength.user_name)
        {
            L_PrivateUserError.Visible = true;
            L_PrivateUserError.Text = string.Format("The username must not contain more than {0} characters.", Constants.MaxLength.user_name);
            return;
        }

        if (Regex.IsMatch(userName, @"[^\x20-\x7F]", RegexOptions.None))
        {
            L_PrivateUserError.Visible = true;
            L_PrivateUserError.Text = "Valid symbols in the username are latin letters, numbers, spaces and the following characters: $ . * + # @ % & = ' : ^ ( ) [ ] - / !";
            return;
        }

        if (Regex.IsMatch(userName, @"^[a-zA-Z]", RegexOptions.None) == false)
        {
            L_PrivateUserError.Visible = true;
            L_PrivateUserError.Text = "The leading character must be a latin letter.";
            return;
        }

        if (Regex.IsMatch(userName, @"[\s]$", RegexOptions.None))
        {
            L_PrivateUserError.Visible = true;
            L_PrivateUserError.Text = "The trailing space is illegal.";
            return;
        }

        if (Regex.IsMatch(userName, @"[^\w\s.$*+#@%&=':\^()\[\]\-/!]", RegexOptions.None))
        {
            L_PrivateUserError.Visible = true;
            L_PrivateUserError.Text = "Valid symbols in the user name are latin letters, numbers, spaces and the following characters: $ . * + # @ % & = ' : ^ ( ) [ ] - / !";
            return;
        }

        if (Regex.IsMatch(userName, @"[\s]{2,}", RegexOptions.None))
        {
            L_PrivateUserError.Visible = true;
            L_PrivateUserError.Text = "Two or more consecutive spaces are not allowed.";
            return;
        }

        if (m_userDataset.users.Find(x => x.name.Equals(userName, StringComparison.OrdinalIgnoreCase)) != null)
        {
            L_PrivateUserError.Visible = true;
            L_PrivateUserError.Text = string.Format("The user '{0}' has already exist in the domain.", userName);
            return;
        }

        try
        {
            SoftnetTracker.createPrivateUser(m_userDataset.domainId, userName, CB_PrivateUserDedicated.Checked);
            string retUrl = m_urlBuider.getBackUrl();
            if (retUrl != null)
                Response.Redirect(retUrl);
            else
                Response.Redirect(string.Format("~/domains/domain.aspx?did={0}", m_userDataset.domainId));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void DDL_ContactSelected(object sender, EventArgs e)
    {
        L_ContactUserError.Visible = false;
        L_ContactUserError.Text = "";

        DropDownList ddList = (DropDownList)sender;
        if (string.IsNullOrEmpty(ddList.SelectedValue) == false)
        {
            long contactId = long.Parse(ddList.SelectedValue);
            ContactData contactData = m_userDataset.contacts.Find(x => x.contactId == contactId);
            if (contactData != null && string.IsNullOrEmpty(contactData.userDefaultName) == false)
            {
                TB_ContactUserName.Text = contactData.userDefaultName;
            }
            else
            {
                TB_ContactUserName.Text = "";
            }
        }
        else
        {
            TB_ContactUserName.Text = "";        
        }
    }

    protected void CreateContactUser_Click(object sender, EventArgs e)
    {
        L_PrivateUserError.Visible = false;

        if (string.IsNullOrEmpty(DDL_ContactList.SelectedValue))
        {
            L_ContactUserError.Visible = true;
            L_ContactUserError.Text = "A contact must be selected.";
            return;
        }

        string userName = TB_ContactUserName.Text;
        if (string.IsNullOrWhiteSpace(userName))
        {
            L_ContactUserError.Visible = true;
            L_ContactUserError.Text = "An empty name is not allowed.";
            return;
        }

        if (userName.Length > Constants.MaxLength.user_name)
        {
            L_ContactUserError.Visible = true;
            L_ContactUserError.Text = string.Format("The username must not contain more than {0} characters.", Constants.MaxLength.user_name);
            return;
        }
        
        if (Regex.IsMatch(userName, @"[^\x20-\x7F]", RegexOptions.None))
        {
            L_ContactUserError.Visible = true;
            L_ContactUserError.Text = "Valid symbols in the user name are latin letters, numbers, spaces and the following characters: $ * + # @ % & = . ' : ^ ( ) [ ] - / !";
            return;
        }

        if (Regex.IsMatch(userName, @"^[^a-zA-Z]", RegexOptions.None))
        {
            L_ContactUserError.Visible = true;
            L_ContactUserError.Text = "The leading character must be a letter: a-z A-Z";
            return;
        }

        if (Regex.IsMatch(userName, @"[\s]$", RegexOptions.None))
        {
            L_ContactUserError.Visible = true;
            L_ContactUserError.Text = "The trailing space is illegal.";
            return;
        }

        if (Regex.IsMatch(userName, @"[^\w\s.$*+#@%&=':\^()\[\]\-/!]", RegexOptions.None))
        {
            L_ContactUserError.Visible = true;
            L_ContactUserError.Text = "Valid symbols in the user name are latin letters, numbers, spaces and the following characters: $ * + # @ % & = . ' : ^ ( ) [ ] - / !";
            return;
        }

        if (Regex.IsMatch(userName, @"[\s]{2,}", RegexOptions.None))
        {
            L_ContactUserError.Visible = true;
            L_ContactUserError.Text = "Two or more consecutive spaces are not allowed.";
            return;        
        }

        if (m_userDataset.users.Find(x => x.name.Equals(userName, StringComparison.OrdinalIgnoreCase)) != null)
        {
            L_ContactUserError.Visible = true;
            L_ContactUserError.Text = string.Format("The user '{0}' has already exist in the domain.", userName);
            return;
        }

        try
        {
            long contactId = long.Parse(DDL_ContactList.SelectedValue);
            SoftnetTracker.createContactUser(m_userDataset.domainId, contactId, userName, CB_ContactUserDedicated.Checked, CB_ContactUserEnabled.Checked);
            string retUrl = m_urlBuider.getBackUrl();
            if (retUrl != null)
                Response.Redirect(retUrl);
            else
                Response.Redirect(string.Format("~/domains/domain.aspx?did={0}", m_userDataset.domainId));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }
}