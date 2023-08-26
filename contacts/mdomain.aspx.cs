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

public partial class contacts_mdomain : System.Web.UI.Page
{
    SharedDomainDataset m_dataset;
    UrlBuider m_urlBuider;

    protected void Back_Click(object sender, EventArgs e)
    {
        string retUrl = m_urlBuider.getBackUrl();
        if (retUrl != null)
            Response.Redirect(retUrl);
        else
            Response.Redirect(string.Format("~/contacts/contact.aspx?cid={0}", m_dataset.contactId));
    }

    protected void Refresh_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/contacts/mdomain.aspx?cid={0}&did={1}", m_dataset.contactId, m_dataset.domainId)));
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.Context.User.IsInRole("Provider") == false && this.Context.User.IsInRole("Administrator") == false)
            Response.Redirect("~/contacts/default.aspx");

        try
        {
            long contactId;
            if (long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("cid"), out contactId) == false)
                throw new InvalidIdentifierSoftnetException();

            long domainId;
            if (long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("did"), out domainId) == false)
                throw new InvalidIdentifierSoftnetException();
            
            m_dataset = new SharedDomainDataset();
            m_dataset.contactId = contactId;
            m_dataset.domainId = domainId;
            SoftnetRegistry.GetSharedDomainDataset(this.Context.User.Identity.Name, m_dataset);

            string retString = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("ret");
            m_urlBuider = new UrlBuider(retString);

            this.Title = string.Format("{0} - {1}", m_dataset.domainName, m_dataset.contactName); 

            HL_Contact.Text = m_dataset.contactName;
            if (m_dataset.partnerAuthority == 0)
                HL_Contact.CssClass += " consumer_color";
            else
                HL_Contact.CssClass += " provider_color";

            HL_Contact.NavigateUrl = string.Format("~/contacts/contact.aspx?cid={0}", m_dataset.contactId);            
            if (m_dataset.contactStatus == 1)
            {
                HL_Contact.CssClass = "h3_name contact_in_status_1";
                HL_Contact.ToolTip = "Your partner deleted the contact.";
            }
            else if (m_dataset.contactStatus == 2)
            {
                HL_Contact.CssClass = "h3_name contact_in_status_2";
                HL_Contact.ToolTip = "Your partner has been deleted from the network.";
            }            

            L_Domain.Text = m_dataset.domainName;            
            
            foreach (SiteData siteData in m_dataset.sites)
            {
                if (siteData.rolesSupported)
                {
                    if (siteData.siteKind == 1)
                    {
                        drawSRSite(siteData);
                    }
                    else
                    {
                        drawMRSite(siteData);
                    }
                }
                else
                {
                    if (siteData.siteKind == 1)
                    {
                        drawSUSite(siteData);
                    }
                    else
                    {
                        drawMUSite(siteData);
                    }
                }
            }
            
            if (this.IsPostBack == false)
            {
                long scrollId;
                if (long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("sp"), out scrollId))
                {
                    Control spButton = PH_Sites.FindControl(string.Format("ScrollPosition_{0}", scrollId));
                    if (spButton != null)
                        spButton.Focus();
                }
            }            
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void ConfigSRSite_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        SiteData siteData = (SiteData)button.Args[0];
        Response.Redirect(m_urlBuider.getNextUrl(string.Format("~/domains/srsite.aspx?sid={0}", siteData.siteId), string.Format("~/contacts/mdomain.aspx?cid={0}&did={1}&sp={2}", m_dataset.contactId, m_dataset.domainId, siteData.siteId)));
    }

    protected void ConfigMRSite_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        SiteData siteData = (SiteData)button.Args[0];
        Response.Redirect(m_urlBuider.getNextUrl(string.Format("~/domains/mrsite.aspx?sid={0}", siteData.siteId), string.Format("~/contacts/mdomain.aspx?cid={0}&did={1}&sp={2}", m_dataset.contactId, m_dataset.domainId, siteData.siteId)));
    }

    protected void ConfigSUSite_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        SiteData siteData = (SiteData)button.Args[0];
        Response.Redirect(m_urlBuider.getNextUrl(string.Format("~/domains/susite.aspx?sid={0}", siteData.siteId), string.Format("~/contacts/mdomain.aspx?cid={0}&did={1}&sp={2}", m_dataset.contactId, m_dataset.domainId, siteData.siteId)));
    }

    protected void ConfigMUSite_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        SiteData siteData = (SiteData)button.Args[0];
        Response.Redirect(m_urlBuider.getNextUrl(string.Format("~/domains/musite.aspx?sid={0}", siteData.siteId), string.Format("~/contacts/mdomain.aspx?cid={0}&did={1}&sp={2}", m_dataset.contactId, m_dataset.domainId, siteData.siteId)));
    }

    void drawSRSite(SiteData siteData)
    {
        HtmlGenericControl divSiteFrame = new HtmlGenericControl("div");
        PH_Sites.Controls.Add(divSiteFrame);
        divSiteFrame.Attributes["class"] = "site_frame";

        HtmlGenericControl divSiteDescription = new HtmlGenericControl("div");
        divSiteFrame.Controls.Add(divSiteDescription);
        divSiteDescription.Attributes["style"] = "padding-top: 0px; padding-bottom: 7px; padding-left: 25px; padding-right: 30px;";

        HtmlGenericControl span = new HtmlGenericControl("span");
        divSiteDescription.Controls.Add(span);
        span.Attributes["class"] = "site_name";
        span.InnerText = siteData.description;

        Button buttonScrollPosition = new Button();
        buttonScrollPosition.ID = string.Format("ScrollPosition_{0}", siteData.siteId);
        PH_Sites.Controls.Add(buttonScrollPosition);
        buttonScrollPosition.Attributes["style"] = "width:0px; height:0px; border-width:0px; padding: 0px; margin:0px";

        HtmlGenericControl divSiteMenu = new HtmlGenericControl("div");
        divSiteFrame.Controls.Add(divSiteMenu);
        divSiteMenu.Attributes["class"] = "site_menu";

        HtmlGenericControl table = new HtmlGenericControl("table");
        divSiteMenu.Controls.Add(table);
        table.Attributes["class"] = "wide_table";
        HtmlGenericControl tr = new HtmlGenericControl("tr");
        table.Controls.Add(tr);

        HtmlGenericControl td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table";
        td.Attributes["style"] = "width: 55px; padding: 1px; padding-left: 3px;";

        HtmlGenericControl divConfigButton = new HtmlGenericControl("div");
        td.Controls.Add(divConfigButton);
        divConfigButton.Attributes["class"] = "SubmitButtonMini Blue";

        TButton buttonConfig = new TButton();
        divConfigButton.Controls.Add(buttonConfig);
        buttonConfig.Args.Add(siteData);
        buttonConfig.Text = "config";
        buttonConfig.ID = string.Format("Config_{0}", siteData.siteId);
        buttonConfig.Click += new EventHandler(ConfigSRSite_Click);

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table";
        td.Attributes["style"] = "text-align: left; padding: 2px; padding-left: 10px;";
        HtmlGenericControl spanSiteType = new HtmlGenericControl("span");
        td.Controls.Add(spanSiteType);
        spanSiteType.Attributes["class"] = "site_type";
        spanSiteType.InnerHtml = siteData.serviceType + "<span class='gray_text'> ( </span>" + siteData.contractAuthor + "<span class='gray_text'> )</span>";

        if (siteData.enabled == false)
        {
            td = new HtmlGenericControl("td");
            tr.Controls.Add(td);

            td.Attributes["class"] = "wide_table";
            td.Attributes["style"] = "text-align:right; padding-left: 4px; padding-right: 4px; white-space: nowrap;";

            HtmlGenericControl spanSiteStatus = new HtmlGenericControl("span");
            td.Controls.Add(spanSiteStatus);
            spanSiteStatus.Attributes["class"] = "object_status";
            spanSiteStatus.InnerText = "site disabled";
        }

        HtmlGenericControl divSiteBody = new HtmlGenericControl("div");
        divSiteFrame.Controls.Add(divSiteBody);
        divSiteBody.Attributes["class"] = "site_body";

        try
        {
            List<ServiceData> services = m_dataset.services.FindAll(x => x.siteId == siteData.siteId);
            if (services.Count != 1)
                throw new FormatException();
            ServiceData serviceData = services[0];

            HtmlGenericControl divSiteBlock = new HtmlGenericControl("div");
            divSiteBody.Controls.Add(divSiteBlock);
            divSiteBlock.Attributes["class"] = "site_block";

            HtmlGenericControl divSiteBlockHeader = new HtmlGenericControl("div");
            divSiteBlock.Controls.Add(divSiteBlockHeader);
            divSiteBlockHeader.Attributes["class"] = "site_block_header left";
            divSiteBlockHeader.InnerText = "service";

            HtmlGenericControl divSiteBlockItem = new HtmlGenericControl("div");
            divSiteBlock.Controls.Add(divSiteBlockItem);
            divSiteBlockItem.Attributes["class"] = "site_block_item";

            table = new HtmlGenericControl("table");
            divSiteBlockItem.Controls.Add(table);
            table.Attributes["class"] = "wide_table";
            tr = new HtmlGenericControl("tr");
            table.Controls.Add(tr);

            HtmlGenericControl tdType = new HtmlGenericControl("td");
            tr.Controls.Add(tdType);
            tdType.Attributes["class"] = "wide_table";
            tdType.Attributes["style"] = "text-align: left";

            HtmlGenericControl tdStatus = new HtmlGenericControl("td");
            tr.Controls.Add(tdStatus);
            tdStatus.Attributes["class"] = "wide_table";
            tdStatus.Attributes["style"] = "text-align: right";

            table = new HtmlGenericControl("table");
            tdType.Controls.Add(table);
            table.Attributes["class"] = "auto_table";
            tr = new HtmlGenericControl("tr");
            table.Controls.Add(tr);

            td = new HtmlGenericControl("td");
            tr.Controls.Add(td);
            td.Attributes["class"] = "auto_table";
            td.Attributes["style"] = "padding-right: 15px;";

            HtmlGenericControl spanHostname = new HtmlGenericControl("span");
            td.Controls.Add(spanHostname);
            spanHostname.InnerText = serviceData.hostname;
            spanHostname.Attributes["class"] = "name";

            if (string.IsNullOrEmpty(serviceData.serviceType) == false)
            {
                if (serviceData.serviceType.Equals(siteData.serviceType) == false || serviceData.contractAuthor.Equals(siteData.contractAuthor) == false)
                {
                    td = new HtmlGenericControl("td");
                    tr.Controls.Add(td);
                    td.Attributes["class"] = "auto_table";
                    td.Attributes["style"] = "padding-right: 5px;";

                    span = new HtmlGenericControl("span");
                    td.Controls.Add(span);
                    span.Attributes["class"] = "service_type";
                    span.InnerHtml = serviceData.serviceType + "<span class='gray_text'> ( </span>" + serviceData.contractAuthor + "<span class='gray_text'> )</span>";

                    span = new HtmlGenericControl("span");
                    tdStatus.Controls.Add(span);
                    span.Attributes["class"] = "object_status";
                    span.InnerText = "service type conflict";
                }
                else if (serviceData.ssHash.Equals(siteData.ssHash) == false)
                {
                    HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                    tdStatus.Controls.Add(spanStatus);
                    spanStatus.Attributes["class"] = "object_status";
                    spanStatus.InnerText = "site structure mismatch";
                }

                if (string.IsNullOrEmpty(serviceData.version) == false)
                {
                    td = new HtmlGenericControl("td");
                    tr.Controls.Add(td);
                    td.Attributes["class"] = "auto_table";

                    span = new HtmlGenericControl("span");
                    td.Controls.Add(span);
                    span.Attributes["class"] = "service_type";
                    span.InnerHtml = "<span class='version_label'>ver. </span>" + serviceData.version;
                }
            }
            else
            {
                HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                tdStatus.Controls.Add(spanStatus);
                spanStatus.Attributes["class"] = "object_status";
                spanStatus.InnerText = "service type undefined";
            }

            if (serviceData.pingPeriod > 0)
            {
                td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";
                td.Attributes["style"] = "padding-left: 30px;";

                span = new HtmlGenericControl("span");
                td.Controls.Add(span);
                span.InnerHtml = "<span style='color:green;'>P:</span> " + serviceData.pingPeriod.ToString();
            }

            divSiteBlock = new HtmlGenericControl("div");
            divSiteBody.Controls.Add(divSiteBlock);
            divSiteBlock.Attributes["class"] = "site_block";

            divSiteBlockHeader = new HtmlGenericControl("div");
            divSiteBlock.Controls.Add(divSiteBlockHeader);
            divSiteBlockHeader.Attributes["class"] = "site_block_header left";
            divSiteBlockHeader.InnerText = "supported roles";

            divSiteBlockItem = new HtmlGenericControl("div");
            divSiteBlock.Controls.Add(divSiteBlockItem);
            divSiteBlockItem.Attributes["class"] = "site_block_item";

            HtmlGenericControl spanRoleList = new HtmlGenericControl("span");
            divSiteBlockItem.Controls.Add(spanRoleList);

            List<RoleData> roles = m_dataset.roles.FindAll(x => x.siteId == siteData.siteId);
            if (roles.Count == 0)
                throw new FormatException();

            string listView = "";
            if (roles.Count > 1)
            {
                for (int i = 0; i < roles.Count; i++)
                {
                    if (i == 0)
                        listView = "<span class='user_role'>" + roles[i].name + "</span>";
                    else
                        listView = listView + "<span class='delimeter'>,&nbsp;&nbsp;</span><span class='user_role'>" + roles[i].name + "</span>";
                }
            }
            else
            {
                listView = "<span class='user_role'>" + roles[0].name + "</span>";
            }
            spanRoleList.InnerHtml = listView;

            if (siteData.guestAllowed)
            {
                divSiteBlock = new HtmlGenericControl("div");
                divSiteBody.Controls.Add(divSiteBlock);
                divSiteBlock.Attributes["class"] = "site_block";

                divSiteBlockHeader = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockHeader);
                divSiteBlockHeader.Attributes["class"] = "site_block_header left";
                divSiteBlockHeader.InnerText = "guest page";

                divSiteBlockItem = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockItem);
                divSiteBlockItem.Attributes["class"] = "site_block_item";

                HtmlGenericControl spanGuestPage = new HtmlGenericControl("span");
                divSiteBlockItem.Controls.Add(spanGuestPage);
                spanGuestPage.Attributes["class"] = "guest_page_url";
                spanGuestPage.InnerText = string.Format("{0}/guest.aspx?site={1}", SoftnetRegistry.settings_getSiteUrl(), siteData.siteKey);

                if (siteData.statelessGuestSupported)
                {
                    divSiteBlock = new HtmlGenericControl("div");
                    divSiteBody.Controls.Add(divSiteBlock);
                    divSiteBlock.Attributes["class"] = "site_block";

                    divSiteBlockHeader = new HtmlGenericControl("div");
                    divSiteBlock.Controls.Add(divSiteBlockHeader);
                    divSiteBlockHeader.Attributes["class"] = "site_block_header left";
                    divSiteBlockHeader.InnerText = "guest shared uri";

                    divSiteBlockItem = new HtmlGenericControl("div");
                    divSiteBlock.Controls.Add(divSiteBlockItem);
                    divSiteBlockItem.Attributes["class"] = "site_block_item";

                    span = new HtmlGenericControl("span");
                    divSiteBlockItem.Controls.Add(span);
                    span.Attributes["class"] = "guest_shared_uri";
                    span.InnerText = string.Format("softnet-ss://{0}@{1}", siteData.siteKey, SoftnetRegistry.settings_getServerAddress());
                }
            }

            divSiteBlock = new HtmlGenericControl("div");
            divSiteBody.Controls.Add(divSiteBlock);
            divSiteBlock.Attributes["class"] = "site_block";

            divSiteBlockHeader = new HtmlGenericControl("div");
            divSiteBlock.Controls.Add(divSiteBlockHeader);
            divSiteBlockHeader.Attributes["class"] = "site_block_header left";
            divSiteBlockHeader.InnerText = "clients";            

            List<AllowedUser> allowedUsers = new List<AllowedUser>();
            foreach (UserData userData in m_dataset.users)
            {
                if (userData.kind == 3)
                {
                    AllowedUser allowedUser = new AllowedUser(userData);
                    foreach (RoleData roleData in roles)
                    {
                        UserInRole userInRole = m_dataset.usersInRoles.Find(x => x.userId == userData.userId && x.roleId == roleData.roleId);
                        if (userInRole != null)
                            allowedUser.roles.Add(roleData);
                        else if (userData.dedicated == false && roleData.roleId == siteData.defaultRoleId)
                            allowedUser.defaultRole = roleData;
                    }

                    if (allowedUser.roles.Count > 0 || allowedUser.defaultRole != null)
                        allowedUsers.Add(allowedUser);
                }
            }

            HtmlGenericControl tdLeft, tdRight;

            foreach (AllowedUser allowedUser in allowedUsers)
            {
                divSiteBlockItem = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockItem);
                divSiteBlockItem.Attributes["class"] = "site_block_item underline";

                table = new HtmlGenericControl("table");
                divSiteBlockItem.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";

                HtmlGenericControl labelUserName = new HtmlGenericControl("span");
                td.Controls.Add(labelUserName);
                labelUserName.InnerText = allowedUser.userData.name;
                labelUserName.Attributes["class"] = "user";

                if (m_dataset.contactStatus == 2)
                {
                    labelUserName.Attributes["class"] = "user disabled_status";
                    labelUserName.Attributes["title"] = "Your partner has been deleted from the network.";
                }
                else if (allowedUser.userData.enabled == false)
                    labelUserName.Attributes["class"] = "user disabled_status";                    

                HtmlGenericControl labelRoles = new HtmlGenericControl("span");
                td.Controls.Add(labelRoles);
                string roleListView = "";
                if (allowedUser.roles.Count > 0)
                {
                    roleListView = "&nbsp;&nbsp;<span class='gray_text'>(</span>&nbsp;<span class='user_role'>" + allowedUser.roles[0].name + "</span>";
                    for (int i = 1; i < allowedUser.roles.Count; i++)
                        roleListView = roleListView + ",&nbsp;&nbsp;<span class='user_role'>" + allowedUser.roles[i].name + "</span>";
                    if (allowedUser.defaultRole != null)
                        roleListView = roleListView + ",&nbsp;&nbsp;<span class='user_role'>" + allowedUser.defaultRole.name + "</span>&nbsp;<span class='gray_text'>)</span>";
                    else
                        roleListView = roleListView + "&nbsp;<span class='gray_text'>)</span>";
                }
                else
                {
                    roleListView = "&nbsp;&nbsp;<span class='gray_text'>(</span>&nbsp;<span class='user_role'>" + allowedUser.defaultRole.name + "</span>&nbsp;<span class='gray_text'>)</span>";
                }
                labelRoles.InnerHtml = roleListView;                

                List<ClientData> userClients = m_dataset.clients.FindAll(x => x.siteId == siteData.siteId && x.userId == allowedUser.userData.userId);
                if (userClients.Count > 0)
                {
                    HtmlGenericControl divClientList = new HtmlGenericControl("div");
                    divSiteBlockItem.Controls.Add(divClientList);
                    divClientList.Attributes["style"] = "padding-left: 20px; padding-top: 5px;";

                    foreach (ClientData clientData in userClients)
                    {
                        HtmlGenericControl divClient = new HtmlGenericControl("div");
                        divClientList.Controls.Add(divClient);
                        divClient.Attributes["class"] = "site_block_item";

                        HtmlGenericControl tableLayout = new HtmlGenericControl("table");
                        divClient.Controls.Add(tableLayout);
                        tableLayout.Attributes["class"] = "wide_table";
                        tr = new HtmlGenericControl("tr");
                        tableLayout.Controls.Add(tr);

                        tdLeft = new HtmlGenericControl("td");
                        tr.Controls.Add(tdLeft);
                        tdLeft.Attributes["class"] = "wide_table";
                        tdLeft.Attributes["style"] = "width: 7px; padding-right: 14px;"; 

                        tdRight = new HtmlGenericControl("td");
                        tr.Controls.Add(tdRight);
                        tdRight.Attributes["class"] = "wide_table";

                        HtmlGenericControl divListItem = new HtmlGenericControl("div");
                        tdLeft.Controls.Add(divListItem);
                        divListItem.Attributes["style"] = "width: 7px; height: 7px; background-color: #4F8DA6";

                        HtmlGenericControl divClientBody = new HtmlGenericControl("div");
                        tdRight.Controls.Add(divClientBody);

                        table = new HtmlGenericControl("table");
                        divClientBody.Controls.Add(table);
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
                                (clientData.serviceType.Equals(siteData.serviceType) == false || clientData.contractAuthor.Equals(siteData.contractAuthor) == false))
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

                            span = new HtmlGenericControl("span");
                            td.Controls.Add(span);
                            span.InnerHtml = "<span class='ping_label'>P:</span> " + clientData.pingPeriod.ToString();
                        }
                    }
                }
            }

            if (siteData.guestAllowed)
            {
                UserData guestData = m_dataset.users.Find(x => x.kind == 4);
                if (guestData == null)
                    throw new FormatException();

                divSiteBlockItem = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockItem);
                divSiteBlockItem.Attributes["class"] = "site_block_item underline";

                table = new HtmlGenericControl("table");
                divSiteBlockItem.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";

                Label labelUserName = new Label();
                td.Controls.Add(labelUserName);
                labelUserName.Text = guestData.name;
                labelUserName.CssClass = "user_guest";

                if (guestData.enabled == false)
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";  
              
                List<ClientData> guestClients = m_dataset.guestClients.FindAll(x => x.siteId == siteData.siteId);
                if (guestClients.Count > 0)
                {
                    HtmlGenericControl divClientList = new HtmlGenericControl("div");
                    divSiteBlockItem.Controls.Add(divClientList);
                    divClientList.Attributes["style"] = "padding-left: 20px; padding-top: 5px;";

                    foreach (ClientData clientData in guestClients)
                    {
                        HtmlGenericControl divClient = new HtmlGenericControl("div");
                        divClientList.Controls.Add(divClient);
                        divClient.Attributes["class"] = "site_block_item";

                        HtmlGenericControl tableLayout = new HtmlGenericControl("table");
                        divClient.Controls.Add(tableLayout);
                        tableLayout.Attributes["class"] = "wide_table";
                        tr = new HtmlGenericControl("tr");
                        tableLayout.Controls.Add(tr);

                        tdLeft = new HtmlGenericControl("td");
                        tr.Controls.Add(tdLeft);
                        tdLeft.Attributes["class"] = "wide_table";
                        tdLeft.Attributes["style"] = "width: 7px; padding-right: 14px;";

                        tdRight = new HtmlGenericControl("td");
                        tr.Controls.Add(tdRight);
                        tdRight.Attributes["class"] = "wide_table";

                        HtmlGenericControl divListItem = new HtmlGenericControl("div");
                        tdLeft.Controls.Add(divListItem);
                        divListItem.Attributes["style"] = "width: 7px; height: 7px; background-color: #4F8DA6";

                        HtmlGenericControl divClientBody = new HtmlGenericControl("div");
                        tdRight.Controls.Add(divClientBody);

                        table = new HtmlGenericControl("table");
                        divClientBody.Controls.Add(table);
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
                                (clientData.serviceType.Equals(siteData.serviceType) == false || clientData.contractAuthor.Equals(siteData.contractAuthor) == false))
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

                            span = new HtmlGenericControl("span");
                            td.Controls.Add(span);
                            span.InnerHtml = "<span class='ping_label'>P:</span> " + clientData.pingPeriod.ToString();
                        }
                    }
                }
            }

            if (allowedUsers.Count == 0 && siteData.guestAllowed == false)
            {
                divSiteFrame.Visible = false;
            }
        }
        catch (FormatException)
        {
            divSiteBody.Controls.Clear();

            HtmlGenericControl divSiteBlock = new HtmlGenericControl("div");
            divSiteBody.Controls.Add(divSiteBlock);
            divSiteBlock.Attributes["class"] = "site_block";

            HtmlGenericControl spanSiteError = new HtmlGenericControl("span");
            divSiteBlock.Controls.Add(spanSiteError);
            spanSiteError.Attributes["class"] = "error_text";
            spanSiteError.InnerText = "The site integrity is broken.";
        }
    }

    void drawMRSite(SiteData siteData)
    {
        HtmlGenericControl divSiteFrame = new HtmlGenericControl("div");
        PH_Sites.Controls.Add(divSiteFrame);
        divSiteFrame.Attributes["class"] = "site_frame";

        HtmlGenericControl divSiteDescription = new HtmlGenericControl("div");
        divSiteFrame.Controls.Add(divSiteDescription);
        divSiteDescription.Attributes["style"] = "padding-top: 0px; padding-bottom: 7px; padding-left: 25px; padding-right: 30px;";

        HtmlGenericControl span = new HtmlGenericControl("span");
        divSiteDescription.Controls.Add(span);
        span.Attributes["class"] = "site_name";
        span.InnerText = siteData.description;

        Button buttonScrollPosition = new Button();
        buttonScrollPosition.ID = string.Format("ScrollPosition_{0}", siteData.siteId);
        PH_Sites.Controls.Add(buttonScrollPosition);
        buttonScrollPosition.Attributes["style"] = "width:0px; height:0px; border-width:0px; padding: 0px; margin:0px";

        HtmlGenericControl divSiteMenu = new HtmlGenericControl("div");
        divSiteFrame.Controls.Add(divSiteMenu);
        divSiteMenu.Attributes["class"] = "site_menu";

        HtmlGenericControl table = new HtmlGenericControl("table");
        divSiteMenu.Controls.Add(table);
        table.Attributes["class"] = "wide_table";
        HtmlGenericControl tr = new HtmlGenericControl("tr");
        table.Controls.Add(tr);

        HtmlGenericControl td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table";
        td.Attributes["style"] = "width: 55px; padding: 1px; padding-left: 3px;";

        HtmlGenericControl divConfigButton = new HtmlGenericControl("div");
        td.Controls.Add(divConfigButton);
        divConfigButton.Attributes["class"] = "SubmitButtonMini Blue";

        TButton buttonConfig = new TButton();
        divConfigButton.Controls.Add(buttonConfig);
        buttonConfig.Args.Add(siteData);
        buttonConfig.Text = "config";
        buttonConfig.ID = string.Format("Config_{0}", siteData.siteId);
        buttonConfig.Click += new EventHandler(ConfigMRSite_Click);

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table";
        td.Attributes["style"] = "text-align: left; padding: 2px; padding-left: 10px;";
        HtmlGenericControl spanSiteType = new HtmlGenericControl("span");
        td.Controls.Add(spanSiteType);
        spanSiteType.Attributes["class"] = "site_type";
        spanSiteType.InnerHtml = siteData.serviceType + "<span class='gray_text'> ( </span>" + siteData.contractAuthor + "<span class='gray_text'> )</span>";

        if (siteData.enabled == false)
        {
            td = new HtmlGenericControl("td");
            tr.Controls.Add(td);

            td.Attributes["class"] = "wide_table";
            td.Attributes["style"] = "text-align:right; padding-left: 4px; padding-right: 4px; white-space: nowrap;";

            HtmlGenericControl spanSiteStatus = new HtmlGenericControl("span");
            td.Controls.Add(spanSiteStatus);
            spanSiteStatus.Attributes["class"] = "object_status";
            spanSiteStatus.InnerText = "site disabled";
        }

        HtmlGenericControl divSiteBody = new HtmlGenericControl("div");
        divSiteFrame.Controls.Add(divSiteBody);
        divSiteBody.Attributes["class"] = "site_body";

        try
        {
            HtmlGenericControl divSiteBlock = new HtmlGenericControl("div");
            divSiteBody.Controls.Add(divSiteBlock);
            divSiteBlock.Attributes["class"] = "site_block";

            HtmlGenericControl divSiteBlockHeader = new HtmlGenericControl("div");
            divSiteBlock.Controls.Add(divSiteBlockHeader);
            divSiteBlockHeader.Attributes["class"] = "site_block_header left";
            divSiteBlockHeader.InnerText = "list of services";            

            HtmlGenericControl divSiteBlockItem, tdLeft, tdRight;

            List<ServiceData> services = m_dataset.services.FindAll(x => x.siteId == siteData.siteId);
            foreach (ServiceData serviceData in services)
            {
                divSiteBlockItem = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockItem);
                divSiteBlockItem.Attributes["class"] = "site_block_item underline";

                table = new HtmlGenericControl("table");
                divSiteBlockItem.Controls.Add(table);
                table.Attributes["class"] = "wide_table";
                tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                HtmlGenericControl tdType = new HtmlGenericControl("td");
                tr.Controls.Add(tdType);
                tdType.Attributes["class"] = "wide_table";
                tdType.Attributes["style"] = "text-align: left";

                HtmlGenericControl tdStatus = new HtmlGenericControl("td");
                tr.Controls.Add(tdStatus);
                tdStatus.Attributes["class"] = "wide_table";
                tdStatus.Attributes["style"] = "text-align: right";

                table = new HtmlGenericControl("table");
                tdType.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";
                td.Attributes["style"] = "padding-right: 15px;";

                HtmlGenericControl spanHostname = new HtmlGenericControl("span");
                td.Controls.Add(spanHostname);
                spanHostname.InnerText = serviceData.hostname;
                spanHostname.Attributes["class"] = "name";

                if (string.IsNullOrEmpty(serviceData.serviceType) == false)
                {
                    if (serviceData.serviceType.Equals(siteData.serviceType) == false || serviceData.contractAuthor.Equals(siteData.contractAuthor) == false)
                    {
                        td = new HtmlGenericControl("td");
                        tr.Controls.Add(td);
                        td.Attributes["class"] = "auto_table";
                        td.Attributes["style"] = "padding-right: 5px;";

                        span = new HtmlGenericControl("span");
                        td.Controls.Add(span);
                        span.Attributes["class"] = "service_type";
                        span.InnerHtml = serviceData.serviceType + "<span class='gray_text'> ( </span>" + serviceData.contractAuthor + "<span class='gray_text'> )</span>";

                        span = new HtmlGenericControl("span");
                        tdStatus.Controls.Add(span);
                        span.Attributes["class"] = "object_status";
                        span.InnerText = "service type conflict";
                    }
                    else if (serviceData.ssHash.Equals(siteData.ssHash) == false)
                    {
                        HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                        tdStatus.Controls.Add(spanStatus);
                        spanStatus.Attributes["class"] = "object_status";
                        spanStatus.InnerText = "site structure mismatch";
                    }

                    if (string.IsNullOrEmpty(serviceData.version) == false)
                    {
                        td = new HtmlGenericControl("td");
                        tr.Controls.Add(td);
                        td.Attributes["class"] = "auto_table";

                        span = new HtmlGenericControl("span");
                        td.Controls.Add(span);
                        span.Attributes["class"] = "service_type";
                        span.InnerHtml = "<span class='version_label'>ver. </span>" + serviceData.version;
                    }
                }
                else
                {
                    HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                    tdStatus.Controls.Add(spanStatus);
                    spanStatus.Attributes["class"] = "object_status";
                    spanStatus.InnerText = "service type undefined";
                }

                if (serviceData.pingPeriod > 0)
                {
                    td = new HtmlGenericControl("td");
                    tr.Controls.Add(td);
                    td.Attributes["class"] = "auto_table";
                    td.Attributes["style"] = "padding-left: 30px;";

                    span = new HtmlGenericControl("span");
                    td.Controls.Add(span);
                    span.InnerHtml = "<span style='color:green;'>P:</span> " + serviceData.pingPeriod.ToString();
                }

                if (serviceData.enabled == false)
                {
                    HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                    tdStatus.Controls.Add(spanStatus);
                    spanStatus.Attributes["class"] = "object_status";
                    if (tdStatus.Controls.Count == 0)
                        spanStatus.InnerText = "disabled";
                    else
                        spanStatus.InnerText = "; disabled";
                }
            }

            divSiteBlock = new HtmlGenericControl("div");
            divSiteBody.Controls.Add(divSiteBlock);
            divSiteBlock.Attributes["class"] = "site_block";

            divSiteBlockHeader = new HtmlGenericControl("div");
            divSiteBlock.Controls.Add(divSiteBlockHeader);
            divSiteBlockHeader.Attributes["class"] = "site_block_header left";
            divSiteBlockHeader.InnerText = "supported roles";

            divSiteBlockItem = new HtmlGenericControl("div");
            divSiteBlock.Controls.Add(divSiteBlockItem);
            divSiteBlockItem.Attributes["class"] = "site_block_item";

            HtmlGenericControl spanRoleList = new HtmlGenericControl("span");
            divSiteBlockItem.Controls.Add(spanRoleList);

            List<RoleData> roles = m_dataset.roles.FindAll(x => x.siteId == siteData.siteId);
            if (roles.Count == 0)
                throw new FormatException();

            string listView = "";
            if (roles.Count > 1)
            {
                for (int i = 0; i < roles.Count; i++)
                {
                    if (i == 0)
                        listView = "<span class='user_role'>" + roles[i].name + "</span>";
                    else
                        listView = listView + "<span class='delimeter'>,&nbsp;&nbsp;</span><span class='user_role'>" + roles[i].name + "</span>";
                }
            }
            else
            {
                listView = "<span class='user_role'>" + roles[0].name + "</span>";
            }
            spanRoleList.InnerHtml = listView;

            if (siteData.guestAllowed)
            {
                divSiteBlock = new HtmlGenericControl("div");
                divSiteBody.Controls.Add(divSiteBlock);
                divSiteBlock.Attributes["class"] = "site_block";

                divSiteBlockHeader = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockHeader);
                divSiteBlockHeader.Attributes["class"] = "site_block_header left";
                divSiteBlockHeader.InnerText = "guest page";

                divSiteBlockItem = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockItem);
                divSiteBlockItem.Attributes["class"] = "site_block_item";

                HtmlGenericControl spanGuestPage = new HtmlGenericControl("span");
                divSiteBlockItem.Controls.Add(spanGuestPage);
                spanGuestPage.Attributes["class"] = "guest_page_url";
                spanGuestPage.InnerText = string.Format("{0}/guest.aspx?site={1}", SoftnetRegistry.settings_getSiteUrl(), siteData.siteKey);

                if (siteData.statelessGuestSupported)
                {
                    divSiteBlock = new HtmlGenericControl("div");
                    divSiteBody.Controls.Add(divSiteBlock);
                    divSiteBlock.Attributes["class"] = "site_block";

                    divSiteBlockHeader = new HtmlGenericControl("div");
                    divSiteBlock.Controls.Add(divSiteBlockHeader);
                    divSiteBlockHeader.Attributes["class"] = "site_block_header left";
                    divSiteBlockHeader.InnerText = "guest shared uri";

                    divSiteBlockItem = new HtmlGenericControl("div");
                    divSiteBlock.Controls.Add(divSiteBlockItem);
                    divSiteBlockItem.Attributes["class"] = "site_block_item";

                    span = new HtmlGenericControl("span");
                    divSiteBlockItem.Controls.Add(span);
                    span.Attributes["class"] = "guest_shared_uri";
                    span.InnerText = string.Format("softnet-ms://{0}@{1}", siteData.siteKey, SoftnetRegistry.settings_getServerAddress());
                }
            }

            divSiteBlock = new HtmlGenericControl("div");
            divSiteBody.Controls.Add(divSiteBlock);
            divSiteBlock.Attributes["class"] = "site_block";

            divSiteBlockHeader = new HtmlGenericControl("div");
            divSiteBlock.Controls.Add(divSiteBlockHeader);
            divSiteBlockHeader.Attributes["class"] = "site_block_header left";
            divSiteBlockHeader.InnerText = "clients";

            List<AllowedUser> allowedUsers = new List<AllowedUser>();
            foreach (UserData userData in m_dataset.users)
            {
                if (userData.kind == 3)
                {
                    AllowedUser allowedUser = new AllowedUser(userData);
                    foreach (RoleData roleData in roles)
                    {
                        UserInRole userInRole = m_dataset.usersInRoles.Find(x => x.userId == userData.userId && x.roleId == roleData.roleId);
                        if (userInRole != null)
                            allowedUser.roles.Add(roleData);
                        else if (userData.dedicated == false && roleData.roleId == siteData.defaultRoleId)
                            allowedUser.defaultRole = roleData;
                    }
                    if (allowedUser.roles.Count > 0 || allowedUser.defaultRole != null)
                        allowedUsers.Add(allowedUser);
                }
            }

            foreach (AllowedUser allowedUser in allowedUsers)
            {
                divSiteBlockItem = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockItem);
                divSiteBlockItem.Attributes["class"] = "site_block_item underline";

                table = new HtmlGenericControl("table");
                divSiteBlockItem.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";

                HtmlGenericControl labelUserName = new HtmlGenericControl("span");
                td.Controls.Add(labelUserName);
                labelUserName.Attributes["class"] = "user";
                labelUserName.InnerText = allowedUser.userData.name;

                if (m_dataset.contactStatus == 2)
                {
                    labelUserName.Attributes["class"] = "user disabled_status";
                    labelUserName.Attributes["title"] = "Your partner has been deleted from the network.";
                }
                else if (allowedUser.userData.enabled == false)
                    labelUserName.Attributes["class"] = "user disabled_status"; 

                HtmlGenericControl labelRoles = new HtmlGenericControl("span");
                td.Controls.Add(labelRoles);
                string roleListView = "";
                if (allowedUser.roles.Count > 0)
                {
                    roleListView = "&nbsp;&nbsp;<span class='gray_text'>(</span>&nbsp;<span class='user_role'>" + allowedUser.roles[0].name + "</span>";
                    for (int i = 1; i < allowedUser.roles.Count; i++)
                        roleListView = roleListView + ",&nbsp;&nbsp;<span class='user_role'>" + allowedUser.roles[i].name + "</span>";
                    if (allowedUser.defaultRole != null)
                        roleListView = roleListView + ",&nbsp;&nbsp;<span class='user_role'>" + allowedUser.defaultRole.name + "</span>&nbsp;<span class='gray_text'>)</span>";
                    else
                        roleListView = roleListView + "&nbsp;<span class='gray_text'>)</span>";
                }
                else
                {
                    roleListView = "&nbsp;&nbsp;<span class='gray_text'>(</span>&nbsp;<span class='user_role'>" + allowedUser.defaultRole.name + "</span>&nbsp;<span class='gray_text'>)</span>";
                }
                labelRoles.InnerHtml = roleListView;

                List<ClientData> userClients = m_dataset.clients.FindAll(x => x.siteId == siteData.siteId && x.userId == allowedUser.userData.userId);
                if (userClients.Count > 0)
                {
                    HtmlGenericControl divClientList = new HtmlGenericControl("div");
                    divSiteBlockItem.Controls.Add(divClientList);
                    divClientList.Attributes["style"] = "padding-left: 20px; padding-top: 5px;";

                    foreach (ClientData clientData in userClients)
                    {
                        HtmlGenericControl divClient = new HtmlGenericControl("div");
                        divClientList.Controls.Add(divClient);
                        divClient.Attributes["class"] = "site_block_item";

                        HtmlGenericControl tableLayout = new HtmlGenericControl("table");
                        divClient.Controls.Add(tableLayout);
                        tableLayout.Attributes["class"] = "wide_table";
                        tr = new HtmlGenericControl("tr");
                        tableLayout.Controls.Add(tr);

                        tdLeft = new HtmlGenericControl("td");
                        tr.Controls.Add(tdLeft);
                        tdLeft.Attributes["class"] = "wide_table";
                        tdLeft.Attributes["style"] = "width: 7px; padding-right: 14px;";

                        tdRight = new HtmlGenericControl("td");
                        tr.Controls.Add(tdRight);
                        tdRight.Attributes["class"] = "wide_table";

                        HtmlGenericControl divListItem = new HtmlGenericControl("div");
                        tdLeft.Controls.Add(divListItem);
                        divListItem.Attributes["style"] = "width: 7px; height: 7px; background-color: #4F8DA6";

                        HtmlGenericControl divClientBody = new HtmlGenericControl("div");
                        tdRight.Controls.Add(divClientBody);

                        table = new HtmlGenericControl("table");
                        divClientBody.Controls.Add(table);
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
                                (clientData.serviceType.Equals(siteData.serviceType) == false || clientData.contractAuthor.Equals(siteData.contractAuthor) == false))
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

                            span = new HtmlGenericControl("span");
                            td.Controls.Add(span);
                            span.InnerHtml = "<span class='ping_label'>P:</span> " + clientData.pingPeriod.ToString();
                        }
                    }
                }
            }

            if (siteData.guestAllowed)
            {
                UserData guestData = m_dataset.users.Find(x => x.kind == 4);
                if (guestData == null)
                    throw new FormatException();

                divSiteBlockItem = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockItem);
                divSiteBlockItem.Attributes["class"] = "site_block_item underline";

                table = new HtmlGenericControl("table");
                divSiteBlockItem.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";

                Label labelUserName = new Label();
                td.Controls.Add(labelUserName);
                labelUserName.Text = guestData.name;
                labelUserName.CssClass = "user_guest";

                if (guestData.enabled == false)
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";

                List<ClientData> guestClients = m_dataset.guestClients.FindAll(x => x.siteId == siteData.siteId);
                if (guestClients.Count > 0)
                {
                    HtmlGenericControl divClientList = new HtmlGenericControl("div");
                    divSiteBlockItem.Controls.Add(divClientList);
                    divClientList.Attributes["style"] = "padding-left: 20px; padding-top: 5px;";

                    foreach (ClientData clientData in guestClients)
                    {
                        HtmlGenericControl divClient = new HtmlGenericControl("div");
                        divClientList.Controls.Add(divClient);
                        divClient.Attributes["class"] = "site_block_item";

                        HtmlGenericControl tableLayout = new HtmlGenericControl("table");
                        divClient.Controls.Add(tableLayout);
                        tableLayout.Attributes["class"] = "wide_table";
                        tr = new HtmlGenericControl("tr");
                        tableLayout.Controls.Add(tr);

                        tdLeft = new HtmlGenericControl("td");
                        tr.Controls.Add(tdLeft);
                        tdLeft.Attributes["class"] = "wide_table";
                        tdLeft.Attributes["style"] = "width: 7px; padding-right: 14px;";

                        tdRight = new HtmlGenericControl("td");
                        tr.Controls.Add(tdRight);
                        tdRight.Attributes["class"] = "wide_table";

                        HtmlGenericControl divListItem = new HtmlGenericControl("div");
                        tdLeft.Controls.Add(divListItem);
                        divListItem.Attributes["style"] = "width: 7px; height: 7px; background-color: #4F8DA6";

                        HtmlGenericControl divClientBody = new HtmlGenericControl("div");
                        tdRight.Controls.Add(divClientBody);

                        table = new HtmlGenericControl("table");
                        divClientBody.Controls.Add(table);
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
                                (clientData.serviceType.Equals(siteData.serviceType) == false || clientData.contractAuthor.Equals(siteData.contractAuthor) == false))
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

                            span = new HtmlGenericControl("span");
                            td.Controls.Add(span);
                            span.InnerHtml = "<span class='ping_label'>P:</span> " + clientData.pingPeriod.ToString();
                        }
                    }
                }
            }

            if (allowedUsers.Count == 0 && siteData.guestAllowed == false)
            {
                divSiteFrame.Visible = false;
            }
        }
        catch (FormatException)
        {
            divSiteBody.Controls.Clear();

            HtmlGenericControl divSiteBlock = new HtmlGenericControl("div");
            divSiteBody.Controls.Add(divSiteBlock);
            divSiteBlock.Attributes["class"] = "site_block";

            HtmlGenericControl spanSiteError = new HtmlGenericControl("span");
            divSiteBlock.Controls.Add(spanSiteError);
            spanSiteError.Attributes["class"] = "error_text";
            spanSiteError.InnerText = "The site integrity is broken.";
        }
    }

    void drawSUSite(SiteData siteData)
    {
        HtmlGenericControl divSiteFrame = new HtmlGenericControl("div");
        PH_Sites.Controls.Add(divSiteFrame);
        divSiteFrame.Attributes["class"] = "site_frame";

        HtmlGenericControl divSiteDescription = new HtmlGenericControl("div");
        divSiteFrame.Controls.Add(divSiteDescription);
        divSiteDescription.Attributes["style"] = "padding-top: 0px; padding-bottom: 7px; padding-left: 25px; padding-right: 30px;";

        HtmlGenericControl span = new HtmlGenericControl("span");
        divSiteDescription.Controls.Add(span);
        span.Attributes["class"] = "site_name";
        span.InnerText = siteData.description;

        Button buttonScrollPosition = new Button();
        buttonScrollPosition.ID = string.Format("ScrollPosition_{0}", siteData.siteId);
        PH_Sites.Controls.Add(buttonScrollPosition);
        buttonScrollPosition.Attributes["style"] = "width:0px; height:0px; border-width:0px; padding: 0px; margin:0px";

        HtmlGenericControl divSiteMenu = new HtmlGenericControl("div");
        divSiteFrame.Controls.Add(divSiteMenu);
        divSiteMenu.Attributes["class"] = "site_menu";

        HtmlGenericControl table = new HtmlGenericControl("table");
        divSiteMenu.Controls.Add(table);
        table.Attributes["class"] = "wide_table";
        HtmlGenericControl tr = new HtmlGenericControl("tr");
        table.Controls.Add(tr);

        HtmlGenericControl td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table";
        td.Attributes["style"] = "width: 55px; padding: 1px; padding-left: 3px;";

        HtmlGenericControl divConfigButton = new HtmlGenericControl("div");
        td.Controls.Add(divConfigButton);
        divConfigButton.Attributes["class"] = "SubmitButtonMini Blue";

        TButton buttonConfig = new TButton();
        divConfigButton.Controls.Add(buttonConfig);
        buttonConfig.Args.Add(siteData);
        buttonConfig.Text = "config";
        buttonConfig.ID = string.Format("Config_{0}", siteData.siteId);
        buttonConfig.Click += new EventHandler(ConfigSUSite_Click);

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table";
        td.Attributes["style"] = "text-align: left; padding: 2px; padding-left: 10px;";
        HtmlGenericControl spanSiteType = new HtmlGenericControl("span");
        td.Controls.Add(spanSiteType);
        spanSiteType.Attributes["class"] = "site_type";
        spanSiteType.InnerHtml = siteData.serviceType + "<span class='gray_text'> ( </span>" + siteData.contractAuthor + "<span class='gray_text'> )</span>";

        if (siteData.enabled == false)
        {
            td = new HtmlGenericControl("td");
            tr.Controls.Add(td);

            td.Attributes["class"] = "wide_table";
            td.Attributes["style"] = "text-align:right; padding-left: 4px; padding-right: 4px; white-space: nowrap;";

            HtmlGenericControl spanSiteStatus = new HtmlGenericControl("span");
            td.Controls.Add(spanSiteStatus);
            spanSiteStatus.Attributes["class"] = "object_status";
            spanSiteStatus.InnerText = "site disabled";
        }

        HtmlGenericControl divSiteBody = new HtmlGenericControl("div");
        divSiteFrame.Controls.Add(divSiteBody);
        divSiteBody.Attributes["class"] = "site_body";

        try
        {
            List<ServiceData> services = m_dataset.services.FindAll(x => x.siteId == siteData.siteId);
            if (services.Count != 1)
                throw new FormatException();
            ServiceData serviceData = services[0];

            HtmlGenericControl divSiteBlock = new HtmlGenericControl("div");
            divSiteBody.Controls.Add(divSiteBlock);
            divSiteBlock.Attributes["class"] = "site_block";

            HtmlGenericControl divSiteBlockHeader = new HtmlGenericControl("div");
            divSiteBlock.Controls.Add(divSiteBlockHeader);
            divSiteBlockHeader.Attributes["class"] = "site_block_header left";
            divSiteBlockHeader.InnerText = "service";

            HtmlGenericControl divSiteBlockItem = new HtmlGenericControl("div");
            divSiteBlock.Controls.Add(divSiteBlockItem);
            divSiteBlockItem.Attributes["class"] = "site_block_item";

            table = new HtmlGenericControl("table");
            divSiteBlockItem.Controls.Add(table);
            table.Attributes["class"] = "wide_table";
            tr = new HtmlGenericControl("tr");
            table.Controls.Add(tr);

            HtmlGenericControl tdType = new HtmlGenericControl("td");
            tr.Controls.Add(tdType);
            tdType.Attributes["class"] = "wide_table";
            tdType.Attributes["style"] = "text-align: left";

            HtmlGenericControl tdStatus = new HtmlGenericControl("td");
            tr.Controls.Add(tdStatus);
            tdStatus.Attributes["class"] = "wide_table";
            tdStatus.Attributes["style"] = "text-align: right";

            table = new HtmlGenericControl("table");
            tdType.Controls.Add(table);
            table.Attributes["class"] = "auto_table";
            tr = new HtmlGenericControl("tr");
            table.Controls.Add(tr);

            td = new HtmlGenericControl("td");
            tr.Controls.Add(td);
            td.Attributes["class"] = "auto_table";
            td.Attributes["style"] = "padding-right: 15px;";

            HtmlGenericControl spanHostname = new HtmlGenericControl("span");
            td.Controls.Add(spanHostname);
            spanHostname.InnerText = serviceData.hostname;
            spanHostname.Attributes["class"] = "name";

            if (string.IsNullOrEmpty(serviceData.serviceType) == false)
            {
                if (serviceData.serviceType.Equals(siteData.serviceType) == false || serviceData.contractAuthor.Equals(siteData.contractAuthor) == false)
                {
                    td = new HtmlGenericControl("td");
                    tr.Controls.Add(td);
                    td.Attributes["class"] = "auto_table";
                    td.Attributes["style"] = "padding-right: 5px;";

                    span = new HtmlGenericControl("span");
                    td.Controls.Add(span);
                    span.Attributes["class"] = "service_type";
                    span.InnerHtml = serviceData.serviceType + "<span class='gray_text'> ( </span>" + serviceData.contractAuthor + "<span class='gray_text'> )</span>";

                    span = new HtmlGenericControl("span");
                    tdStatus.Controls.Add(span);
                    span.Attributes["class"] = "object_status";
                    span.InnerText = "service type conflict";
                }
                else if (serviceData.ssHash.Equals(siteData.ssHash) == false)
                {
                    HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                    tdStatus.Controls.Add(spanStatus);
                    spanStatus.Attributes["class"] = "object_status";
                    spanStatus.InnerText = "site structure mismatch";
                }

                if (string.IsNullOrEmpty(serviceData.version) == false)
                {
                    td = new HtmlGenericControl("td");
                    tr.Controls.Add(td);
                    td.Attributes["class"] = "auto_table";

                    span = new HtmlGenericControl("span");
                    td.Controls.Add(span);
                    span.Attributes["class"] = "service_type";
                    span.InnerHtml = "<span class='version_label'>ver. </span>" + serviceData.version;
                }
            }
            else
            {
                HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                tdStatus.Controls.Add(spanStatus);
                spanStatus.Attributes["class"] = "object_status";
                spanStatus.InnerText = "service type undefined";
            }

            if (serviceData.pingPeriod > 0)
            {
                td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";
                td.Attributes["style"] = "padding-left: 30px;";

                span = new HtmlGenericControl("span");
                td.Controls.Add(span);
                span.InnerHtml = "<span style='color:green;'>P:</span> " + serviceData.pingPeriod.ToString();
            }

            if (siteData.guestAllowed)
            {
                divSiteBlock = new HtmlGenericControl("div");
                divSiteBody.Controls.Add(divSiteBlock);
                divSiteBlock.Attributes["class"] = "site_block";

                divSiteBlockHeader = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockHeader);
                divSiteBlockHeader.Attributes["class"] = "site_block_header left";
                divSiteBlockHeader.InnerText = "guest page";

                divSiteBlockItem = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockItem);
                divSiteBlockItem.Attributes["class"] = "site_block_item";

                HtmlGenericControl spanGuestPage = new HtmlGenericControl("span");
                divSiteBlockItem.Controls.Add(spanGuestPage);
                spanGuestPage.Attributes["class"] = "guest_page_url";
                spanGuestPage.InnerText = string.Format("{0}/guest.aspx?site={1}", SoftnetRegistry.settings_getSiteUrl(), siteData.siteKey);

                if (siteData.statelessGuestSupported)
                {
                    divSiteBlock = new HtmlGenericControl("div");
                    divSiteBody.Controls.Add(divSiteBlock);
                    divSiteBlock.Attributes["class"] = "site_block";

                    divSiteBlockHeader = new HtmlGenericControl("div");
                    divSiteBlock.Controls.Add(divSiteBlockHeader);
                    divSiteBlockHeader.Attributes["class"] = "site_block_header left";
                    divSiteBlockHeader.InnerText = "guest shared uri";

                    divSiteBlockItem = new HtmlGenericControl("div");
                    divSiteBlock.Controls.Add(divSiteBlockItem);
                    divSiteBlockItem.Attributes["class"] = "site_block_item";

                    span = new HtmlGenericControl("span");
                    divSiteBlockItem.Controls.Add(span);
                    span.Attributes["class"] = "guest_shared_uri";
                    span.InnerText = string.Format("softnet-ss://{0}@{1}", siteData.siteKey, SoftnetRegistry.settings_getServerAddress());
                }
            }

            divSiteBlock = new HtmlGenericControl("div");
            divSiteBody.Controls.Add(divSiteBlock);
            divSiteBlock.Attributes["class"] = "site_block";

            divSiteBlockHeader = new HtmlGenericControl("div");
            divSiteBlock.Controls.Add(divSiteBlockHeader);
            divSiteBlockHeader.Attributes["class"] = "site_block_header left";
            divSiteBlockHeader.InnerText = "clients";

            List<UserData> allowedUsers = new List<UserData>();
            foreach (UserData userData in m_dataset.users)
            {
                if (userData.kind == 3)
                {
                    if (m_dataset.siteUsers.FindIndex(x => x.siteId == siteData.siteId && x.userId == userData.userId) >= 0)
                        allowedUsers.Add(userData);
                    else if (siteData.implicitUsersAllowed && userData.dedicated == false)
                        allowedUsers.Add(userData);
                }
            }

            HtmlGenericControl tdLeft, tdRight;

            foreach (UserData userData in allowedUsers)
            {
                divSiteBlockItem = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockItem);
                divSiteBlockItem.Attributes["class"] = "site_block_item underline";

                table = new HtmlGenericControl("table");
                divSiteBlockItem.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";

                HtmlGenericControl labelUserName = new HtmlGenericControl("span");
                td.Controls.Add(labelUserName);
                labelUserName.InnerText = userData.name;
                labelUserName.Attributes["class"] = "user";

                if (m_dataset.contactStatus == 2)
                {
                    labelUserName.Attributes["class"] = "user disabled_status";
                    labelUserName.Attributes["title"] = "Your partner has been deleted from the network.";
                }
                else if (userData.enabled == false)
                    labelUserName.Attributes["class"] = "user disabled_status"; 

                List<ClientData> userClients = m_dataset.clients.FindAll(x => x.siteId == siteData.siteId && x.userId == userData.userId);
                if (userClients.Count > 0)
                {
                    HtmlGenericControl divClientList = new HtmlGenericControl("div");
                    divSiteBlockItem.Controls.Add(divClientList);
                    divClientList.Attributes["style"] = "padding-left: 20px; padding-top: 5px;";

                    foreach (ClientData clientData in userClients)
                    {
                        HtmlGenericControl divClient = new HtmlGenericControl("div");
                        divClientList.Controls.Add(divClient);
                        divClient.Attributes["class"] = "site_block_item";

                        HtmlGenericControl tableLayout = new HtmlGenericControl("table");
                        divClient.Controls.Add(tableLayout);
                        tableLayout.Attributes["class"] = "wide_table";
                        tr = new HtmlGenericControl("tr");
                        tableLayout.Controls.Add(tr);

                        tdLeft = new HtmlGenericControl("td");
                        tr.Controls.Add(tdLeft);
                        tdLeft.Attributes["class"] = "wide_table";
                        tdLeft.Attributes["style"] = "width: 7px; padding-right: 12px;";

                        tdRight = new HtmlGenericControl("td");
                        tr.Controls.Add(tdRight);
                        tdRight.Attributes["class"] = "wide_table";

                        HtmlGenericControl divListItem = new HtmlGenericControl("div");
                        tdLeft.Controls.Add(divListItem);
                        divListItem.Attributes["style"] = "width: 7px; height: 7px; background-color: #4F8DA6";

                        HtmlGenericControl divClientBody = new HtmlGenericControl("div");
                        tdRight.Controls.Add(divClientBody);

                        table = new HtmlGenericControl("table");
                        divClientBody.Controls.Add(table);
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
                                (clientData.serviceType.Equals(siteData.serviceType) == false || clientData.contractAuthor.Equals(siteData.contractAuthor) == false))
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

                            span = new HtmlGenericControl("span");
                            td.Controls.Add(span);
                            span.InnerHtml = "<span class='ping_label'>P:</span> " + clientData.pingPeriod.ToString();
                        }
                    }
                }
            }

            if (siteData.guestAllowed)
            {
                UserData guestData = m_dataset.users.Find(x => x.kind == 4);
                if (guestData == null)
                    throw new FormatException();

                divSiteBlockItem = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockItem);
                divSiteBlockItem.Attributes["class"] = "site_block_item underline";

                table = new HtmlGenericControl("table");
                divSiteBlockItem.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";

                Label labelUserName = new Label();
                td.Controls.Add(labelUserName);
                labelUserName.Text = guestData.name;
                labelUserName.CssClass = "user_guest";

                if (guestData.enabled == false)
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";

                List<ClientData> guestClients = m_dataset.guestClients.FindAll(x => x.siteId == siteData.siteId);
                if (guestClients.Count > 0)
                {
                    HtmlGenericControl divClientList = new HtmlGenericControl("div");
                    divSiteBlockItem.Controls.Add(divClientList);
                    divClientList.Attributes["style"] = "padding-left: 20px; padding-top: 5px;";

                    foreach (ClientData clientData in guestClients)
                    {
                        HtmlGenericControl divClient = new HtmlGenericControl("div");
                        divClientList.Controls.Add(divClient);
                        divClient.Attributes["class"] = "site_block_item";

                        HtmlGenericControl tableLayout = new HtmlGenericControl("table");
                        divClient.Controls.Add(tableLayout);
                        tableLayout.Attributes["class"] = "wide_table";
                        tr = new HtmlGenericControl("tr");
                        tableLayout.Controls.Add(tr);

                        tdLeft = new HtmlGenericControl("td");
                        tr.Controls.Add(tdLeft);
                        tdLeft.Attributes["class"] = "wide_table";
                        tdLeft.Attributes["style"] = "width: 7px; padding-right: 14px;";

                        tdRight = new HtmlGenericControl("td");
                        tr.Controls.Add(tdRight);
                        tdRight.Attributes["class"] = "wide_table";

                        HtmlGenericControl divListItem = new HtmlGenericControl("div");
                        tdLeft.Controls.Add(divListItem);
                        divListItem.Attributes["style"] = "width: 7px; height: 7px; background-color: #4F8DA6";

                        HtmlGenericControl divClientBody = new HtmlGenericControl("div");
                        tdRight.Controls.Add(divClientBody);

                        table = new HtmlGenericControl("table");
                        divClientBody.Controls.Add(table);
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
                                (clientData.serviceType.Equals(siteData.serviceType) == false || clientData.contractAuthor.Equals(siteData.contractAuthor) == false))
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

                            span = new HtmlGenericControl("span");
                            td.Controls.Add(span);
                            span.InnerHtml = "<span class='ping_label'>P:</span> " + clientData.pingPeriod.ToString();
                        }
                    }
                }
            }

            if (allowedUsers.Count == 0 && siteData.guestAllowed == false)
            {
                divSiteFrame.Visible = false;
            }
        }
        catch (FormatException)
        {
            divSiteBody.Controls.Clear();

            HtmlGenericControl divSiteBlock = new HtmlGenericControl("div");
            divSiteBody.Controls.Add(divSiteBlock);
            divSiteBlock.Attributes["class"] = "site_block";

            HtmlGenericControl spanSiteError = new HtmlGenericControl("span");
            divSiteBlock.Controls.Add(spanSiteError);
            spanSiteError.Attributes["class"] = "error_text";
            spanSiteError.InnerText = "The site integrity is broken.";
        }
    }

    void drawMUSite(SiteData siteData)
    {
        HtmlGenericControl divSiteFrame = new HtmlGenericControl("div");
        PH_Sites.Controls.Add(divSiteFrame);
        divSiteFrame.Attributes["class"] = "site_frame";

        HtmlGenericControl divSiteDescription = new HtmlGenericControl("div");
        divSiteFrame.Controls.Add(divSiteDescription);
        divSiteDescription.Attributes["style"] = "padding-top: 0px; padding-bottom: 7px; padding-left: 25px; padding-right: 30px;";

        HtmlGenericControl span = new HtmlGenericControl("span");
        divSiteDescription.Controls.Add(span);
        span.Attributes["class"] = "site_name";
        span.InnerText = siteData.description;

        Button buttonScrollPosition = new Button();
        buttonScrollPosition.ID = string.Format("ScrollPosition_{0}", siteData.siteId);
        PH_Sites.Controls.Add(buttonScrollPosition);
        buttonScrollPosition.Attributes["style"] = "width:0px; height:0px; border-width:0px; padding: 0px; margin:0px";

        HtmlGenericControl divSiteMenu = new HtmlGenericControl("div");
        divSiteFrame.Controls.Add(divSiteMenu);
        divSiteMenu.Attributes["class"] = "site_menu";

        HtmlGenericControl table = new HtmlGenericControl("table");
        divSiteMenu.Controls.Add(table);
        table.Attributes["class"] = "wide_table";
        HtmlGenericControl tr = new HtmlGenericControl("tr");
        table.Controls.Add(tr);

        HtmlGenericControl td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table";
        td.Attributes["style"] = "width: 55px; padding: 1px; padding-left: 3px;";

        HtmlGenericControl divConfigButton = new HtmlGenericControl("div");
        td.Controls.Add(divConfigButton);
        divConfigButton.Attributes["class"] = "SubmitButtonMini Blue";

        TButton buttonConfig = new TButton();
        divConfigButton.Controls.Add(buttonConfig);
        buttonConfig.Args.Add(siteData);
        buttonConfig.Text = "config";
        buttonConfig.ID = string.Format("Config_{0}", siteData.siteId);
        buttonConfig.Click += new EventHandler(ConfigMUSite_Click);

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table";
        td.Attributes["style"] = "text-align: left; padding: 2px; padding-left: 10px;";
        HtmlGenericControl spanSiteType = new HtmlGenericControl("span");
        td.Controls.Add(spanSiteType);
        spanSiteType.Attributes["class"] = "site_type";
        spanSiteType.InnerHtml = siteData.serviceType + "<span class='gray_text'> ( </span>" + siteData.contractAuthor + "<span class='gray_text'> )</span>";

        if (siteData.enabled == false)
        {
            td = new HtmlGenericControl("td");
            tr.Controls.Add(td);

            td.Attributes["class"] = "wide_table";
            td.Attributes["style"] = "text-align:right; padding-left: 4px; padding-right: 4px; white-space: nowrap;";

            HtmlGenericControl spanSiteStatus = new HtmlGenericControl("span");
            td.Controls.Add(spanSiteStatus);
            spanSiteStatus.Attributes["class"] = "object_status";
            spanSiteStatus.InnerText = "site disabled";
        }

        HtmlGenericControl divSiteBody = new HtmlGenericControl("div");
        divSiteFrame.Controls.Add(divSiteBody);
        divSiteBody.Attributes["class"] = "site_body";

        try
        {
            HtmlGenericControl divSiteBlock = new HtmlGenericControl("div");
            divSiteBody.Controls.Add(divSiteBlock);
            divSiteBlock.Attributes["class"] = "site_block";

            HtmlGenericControl divSiteBlockHeader = new HtmlGenericControl("div");
            divSiteBlock.Controls.Add(divSiteBlockHeader);
            divSiteBlockHeader.Attributes["class"] = "site_block_header left";
            divSiteBlockHeader.InnerText = "list of services";

            HtmlGenericControl divSiteBlockItem, tdLeft, tdRight;

            List<ServiceData> services = m_dataset.services.FindAll(x => x.siteId == siteData.siteId);
            foreach (ServiceData serviceData in services)
            {
                divSiteBlockItem = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockItem);
                divSiteBlockItem.Attributes["class"] = "site_block_item underline";

                table = new HtmlGenericControl("table");
                divSiteBlockItem.Controls.Add(table);
                table.Attributes["class"] = "wide_table";
                tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                HtmlGenericControl tdType = new HtmlGenericControl("td");
                tr.Controls.Add(tdType);
                tdType.Attributes["class"] = "wide_table";
                tdType.Attributes["style"] = "text-align: left";

                HtmlGenericControl tdStatus = new HtmlGenericControl("td");
                tr.Controls.Add(tdStatus);
                tdStatus.Attributes["class"] = "wide_table";
                tdStatus.Attributes["style"] = "text-align: right";

                table = new HtmlGenericControl("table");
                tdType.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";
                td.Attributes["style"] = "padding-right: 15px;";

                HtmlGenericControl spanHostname = new HtmlGenericControl("span");
                td.Controls.Add(spanHostname);
                spanHostname.InnerText = serviceData.hostname;
                spanHostname.Attributes["class"] = "name";

                if (string.IsNullOrEmpty(serviceData.serviceType) == false)
                {
                    if (serviceData.serviceType.Equals(siteData.serviceType) == false || serviceData.contractAuthor.Equals(siteData.contractAuthor) == false)
                    {
                        td = new HtmlGenericControl("td");
                        tr.Controls.Add(td);
                        td.Attributes["class"] = "auto_table";
                        td.Attributes["style"] = "padding-right: 5px;";

                        span = new HtmlGenericControl("span");
                        td.Controls.Add(span);
                        span.Attributes["class"] = "service_type";
                        span.InnerHtml = serviceData.serviceType + "<span class='gray_text'> ( </span>" + serviceData.contractAuthor + "<span class='gray_text'> )</span>";

                        span = new HtmlGenericControl("span");
                        tdStatus.Controls.Add(span);
                        span.Attributes["class"] = "object_status";
                        span.InnerText = "service type conflict";
                    }
                    else if (serviceData.ssHash.Equals(siteData.ssHash) == false)
                    {
                        HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                        tdStatus.Controls.Add(spanStatus);
                        spanStatus.Attributes["class"] = "object_status";
                        spanStatus.InnerText = "site structure mismatch";
                    }

                    if (string.IsNullOrEmpty(serviceData.version) == false)
                    {
                        td = new HtmlGenericControl("td");
                        tr.Controls.Add(td);
                        td.Attributes["class"] = "auto_table";

                        span = new HtmlGenericControl("span");
                        td.Controls.Add(span);
                        span.Attributes["class"] = "service_type";
                        span.InnerHtml = "<span class='version_label'>ver. </span>" + serviceData.version;
                    }
                }
                else
                {
                    HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                    tdStatus.Controls.Add(spanStatus);
                    spanStatus.Attributes["class"] = "object_status";
                    spanStatus.InnerText = "service type undefined";
                }

                if (serviceData.pingPeriod > 0)
                {
                    td = new HtmlGenericControl("td");
                    tr.Controls.Add(td);
                    td.Attributes["class"] = "auto_table";
                    td.Attributes["style"] = "padding-left: 30px;";

                    span = new HtmlGenericControl("span");
                    td.Controls.Add(span);
                    span.InnerHtml = "<span style='color:green;'>P:</span> " + serviceData.pingPeriod.ToString();
                }

                if (serviceData.enabled == false)
                {
                    HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                    tdStatus.Controls.Add(spanStatus);
                    spanStatus.Attributes["class"] = "object_status";
                    if (tdStatus.Controls.Count == 0)
                        spanStatus.InnerText = "disabled";
                    else
                        spanStatus.InnerText = "; disabled";
                }
            }

            if (siteData.guestAllowed)
            {
                divSiteBlock = new HtmlGenericControl("div");
                divSiteBody.Controls.Add(divSiteBlock);
                divSiteBlock.Attributes["class"] = "site_block";

                divSiteBlockHeader = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockHeader);
                divSiteBlockHeader.Attributes["class"] = "site_block_header left";
                divSiteBlockHeader.InnerText = "guest page";

                divSiteBlockItem = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockItem);
                divSiteBlockItem.Attributes["class"] = "site_block_item";

                HtmlGenericControl spanGuestPage = new HtmlGenericControl("span");
                divSiteBlockItem.Controls.Add(spanGuestPage);
                spanGuestPage.Attributes["class"] = "guest_page_url";
                spanGuestPage.InnerText = string.Format("{0}/guest.aspx?site={1}", SoftnetRegistry.settings_getSiteUrl(), siteData.siteKey);

                if (siteData.statelessGuestSupported)
                {
                    divSiteBlock = new HtmlGenericControl("div");
                    divSiteBody.Controls.Add(divSiteBlock);
                    divSiteBlock.Attributes["class"] = "site_block";

                    divSiteBlockHeader = new HtmlGenericControl("div");
                    divSiteBlock.Controls.Add(divSiteBlockHeader);
                    divSiteBlockHeader.Attributes["class"] = "site_block_header left";
                    divSiteBlockHeader.InnerText = "guest shared uri";

                    divSiteBlockItem = new HtmlGenericControl("div");
                    divSiteBlock.Controls.Add(divSiteBlockItem);
                    divSiteBlockItem.Attributes["class"] = "site_block_item";

                    span = new HtmlGenericControl("span");
                    divSiteBlockItem.Controls.Add(span);
                    span.Attributes["class"] = "guest_shared_uri";
                    span.InnerText = string.Format("softnet-ms://{0}@{1}", siteData.siteKey, SoftnetRegistry.settings_getServerAddress());
                }
            }

            divSiteBlock = new HtmlGenericControl("div");
            divSiteBody.Controls.Add(divSiteBlock);
            divSiteBlock.Attributes["class"] = "site_block";

            divSiteBlockHeader = new HtmlGenericControl("div");
            divSiteBlock.Controls.Add(divSiteBlockHeader);
            divSiteBlockHeader.Attributes["class"] = "site_block_header left";
            divSiteBlockHeader.InnerText = "clients";

            List<UserData> allowedUsers = new List<UserData>();
            foreach (UserData userData in m_dataset.users)
            {
                if (userData.kind == 3)
                {
                    if (m_dataset.siteUsers.FindIndex(x => x.siteId == siteData.siteId && x.userId == userData.userId) >= 0)
                        allowedUsers.Add(userData);
                    else if (siteData.implicitUsersAllowed && userData.dedicated == false)
                        allowedUsers.Add(userData);
                }
            }

            foreach (UserData userData in allowedUsers)
            {
                divSiteBlockItem = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockItem);
                divSiteBlockItem.Attributes["class"] = "site_block_item underline";

                table = new HtmlGenericControl("table");
                divSiteBlockItem.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";

                HtmlGenericControl labelUserName = new HtmlGenericControl("span");
                td.Controls.Add(labelUserName);
                labelUserName.InnerText = userData.name;
                labelUserName.Attributes["class"] = "user";

                if (m_dataset.contactStatus == 2)
                {
                    labelUserName.Attributes["class"] = "user disabled_status";
                    labelUserName.Attributes["title"] = "Your partner has been deleted from the network.";
                }
                else if (userData.enabled == false)
                    labelUserName.Attributes["class"] = "user disabled_status";

                List<ClientData> userClients = m_dataset.clients.FindAll(x => x.siteId == siteData.siteId && x.userId == userData.userId);
                if (userClients.Count > 0)
                {
                    HtmlGenericControl divClientList = new HtmlGenericControl("div");
                    divSiteBlockItem.Controls.Add(divClientList);
                    divClientList.Attributes["style"] = "padding-left: 20px; padding-top: 5px;";

                    foreach (ClientData clientData in userClients)
                    {
                        HtmlGenericControl divClient = new HtmlGenericControl("div");
                        divClientList.Controls.Add(divClient);
                        divClient.Attributes["class"] = "site_block_item";

                        HtmlGenericControl tableLayout = new HtmlGenericControl("table");
                        divClient.Controls.Add(tableLayout);
                        tableLayout.Attributes["class"] = "wide_table";
                        tr = new HtmlGenericControl("tr");
                        tableLayout.Controls.Add(tr);

                        tdLeft = new HtmlGenericControl("td");
                        tr.Controls.Add(tdLeft);
                        tdLeft.Attributes["class"] = "wide_table";
                        tdLeft.Attributes["style"] = "width: 7px; padding-right: 14px;";

                        tdRight = new HtmlGenericControl("td");
                        tr.Controls.Add(tdRight);
                        tdRight.Attributes["class"] = "wide_table";

                        HtmlGenericControl divListItem = new HtmlGenericControl("div");
                        tdLeft.Controls.Add(divListItem);
                        divListItem.Attributes["style"] = "width: 7px; height: 7px; background-color: #4F8DA6";

                        HtmlGenericControl divClientBody = new HtmlGenericControl("div");
                        tdRight.Controls.Add(divClientBody);

                        table = new HtmlGenericControl("table");
                        divClientBody.Controls.Add(table);
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
                                (clientData.serviceType.Equals(siteData.serviceType) == false || clientData.contractAuthor.Equals(siteData.contractAuthor) == false))
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

                            span = new HtmlGenericControl("span");
                            td.Controls.Add(span);
                            span.InnerHtml = "<span class='ping_label'>P:</span> " + clientData.pingPeriod.ToString();
                        }
                    }
                }
            }

            if (siteData.guestAllowed)
            {
                UserData guestData = m_dataset.users.Find(x => x.kind == 4);
                if (guestData == null)
                    throw new FormatException();

                divSiteBlockItem = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockItem);
                divSiteBlockItem.Attributes["class"] = "site_block_item underline";

                table = new HtmlGenericControl("table");
                divSiteBlockItem.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";

                Label labelUserName = new Label();
                td.Controls.Add(labelUserName);
                labelUserName.Text = guestData.name;
                labelUserName.CssClass = "user_guest";

                if (guestData.enabled == false)
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";

                List<ClientData> guestClients = m_dataset.guestClients.FindAll(x => x.siteId == siteData.siteId);
                if (guestClients.Count > 0)
                {
                    HtmlGenericControl divClientList = new HtmlGenericControl("div");
                    divSiteBlockItem.Controls.Add(divClientList);
                    divClientList.Attributes["style"] = "padding-left: 20px; padding-top: 5px;";

                    foreach (ClientData clientData in guestClients)
                    {
                        HtmlGenericControl divClient = new HtmlGenericControl("div");
                        divClientList.Controls.Add(divClient);
                        divClient.Attributes["class"] = "site_block_item";

                        HtmlGenericControl tableLayout = new HtmlGenericControl("table");
                        divClient.Controls.Add(tableLayout);
                        tableLayout.Attributes["class"] = "wide_table";
                        tr = new HtmlGenericControl("tr");
                        tableLayout.Controls.Add(tr);

                        tdLeft = new HtmlGenericControl("td");
                        tr.Controls.Add(tdLeft);
                        tdLeft.Attributes["class"] = "wide_table";
                        tdLeft.Attributes["style"] = "width: 7px; padding-right: 14px;";

                        tdRight = new HtmlGenericControl("td");
                        tr.Controls.Add(tdRight);
                        tdRight.Attributes["class"] = "wide_table";

                        HtmlGenericControl divListItem = new HtmlGenericControl("div");
                        tdLeft.Controls.Add(divListItem);
                        divListItem.Attributes["style"] = "width: 7px; height: 7px; background-color: #4F8DA6";

                        HtmlGenericControl divClientBody = new HtmlGenericControl("div");
                        tdRight.Controls.Add(divClientBody);

                        table = new HtmlGenericControl("table");
                        divClientBody.Controls.Add(table);
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
                                (clientData.serviceType.Equals(siteData.serviceType) == false || clientData.contractAuthor.Equals(siteData.contractAuthor) == false))
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

                            span = new HtmlGenericControl("span");
                            td.Controls.Add(span);
                            span.InnerHtml = "<span class='ping_label'>P:</span> " + clientData.pingPeriod.ToString();
                        }
                    }
                }
            }

            if (allowedUsers.Count == 0 && siteData.guestAllowed == false)
            {
                divSiteFrame.Visible = false;
            }
        }
        catch (FormatException)
        {
            divSiteBody.Controls.Clear();

            HtmlGenericControl divSiteBlock = new HtmlGenericControl("div");
            divSiteBody.Controls.Add(divSiteBlock);
            divSiteBlock.Attributes["class"] = "site_block";

            HtmlGenericControl spanSiteError = new HtmlGenericControl("span");
            divSiteBlock.Controls.Add(spanSiteError);
            spanSiteError.Attributes["class"] = "error_text";
            spanSiteError.InnerText = "The site integrity is broken.";
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