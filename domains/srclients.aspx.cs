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
using System.Web.UI.HtmlControls;

public partial class srclients : System.Web.UI.Page
{
    SiteClientsDataset m_siteDataset;
    SiteData m_siteData;
    ServiceData m_serviceData;
    UrlBuider m_urlBuider;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        try
        {
            long siteId;
            if (long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("sid"), out siteId) == false)
                throw new InvalidIdentifierSoftnetException();

            m_siteDataset = new SiteClientsDataset();
            SoftnetRegistry.GetRSiteClientsDataset(this.Context.User.Identity.Name, siteId, m_siteDataset);

            if (m_siteDataset.siteData.siteKind != 1 || m_siteDataset.services.Count != 1 || m_siteDataset.siteData.structured == false || m_siteDataset.siteData.rolesSupported == false)
            {
                Response.Redirect(string.Format("~/domains/domain.aspx?did={0}", m_siteDataset.domainId));
                return;
            }
            m_siteData = m_siteDataset.siteData;
            m_serviceData = m_siteDataset.services[0];

            string retString = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("ret");
            m_urlBuider = new UrlBuider(retString);

            LoadData();
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void Back_Click(object sender, EventArgs e)
    {
        if (m_urlBuider.hasBackUrl())
            Response.Redirect(m_urlBuider.getBackUrl());
        else
            Response.Redirect(string.Format("~/domains/domain.aspx?did={0}", m_siteDataset.domainId));
    }

    protected void Refresh_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/srclients.aspx?sid={0}", m_siteData.siteId)));
    }

    protected void Config_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/srsite.aspx?sid={0}", m_siteData.siteId)));
    }

    protected void DeleteSite_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getNextUrl(string.Format("~/domains/deletesite.aspx?sid={0}", m_siteData.siteId), string.Format("~/domains/srclients.aspx?sid={0}", m_siteData.siteId)));
    }

    protected void AddClient_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        UserData userData = (UserData)button.Args[0];
        try
        {
            long clientId = SoftnetRegistry.CreateClient(m_siteDataset.ownerId, m_siteData.siteId, userData.userId);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/srclients.aspx?sid={0}&cid={1}&cpr=1", m_siteData.siteId, clientId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void AddGuestClient_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        UserData userData = (UserData)button.Args[0];
        try
        {
            long clientId = SoftnetRegistry.CreateGuestClient(m_siteDataset.ownerId, m_siteData.siteId, userData.userId);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/srclients.aspx?sid={0}&cid={1}&cpr=1", m_siteData.siteId, clientId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void EditClient_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        ClientData clientData = (ClientData)button.Args[0];
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/srclients.aspx?sid={0}&cid={1}&cpr=1", m_siteData.siteId, clientData.clientId)));
    }

    protected void ViewClient_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/srclients.aspx?sid={0}", m_siteData.siteId)));
    }

    protected void DeleteClient_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        ClientData clientData = (ClientData)button.Args[0];
        try
        {
            SoftnetTracker.deleteClient(m_siteData.siteId, clientData.clientId);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/srclients.aspx?sid={0}", m_siteData.siteId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void GenerateClientPassword_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        ClientData clientData = (ClientData)button.Args[0];
        Panel panelClientPassword = (Panel)button.Args[1];
        try
        {
            int passwordLength = SoftnetRegistry.settings_getClientPasswordLength();
            string password = Randomizer.generatePassword(passwordLength);
            byte[] salt = Randomizer.generateOctetString(16);
            byte[] saltedPassword = PasswordHash.Compute(salt, password);

            SoftnetTracker.setClientPassword(m_siteData.siteId, clientData.clientId, Convert.ToBase64String(salt), Convert.ToBase64String(saltedPassword));

            panelClientPassword.Controls.Clear();
            HtmlGenericControl tablePassword = new HtmlGenericControl("table");
            panelClientPassword.Controls.Add(tablePassword);
            HtmlGenericControl tr = new HtmlGenericControl("tr");
            tablePassword.Controls.Add(tr);
            tablePassword.Attributes["class"] = "auto_table";

            HtmlGenericControl td = new HtmlGenericControl("td");
            tr.Controls.Add(td);
            td.Attributes["class"] = "auto_table";
            td.Attributes["style"] = "padding-right: 5px;";

            HtmlGenericControl spanParamName = new HtmlGenericControl("span");
            td.Controls.Add(spanParamName);
            spanParamName.Attributes["class"] = "param_name";
            spanParamName.InnerText = "password:";

            td = new HtmlGenericControl("td");
            tr.Controls.Add(td);
            td.Attributes["class"] = "auto_table";
            
            HtmlGenericControl spanParamValue = new HtmlGenericControl("span");
            td.Controls.Add(spanParamValue);
            spanParamValue.Attributes["class"] = "param_value";
            spanParamValue.InnerText = password;

            HtmlGenericControl divOkButton = new HtmlGenericControl("div");
            panelClientPassword.Controls.Add(divOkButton);
            divOkButton.Attributes["class"] = "SubmitButtonMini Gray";
            divOkButton.Attributes["style"] = "width: 50px; margin-left: 40px; margin-top: 15px";

            TButton buttonOk = new TButton();
            divOkButton.Controls.Add(buttonOk);
            buttonOk.Text = "ok";
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void EditClientAccount_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        ClientData clientData = (ClientData)button.Args[0];
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/srclients.aspx?sid={0}&cid={1}&cpr=1", m_siteData.siteId, clientData.clientId)));
    }

    protected void EditClientPing_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        ClientData clientData = (ClientData)button.Args[0];
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/srclients.aspx?sid={0}&cid={1}&cpr=2", m_siteData.siteId, clientData.clientId)));    
    }

    protected void SetPingPeriod_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        ClientData clientData = (ClientData)button.Args[0];
        TextBox textboxPingPeriod = (TextBox)button.Args[1];
        try
        {
            int pingPeriod;
            if (int.TryParse(textboxPingPeriod.Text, out pingPeriod) == false)
                throw new ArgumentException("Invalid format.");

            if (pingPeriod != 0 && (pingPeriod < 10 || pingPeriod > 300))
                throw new ArgumentException("The value of the ping period must be in the range from 10 seconds to 300 seconds or 0.");

            SoftnetTracker.setClientPingPeriod(m_siteData.siteId, clientData.clientId, pingPeriod);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/srclients.aspx?sid={0}&cid={1}", m_siteData.siteId, clientData.clientId)));
        }
        catch (ArgumentException ex)
        {
            HtmlGenericControl td = (HtmlGenericControl)button.Args[2];
            HtmlGenericControl spanMessage = new HtmlGenericControl("span");
            td.Controls.Add(spanMessage);
            spanMessage.Attributes["class"] = "error_message";
            spanMessage.Attributes["style"] = "display: block; margin-top: 10px;";
            spanMessage.InnerText = "Error: " + ex.Message;
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    void LoadData()
    {
        this.Title = string.Format("{0} - {1}", m_siteData.description, m_siteDataset.domainName);
        
        HL_Domain.NavigateUrl = string.Format("~/domains/domain.aspx?did={0}", m_siteDataset.domainId);
        HL_Domain.Text = m_siteDataset.domainName;
        L_Description.Text = m_siteData.description;

        L_SiteType.Text = m_siteData.serviceType + "<span class='gray_text'> ( </span>" + m_siteData.contractAuthor + "<span class='gray_text'> )</span>";
        if (m_siteData.enabled == false)
        {
            L_SiteStatus.Visible = true;
            L_SiteStatus.Text = "site disabled";
        }

        {
            HtmlGenericControl table = new HtmlGenericControl("table");
            TD_ServiceProps.Controls.Add(table);
            table.Attributes["class"] = "auto_table";
            HtmlGenericControl tr = new HtmlGenericControl("tr");
            table.Controls.Add(tr);

            HtmlGenericControl td = new HtmlGenericControl("td");
            tr.Controls.Add(td);
            td.Attributes["class"] = "auto_table";
            td.Attributes["style"] = "padding-right: 15px;";

            HtmlGenericControl span = new HtmlGenericControl("span");
            td.Controls.Add(span);
            span.Attributes["class"] = "name";
            span.InnerText = m_serviceData.hostname;

            if (string.IsNullOrEmpty(m_serviceData.serviceType) == false)
            {
                if (m_serviceData.serviceType.Equals(m_siteData.serviceType) == false || m_serviceData.contractAuthor.Equals(m_siteData.contractAuthor) == false)
                {
                    td = new HtmlGenericControl("td");
                    tr.Controls.Add(td);
                    td.Attributes["class"] = "auto_table";
                    td.Attributes["style"] = "padding-right: 5px;";

                    span = new HtmlGenericControl("span");
                    td.Controls.Add(span);
                    span.Attributes["class"] = "service_type";
                    span.InnerHtml = m_serviceData.serviceType + "<span class='gray_text'> ( </span>" + m_serviceData.contractAuthor + "<span class='gray_text'> )</span>";

                    span = new HtmlGenericControl("span");
                    TD_ServiceStatus.Controls.Add(span);
                    span.Attributes["class"] = "object_status";
                    span.InnerText = "service type conflict";
                }
                else if (m_serviceData.ssHash.Equals(m_siteData.ssHash) == false)
                {
                    span = new HtmlGenericControl("span");
                    TD_ServiceStatus.Controls.Add(span);
                    span.Attributes["class"] = "object_status";
                    span.InnerText = "site structure mismatch";
                }

                if (string.IsNullOrEmpty(m_serviceData.version) == false)
                {
                    td = new HtmlGenericControl("td");
                    tr.Controls.Add(td);
                    td.Attributes["class"] = "auto_table";

                    span = new HtmlGenericControl("span");
                    td.Controls.Add(span);
                    span.Attributes["class"] = "service_type";
                    span.InnerHtml = "<span class='version_label'>ver. </span>" + m_serviceData.version;
                }
            }
            else
            {
                span = new HtmlGenericControl("span");
                TD_ServiceStatus.Controls.Add(span);
                span.Attributes["class"] = "object_status";
                span.InnerText = "service type undefined";
            }

            if (m_serviceData.pingPeriod > 0)
            {
                td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";
                td.Attributes["style"] = "padding-left: 30px;";

                span = new HtmlGenericControl("span");
                td.Controls.Add(span);
                span.InnerHtml = "<span style='color:green;'>P:</span> " + m_serviceData.pingPeriod.ToString();
            }
        }

        List<RoleData> roles = m_siteDataset.roles;
        if (roles.Count == 0)
            throw new DataIntegritySoftnetException();

        string roleListView = "";
        if (roles.Count > 1)
        {
            for (int i = 0; i < roles.Count; i++)
            {
                if (i == 0)
                    roleListView = "<span class='user_role'>" + roles[i].name + "</span>";
                else
                    roleListView = roleListView + "<span class='delimeter'>,&nbsp;&nbsp;</span><span class='user_role'>" + roles[i].name + "</span>";
            }
        }
        else if (roles.Count == 1)
        {
            roleListView = "<span class='user_role'>" + roles[0].name + "</span>";
        }
        L_SupportedRoles.Text = roleListView;

        if (m_siteData.defaultRoleId != 0)
        {
            RoleData defaultRoleData = roles.Find(x => x.roleId == m_siteData.defaultRoleId);
            if (defaultRoleData != null)
            {
                P_DefaultRole.Visible = true;
                L_DefaultRole.Text = defaultRoleData.name;
            }
            else
            {
                P_DefaultRole.Visible = false;
            }
        }
        else
        {
            P_DefaultRole.Visible = false;
        }

        if (m_siteData.guestSupported)
        {
            P_GuestStatus.Visible = true;
            if (m_siteData.guestAllowed)
            {
                L_GuestStatus.CssClass = "enabled_status";
                L_GuestStatus.Text = "allowed";
            }
            else
            {
                L_GuestStatus.CssClass = "disabled_status";
                L_GuestStatus.Text = "denied";
            }

            if (m_siteData.guestAllowed)
            {
                P_GuestPage.Visible = true;
                L_GuestPage.Text = string.Format("{0}/guest.aspx?site={1}", SoftnetRegistry.settings_getSiteUrl(), m_siteData.siteKey);

                if (m_siteData.statelessGuestSupported)
                {
                    P_SharedClient.Visible = true;
                    L_SharedGuestURI.Text = string.Format("softnet-ss://{0}@{1}", m_siteData.siteKey, SoftnetRegistry.settings_getServerAddress());
                }
            }
        }

        int ownerIndex = m_siteDataset.users.FindIndex(x => x.kind == 1);
        if (ownerIndex >= 0)
        {
            UserData ownerData = m_siteDataset.users[ownerIndex];
            m_siteDataset.users.RemoveAt(ownerIndex);
            m_siteDataset.users.Insert(0, ownerData);
        }

        int guestIndex = m_siteDataset.users.FindIndex(x => x.kind == 4);
        if (guestIndex >= 0)
        {
            UserData guestData = m_siteDataset.users[guestIndex];
            m_siteDataset.users.RemoveAt(guestIndex);
            m_siteDataset.users.Add(guestData);
        }

        List<AllowedUser> allowedUsers = new List<AllowedUser>();
        List<UserInRole> usersInRoles = m_siteDataset.usersInRoles;
        roles = m_siteDataset.roles;
        foreach (UserData userData in m_siteDataset.users)
        {
            if (userData.kind == 2)
            {
                AllowedUser allowedUser = new AllowedUser(userData);
                foreach (RoleData roleData in roles)
                {
                    UserInRole userInRole = usersInRoles.Find(x => x.roleId == roleData.roleId && x.userId == userData.userId);
                    if (userInRole != null)
                        allowedUser.roles.Add(roleData);
                    else if (userData.dedicated == false && roleData.roleId == m_siteData.defaultRoleId)
                        allowedUser.defaultRole = roleData;
                }

                if (allowedUser.roles.Count > 0 || allowedUser.defaultRole != null)
                    allowedUsers.Add(allowedUser);
            }
            else if (userData.kind == 3)
            {
                userData.contactData = m_siteDataset.contacts.Find(x => x.contactId == userData.contactId);
                if (userData.contactData == null)
                    userData.contactData = ContactData.nullContact;

                AllowedUser allowedUser = new AllowedUser(userData);
                foreach (RoleData roleData in roles)
                {
                    UserInRole userInRole = usersInRoles.Find(x => x.roleId == roleData.roleId && x.userId == userData.userId);
                    if (userInRole != null)
                        allowedUser.roles.Add(roleData);
                    else if (userData.dedicated == false && roleData.roleId == m_siteData.defaultRoleId)
                        allowedUser.defaultRole = roleData;
                }

                if (allowedUser.roles.Count > 0 || allowedUser.defaultRole != null)
                    allowedUsers.Add(allowedUser);
            }
            else if (userData.kind == 1)
            {
                AllowedUser allowedUser = new AllowedUser(userData);
                foreach (RoleData roleData in roles)
                {
                    UserInRole userInRole = usersInRoles.Find(x => x.roleId == roleData.roleId && x.userId == userData.userId);
                    if (userInRole != null)
                        allowedUser.roles.Add(roleData);
                    else if (roleData.roleId == m_siteData.defaultRoleId)
                        allowedUser.defaultRole = roleData;
                }

                if (allowedUser.roles.Count > 0 || allowedUser.defaultRole != null)
                    allowedUsers.Add(allowedUser);
            }
            else if (m_siteData.guestSupported && m_siteData.guestAllowed) // userData.kind == 4
            {
                AllowedUser allowedUser = new AllowedUser(userData);
                allowedUsers.Add(allowedUser);
            }
        }

        long editedClientId = 0;
        long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("cid"), out editedClientId);

        int cntProp = 0;
        int.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("cpr"), out cntProp);

        List<ClientData> clients = m_siteDataset.clients;        
        PH_Clients.Controls.Clear();
        foreach (AllowedUser allowedUser in allowedUsers)
        {
            Panel panelUser = new Panel();
            PH_Clients.Controls.Add(panelUser);
            panelUser.CssClass = "site_block_item underline";            

            if (allowedUser.userData.kind == 2)
            {                
                HtmlGenericControl table = new HtmlGenericControl("table");
                panelUser.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                HtmlGenericControl tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                HtmlGenericControl td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";

                Label labelUserName = new Label();
                td.Controls.Add(labelUserName);
                labelUserName.Text = allowedUser.userData.name;
                labelUserName.CssClass = "user";

                if (allowedUser.userData.dedicated)
                    labelUserName.CssClass = labelUserName.CssClass + " user_dedicated";

                if (allowedUser.userData.enabled == false)
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";

                Label labelRoles = new Label();
                td.Controls.Add(labelRoles);
                if (allowedUser.roles.Count > 0)
                {
                    labelRoles.Text = " &nbsp;<span class='gray_text'>(</span>&nbsp;<span class='user_role'>" + allowedUser.roles[0].name + "</span>";
                    for (int i = 1; i < allowedUser.roles.Count; i++)
                        labelRoles.Text = labelRoles.Text + ", &nbsp;<span class='user_role'>" + allowedUser.roles[i].name + "</span>";
                    if (allowedUser.defaultRole != null)
                        labelRoles.Text = labelRoles.Text + ", &nbsp;<span class='user_default_role'>" + allowedUser.defaultRole.name + "</span>&nbsp;<span class='gray_text'>)</span>";
                    else
                        labelRoles.Text = labelRoles.Text + "&nbsp;<span class='gray_text'>)</span>";
                }
                else
                {
                    labelRoles.Text = " &nbsp;<span class='gray_text'>(</span>&nbsp;<span class='user_default_role'>" + allowedUser.defaultRole.name + "</span>&nbsp;<span class='gray_text'>)</span>";
                }

                td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";
                td.Attributes["style"] = "padding-left: 20px";

                HtmlGenericControl divAddClientButton = new HtmlGenericControl("div");
                td.Controls.Add(divAddClientButton);
                divAddClientButton.Attributes["class"] = "SubmitButtonMini Green";

                TButton buttonAddClient = new TButton();
                divAddClientButton.Controls.Add(buttonAddClient);
                buttonAddClient.Args.Add(allowedUser.userData);
                buttonAddClient.Text = "add client";
                buttonAddClient.ID = string.Format("B_CreateClient_{0}", allowedUser.userData.userId);
                buttonAddClient.Click += new EventHandler(AddClient_Click);                

                List<ClientData> userClients = clients.FindAll(x => x.userId == allowedUser.userData.userId);
                if (userClients.Count > 0)
                {
                    Panel panelClientList = new Panel();
                    panelUser.Controls.Add(panelClientList);
                    panelClientList.Attributes["style"] = "padding-top: 5px;";

                    foreach (ClientData clientData in userClients)
                    {
                        if (clientData.clientId != editedClientId)
                        {
                            HtmlGenericControl divClient = new HtmlGenericControl("div");
                            panelClientList.Controls.Add(divClient);
                            divClient.Attributes["class"] = "site_block_item";

                            HtmlGenericControl tableLayout = new HtmlGenericControl("table");
                            divClient.Controls.Add(tableLayout);
                            tableLayout.Attributes["class"] = "wide_table";
                            tr = new HtmlGenericControl("tr");
                            tableLayout.Controls.Add(tr);

                            HtmlGenericControl tdLeft = new HtmlGenericControl("td");
                            tr.Controls.Add(tdLeft);
                            tdLeft.Attributes["class"] = "wide_table";
                            tdLeft.Attributes["style"] = "width: 47px; padding-right: 10px;";

                            HtmlGenericControl tdRight = new HtmlGenericControl("td");
                            tr.Controls.Add(tdRight);
                            tdRight.Attributes["class"] = "wide_table";

                            HtmlGenericControl divEditClientButton = new HtmlGenericControl("div");
                            tdLeft.Controls.Add(divEditClientButton);
                            divEditClientButton.Attributes["class"] = "SubmitButtonMini Blue";

                            TButton buttonEditClient = new TButton();
                            divEditClientButton.Controls.Add(buttonEditClient);
                            buttonEditClient.Args.Add(clientData);
                            buttonEditClient.Text = ">>";
                            buttonEditClient.ID = string.Format("B_EditClient_{0}", clientData.clientId);
                            buttonEditClient.Click += new EventHandler(EditClient_Click);

                            Panel panelClientBody = new Panel();
                            tdRight.Controls.Add(panelClientBody);

                            table = new HtmlGenericControl("table");
                            panelClientBody.Controls.Add(table);
                            table.Attributes["class"] = "auto_table";
                            tr = new HtmlGenericControl("tr");
                            table.Controls.Add(tr);

                            td = new HtmlGenericControl("td");
                            tr.Controls.Add(td);
                            td.Attributes["class"] = "auto_table";

                            HtmlGenericControl spanClientKey = new HtmlGenericControl("span");
                            td.Controls.Add(spanClientKey);
                            spanClientKey.InnerText = string.Format("{0}", clientData.clientKey);
                            spanClientKey.Attributes["class"] = "name";

                            if (string.IsNullOrEmpty(clientData.serviceType) == false &&
                                (clientData.serviceType.Equals(m_siteData.serviceType) == false || clientData.contractAuthor.Equals(m_siteData.contractAuthor) == false))
                            {
                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 20px;";

                                HtmlGenericControl spanServiceType = new HtmlGenericControl("span");
                                td.Controls.Add(spanServiceType);
                                spanServiceType.InnerHtml = string.Format("{0} <span class='gray_text'>(</span> {1} <span class='gray_text'>)</span>", clientData.serviceType, clientData.contractAuthor);
                                spanServiceType.Attributes["class"] = "error_text";
                            }

                            if (string.IsNullOrEmpty(clientData.clientDescription) == false)
                            {
                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 20px;";

                                HtmlGenericControl spanClientType = new HtmlGenericControl("span");
                                td.Controls.Add(spanClientType);
                                spanClientType.InnerText = clientData.clientDescription;
                                spanClientType.Attributes["class"] = "client_description";
                            }

                            if (clientData.pingPeriod > 0)
                            {
                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 30px;";

                                HtmlGenericControl span = new HtmlGenericControl("span");
                                td.Controls.Add(span);
                                span.InnerHtml = "<span class='ping_label'>P:</span> " + clientData.pingPeriod.ToString();
                            }
                        }
                        else
                        {
                            HtmlGenericControl divClient = new HtmlGenericControl("div");
                            panelClientList.Controls.Add(divClient);
                            divClient.Attributes["class"] = "site_block_item";                            

                            HtmlGenericControl divDashedFrame = new HtmlGenericControl("div");
                            divClient.Controls.Add(divDashedFrame);
                            divDashedFrame.Attributes["style"] = "border: 1px dashed gray; padding: 1px; padding-bottom: 10px;";

                            HtmlGenericControl tableLayout = new HtmlGenericControl("table");
                            divDashedFrame.Controls.Add(tableLayout);
                            tableLayout.Attributes["class"] = "wide_table";
                            tr = new HtmlGenericControl("tr");
                            tableLayout.Controls.Add(tr);
                            tr.Attributes["style"] = "background-color: #E0E0E0;";

                            HtmlGenericControl tdLeft = new HtmlGenericControl("td");
                            tr.Controls.Add(tdLeft);
                            tdLeft.Attributes["class"] = "wide_table";
                            tdLeft.Attributes["style"] = "width: 47px; padding-right: 10px;";

                            HtmlGenericControl tdMiddle = new HtmlGenericControl("td");
                            tr.Controls.Add(tdMiddle);
                            tdMiddle.Attributes["class"] = "wide_table";
                            tdMiddle.Attributes["style"] = "padding-right: 3px;";

                            HtmlGenericControl tdRight = new HtmlGenericControl("td");
                            tr.Controls.Add(tdRight);
                            tdRight.Attributes["class"] = "wide_table";
                            tdRight.Attributes["style"] = "width: 22px;";

                            HtmlGenericControl divViewClientButton = new HtmlGenericControl("div");
                            tdLeft.Controls.Add(divViewClientButton);
                            divViewClientButton.Attributes["class"] = "SubmitButtonMini Selected Blue";

                            TButton buttonViewClient = new TButton();
                            divViewClientButton.Controls.Add(buttonViewClient);
                            buttonViewClient.Args.Add(clientData);
                            buttonViewClient.Text = "<<";
                            buttonViewClient.ID = string.Format("B_ViewClient_{0}", clientData.clientId);
                            buttonViewClient.Click += new EventHandler(ViewClient_Click);

                            HtmlGenericControl divDeleteClientButton = new HtmlGenericControl("div");
                            tdRight.Controls.Add(divDeleteClientButton);
                            divDeleteClientButton.Attributes["class"] = "SubmitButtonSquareMini RedOrange";

                            TButton buttonDeleteClient = new TButton();
                            divDeleteClientButton.Controls.Add(buttonDeleteClient);
                            buttonDeleteClient.Args.Add(clientData);
                            buttonDeleteClient.Text = "X";
                            buttonDeleteClient.ToolTip = "Delete Client";
                            buttonDeleteClient.ID = string.Format("B_DeleteClient_{0}", clientData.clientId);
                            buttonDeleteClient.Click += new EventHandler(DeleteClient_Click);

                            table = new HtmlGenericControl("table");
                            tdMiddle.Controls.Add(table);
                            table.Attributes["class"] = "auto_table";
                            tr = new HtmlGenericControl("tr");
                            table.Controls.Add(tr);

                            td = new HtmlGenericControl("td");
                            tr.Controls.Add(td);
                            td.Attributes["class"] = "auto_table";

                            HtmlGenericControl spanClientKey = new HtmlGenericControl("span");
                            td.Controls.Add(spanClientKey);
                            spanClientKey.InnerText = string.Format("{0}", clientData.clientKey);
                            spanClientKey.Attributes["class"] = "name";

                            if (string.IsNullOrEmpty(clientData.serviceType) == false &&
                                (clientData.serviceType.Equals(m_siteData.serviceType) == false || clientData.contractAuthor.Equals(m_siteData.contractAuthor) == false))
                            {
                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 20px;";

                                HtmlGenericControl spanServiceType = new HtmlGenericControl("span");
                                td.Controls.Add(spanServiceType);
                                spanServiceType.InnerHtml = string.Format("{0} <span class='gray_text'>(</span> {1} <span class='gray_text'>)</span>", clientData.serviceType, clientData.contractAuthor);
                                spanServiceType.Attributes["class"] = "error_text";
                            }

                            if (string.IsNullOrEmpty(clientData.clientDescription) == false)
                            {
                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 20px;";

                                HtmlGenericControl spanClientType = new HtmlGenericControl("span");
                                td.Controls.Add(spanClientType);
                                spanClientType.InnerText = clientData.clientDescription;
                                spanClientType.Attributes["class"] = "client_description";
                            }

                            if (clientData.pingPeriod > 0)
                            {
                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 30px;";

                                HtmlGenericControl span = new HtmlGenericControl("span");
                                td.Controls.Add(span);
                                span.InnerHtml = "<span class='ping_label'>P:</span> " + clientData.pingPeriod.ToString();
                            }

                            tr = new HtmlGenericControl("tr");
                            tableLayout.Controls.Add(tr);

                            tdLeft = new HtmlGenericControl("td");
                            tr.Controls.Add(tdLeft);
                            tdLeft.Attributes["class"] = "wide_table";

                            tdMiddle = new HtmlGenericControl("td");
                            tr.Controls.Add(tdMiddle);
                            tdMiddle.Attributes["class"] = "wide_table";
                            tdMiddle.Attributes["style"] = "padding-right: 3px;";

                            tdRight = new HtmlGenericControl("td");
                            tr.Controls.Add(tdRight);
                            tdRight.Attributes["class"] = "wide_table";

                            table = new HtmlGenericControl("table");
                            tdMiddle.Controls.Add(table);
                            table.Attributes["class"] = "auto_table";
                            table.Attributes["style"] = "margin-top: 10px;";
                            tr = new HtmlGenericControl("tr");
                            table.Controls.Add(tr);

                            td = new HtmlGenericControl("td");
                            tr.Controls.Add(td);
                            td.Attributes["class"] = "auto_table";

                            HtmlGenericControl divAccountButton = new HtmlGenericControl("div");
                            td.Controls.Add(divAccountButton);
                            divAccountButton.Attributes["class"] = "SubmitButtonSquare Blue";

                            TButton tButton = new TButton();
                            tButton.Args.Add(clientData);
                            divAccountButton.Controls.Add(tButton);
                            tButton.Text = "account";
                            tButton.ID = string.Format("B_EditClientAccount_{0}", clientData.clientId);
                            tButton.Click += new EventHandler(EditClientAccount_Click);

                            td = new HtmlGenericControl("td");
                            tr.Controls.Add(td);
                            td.Attributes["class"] = "auto_table";
                            td.Attributes["style"] = "padding-left: 15px;";

                            HtmlGenericControl divPingButton = new HtmlGenericControl("div");
                            td.Controls.Add(divPingButton);
                            divPingButton.Attributes["class"] = "SubmitButtonSquare Blue";

                            tButton = new TButton();
                            tButton.Args.Add(clientData);
                            divPingButton.Controls.Add(tButton);
                            tButton.Text = "ping period";
                            tButton.ID = string.Format("B_EditClientPing_{0}", clientData.clientId);
                            tButton.Click += new EventHandler(EditClientPing_Click);

                            if (cntProp == 1)
                            {
                                divAccountButton.Attributes["class"] = "SubmitButtonSquare Selected Blue";

                                HtmlGenericControl spanAccount = new HtmlGenericControl("span");
                                tdMiddle.Controls.Add(spanAccount);
                                spanAccount.Attributes["class"] = "client_uri";
                                spanAccount.Attributes["style"] = "display:block; margin-top: 12px;";
                                spanAccount.InnerText = string.Format("softnet-s://{0}@{1}", clientData.clientKey, SoftnetRegistry.settings_getServerAddress());

                                Panel panelClientPassword = new Panel();
                                tdMiddle.Controls.Add(panelClientPassword);
                                panelClientPassword.Attributes["style"] = "padding-top: 10px;";
                                panelClientPassword.EnableViewState = false;

                                HtmlGenericControl tableGeneratePasswordButton = new HtmlGenericControl("table");
                                panelClientPassword.Controls.Add(tableGeneratePasswordButton);
                                tableGeneratePasswordButton.Attributes["style"] = "margin-top: 5px;";
                                tr = new HtmlGenericControl("tr");
                                tableGeneratePasswordButton.Controls.Add(tr);
                                tableGeneratePasswordButton.Attributes["class"] = "auto_table";

                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";

                                HtmlGenericControl divGeneratePasswordButton = new HtmlGenericControl("div");
                                td.Controls.Add(divGeneratePasswordButton);
                                divGeneratePasswordButton.Attributes["class"] = "SubmitButtonMini Green";

                                TButton buttonGeneratePassword = new TButton();
                                divGeneratePasswordButton.Controls.Add(buttonGeneratePassword);
                                buttonGeneratePassword.Args.Add(clientData);
                                buttonGeneratePassword.Args.Add(panelClientPassword);
                                buttonGeneratePassword.Text = "generate password";
                                buttonGeneratePassword.ID = string.Format("B_GeneratePassword_{0}", clientData.clientId);
                                buttonGeneratePassword.Click += new EventHandler(GenerateClientPassword_Click);
                            }
                            else if (cntProp == 2)
                            {
                                divPingButton.Attributes["class"] = "SubmitButtonSquare Selected Blue";

                                table = new HtmlGenericControl("table");
                                tdMiddle.Controls.Add(table);
                                table.Attributes["class"] = "auto_table";
                                table.Attributes["style"] = "margin-top: 15px;";
                                tr = new HtmlGenericControl("tr");
                                table.Controls.Add(tr);

                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";

                                HtmlGenericControl span = new HtmlGenericControl("span");
                                td.Controls.Add(span);
                                span.Attributes["class"] = "param_name";
                                span.InnerText = "ping period:";

                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 5px;";

                                TextBox textboxPingPeriod = new TextBox();
                                td.Controls.Add(textboxPingPeriod);
                                textboxPingPeriod.Attributes["style"] = "border: 1px solid #7FBA00; outline:none; width:40px; margin: 0px; padding: 3px;";
                                textboxPingPeriod.Text = clientData.pingPeriod.ToString();
                                textboxPingPeriod.ID = "TB_PingPeriod";
                                textboxPingPeriod.Attributes["autocomplete"] = "off";

                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 5px;";

                                HtmlGenericControl spanSec = new HtmlGenericControl("span");
                                td.Controls.Add(spanSec);
                                spanSec.Attributes["class"] = "black_text";
                                spanSec.InnerText = "sec.";

                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 15px;";

                                HtmlGenericControl divSavePingPeriod = new HtmlGenericControl("div");
                                td.Controls.Add(divSavePingPeriod);
                                divSavePingPeriod.Attributes["class"] = "SubmitButtonMini Green";

                                TButton buttonSavePingPeriod = new TButton();
                                divSavePingPeriod.Controls.Add(buttonSavePingPeriod);
                                buttonSavePingPeriod.Args.Add(clientData);
                                buttonSavePingPeriod.Args.Add(textboxPingPeriod);
                                buttonSavePingPeriod.Args.Add(tdMiddle);
                                buttonSavePingPeriod.Text = "save";
                                buttonSavePingPeriod.ID = string.Format("B_SavePingPeriod_{0}", clientData.clientId);
                                buttonSavePingPeriod.Click += new EventHandler(SetPingPeriod_Click);

                                span = new HtmlGenericControl("span");
                                tdMiddle.Controls.Add(span);
                                span.Attributes["style"] = "display: block; margin-top: 10px; color: #3C6C80";
                                span.InnerHtml =
                                    "The minimum value is 10 seconds and the maximum is 300 seconds.<br/>" +
                                    "The default value is 0 which sets the ping period to the endpoint's local value.";
                            }

                            TextBox textboxScrollPosition = new TextBox();
                            panelUser.Controls.Add(textboxScrollPosition);
                            textboxScrollPosition.Attributes["style"] = "width:0px; height:0px; border-width:0px; padding: 0px; margin:0px";
                            textboxScrollPosition.Focus();
                        }
                    }
                }            
            }
            else if (allowedUser.userData.kind == 3)
            {
                Label labelUserName = new Label();
                panelUser.Controls.Add(labelUserName);
                labelUserName.Text = allowedUser.userData.name;
                labelUserName.CssClass = "user";

                if (allowedUser.userData.dedicated)
                    labelUserName.CssClass += " user_dedicated";

                if (allowedUser.userData.contactData.status == 2)
                {
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                    labelUserName.ToolTip = "Your partner has been deleted from the network.";
                }
                else if (allowedUser.userData.contactData.status == 3)
                {
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                    labelUserName.ToolTip = "Your partner has been deleted from the network.";
                }
                else if (allowedUser.userData.enabled == false)
                {
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                    labelUserName.ToolTip = "The user is disabled.";
                }   

                Label labelContactName = new Label();
                panelUser.Controls.Add(labelContactName);
                labelContactName.Text = " &nbsp;<span class='gray_text'>&#60;</span>" + ContactDisplayName.Adjust(allowedUser.userData.contactData.contactName) + "<span class='gray_text'>&#62;</span>";
                labelContactName.CssClass = "contact_in_status_0";

                if (allowedUser.userData.contactData.status == 1)
                {
                    labelContactName.CssClass = "contact_in_status_1";
                    labelContactName.ToolTip = "Your partner deleted the contact.";
                }
                else if (allowedUser.userData.contactData.status == 2)
                {
                    labelContactName.CssClass = "contact_in_status_2";
                    labelContactName.ToolTip = "The contact is no longer usable as your partner has been deleted from the network.";
                }
                else if (allowedUser.userData.contactData.status == 3)
                {
                    labelContactName.CssClass = "contact_in_status_3";
                    labelContactName.ToolTip = "The contact is unknown.";
                }                

                Label labelRoles = new Label();
                panelUser.Controls.Add(labelRoles);
                if (allowedUser.roles.Count > 0)
                {
                    labelRoles.Text = " &nbsp;<span class='gray_text'>(</span>&nbsp;<span class='user_role'>" + allowedUser.roles[0].name + "</span>";
                    for (int i = 1; i < allowedUser.roles.Count; i++)
                        labelRoles.Text = labelRoles.Text + ", &nbsp;<span class='user_role'>" + allowedUser.roles[i].name + "</span>";
                    if (allowedUser.defaultRole != null)
                        labelRoles.Text = labelRoles.Text + ", &nbsp;<span class='user_default_role'>" + allowedUser.defaultRole.name + "</span>&nbsp;<span class='gray_text'>)</span>";
                    else
                        labelRoles.Text = labelRoles.Text + "&nbsp;<span class='gray_text'>)</span>";
                }
                else
                {
                    labelRoles.Text = " &nbsp;<span class='gray_text'>(</span>&nbsp;<span class='user_default_role'>" + allowedUser.defaultRole.name + "</span>&nbsp;<span class='gray_text'>)</span>";
                }
                
                List<ClientData> userClients = clients.FindAll(x => x.userId == allowedUser.userData.userId);
                if (userClients.Count > 0)
                {
                    Panel panelClientList = new Panel();
                    panelUser.Controls.Add(panelClientList);
                    panelClientList.Attributes["style"] = "padding-top: 5px;";

                    foreach (ClientData clientData in userClients)
                    {
                        HtmlGenericControl divClient = new HtmlGenericControl("div");
                        panelClientList.Controls.Add(divClient);
                        divClient.Attributes["class"] = "site_block_item";

                        HtmlGenericControl tableLayout = new HtmlGenericControl("table");
                        divClient.Controls.Add(tableLayout);
                        tableLayout.Attributes["class"] = "wide_table";
                        HtmlGenericControl tr = new HtmlGenericControl("tr");
                        tableLayout.Controls.Add(tr);

                        HtmlGenericControl tdLeft = new HtmlGenericControl("td");
                        tr.Controls.Add(tdLeft);
                        tdLeft.Attributes["class"] = "wide_table";
                        tdLeft.Attributes["style"] = "width: 7px; padding-left: 36px; padding-right: 14px;";    

                        HtmlGenericControl tdRight = new HtmlGenericControl("td");
                        tr.Controls.Add(tdRight);
                        tdRight.Attributes["class"] = "wide_table";

                        HtmlGenericControl divListItem = new HtmlGenericControl("div");
                        tdLeft.Controls.Add(divListItem);
                        divListItem.Attributes["style"] = "width: 7px; height: 7px; background-color: #4F8DA6";

                        Panel panelClientBody = new Panel();
                        tdRight.Controls.Add(panelClientBody);

                        HtmlGenericControl table = new HtmlGenericControl("table");
                        panelClientBody.Controls.Add(table);
                        table.Attributes["class"] = "auto_table";
                        tr = new HtmlGenericControl("tr");
                        table.Controls.Add(tr);

                        HtmlGenericControl td = new HtmlGenericControl("td");
                        tr.Controls.Add(td);
                        td.Attributes["class"] = "auto_table";

                        HtmlGenericControl spanClientKey = new HtmlGenericControl("span");
                        td.Controls.Add(spanClientKey);
                        spanClientKey.InnerText = string.Format("{0}", clientData.clientKey); 
                        spanClientKey.Attributes["class"] = "name";

                        if (string.IsNullOrEmpty(clientData.serviceType) == false &&
                                (clientData.serviceType.Equals(m_siteData.serviceType) == false || clientData.contractAuthor.Equals(m_siteData.contractAuthor) == false))
                        {
                            td = new HtmlGenericControl("td");
                            tr.Controls.Add(td);
                            td.Attributes["class"] = "auto_table";
                            td.Attributes["style"] = "padding-left: 20px;";

                            HtmlGenericControl spanServiceType = new HtmlGenericControl("span");
                            td.Controls.Add(spanServiceType);
                            spanServiceType.InnerHtml = string.Format("{0} <span class='gray_text'>(</span> {1} <span class='gray_text'>)</span>", clientData.serviceType, clientData.contractAuthor);
                            spanServiceType.Attributes["class"] = "error_text";
                        }

                        if (string.IsNullOrEmpty(clientData.clientDescription) == false)
                        {
                            td = new HtmlGenericControl("td");
                            tr.Controls.Add(td);
                            td.Attributes["class"] = "auto_table";
                            td.Attributes["style"] = "padding-left: 20px;";

                            HtmlGenericControl spanClientType = new HtmlGenericControl("span");
                            td.Controls.Add(spanClientType);
                            spanClientType.InnerText = clientData.clientDescription;
                            spanClientType.Attributes["class"] = "client_description";
                        }

                        if (clientData.pingPeriod > 0)
                        {
                            td = new HtmlGenericControl("td");
                            tr.Controls.Add(td);
                            td.Attributes["class"] = "auto_table";
                            td.Attributes["style"] = "padding-left: 30px;";

                            HtmlGenericControl span = new HtmlGenericControl("span");
                            td.Controls.Add(span);
                            span.InnerHtml = "<span class='ping_label'>P:</span> " + clientData.pingPeriod.ToString();
                        }
                    }
                }                
            }
            else if (allowedUser.userData.kind == 1)
            {
                HtmlGenericControl table = new HtmlGenericControl("table");
                panelUser.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                HtmlGenericControl tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                HtmlGenericControl td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";

                Label labelUserName = new Label();
                td.Controls.Add(labelUserName);
                labelUserName.Text = allowedUser.userData.name;
                labelUserName.CssClass = "user_owner";

                if (allowedUser.userData.dedicated)
                    labelUserName.CssClass = labelUserName.CssClass + " user_dedicated";

                if (allowedUser.userData.enabled == false)
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";

                Label labelRoles = new Label();
                td.Controls.Add(labelRoles);
                if (allowedUser.roles.Count > 0)
                {
                    labelRoles.Text = " &nbsp;<span class='gray_text'>(</span>&nbsp;<span class='user_role'>" + allowedUser.roles[0].name + "</span>";
                    for (int i = 1; i < allowedUser.roles.Count; i++)
                        labelRoles.Text = labelRoles.Text + ", &nbsp;<span class='user_role'>" + allowedUser.roles[i].name + "</span>";
                    if (allowedUser.defaultRole != null)
                        labelRoles.Text = labelRoles.Text + ", &nbsp;<span class='user_default_role'>" + allowedUser.defaultRole.name + "</span>&nbsp;<span class='gray_text'>)</span>";
                    else
                        labelRoles.Text = labelRoles.Text + "&nbsp;<span class='gray_text'>)</span>";
                }
                else
                {
                    labelRoles.Text = " &nbsp;<span class='gray_text'>(</span>&nbsp;<span class='user_default_role'>" + allowedUser.defaultRole.name + "</span>&nbsp;<span class='gray_text'>)</span>";
                }

                td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";
                td.Attributes["style"] = "padding-left: 20px";

                HtmlGenericControl divAddClientButton = new HtmlGenericControl("div");
                td.Controls.Add(divAddClientButton);
                divAddClientButton.Attributes["class"] = "SubmitButtonMini Green";

                TButton buttonAddClient = new TButton();
                divAddClientButton.Controls.Add(buttonAddClient);
                buttonAddClient.Args.Add(allowedUser.userData);
                buttonAddClient.Text = "add client";
                buttonAddClient.ID = string.Format("B_CreateClient_{0}", allowedUser.userData.userId);
                buttonAddClient.Click += new EventHandler(AddClient_Click);                

                List<ClientData> userClients = clients.FindAll(x => x.userId == allowedUser.userData.userId);
                if (userClients.Count > 0)
                {
                    Panel panelClientList = new Panel();
                    panelUser.Controls.Add(panelClientList);
                    panelClientList.Attributes["style"] = "padding-top: 5px;";                

                    foreach (ClientData clientData in userClients)
                    {
                        if (clientData.clientId != editedClientId)
                        {
                            HtmlGenericControl divClient = new HtmlGenericControl("div");
                            panelClientList.Controls.Add(divClient);
                            divClient.Attributes["class"] = "site_block_item";

                            HtmlGenericControl tableLayout = new HtmlGenericControl("table");
                            divClient.Controls.Add(tableLayout);
                            tableLayout.Attributes["class"] = "wide_table";
                            tr = new HtmlGenericControl("tr");
                            tableLayout.Controls.Add(tr);

                            HtmlGenericControl tdLeft = new HtmlGenericControl("td");
                            tr.Controls.Add(tdLeft);
                            tdLeft.Attributes["class"] = "wide_table";
                            tdLeft.Attributes["style"] = "width: 47px; padding-right: 10px;"; 

                            HtmlGenericControl tdRight = new HtmlGenericControl("td");
                            tr.Controls.Add(tdRight);
                            tdRight.Attributes["class"] = "wide_table";

                            HtmlGenericControl divEditClientButton = new HtmlGenericControl("div");
                            tdLeft.Controls.Add(divEditClientButton);
                            divEditClientButton.Attributes["class"] = "SubmitButtonMini Blue";                            

                            TButton buttonEditClient = new TButton();
                            divEditClientButton.Controls.Add(buttonEditClient);
                            buttonEditClient.Args.Add(clientData);
                            buttonEditClient.Text = ">>";
                            buttonEditClient.ID = string.Format("B_EditClient_{0}", clientData.clientId);
                            buttonEditClient.Click += new EventHandler(EditClient_Click);

                            Panel panelClientBody = new Panel();
                            tdRight.Controls.Add(panelClientBody);                           

                            table = new HtmlGenericControl("table");
                            panelClientBody.Controls.Add(table);
                            table.Attributes["class"] = "auto_table";
                            tr = new HtmlGenericControl("tr");
                            table.Controls.Add(tr);

                            td = new HtmlGenericControl("td");
                            tr.Controls.Add(td);
                            td.Attributes["class"] = "auto_table";

                            HtmlGenericControl spanClientKey = new HtmlGenericControl("span");
                            td.Controls.Add(spanClientKey);
                            spanClientKey.InnerText = string.Format("{0}", clientData.clientKey); 
                            spanClientKey.Attributes["class"] = "name";

                            if (string.IsNullOrEmpty(clientData.serviceType) == false &&
                                (clientData.serviceType.Equals(m_siteData.serviceType) == false || clientData.contractAuthor.Equals(m_siteData.contractAuthor) == false))
                            {
                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 20px;";

                                HtmlGenericControl spanServiceType = new HtmlGenericControl("span");
                                td.Controls.Add(spanServiceType);
                                spanServiceType.InnerHtml = string.Format("{0} <span class='gray_text'>(</span> {1} <span class='gray_text'>)</span>", clientData.serviceType, clientData.contractAuthor);
                                spanServiceType.Attributes["class"] = "error_text";
                            }

                            if (string.IsNullOrEmpty(clientData.clientDescription) == false)
                            {
                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 20px;";

                                HtmlGenericControl spanClientType = new HtmlGenericControl("span");
                                td.Controls.Add(spanClientType);
                                spanClientType.InnerText = clientData.clientDescription;
                                spanClientType.Attributes["class"] = "client_description";
                            }

                            if (clientData.pingPeriod > 0)
                            {
                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 30px;";

                                HtmlGenericControl span = new HtmlGenericControl("span");
                                td.Controls.Add(span);
                                span.InnerHtml = "<span class='ping_label'>P:</span> " + clientData.pingPeriod.ToString();
                            }
                        }
                        else
                        {
                            HtmlGenericControl divClient = new HtmlGenericControl("div");
                            panelClientList.Controls.Add(divClient);
                            divClient.Attributes["class"] = "site_block_item";                            

                            HtmlGenericControl divDashedFrame = new HtmlGenericControl("div");
                            divClient.Controls.Add(divDashedFrame);
                            divDashedFrame.Attributes["style"] = "border: 1px dashed gray; padding: 1px; padding-bottom: 10px;";

                            HtmlGenericControl tableLayout = new HtmlGenericControl("table");
                            divDashedFrame.Controls.Add(tableLayout);
                            tableLayout.Attributes["class"] = "wide_table";
                            tr = new HtmlGenericControl("tr");
                            tableLayout.Controls.Add(tr);
                            tr.Attributes["style"] = "background-color: #E0E0E0;";

                            HtmlGenericControl tdLeft = new HtmlGenericControl("td");
                            tr.Controls.Add(tdLeft);
                            tdLeft.Attributes["class"] = "wide_table";
                            tdLeft.Attributes["style"] = "width: 47px; padding-right: 10px;";

                            HtmlGenericControl tdMiddle = new HtmlGenericControl("td");
                            tr.Controls.Add(tdMiddle);
                            tdMiddle.Attributes["class"] = "wide_table";
                            tdMiddle.Attributes["style"] = "padding-right: 3px;";

                            HtmlGenericControl tdRight = new HtmlGenericControl("td");
                            tr.Controls.Add(tdRight);
                            tdRight.Attributes["class"] = "wide_table";
                            tdRight.Attributes["style"] = "width: 22px;";

                            HtmlGenericControl divViewClientButton = new HtmlGenericControl("div");
                            tdLeft.Controls.Add(divViewClientButton);
                            divViewClientButton.Attributes["class"] = "SubmitButtonMini Selected Blue";                            

                            TButton buttonViewClient = new TButton();
                            divViewClientButton.Controls.Add(buttonViewClient);
                            buttonViewClient.Args.Add(clientData);
                            buttonViewClient.Text = "<<";
                            buttonViewClient.ID = string.Format("B_ViewClient_{0}", clientData.clientId);
                            buttonViewClient.Click += new EventHandler(ViewClient_Click);

                            HtmlGenericControl divDeleteClientButton = new HtmlGenericControl("div");
                            tdRight.Controls.Add(divDeleteClientButton);
                            divDeleteClientButton.Attributes["class"] = "SubmitButtonSquareMini RedOrange";

                            TButton buttonDeleteClient = new TButton();
                            divDeleteClientButton.Controls.Add(buttonDeleteClient);
                            buttonDeleteClient.Args.Add(clientData);
                            buttonDeleteClient.Text = "X";
                            buttonDeleteClient.ToolTip = "Delete Client";
                            buttonDeleteClient.ID = string.Format("B_DeleteClient_{0}", clientData.clientId);
                            buttonDeleteClient.Click += new EventHandler(DeleteClient_Click);

                            table = new HtmlGenericControl("table");
                            tdMiddle.Controls.Add(table);
                            table.Attributes["class"] = "auto_table";
                            tr = new HtmlGenericControl("tr");
                            table.Controls.Add(tr);

                            td = new HtmlGenericControl("td");
                            tr.Controls.Add(td);
                            td.Attributes["class"] = "auto_table";

                            HtmlGenericControl spanClientKey = new HtmlGenericControl("span");
                            td.Controls.Add(spanClientKey);
                            spanClientKey.InnerText = string.Format("{0}", clientData.clientKey);
                            spanClientKey.Attributes["class"] = "name";

                            if (string.IsNullOrEmpty(clientData.serviceType) == false &&
                                (clientData.serviceType.Equals(m_siteData.serviceType) == false || clientData.contractAuthor.Equals(m_siteData.contractAuthor) == false))
                            {
                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 20px;";

                                HtmlGenericControl spanServiceType = new HtmlGenericControl("span");
                                td.Controls.Add(spanServiceType);
                                spanServiceType.InnerHtml = string.Format("{0} <span class='gray_text'>(</span> {1} <span class='gray_text'>)</span>", clientData.serviceType, clientData.contractAuthor);
                                spanServiceType.Attributes["class"] = "error_text";
                            }

                            if (string.IsNullOrEmpty(clientData.clientDescription) == false)
                            {
                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 20px;";

                                HtmlGenericControl spanClientType = new HtmlGenericControl("span");
                                td.Controls.Add(spanClientType);
                                spanClientType.InnerText = clientData.clientDescription;
                                spanClientType.Attributes["class"] = "client_description";
                            }

                            if (clientData.pingPeriod > 0)
                            {
                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 30px;";

                                HtmlGenericControl span = new HtmlGenericControl("span");
                                td.Controls.Add(span);
                                span.InnerHtml = "<span class='ping_label'>P:</span> " + clientData.pingPeriod.ToString();
                            }

                            tr = new HtmlGenericControl("tr");
                            tableLayout.Controls.Add(tr);

                            tdLeft = new HtmlGenericControl("td");
                            tr.Controls.Add(tdLeft);
                            tdLeft.Attributes["class"] = "wide_table";                            

                            tdMiddle = new HtmlGenericControl("td");
                            tr.Controls.Add(tdMiddle);
                            tdMiddle.Attributes["class"] = "wide_table";
                            tdMiddle.Attributes["style"] = "padding-right: 3px;";                            

                            tdRight = new HtmlGenericControl("td");
                            tr.Controls.Add(tdRight);
                            tdRight.Attributes["class"] = "wide_table";

                            table = new HtmlGenericControl("table");
                            tdMiddle.Controls.Add(table);
                            table.Attributes["class"] = "auto_table";
                            table.Attributes["style"] = "margin-top: 10px;";
                            tr = new HtmlGenericControl("tr");
                            table.Controls.Add(tr);

                            td = new HtmlGenericControl("td");
                            tr.Controls.Add(td);
                            td.Attributes["class"] = "auto_table";

                            HtmlGenericControl divAccountButton = new HtmlGenericControl("div");
                            td.Controls.Add(divAccountButton);
                            divAccountButton.Attributes["class"] = "SubmitButtonSquare Blue";

                            TButton tButton = new TButton();
                            tButton.Args.Add(clientData);
                            divAccountButton.Controls.Add(tButton);
                            tButton.Text = "account";
                            tButton.ID = string.Format("B_EditClientAccount_{0}", clientData.clientId);
                            tButton.Click += new EventHandler(EditClientAccount_Click);

                            td = new HtmlGenericControl("td");
                            tr.Controls.Add(td);
                            td.Attributes["class"] = "auto_table";
                            td.Attributes["style"] = "padding-left: 15px;";

                            HtmlGenericControl divPingButton = new HtmlGenericControl("div");
                            td.Controls.Add(divPingButton);
                            divPingButton.Attributes["class"] = "SubmitButtonSquare Blue";

                            tButton = new TButton();
                            tButton.Args.Add(clientData);
                            divPingButton.Controls.Add(tButton);
                            tButton.Text = "ping period";
                            tButton.ID = string.Format("B_EditClientPing_{0}", clientData.clientId);
                            tButton.Click += new EventHandler(EditClientPing_Click);

                            if (cntProp == 1)
                            {
                                divAccountButton.Attributes["class"] = "SubmitButtonSquare Selected Blue";

                                HtmlGenericControl spanAccount = new HtmlGenericControl("span");
                                tdMiddle.Controls.Add(spanAccount);
                                spanAccount.Attributes["class"] = "client_uri";
                                spanAccount.Attributes["style"] = "display:block; margin-top: 12px;";
                                spanAccount.InnerText = string.Format("softnet-s://{0}@{1}", clientData.clientKey, SoftnetRegistry.settings_getServerAddress());

                                Panel panelClientPassword = new Panel();
                                tdMiddle.Controls.Add(panelClientPassword);
                                panelClientPassword.Attributes["style"] = "padding-top: 10px;";
                                panelClientPassword.EnableViewState = false;

                                HtmlGenericControl tableGeneratePasswordButton = new HtmlGenericControl("table");
                                panelClientPassword.Controls.Add(tableGeneratePasswordButton);
                                tableGeneratePasswordButton.Attributes["style"] = "margin-top: 5px;";
                                tr = new HtmlGenericControl("tr");
                                tableGeneratePasswordButton.Controls.Add(tr);
                                tableGeneratePasswordButton.Attributes["class"] = "auto_table";

                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";

                                HtmlGenericControl divGeneratePasswordButton = new HtmlGenericControl("div");
                                td.Controls.Add(divGeneratePasswordButton);
                                divGeneratePasswordButton.Attributes["class"] = "SubmitButtonMini Green";

                                TButton buttonGeneratePassword = new TButton();
                                divGeneratePasswordButton.Controls.Add(buttonGeneratePassword);
                                buttonGeneratePassword.Args.Add(clientData);
                                buttonGeneratePassword.Args.Add(panelClientPassword);
                                buttonGeneratePassword.Text = "generate password";
                                buttonGeneratePassword.ID = string.Format("B_GeneratePassword_{0}", clientData.clientId);
                                buttonGeneratePassword.Click += new EventHandler(GenerateClientPassword_Click);
                            }
                            else if (cntProp == 2)
                            {
                                divPingButton.Attributes["class"] = "SubmitButtonSquare Selected Blue";

                                table = new HtmlGenericControl("table");
                                tdMiddle.Controls.Add(table);
                                table.Attributes["class"] = "auto_table";
                                table.Attributes["style"] = "margin-top: 15px;";
                                tr = new HtmlGenericControl("tr");
                                table.Controls.Add(tr);

                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";

                                HtmlGenericControl span = new HtmlGenericControl("span");
                                td.Controls.Add(span);
                                span.Attributes["class"] = "param_name";
                                span.InnerText = "ping period:";

                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 5px;";

                                TextBox textboxPingPeriod = new TextBox();
                                td.Controls.Add(textboxPingPeriod);
                                textboxPingPeriod.Attributes["style"] = "border: 1px solid #7FBA00; outline:none; width:40px; margin: 0px; padding: 3px;";
                                textboxPingPeriod.Text = clientData.pingPeriod.ToString();
                                textboxPingPeriod.ID = "TB_PingPeriod";
                                textboxPingPeriod.Attributes["autocomplete"] = "off";

                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 5px;";

                                HtmlGenericControl spanSec = new HtmlGenericControl("span");
                                td.Controls.Add(spanSec);
                                spanSec.Attributes["class"] = "black_text";
                                spanSec.InnerText = "sec.";

                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 15px;";

                                HtmlGenericControl divSavePingPeriod = new HtmlGenericControl("div");
                                td.Controls.Add(divSavePingPeriod);
                                divSavePingPeriod.Attributes["class"] = "SubmitButtonMini Green";

                                TButton buttonSavePingPeriod = new TButton();
                                divSavePingPeriod.Controls.Add(buttonSavePingPeriod);
                                buttonSavePingPeriod.Args.Add(clientData);
                                buttonSavePingPeriod.Args.Add(textboxPingPeriod);
                                buttonSavePingPeriod.Args.Add(tdMiddle);
                                buttonSavePingPeriod.Text = "save";
                                buttonSavePingPeriod.ID = string.Format("B_SavePingPeriod_{0}", clientData.clientId);
                                buttonSavePingPeriod.Click += new EventHandler(SetPingPeriod_Click);

                                span = new HtmlGenericControl("span");
                                tdMiddle.Controls.Add(span);
                                span.Attributes["style"] = "display: block; margin-top: 10px; color: #3C6C80";
                                span.InnerHtml =
                                    "The minimum value is 10 seconds and the maximum is 300 seconds.<br/>" +
                                    "The default value is 0 which sets the ping period to the endpoint's local value.";
                            }
                            
                            TextBox textboxScrollPosition = new TextBox();
                            panelUser.Controls.Add(textboxScrollPosition);
                            textboxScrollPosition.Attributes["style"] = "width:0px; height:0px; border-width:0px; padding: 0px; margin:0px";
                            textboxScrollPosition.Focus();
                        }
                    }
                }            
            }
            else if (allowedUser.userData.kind == 4)
            {
                HtmlGenericControl table = new HtmlGenericControl("table");
                panelUser.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                HtmlGenericControl tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                HtmlGenericControl td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";

                Label labelUserName = new Label();
                td.Controls.Add(labelUserName);
                labelUserName.Text = allowedUser.userData.name;
                labelUserName.CssClass = "user_guest";

                if (allowedUser.userData.enabled == false)
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";

                td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";
                td.Attributes["style"] = "padding-left: 20px";
                
                HtmlGenericControl divAddClientButton = new HtmlGenericControl("div");
                td.Controls.Add(divAddClientButton);
                divAddClientButton.Attributes["class"] = "SubmitButtonMini Green";

                TButton buttonAddClient = new TButton();
                divAddClientButton.Controls.Add(buttonAddClient);
                buttonAddClient.Args.Add(allowedUser.userData);
                buttonAddClient.Text = "add client";
                buttonAddClient.ID = string.Format("B_CreateClient_{0}", allowedUser.userData.userId);
                buttonAddClient.Click += new EventHandler(AddGuestClient_Click);

                if (m_siteDataset.gclientCount > 0)
                {
                    td = new HtmlGenericControl("td");
                    tr.Controls.Add(td);
                    td.Attributes["class"] = "auto_table";
                    td.Attributes["style"] = "padding-left: 50px";

                    HtmlGenericControl span = new HtmlGenericControl("span");
                    td.Controls.Add(span);
                    span.Attributes["style"] = "color: gray;";
                    span.InnerHtml = string.Format("guest clients total: <span style='color:black'>{0}</span>", m_siteDataset.gclientCount);
                }

                if (m_siteDataset.guestClients.Count > 0)
                {
                    Panel panelClientList = new Panel();
                    panelUser.Controls.Add(panelClientList);
                    panelClientList.Attributes["style"] = "padding-top: 5px;";

                    foreach (ClientData clientData in m_siteDataset.guestClients)
                    {
                        if (clientData.clientId != editedClientId)
                        {
                            HtmlGenericControl divClient = new HtmlGenericControl("div");
                            panelClientList.Controls.Add(divClient);
                            divClient.Attributes["class"] = "site_block_item";

                            HtmlGenericControl tableLayout = new HtmlGenericControl("table");
                            divClient.Controls.Add(tableLayout);
                            tableLayout.Attributes["class"] = "wide_table";
                            tr = new HtmlGenericControl("tr");
                            tableLayout.Controls.Add(tr);

                            HtmlGenericControl tdLeft = new HtmlGenericControl("td");
                            tr.Controls.Add(tdLeft);
                            tdLeft.Attributes["class"] = "wide_table";
                            tdLeft.Attributes["style"] = "width: 47px; padding-right: 10px;";

                            HtmlGenericControl tdRight = new HtmlGenericControl("td");
                            tr.Controls.Add(tdRight);
                            tdRight.Attributes["class"] = "wide_table";

                            HtmlGenericControl divEditClientButton = new HtmlGenericControl("div");
                            tdLeft.Controls.Add(divEditClientButton);
                            divEditClientButton.Attributes["class"] = "SubmitButtonMini Blue";

                            TButton buttonEditClient = new TButton();
                            divEditClientButton.Controls.Add(buttonEditClient);
                            buttonEditClient.Args.Add(clientData);
                            buttonEditClient.Text = ">>";
                            buttonEditClient.ID = string.Format("B_EditClient_{0}", clientData.clientId);
                            buttonEditClient.Click += new EventHandler(EditClient_Click);

                            Panel panelClientBody = new Panel();
                            tdRight.Controls.Add(panelClientBody);

                            table = new HtmlGenericControl("table");
                            panelClientBody.Controls.Add(table);
                            table.Attributes["class"] = "auto_table";
                            tr = new HtmlGenericControl("tr");
                            table.Controls.Add(tr);

                            td = new HtmlGenericControl("td");
                            tr.Controls.Add(td);
                            td.Attributes["class"] = "auto_table";

                            HtmlGenericControl spanClientKey = new HtmlGenericControl("span");
                            td.Controls.Add(spanClientKey);
                            spanClientKey.InnerText = string.Format("{0}", clientData.clientKey);
                            spanClientKey.Attributes["class"] = "name";

                            if (string.IsNullOrEmpty(clientData.serviceType) == false &&
                                (clientData.serviceType.Equals(m_siteData.serviceType) == false || clientData.contractAuthor.Equals(m_siteData.contractAuthor) == false))
                            {
                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 20px;";

                                HtmlGenericControl spanServiceType = new HtmlGenericControl("span");
                                td.Controls.Add(spanServiceType);
                                spanServiceType.InnerHtml = string.Format("{0} <span class='gray_text'>(</span> {1} <span class='gray_text'>)</span>", clientData.serviceType, clientData.contractAuthor);
                                spanServiceType.Attributes["class"] = "error_text";
                            }

                            if (string.IsNullOrEmpty(clientData.clientDescription) == false)
                            {
                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 20px;";

                                HtmlGenericControl spanClientType = new HtmlGenericControl("span");
                                td.Controls.Add(spanClientType);
                                spanClientType.InnerText = clientData.clientDescription;
                                spanClientType.Attributes["class"] = "client_description";
                            }

                            if (clientData.pingPeriod > 0)
                            {
                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 30px;";

                                HtmlGenericControl span = new HtmlGenericControl("span");
                                td.Controls.Add(span);
                                span.InnerHtml = "<span class='ping_label'>P:</span> " + clientData.pingPeriod.ToString();
                            }
                        }
                        else
                        {
                            HtmlGenericControl divClient = new HtmlGenericControl("div");
                            panelClientList.Controls.Add(divClient);
                            divClient.Attributes["class"] = "site_block_item";

                            HtmlGenericControl divDashedFrame = new HtmlGenericControl("div");
                            divClient.Controls.Add(divDashedFrame);
                            divDashedFrame.Attributes["style"] = "border: 1px dashed gray; padding: 1px; padding-bottom: 10px;";

                            HtmlGenericControl tableLayout = new HtmlGenericControl("table");
                            divDashedFrame.Controls.Add(tableLayout);
                            tableLayout.Attributes["class"] = "wide_table";
                            tr = new HtmlGenericControl("tr");
                            tableLayout.Controls.Add(tr);
                            tr.Attributes["style"] = "background-color: #E0E0E0;";

                            HtmlGenericControl tdLeft = new HtmlGenericControl("td");
                            tr.Controls.Add(tdLeft);
                            tdLeft.Attributes["class"] = "wide_table";
                            tdLeft.Attributes["style"] = "width: 47px; padding-right: 10px;";

                            HtmlGenericControl tdMiddle = new HtmlGenericControl("td");
                            tr.Controls.Add(tdMiddle);
                            tdMiddle.Attributes["class"] = "wide_table";
                            tdMiddle.Attributes["style"] = "padding-right: 3px;";

                            HtmlGenericControl tdRight = new HtmlGenericControl("td");
                            tr.Controls.Add(tdRight);
                            tdRight.Attributes["class"] = "wide_table";
                            tdRight.Attributes["style"] = "width: 22px;";

                            HtmlGenericControl divViewClientButton = new HtmlGenericControl("div");
                            tdLeft.Controls.Add(divViewClientButton);
                            divViewClientButton.Attributes["class"] = "SubmitButtonMini Selected Blue";

                            TButton buttonViewClient = new TButton();
                            divViewClientButton.Controls.Add(buttonViewClient);
                            buttonViewClient.Args.Add(clientData);
                            buttonViewClient.Text = "<<";
                            buttonViewClient.ID = string.Format("B_ViewClient_{0}", clientData.clientId);
                            buttonViewClient.Click += new EventHandler(ViewClient_Click);

                            HtmlGenericControl divDeleteClientButton = new HtmlGenericControl("div");
                            tdRight.Controls.Add(divDeleteClientButton);
                            divDeleteClientButton.Attributes["class"] = "SubmitButtonSquareMini RedOrange";

                            TButton buttonDeleteClient = new TButton();
                            divDeleteClientButton.Controls.Add(buttonDeleteClient);
                            buttonDeleteClient.Args.Add(clientData);
                            buttonDeleteClient.Text = "X";
                            buttonDeleteClient.ToolTip = "Delete Client";
                            buttonDeleteClient.ID = string.Format("B_DeleteClient_{0}", clientData.clientId);
                            buttonDeleteClient.Click += new EventHandler(DeleteClient_Click);

                            table = new HtmlGenericControl("table");
                            tdMiddle.Controls.Add(table);
                            table.Attributes["class"] = "auto_table";
                            tr = new HtmlGenericControl("tr");
                            table.Controls.Add(tr);

                            td = new HtmlGenericControl("td");
                            tr.Controls.Add(td);
                            td.Attributes["class"] = "auto_table";

                            HtmlGenericControl spanClientKey = new HtmlGenericControl("span");
                            td.Controls.Add(spanClientKey);
                            spanClientKey.InnerText = string.Format("{0}", clientData.clientKey);
                            spanClientKey.Attributes["class"] = "name";

                            if (string.IsNullOrEmpty(clientData.serviceType) == false &&
                                (clientData.serviceType.Equals(m_siteData.serviceType) == false || clientData.contractAuthor.Equals(m_siteData.contractAuthor) == false))
                            {
                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 20px;";

                                HtmlGenericControl spanServiceType = new HtmlGenericControl("span");
                                td.Controls.Add(spanServiceType);
                                spanServiceType.InnerHtml = string.Format("{0} <span class='gray_text'>(</span> {1} <span class='gray_text'>)</span>", clientData.serviceType, clientData.contractAuthor);
                                spanServiceType.Attributes["class"] = "error_text";
                            }

                            if (string.IsNullOrEmpty(clientData.clientDescription) == false)
                            {
                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 20px;";

                                HtmlGenericControl spanClientType = new HtmlGenericControl("span");
                                td.Controls.Add(spanClientType);
                                spanClientType.InnerText = clientData.clientDescription;
                                spanClientType.Attributes["class"] = "client_description";
                            }

                            if (clientData.pingPeriod > 0)
                            {
                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 30px;";

                                HtmlGenericControl span = new HtmlGenericControl("span");
                                td.Controls.Add(span);
                                span.InnerHtml = "<span class='ping_label'>P:</span> " + clientData.pingPeriod.ToString();
                            }

                            tr = new HtmlGenericControl("tr");
                            tableLayout.Controls.Add(tr);

                            tdLeft = new HtmlGenericControl("td");
                            tr.Controls.Add(tdLeft);
                            tdLeft.Attributes["class"] = "wide_table";

                            tdMiddle = new HtmlGenericControl("td");
                            tr.Controls.Add(tdMiddle);
                            tdMiddle.Attributes["class"] = "wide_table";
                            tdMiddle.Attributes["style"] = "padding-right: 3px;";

                            tdRight = new HtmlGenericControl("td");
                            tr.Controls.Add(tdRight);
                            tdRight.Attributes["class"] = "wide_table";

                            table = new HtmlGenericControl("table");
                            tdMiddle.Controls.Add(table);
                            table.Attributes["class"] = "auto_table";
                            table.Attributes["style"] = "margin-top: 10px;";
                            tr = new HtmlGenericControl("tr");
                            table.Controls.Add(tr);

                            td = new HtmlGenericControl("td");
                            tr.Controls.Add(td);
                            td.Attributes["class"] = "auto_table";

                            HtmlGenericControl divAccountButton = new HtmlGenericControl("div");
                            td.Controls.Add(divAccountButton);
                            divAccountButton.Attributes["class"] = "SubmitButtonSquare Blue";

                            TButton tButton = new TButton();
                            tButton.Args.Add(clientData);
                            divAccountButton.Controls.Add(tButton);
                            tButton.Text = "account";
                            tButton.ID = string.Format("B_EditClientAccount_{0}", clientData.clientId);
                            tButton.Click += new EventHandler(EditClientAccount_Click);

                            td = new HtmlGenericControl("td");
                            tr.Controls.Add(td);
                            td.Attributes["class"] = "auto_table";
                            td.Attributes["style"] = "padding-left: 15px;";

                            HtmlGenericControl divPingButton = new HtmlGenericControl("div");
                            td.Controls.Add(divPingButton);
                            divPingButton.Attributes["class"] = "SubmitButtonSquare Blue";

                            tButton = new TButton();
                            tButton.Args.Add(clientData);
                            divPingButton.Controls.Add(tButton);
                            tButton.Text = "ping period";
                            tButton.ID = string.Format("B_EditClientPing_{0}", clientData.clientId);
                            tButton.Click += new EventHandler(EditClientPing_Click);

                            if (cntProp == 1)
                            {
                                divAccountButton.Attributes["class"] = "SubmitButtonSquare Selected Blue";

                                HtmlGenericControl spanAccount = new HtmlGenericControl("span");
                                tdMiddle.Controls.Add(spanAccount);
                                spanAccount.Attributes["class"] = "client_uri";
                                spanAccount.Attributes["style"] = "display:block; margin-top: 12px;";
                                spanAccount.InnerText = string.Format("softnet-s://{0}@{1}", clientData.clientKey, SoftnetRegistry.settings_getServerAddress());

                                Panel panelClientPassword = new Panel();
                                tdMiddle.Controls.Add(panelClientPassword);
                                panelClientPassword.Attributes["style"] = "padding-top: 10px;";
                                panelClientPassword.EnableViewState = false;

                                HtmlGenericControl tableGeneratePasswordButton = new HtmlGenericControl("table");
                                panelClientPassword.Controls.Add(tableGeneratePasswordButton);
                                tableGeneratePasswordButton.Attributes["style"] = "margin-top: 5px;";
                                tr = new HtmlGenericControl("tr");
                                tableGeneratePasswordButton.Controls.Add(tr);
                                tableGeneratePasswordButton.Attributes["class"] = "auto_table";

                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";

                                HtmlGenericControl divGeneratePasswordButton = new HtmlGenericControl("div");
                                td.Controls.Add(divGeneratePasswordButton);
                                divGeneratePasswordButton.Attributes["class"] = "SubmitButtonMini Green";

                                TButton buttonGeneratePassword = new TButton();
                                divGeneratePasswordButton.Controls.Add(buttonGeneratePassword);
                                buttonGeneratePassword.Args.Add(clientData);
                                buttonGeneratePassword.Args.Add(panelClientPassword);
                                buttonGeneratePassword.Text = "generate password";
                                buttonGeneratePassword.ID = string.Format("B_GeneratePassword_{0}", clientData.clientId);
                                buttonGeneratePassword.Click += new EventHandler(GenerateClientPassword_Click);
                            }
                            else if (cntProp == 2)
                            {
                                divPingButton.Attributes["class"] = "SubmitButtonSquare Selected Blue";

                                table = new HtmlGenericControl("table");
                                tdMiddle.Controls.Add(table);
                                table.Attributes["class"] = "auto_table";
                                table.Attributes["style"] = "margin-top: 15px;";
                                tr = new HtmlGenericControl("tr");
                                table.Controls.Add(tr);

                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";

                                HtmlGenericControl span = new HtmlGenericControl("span");
                                td.Controls.Add(span);
                                span.Attributes["class"] = "param_name";
                                span.InnerText = "ping period:";

                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 5px;";

                                TextBox textboxPingPeriod = new TextBox();
                                td.Controls.Add(textboxPingPeriod);
                                textboxPingPeriod.Attributes["style"] = "border: 1px solid #7FBA00; outline:none; width:40px; margin: 0px; padding: 3px;";
                                textboxPingPeriod.Text = clientData.pingPeriod.ToString();
                                textboxPingPeriod.ID = "TB_PingPeriod";
                                textboxPingPeriod.Attributes["autocomplete"] = "off";

                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 5px;";

                                HtmlGenericControl spanSec = new HtmlGenericControl("span");
                                td.Controls.Add(spanSec);
                                spanSec.Attributes["class"] = "black_text";
                                spanSec.InnerText = "sec.";

                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "auto_table";
                                td.Attributes["style"] = "padding-left: 15px;";

                                HtmlGenericControl divSavePingPeriod = new HtmlGenericControl("div");
                                td.Controls.Add(divSavePingPeriod);
                                divSavePingPeriod.Attributes["class"] = "SubmitButtonMini Green";

                                TButton buttonSavePingPeriod = new TButton();
                                divSavePingPeriod.Controls.Add(buttonSavePingPeriod);
                                buttonSavePingPeriod.Args.Add(clientData);
                                buttonSavePingPeriod.Args.Add(textboxPingPeriod);
                                buttonSavePingPeriod.Args.Add(tdMiddle);
                                buttonSavePingPeriod.Text = "save";
                                buttonSavePingPeriod.ID = string.Format("B_SavePingPeriod_{0}", clientData.clientId);
                                buttonSavePingPeriod.Click += new EventHandler(SetPingPeriod_Click);

                                span = new HtmlGenericControl("span");
                                tdMiddle.Controls.Add(span);
                                span.Attributes["style"] = "display: block; margin-top: 10px; color: #3C6C80";
                                span.InnerHtml =
                                    "The minimum value is 10 seconds and the maximum is 300 seconds.<br/>" +
                                    "The default value is 0 which sets the ping period to the endpoint's local value.";
                            }

                            TextBox textboxScrollPosition = new TextBox();
                            panelUser.Controls.Add(textboxScrollPosition);
                            textboxScrollPosition.Attributes["style"] = "width:0px; height:0px; border-width:0px; padding: 0px; margin:0px";
                            textboxScrollPosition.Focus();
                        }
                    }
                }            
            }
        }
    }

    private class AllowedUser
    {
        public UserData userData;
        public List<RoleData> roles;
        public RoleData defaultRole;
        public AllowedUser(UserData userData)
        {
            this.userData = userData;
            roles = new List<RoleData>();
            defaultRole = null;
        }
    }
}