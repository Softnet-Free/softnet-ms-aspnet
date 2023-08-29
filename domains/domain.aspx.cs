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
using System.Text.RegularExpressions;

public partial class domain : System.Web.UI.Page
{
    DomainDataset m_dataset = null;
    UrlBuider m_urlBuider;

    protected void Back_Click(object sender, EventArgs e)
    {
        string retUrl = m_urlBuider.getBackUrl();
        if (retUrl != null)
            Response.Redirect(retUrl);
        else
            Response.Redirect("~/domains/default.aspx");
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        try
        {
            long domainId;
            if (long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("did"), out domainId) == false)
                throw new InvalidIdentifierSoftnetException();

            m_dataset = new DomainDataset();
            SoftnetRegistry.GetDomainDataset(this.Context.User.Identity.Name, domainId, m_dataset);

            string retString = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("ret");
            m_urlBuider = new UrlBuider(retString);

            this.Title = m_dataset.domainName;
            L_Domain.Text = m_dataset.domainName;

            bool isInViewMode = true;
            long selectedUserId = 0;
            if (string.IsNullOrEmpty(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("delete")) == false)
            {
                P_DeleteDomain.Visible = true;
            }
            else if (long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("uid"), out selectedUserId))
            {
                isInViewMode = false;        
            }

            LoadUsers(selectedUserId);

            if (isInViewMode)
            {
                if (m_dataset.sites.Count > 0)
                {
                    P_Sites.Visible = true;
                    foreach (SiteData siteData in m_dataset.sites)
                    {
                        if (siteData.structured)
                        {
                            if (siteData.rolesSupported)
                            {
                                if (siteData.siteKind == Constants.SiteKind.SingleService)
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
                                if (siteData.siteKind == Constants.SiteKind.SingleService)
                                {
                                    drawSUSite(siteData);
                                }
                                else
                                {
                                    drawMUSite(siteData);
                                }
                            }
                        }
                        else
                        {
                            if (siteData.siteKind == Constants.SiteKind.SingleService)
                            {
                                drawSSite(siteData);
                            }
                            else
                            {
                                drawMSite(siteData);
                            }
                        }
                    }

                    if (this.IsPostBack == false)
                    {
                        long scrollId;
                        if (long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("sp"), out scrollId))
                        {
                            Control spButton = PH_SiteList.FindControl(string.Format("ScrollPosition_{0}", scrollId));
                            if (spButton != null)
                                spButton.Focus();
                        }
                    }
                }
            }
            else
            {
                P_ViewUserListButton.Visible = true;
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
        Response.Redirect(m_urlBuider.getNextUrl(string.Format("~/domains/srsite.aspx?sid={0}", siteData.siteId), string.Format("~/domains/domain.aspx?did={0}&sp={1}", m_dataset.domainId, siteData.siteId)));
    }

    protected void SRClients_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        SiteData siteData = (SiteData)button.Args[0];
        Response.Redirect(m_urlBuider.getNextUrl(string.Format("~/domains/srclients.aspx?sid={0}", siteData.siteId), string.Format("~/domains/domain.aspx?did={0}&sp={1}", m_dataset.domainId, siteData.siteId)));
    }

    protected void ConfigMRSite_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        SiteData siteData = (SiteData)button.Args[0];
        Response.Redirect(m_urlBuider.getNextUrl(string.Format("~/domains/mrsite.aspx?sid={0}", siteData.siteId), string.Format("~/domains/domain.aspx?did={0}&sp={1}", m_dataset.domainId, siteData.siteId)));
    }

    protected void MRClients_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        SiteData siteData = (SiteData)button.Args[0];
        Response.Redirect(m_urlBuider.getNextUrl(string.Format("~/domains/mrclients.aspx?sid={0}", siteData.siteId), string.Format("~/domains/domain.aspx?did={0}&sp={1}", m_dataset.domainId, siteData.siteId)));
    }

    protected void ConfigSUSite_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        SiteData siteData = (SiteData)button.Args[0];
        Response.Redirect(m_urlBuider.getNextUrl(string.Format("~/domains/susite.aspx?sid={0}", siteData.siteId), string.Format("~/domains/domain.aspx?did={0}&sp={1}", m_dataset.domainId, siteData.siteId)));
    }

    protected void SUClients_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        SiteData siteData = (SiteData)button.Args[0];
        Response.Redirect(m_urlBuider.getNextUrl(string.Format("~/domains/suclients.aspx?sid={0}", siteData.siteId), string.Format("~/domains/domain.aspx?did={0}&sp={1}", m_dataset.domainId, siteData.siteId)));
    }

    protected void ConfigMUSite_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        SiteData siteData = (SiteData)button.Args[0];
        Response.Redirect(m_urlBuider.getNextUrl(string.Format("~/domains/musite.aspx?sid={0}", siteData.siteId), string.Format("~/domains/domain.aspx?did={0}&sp={1}", m_dataset.domainId, siteData.siteId)));
    }

    protected void MUClients_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        SiteData siteData = (SiteData)button.Args[0];
        Response.Redirect(m_urlBuider.getNextUrl(string.Format("~/domains/muclients.aspx?sid={0}", siteData.siteId), string.Format("~/domains/domain.aspx?did={0}&sp={1}", m_dataset.domainId, siteData.siteId)));
    }

    protected void ConfigSSite_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        SiteData siteData = (SiteData)button.Args[0];
        Response.Redirect(m_urlBuider.getNextUrl(string.Format("~/domains/ssite.aspx?sid={0}", siteData.siteId), string.Format("~/domains/domain.aspx?did={0}&sp={1}", m_dataset.domainId, siteData.siteId)));
    }

    protected void ConfigMSite_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        SiteData siteData = (SiteData)button.Args[0];
        Response.Redirect(m_urlBuider.getNextUrl(string.Format("~/domains/msite.aspx?sid={0}", siteData.siteId), string.Format("~/domains/domain.aspx?did={0}&sp={1}", m_dataset.domainId, siteData.siteId)));
    }

    protected void DeleteSite_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        SiteData siteData = (SiteData)button.Args[0];
        Response.Redirect(m_urlBuider.getNextUrl(string.Format("~/domains/deletesite.aspx?sid={0}", siteData.siteId), string.Format("~/domains/domain.aspx?did={0}&sp={1}", m_dataset.domainId, siteData.siteId)));
    }

    void drawSRSite(SiteData siteData)
    {
        HtmlGenericControl divSiteFrame = new HtmlGenericControl("div");
        PH_SiteList.Controls.Add(divSiteFrame);
        divSiteFrame.Attributes["class"] = "site_frame";

        Button buttonScrollPosition = new Button();
        buttonScrollPosition.ID = string.Format("ScrollPosition_{0}", siteData.siteId);
        PH_SiteList.Controls.Add(buttonScrollPosition);
        buttonScrollPosition.Attributes["style"] = "width:0px; height:0px; border-width:0px; padding: 0px; margin:0px";

        HtmlGenericControl divSiteDescription = new HtmlGenericControl("div");
        divSiteFrame.Controls.Add(divSiteDescription);
        divSiteDescription.Attributes["style"] = "padding-top: 0px; padding-bottom: 7px; padding-left: 25px; padding-right: 30px;";

        HtmlGenericControl span = new HtmlGenericControl("span");
        divSiteDescription.Controls.Add(span);
        span.Attributes["class"] = "site_name";
        span.InnerText = siteData.description;

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
        td.Attributes["style"] = "width: 60px; padding: 1px; padding-left: 4px;";

        HtmlGenericControl divClientsButton = new HtmlGenericControl("div");
        td.Controls.Add(divClientsButton);
        divClientsButton.Attributes["class"] = "SubmitButtonMini Blue";

        TButton buttonClients = new TButton();
        divClientsButton.Controls.Add(buttonClients);
        buttonClients.Args.Add(siteData);
        buttonClients.Text = "clients";
        buttonClients.ID = string.Format("Clients_{0}", siteData.siteId);
        buttonClients.Click += new EventHandler(SRClients_Click);

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table";
        td.Attributes["style"] = "width: auto; text-align: left; padding: 1px; padding-left: 20px;";

        HtmlGenericControl spanSiteType = new HtmlGenericControl("span");
        td.Controls.Add(spanSiteType);
        spanSiteType.Attributes["class"] = "site_type";
        spanSiteType.InnerHtml = siteData.serviceType + "<span class='gray_text'> ( </span>" + siteData.contractAuthor + "<span class='gray_text'> )</span>";

        if (siteData.enabled == false)
        {
            td = new HtmlGenericControl("td");
            tr.Controls.Add(td);
            td.Attributes["class"] = "wide_table";
            td.Attributes["style"] = "text-align:right; padding: 1px; padding-left: 4px; padding-right: 4px; white-space: nowrap;";

            HtmlGenericControl spanSiteStatus = new HtmlGenericControl("span");
            td.Controls.Add(spanSiteStatus);
            spanSiteStatus.Attributes["class"] = "object_status";
            spanSiteStatus.InnerText = "site disabled";
        }

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table";
        td.Attributes["style"] = "width:23px; padding: 1px;";

        HtmlGenericControl divDeleteSite = new HtmlGenericControl("div");
        td.Controls.Add(divDeleteSite);
        divDeleteSite.Attributes["class"] = "SubmitButtonSquareMini RedOrange";

        TButton buttonDeleteSite = new TButton();
        divDeleteSite.Controls.Add(buttonDeleteSite);
        buttonDeleteSite.Args.Add(siteData);
        buttonDeleteSite.Text = "X";
        buttonDeleteSite.ToolTip = "Delete Site";
        buttonDeleteSite.ID = string.Format("DeleteSite_{0}", siteData.siteId);
        buttonDeleteSite.Click += new EventHandler(DeleteSite_Click);

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

            List<RoleData> roles = m_dataset.roles.FindAll(x => x.siteId == siteData.siteId);
            if (roles.Count == 0)
                throw new FormatException();

            divSiteBlockItem = new HtmlGenericControl("div");
            divSiteBlock.Controls.Add(divSiteBlockItem);
            divSiteBlockItem.Attributes["class"] = "site_block_item";

            HtmlGenericControl spanRoles = new HtmlGenericControl("span");
            divSiteBlockItem.Controls.Add(spanRoles);

            RoleData defaultRole = null;
            string rolesView = "";
            for (int i = 0; i < roles.Count; i++)
            {
                RoleData roleData = roles[i];
                if (i == 0)
                    rolesView = "<span class='user_role'>" + roleData.name + "</span>";
                else
                    rolesView = rolesView + "<span class='delimeter'>, &nbsp;</span><span class='user_role'>" + roleData.name + "</span>";

                if (roleData.roleId == siteData.defaultRoleId)
                    defaultRole = roleData;
            }
            spanRoles.InnerHtml = rolesView;

            if (defaultRole != null)
            {
                HtmlGenericControl divDefaultRole = new HtmlGenericControl("div");
                divSiteBlockItem.Controls.Add(divDefaultRole);
                divDefaultRole.Attributes["style"] = "margin-top: 8px";

                HtmlGenericControl spanDefaultRole = new HtmlGenericControl("span");
                divDefaultRole.Controls.Add(spanDefaultRole);
                spanDefaultRole.InnerHtml = string.Format("<span style='color:gray'>user default role:</span>&nbsp;&nbsp;<span class='user_default_role'>{0}</span>", defaultRole.name);
            }

            if (siteData.guestSupported)
            {
                divSiteBlock = new HtmlGenericControl("div");
                divSiteBody.Controls.Add(divSiteBlock);
                divSiteBlock.Attributes["class"] = "site_block";

                divSiteBlockHeader = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockHeader);
                divSiteBlockHeader.Attributes["class"] = "site_block_header left";
                divSiteBlockHeader.InnerText = "guest status";

                divSiteBlockItem = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockItem);
                divSiteBlockItem.Attributes["class"] = "site_block_item";

                HtmlGenericControl spanGuestCaption = new HtmlGenericControl("span");
                divSiteBlockItem.Controls.Add(spanGuestCaption);
                spanGuestCaption.Attributes["class"] = "name";
                spanGuestCaption.InnerHtml = "Guest&nbsp;&nbsp;";

                HtmlGenericControl spanGuestStatus = new HtmlGenericControl("span");
                divSiteBlockItem.Controls.Add(spanGuestStatus);
                if (siteData.guestAllowed)
                {
                    spanGuestStatus.Attributes["class"] = "enabled_status";
                    spanGuestStatus.InnerText = "allowed";
                }
                else
                {
                    spanGuestStatus.Attributes["class"] = "disabled_status";
                    spanGuestStatus.InnerText = "denied";                
                }

                if (siteData.guestAllowed)
                {
                    divSiteBlock = new HtmlGenericControl("div");
                    divSiteBody.Controls.Add(divSiteBlock);
                    divSiteBlock.Attributes["class"] = "site_block";

                    divSiteBlockHeader = new HtmlGenericControl("div");
                    divSiteBlock.Controls.Add(divSiteBlockHeader);
                    divSiteBlockHeader.Attributes["class"] = "site_block_header";
                    divSiteBlockHeader.InnerText = "guest page";

                    divSiteBlockItem = new HtmlGenericControl("div");
                    divSiteBlock.Controls.Add(divSiteBlockItem);
                    divSiteBlockItem.Attributes["class"] = "site_block_item";

                    HtmlGenericControl spanGuestPage = new HtmlGenericControl("span");
                    divSiteBlockItem.Controls.Add(spanGuestPage);
                    spanGuestPage.Attributes["class"] = "guest_page_url";
                    spanGuestPage.InnerText = string.Format("{0}/guest.aspx?site={1}", SoftnetRegistry.settings_getManagementSystemUrl(), siteData.siteKey);

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
            }
            
            List<AllowedUser> allowedUsers = new List<AllowedUser>();
            List<UserInRole> usersInRoles = m_dataset.usersInRoles;
            foreach (UserData userData in m_dataset.users)
            {
                if (userData.kind == 2)
                {
                    AllowedUser allowedUser = new AllowedUser(userData);
                    foreach (RoleData roleData in roles)
                    {
                        UserInRole userInRole = usersInRoles.Find(x => x.roleId == roleData.roleId && x.userId == userData.userId);
                        if (userInRole != null)
                            allowedUser.roles.Add(roleData);
                        else if (userData.dedicated == false && roleData.roleId == siteData.defaultRoleId)
                            allowedUser.defaultRole = roleData;
                    }

                    if (allowedUser.roles.Count > 0 || allowedUser.defaultRole != null)
                        allowedUsers.Add(allowedUser);
                }
                else if (userData.kind == 3)
                {
                    AllowedUser allowedUser = new AllowedUser(userData);
                    foreach (RoleData roleData in roles)
                    {
                        UserInRole userInRole = usersInRoles.Find(x => x.roleId == roleData.roleId && x.userId == userData.userId);
                        if (userInRole != null)
                            allowedUser.roles.Add(roleData);
                        else if (userData.dedicated == false && roleData.roleId == siteData.defaultRoleId)
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
                        else if (roleData.roleId == siteData.defaultRoleId)
                            allowedUser.defaultRole = roleData;
                    }

                    if (allowedUser.roles.Count > 0 || allowedUser.defaultRole != null)
                        allowedUsers.Add(allowedUser);
                }
                else if (siteData.guestAllowed)
                {
                    AllowedUser allowedUser = new AllowedUser(userData);
                    allowedUsers.Add(allowedUser);
                }
            }

            if (allowedUsers.Count > 0)
            {
                divSiteBlock = new HtmlGenericControl("div");
                divSiteBody.Controls.Add(divSiteBlock);
                divSiteBlock.Attributes["class"] = "site_block";

                divSiteBlockHeader = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockHeader);
                divSiteBlockHeader.Attributes["class"] = "site_block_header left";
                divSiteBlockHeader.InnerText = "users";

                foreach (AllowedUser allowedUser in allowedUsers)
                {
                    divSiteBlockItem = new HtmlGenericControl("div");
                    divSiteBlock.Controls.Add(divSiteBlockItem);
                    divSiteBlockItem.Attributes["class"] = "site_block_item";

                    if (allowedUser.userData.kind == 2)
                    {
                        Label labelUserName = new Label();
                        divSiteBlockItem.Controls.Add(labelUserName);
                        labelUserName.Text = allowedUser.userData.name;
                        labelUserName.CssClass = "user";

                        if (allowedUser.userData.enabled == false)
                            labelUserName.CssClass = labelUserName.CssClass + " disabled_status";

                        if (allowedUser.userData.dedicated)
                            labelUserName.CssClass = labelUserName.CssClass + " user_dedicated";

                        Label labelRoles = new Label();
                        divSiteBlockItem.Controls.Add(labelRoles);
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
                    }
                    else if (allowedUser.userData.kind == 3)
                    {
                        Label labelUserName = new Label();
                        divSiteBlockItem.Controls.Add(labelUserName);
                        labelUserName.Text = allowedUser.userData.name;
                        labelUserName.CssClass = "user";

                        if (allowedUser.userData.dedicated)
                            labelUserName.CssClass += " user_dedicated";

                        if (allowedUser.userData.contactData.status >= 2)
                        {
                            if (allowedUser.userData.contactData.status == 2)
                            {
                                labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                                labelUserName.ToolTip = "The user should be deleted.";
                            }
                            else
                            {
                                labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                                labelUserName.ToolTip = "The user should be deleted.";
                            }
                        }
                        else if (allowedUser.userData.enabled == false)
                        {
                            labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                            labelUserName.ToolTip = "The user is disabled.";
                        }

                        Label labelContactName = new Label();
                        divSiteBlockItem.Controls.Add(labelContactName);
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
                        divSiteBlockItem.Controls.Add(labelRoles);
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
                    }
                    else if (allowedUser.userData.kind == 1)
                    {
                        Label labelUserName = new Label();
                        divSiteBlockItem.Controls.Add(labelUserName);
                        labelUserName.Text = allowedUser.userData.name;
                        labelUserName.CssClass = "user user_owner";

                        if (allowedUser.userData.enabled == false)
                            labelUserName.CssClass = labelUserName.CssClass + " disabled_status";

                        Label labelRoles = new Label();
                        divSiteBlockItem.Controls.Add(labelRoles);
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
                    }
                    else // allowedUser.userData.kind == 4
                    {
                        Label labelUserName = new Label();
                        divSiteBlockItem.Controls.Add(labelUserName);
                        labelUserName.Text = allowedUser.userData.name;
                        labelUserName.CssClass = "user user_guest";

                        if (allowedUser.userData.enabled == false)
                            labelUserName.CssClass = labelUserName.CssClass + " disabled_status";                        
                    }
                }
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
        PH_SiteList.Controls.Add(divSiteFrame);
        divSiteFrame.Attributes["class"] = "site_frame";

        Button buttonScrollPosition = new Button();
        buttonScrollPosition.ID = string.Format("ScrollPosition_{0}", siteData.siteId);
        PH_SiteList.Controls.Add(buttonScrollPosition);
        buttonScrollPosition.Attributes["style"] = "width:0px; height:0px; border-width:0px; padding: 0px; margin:0px";

        HtmlGenericControl divSiteDescription = new HtmlGenericControl("div");
        divSiteFrame.Controls.Add(divSiteDescription);
        divSiteDescription.Attributes["style"] = "padding-top: 0px; padding-bottom: 7px; padding-left: 25px; padding-right: 30px;";

        HtmlGenericControl span = new HtmlGenericControl("span");
        divSiteDescription.Controls.Add(span);
        span.Attributes["class"] = "site_name";
        span.InnerText = siteData.description;

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
        td.Attributes["style"] = "width: 60px; padding: 1px; padding-left: 4px;";

        HtmlGenericControl divClientsButton = new HtmlGenericControl("div");
        td.Controls.Add(divClientsButton);
        divClientsButton.Attributes["class"] = "SubmitButtonMini Blue";

        TButton buttonClients = new TButton();
        divClientsButton.Controls.Add(buttonClients);
        buttonClients.Args.Add(siteData);
        buttonClients.Text = "clients";
        buttonClients.ID = string.Format("Clients_{0}", siteData.siteId);
        buttonClients.Click += new EventHandler(MRClients_Click);

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table";
        td.Attributes["style"] = "width: auto; text-align: left; padding: 1px; padding-left: 20px;";

        HtmlGenericControl spanSiteType = new HtmlGenericControl("span");
        td.Controls.Add(spanSiteType);
        spanSiteType.Attributes["class"] = "site_type";
        spanSiteType.InnerHtml = siteData.serviceType + "<span class='gray_text'> ( </span>" + siteData.contractAuthor + "<span class='gray_text'> )</span>";

        if (siteData.enabled == false)
        {
            td = new HtmlGenericControl("td");
            tr.Controls.Add(td);
            td.Attributes["class"] = "wide_table";
            td.Attributes["style"] = "text-align:right; padding: 1px; padding-left: 4px; padding-right: 4px; white-space: nowrap;";

            HtmlGenericControl spanSiteStatus = new HtmlGenericControl("span");
            td.Controls.Add(spanSiteStatus);
            spanSiteStatus.Attributes["class"] = "object_status";
            spanSiteStatus.InnerText = "site disabled";
        }

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table";
        td.Attributes["style"] = "width:23px; padding: 1px;";

        HtmlGenericControl divDeleteSite = new HtmlGenericControl("div");
        td.Controls.Add(divDeleteSite);
        divDeleteSite.Attributes["class"] = "SubmitButtonSquareMini RedOrange";

        TButton buttonDeleteSite = new TButton();
        divDeleteSite.Controls.Add(buttonDeleteSite);
        buttonDeleteSite.Args.Add(siteData);
        buttonDeleteSite.Text = "X";
        buttonDeleteSite.ToolTip = "Delete Site";
        buttonDeleteSite.ID = string.Format("DeleteSite_{0}", siteData.siteId);
        buttonDeleteSite.Click += new EventHandler(DeleteSite_Click);

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

            HtmlGenericControl divSiteBlockBody = new HtmlGenericControl("div");
            divSiteBlock.Controls.Add(divSiteBlockBody);
            divSiteBlockBody.Attributes["class"] = "site_block_body";

            HtmlGenericControl divSiteBlockItem;

            List<ServiceData> services = m_dataset.services.FindAll(x => x.siteId == siteData.siteId);
            foreach (ServiceData serviceData in services)
            {
                divSiteBlockItem = new HtmlGenericControl("div");
                divSiteBlockBody.Controls.Add(divSiteBlockItem);
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

            List<RoleData> roles = m_dataset.roles.FindAll(x => x.siteId == siteData.siteId);
            if (roles.Count == 0)
                throw new FormatException();

            divSiteBlockItem = new HtmlGenericControl("div");
            divSiteBlock.Controls.Add(divSiteBlockItem);
            divSiteBlockItem.Attributes["class"] = "site_block_item";

            HtmlGenericControl spanRoles = new HtmlGenericControl("span");
            divSiteBlockItem.Controls.Add(spanRoles);

            RoleData defaultRole = null;
            string rolesView = "";
            for (int i = 0; i < roles.Count; i++)
            {
                RoleData roleData = roles[i];
                if (i == 0)
                    rolesView = "<span class='user_role'>" + roleData.name + "</span>";
                else
                    rolesView = rolesView + "<span class='delimeter'>, &nbsp;</span><span class='user_role'>" + roleData.name + "</span>";

                if (roleData.roleId == siteData.defaultRoleId)
                    defaultRole = roleData;
            }
            spanRoles.InnerHtml = rolesView;

            if (defaultRole != null)
            {
                HtmlGenericControl divDefaultRole = new HtmlGenericControl("div");
                divSiteBlockItem.Controls.Add(divDefaultRole);
                divDefaultRole.Attributes["style"] = "margin-top: 8px";

                HtmlGenericControl spanDefaultRole = new HtmlGenericControl("span");
                divDefaultRole.Controls.Add(spanDefaultRole);
                spanDefaultRole.InnerHtml = string.Format("<span style='color:gray'>user default role:</span>&nbsp;&nbsp;<span class='user_default_role'>{0}</span>", defaultRole.name);
            }

            if (siteData.guestSupported)
            {
                divSiteBlock = new HtmlGenericControl("div");
                divSiteBody.Controls.Add(divSiteBlock);
                divSiteBlock.Attributes["class"] = "site_block";

                divSiteBlockHeader = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockHeader);
                divSiteBlockHeader.Attributes["class"] = "site_block_header";
                divSiteBlockHeader.InnerText = "guest status";

                divSiteBlockItem = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockItem);
                divSiteBlockItem.Attributes["class"] = "site_block_item";

                HtmlGenericControl spanGuestCaption = new HtmlGenericControl("span");
                divSiteBlockItem.Controls.Add(spanGuestCaption);
                spanGuestCaption.Attributes["class"] = "name";
                spanGuestCaption.InnerHtml = "Guest&nbsp;&nbsp;";

                HtmlGenericControl spanGuestStatus = new HtmlGenericControl("span");
                divSiteBlockItem.Controls.Add(spanGuestStatus);
                if (siteData.guestAllowed)
                {
                    spanGuestStatus.Attributes["class"] = "enabled_status";
                    spanGuestStatus.InnerText = "allowed";
                }
                else
                {
                    spanGuestStatus.Attributes["class"] = "disabled_status";
                    spanGuestStatus.InnerText = "denied";
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
                    spanGuestPage.InnerText = string.Format("{0}/guest.aspx?site={1}", SoftnetRegistry.settings_getManagementSystemUrl(), siteData.siteKey);

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
            }

            List<AllowedUser> allowedUsers = new List<AllowedUser>();
            List<UserInRole> usersInRoles = m_dataset.usersInRoles;
            foreach (UserData userData in m_dataset.users)
            {
                if (userData.kind == 2)
                {
                    AllowedUser allowedUser = new AllowedUser(userData);
                    foreach (RoleData roleData in roles)
                    {
                        UserInRole userInRole = usersInRoles.Find(x => x.roleId == roleData.roleId && x.userId == userData.userId);
                        if (userInRole != null)
                            allowedUser.roles.Add(roleData);
                        else if (userData.dedicated == false && roleData.roleId == siteData.defaultRoleId)
                            allowedUser.defaultRole = roleData;
                    }

                    if (allowedUser.roles.Count > 0 || allowedUser.defaultRole != null)
                        allowedUsers.Add(allowedUser);
                }
                else if (userData.kind == 3)
                {
                    AllowedUser allowedUser = new AllowedUser(userData);
                    foreach (RoleData roleData in roles)
                    {
                        UserInRole userInRole = usersInRoles.Find(x => x.roleId == roleData.roleId && x.userId == userData.userId);
                        if (userInRole != null)
                            allowedUser.roles.Add(roleData);
                        else if (userData.dedicated == false && roleData.roleId == siteData.defaultRoleId)
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
                        else if (roleData.roleId == siteData.defaultRoleId)
                            allowedUser.defaultRole = roleData;
                    }

                    if (allowedUser.roles.Count > 0 || allowedUser.defaultRole != null)
                        allowedUsers.Add(allowedUser);
                }
                else if (siteData.guestAllowed)
                {
                    AllowedUser allowedUser = new AllowedUser(userData);
                    allowedUsers.Add(allowedUser);
                }
            }

            if (allowedUsers.Count > 0)
            {
                divSiteBlock = new HtmlGenericControl("div");
                divSiteBody.Controls.Add(divSiteBlock);
                divSiteBlock.Attributes["class"] = "site_block";

                divSiteBlockHeader = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockHeader);
                divSiteBlockHeader.Attributes["class"] = "site_block_header left";
                divSiteBlockHeader.InnerText = "users";

                divSiteBlockBody = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockBody);
                divSiteBlockBody.Attributes["class"] = "site_block_body";

                foreach (AllowedUser allowedUser in allowedUsers)
                {
                    divSiteBlockItem = new HtmlGenericControl("div");
                    divSiteBlockBody.Controls.Add(divSiteBlockItem);
                    divSiteBlockItem.Attributes["class"] = "site_block_item";

                    if (allowedUser.userData.kind == 2)
                    {
                        Label labelUserName = new Label();
                        divSiteBlockItem.Controls.Add(labelUserName);
                        labelUserName.Text = allowedUser.userData.name;
                        labelUserName.CssClass = "user";

                        if (allowedUser.userData.enabled == false)
                            labelUserName.CssClass = labelUserName.CssClass + " disabled_status";

                        if (allowedUser.userData.dedicated)
                            labelUserName.CssClass = labelUserName.CssClass + " user_dedicated";

                        Label labelRoles = new Label();
                        divSiteBlockItem.Controls.Add(labelRoles);
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
                    }
                    else if (allowedUser.userData.kind == 3)
                    {
                        Label labelUserName = new Label();
                        divSiteBlockItem.Controls.Add(labelUserName);
                        labelUserName.Text = allowedUser.userData.name;
                        labelUserName.CssClass = "user";

                        if (allowedUser.userData.dedicated)
                            labelUserName.CssClass += " user_dedicated";

                        if (allowedUser.userData.contactData.status >= 2)
                        {
                            if (allowedUser.userData.contactData.status == 2)
                            {
                                labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                                labelUserName.ToolTip = "The user should be deleted.";
                            }
                            else
                            {
                                labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                                labelUserName.ToolTip = "The user should be deleted.";
                            }
                        }
                        else if (allowedUser.userData.enabled == false)
                        {
                            labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                            labelUserName.ToolTip = "The user is disabled.";
                        }

                        Label labelContactName = new Label();
                        divSiteBlockItem.Controls.Add(labelContactName);
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
                        divSiteBlockItem.Controls.Add(labelRoles);
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
                    }
                    else if (allowedUser.userData.kind == 1)
                    {
                        Label labelUserName = new Label();
                        divSiteBlockItem.Controls.Add(labelUserName);
                        labelUserName.Text = allowedUser.userData.name;
                        labelUserName.CssClass = "user user_owner";

                        if (allowedUser.userData.enabled == false)
                            labelUserName.CssClass = labelUserName.CssClass + " disabled_status";

                        Label labelRoles = new Label();
                        divSiteBlockItem.Controls.Add(labelRoles);
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
                    }
                    else // allowedUser.userData.kind == 4
                    {
                        Label labelUserName = new Label();
                        divSiteBlockItem.Controls.Add(labelUserName);
                        labelUserName.Text = allowedUser.userData.name;
                        labelUserName.CssClass = "user user_guest";

                        if (allowedUser.userData.enabled == false)
                            labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                    }
                }
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
        PH_SiteList.Controls.Add(divSiteFrame);
        divSiteFrame.Attributes["class"] = "site_frame";

        Button buttonScrollPosition = new Button();
        buttonScrollPosition.ID = string.Format("ScrollPosition_{0}", siteData.siteId);
        PH_SiteList.Controls.Add(buttonScrollPosition);
        buttonScrollPosition.Attributes["style"] = "width:0px; height:0px; border-width:0px; padding: 0px; margin:0px";

        HtmlGenericControl divSiteDescription = new HtmlGenericControl("div");
        divSiteFrame.Controls.Add(divSiteDescription);
        divSiteDescription.Attributes["style"] = "padding-top: 0px; padding-bottom: 7px; padding-left: 25px; padding-right: 30px;";

        HtmlGenericControl span = new HtmlGenericControl("span");
        divSiteDescription.Controls.Add(span);
        span.Attributes["class"] = "site_name";
        span.InnerText = siteData.description;

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
        td.Attributes["style"] = "width: 60px; padding: 1px; padding-left: 4px;";

        HtmlGenericControl divClientsButton = new HtmlGenericControl("div");
        td.Controls.Add(divClientsButton);
        divClientsButton.Attributes["class"] = "SubmitButtonMini Blue";

        TButton buttonClients = new TButton();
        divClientsButton.Controls.Add(buttonClients);
        buttonClients.Args.Add(siteData);
        buttonClients.Text = "clients";
        buttonClients.ID = string.Format("Clients_{0}", siteData.siteId);
        buttonClients.Click += new EventHandler(SUClients_Click);

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table";
        td.Attributes["style"] = "width: auto; text-align: left; padding: 1px; padding-left: 20px;";

        HtmlGenericControl spanSiteType = new HtmlGenericControl("span");
        td.Controls.Add(spanSiteType);
        spanSiteType.Attributes["class"] = "site_type";
        spanSiteType.InnerHtml = siteData.serviceType + "<span class='gray_text'> ( </span>" + siteData.contractAuthor + "<span class='gray_text'> )</span>";

        if (siteData.enabled == false)
        {
            td = new HtmlGenericControl("td");
            tr.Controls.Add(td);
            td.Attributes["class"] = "wide_table";
            td.Attributes["style"] = "text-align:right; padding: 1px; padding-left: 4px; padding-right: 4px; white-space: nowrap;";

            HtmlGenericControl spanSiteStatus = new HtmlGenericControl("span");
            td.Controls.Add(spanSiteStatus);
            spanSiteStatus.Attributes["class"] = "object_status";
            spanSiteStatus.InnerText = "site disabled";
        }

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table";
        td.Attributes["style"] = "width:23px; padding: 1px;";

        HtmlGenericControl divDeleteSite = new HtmlGenericControl("div");
        td.Controls.Add(divDeleteSite);
        divDeleteSite.Attributes["class"] = "SubmitButtonSquareMini RedOrange";

        TButton buttonDeleteSite = new TButton();
        divDeleteSite.Controls.Add(buttonDeleteSite);
        buttonDeleteSite.Args.Add(siteData);
        buttonDeleteSite.Text = "X";
        buttonDeleteSite.ToolTip = "Delete Site";
        buttonDeleteSite.ID = string.Format("DeleteSite_{0}", siteData.siteId);
        buttonDeleteSite.Click += new EventHandler(DeleteSite_Click);

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
            divSiteBlockHeader.InnerText = "access defaults";                    

            divSiteBlockItem = new HtmlGenericControl("div");
            divSiteBlock.Controls.Add(divSiteBlockItem);
            divSiteBlockItem.Attributes["class"] = "site_block_item";

            HtmlGenericControl spanImplicitUsersCaption = new HtmlGenericControl("span");
            divSiteBlockItem.Controls.Add(spanImplicitUsersCaption);
            spanImplicitUsersCaption.Attributes["class"] = "name";
            spanImplicitUsersCaption.InnerHtml = "Implicit users&nbsp;&nbsp;";

            HtmlGenericControl spanImplicitUsersStatus = new HtmlGenericControl("span");
            divSiteBlockItem.Controls.Add(spanImplicitUsersStatus);
            if (siteData.implicitUsersAllowed)
            {
                spanImplicitUsersStatus.Attributes["class"] = "enabled_status";
                spanImplicitUsersStatus.InnerText = "allowed";
            }
            else
            {
                spanImplicitUsersStatus.Attributes["class"] = "disabled_status";
                spanImplicitUsersStatus.InnerText = "denied";
            }

            if (siteData.guestSupported)
            {
                divSiteBlock = new HtmlGenericControl("div");
                divSiteBody.Controls.Add(divSiteBlock);
                divSiteBlock.Attributes["class"] = "site_block";

                divSiteBlockHeader = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockHeader);
                divSiteBlockHeader.Attributes["class"] = "site_block_header";
                divSiteBlockHeader.InnerText = "guest status";

                divSiteBlockItem = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockItem);
                divSiteBlockItem.Attributes["class"] = "site_block_item";

                HtmlGenericControl spanGuestCaption = new HtmlGenericControl("span");
                divSiteBlockItem.Controls.Add(spanGuestCaption);
                spanGuestCaption.Attributes["class"] = "name";
                spanGuestCaption.InnerHtml = "Guest&nbsp;&nbsp;";

                HtmlGenericControl spanGuestStatus = new HtmlGenericControl("span");
                divSiteBlockItem.Controls.Add(spanGuestStatus);
                if (siteData.guestAllowed)
                {
                    spanGuestStatus.Attributes["class"] = "enabled_status";
                    spanGuestStatus.InnerText = "allowed";
                }
                else
                {
                    spanGuestStatus.Attributes["class"] = "disabled_status";
                    spanGuestStatus.InnerText = "denied";
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
                    spanGuestPage.InnerText = string.Format("{0}/guest.aspx?site={1}", SoftnetRegistry.settings_getManagementSystemUrl(), siteData.siteKey);

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
            }

            List<UserData> explicitUsers = new List<UserData>();
            List<UserData> implicitUsers = new List<UserData>();
            foreach (UserData userData in m_dataset.users)
            {
                if (userData.kind == 2)
                {
                    if (m_dataset.siteUsers.FindIndex(x => x.siteId == siteData.siteId && x.userId == userData.userId) >= 0)
                        explicitUsers.Add(userData);
                    else if (siteData.implicitUsersAllowed && userData.dedicated == false)
                        implicitUsers.Add(userData);
                }
                else if (userData.kind == 3)
                {                    
                    if (m_dataset.siteUsers.FindIndex(x => x.siteId == siteData.siteId && x.userId == userData.userId) >= 0)
                        explicitUsers.Add(userData);
                    else if (siteData.implicitUsersAllowed && userData.dedicated == false)
                        implicitUsers.Add(userData);
                }
                else if (userData.kind == 1)
                {
                    if (m_dataset.siteUsers.FindIndex(x => x.siteId == siteData.siteId && x.userId == userData.userId) >= 0)
                        explicitUsers.Add(userData);
                    else if (siteData.implicitUsersAllowed)
                        implicitUsers.Add(userData);
                } // userData.kind == 4
                else if (siteData.guestSupported && siteData.guestAllowed)
                {
                    explicitUsers.Add(userData);
                }
            }

            if (explicitUsers.Count > 0)
            {
                divSiteBlock = new HtmlGenericControl("div");
                divSiteBody.Controls.Add(divSiteBlock);
                divSiteBlock.Attributes["class"] = "site_block";

                divSiteBlockHeader = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockHeader);
                divSiteBlockHeader.Attributes["class"] = "site_block_header left";
                divSiteBlockHeader.InnerText = "explicit users";                

                foreach (UserData userData in explicitUsers)
                {
                    divSiteBlockItem = new HtmlGenericControl("div");
                    divSiteBlock.Controls.Add(divSiteBlockItem);
                    divSiteBlockItem.Attributes["class"] = "site_block_item";

                    if (userData.kind == 2)
                    {
                        Label labelUserName = new Label();
                        divSiteBlockItem.Controls.Add(labelUserName);
                        labelUserName.Text = userData.name;
                        labelUserName.CssClass = "user";

                        if (userData.enabled == false)
                            labelUserName.CssClass = labelUserName.CssClass + " disabled_status";

                        if (userData.dedicated)
                            labelUserName.CssClass = labelUserName.CssClass + " user_dedicated";
                    }
                    else if (userData.kind == 3)
                    {
                        Label labelUserName = new Label();
                        divSiteBlockItem.Controls.Add(labelUserName);
                        labelUserName.Text = userData.name;
                        labelUserName.CssClass = "user";

                        if (userData.dedicated)
                            labelUserName.CssClass += " user_dedicated";

                        if (userData.contactData.status >= 2)
                        {
                            if (userData.contactData.status == 2)
                            {
                                labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                                labelUserName.ToolTip = "The user should be deleted.";
                            }
                            else
                            {
                                labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                                labelUserName.ToolTip = "The user should be deleted.";
                            }
                        }
                        else if (userData.enabled == false)
                        {
                            labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                            labelUserName.ToolTip = "The user is disabled.";
                        }

                        Label labelContactName = new Label();
                        divSiteBlockItem.Controls.Add(labelContactName);
                        labelContactName.Text = "&nbsp;&nbsp;<span class='gray_text'>&#60;</span>" + ContactDisplayName.Adjust(userData.contactData.contactName) + "<span class='gray_text'>&#62;</span>";
                        labelContactName.CssClass = "contact_in_status_0";

                        if (userData.contactData.status == 1)
                        {
                            labelContactName.CssClass = "contact_in_status_1";
                            labelContactName.ToolTip = "Your partner deleted the contact.";
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
                        Label labelUserName = new Label();
                        divSiteBlockItem.Controls.Add(labelUserName);
                        labelUserName.Text = userData.name;
                        labelUserName.CssClass = "user user_owner";

                        if (userData.enabled == false)
                            labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                    }
                    else // allowedUser.userData.kind == 4
                    {
                        Label labelUserName = new Label();
                        divSiteBlockItem.Controls.Add(labelUserName);
                        labelUserName.Text = userData.name;
                        labelUserName.CssClass = "user user_guest";

                        if (userData.enabled == false)
                            labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                    }
                }
            }

            if (implicitUsers.Count > 0)
            {
                divSiteBlock = new HtmlGenericControl("div");
                divSiteBody.Controls.Add(divSiteBlock);
                divSiteBlock.Attributes["class"] = "site_block";

                divSiteBlockHeader = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockHeader);
                divSiteBlockHeader.Attributes["class"] = "site_block_header left";
                divSiteBlockHeader.InnerText = "implicit users";                

                foreach (UserData userData in implicitUsers)
                {
                    divSiteBlockItem = new HtmlGenericControl("div");
                    divSiteBlock.Controls.Add(divSiteBlockItem);
                    divSiteBlockItem.Attributes["class"] = "site_block_item";

                    if (userData.kind == 2)
                    {
                        Label labelUserName = new Label();
                        divSiteBlockItem.Controls.Add(labelUserName);
                        labelUserName.Text = userData.name;
                        labelUserName.CssClass = "user";

                        if (userData.dedicated)
                            labelUserName.CssClass = labelUserName.CssClass + " user_dedicated";

                        if (userData.enabled == false)
                            labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                    }
                    else if (userData.kind == 3)
                    {
                        Label labelUserName = new Label();
                        divSiteBlockItem.Controls.Add(labelUserName);
                        labelUserName.Text = userData.name;
                        labelUserName.CssClass = "user";

                        if (userData.dedicated)
                            labelUserName.CssClass += " user_dedicated";

                        if (userData.contactData.status >= 2)
                        {
                            if (userData.contactData.status == 2)
                            {
                                labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                                labelUserName.ToolTip = "The user should be deleted.";
                            }
                            else
                            {
                                labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                                labelUserName.ToolTip = "The user should be deleted.";
                            }
                        }
                        else if (userData.enabled == false)
                        {
                            labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                            labelUserName.ToolTip = "The user is disabled.";
                        }

                        Label labelContactName = new Label();
                        divSiteBlockItem.Controls.Add(labelContactName);
                        labelContactName.Text = "&nbsp;&nbsp;<span class='gray_text'>&#60;</span>" + ContactDisplayName.Adjust(userData.contactData.contactName) + "<span class='gray_text'>&#62;</span>";
                        labelContactName.CssClass = "contact_in_status_0";

                        if (userData.contactData.status == 1)
                        {
                            labelContactName.CssClass = "contact_in_status_1";
                            labelContactName.ToolTip = "Your partner deleted the contact.";
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
                        Label labelUserName = new Label();
                        divSiteBlockItem.Controls.Add(labelUserName);
                        labelUserName.Text = userData.name;
                        labelUserName.CssClass = "user user_owner";

                        if (userData.enabled == false)
                            labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                    }                    
                }
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
        PH_SiteList.Controls.Add(divSiteFrame);
        divSiteFrame.Attributes["class"] = "site_frame";

        Button buttonScrollPosition = new Button();
        buttonScrollPosition.ID = string.Format("ScrollPosition_{0}", siteData.siteId);
        PH_SiteList.Controls.Add(buttonScrollPosition);
        buttonScrollPosition.Attributes["style"] = "width:0px; height:0px; border-width:0px; padding: 0px; margin:0px";

        HtmlGenericControl divSiteDescription = new HtmlGenericControl("div");
        divSiteFrame.Controls.Add(divSiteDescription);
        divSiteDescription.Attributes["style"] = "padding-top: 0px; padding-bottom: 7px; padding-left: 25px; padding-right: 30px;";

        HtmlGenericControl span = new HtmlGenericControl("span");
        divSiteDescription.Controls.Add(span);
        span.Attributes["class"] = "site_name";
        span.InnerText = siteData.description;

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
        td.Attributes["style"] = "width: 60px; padding: 1px; padding-left: 4px;";

        HtmlGenericControl divClientsButton = new HtmlGenericControl("div");
        td.Controls.Add(divClientsButton);
        divClientsButton.Attributes["class"] = "SubmitButtonMini Blue";

        TButton buttonClients = new TButton();
        divClientsButton.Controls.Add(buttonClients);
        buttonClients.Args.Add(siteData);
        buttonClients.Text = "clients";
        buttonClients.ID = string.Format("Clients_{0}", siteData.siteId);
        buttonClients.Click += new EventHandler(MUClients_Click);

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table";
        td.Attributes["style"] = "width: auto; text-align: left; padding: 1px; padding-left: 20px;";

        HtmlGenericControl spanSiteType = new HtmlGenericControl("span");
        td.Controls.Add(spanSiteType);
        spanSiteType.Attributes["class"] = "site_type";
        spanSiteType.InnerHtml = siteData.serviceType + "<span class='gray_text'> ( </span>" + siteData.contractAuthor + "<span class='gray_text'> )</span>";

        if (siteData.enabled == false)
        {
            td = new HtmlGenericControl("td");
            tr.Controls.Add(td);
            td.Attributes["class"] = "wide_table";
            td.Attributes["style"] = "text-align:right; padding: 1px; padding-left: 4px; padding-right: 4px; white-space: nowrap;";

            HtmlGenericControl spanSiteStatus = new HtmlGenericControl("span");
            td.Controls.Add(spanSiteStatus);
            spanSiteStatus.Attributes["class"] = "object_status";
            spanSiteStatus.InnerText = "site disabled";
        }

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table";
        td.Attributes["style"] = "width:23px; padding: 1px;";

        HtmlGenericControl divDeleteSite = new HtmlGenericControl("div");
        td.Controls.Add(divDeleteSite);
        divDeleteSite.Attributes["class"] = "SubmitButtonSquareMini RedOrange";

        TButton buttonDeleteSite = new TButton();
        divDeleteSite.Controls.Add(buttonDeleteSite);
        buttonDeleteSite.Args.Add(siteData);
        buttonDeleteSite.Text = "X";
        buttonDeleteSite.ToolTip = "Delete Site";
        buttonDeleteSite.ID = string.Format("DeleteSite_{0}", siteData.siteId);
        buttonDeleteSite.Click += new EventHandler(DeleteSite_Click);

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

            HtmlGenericControl divSiteBlockItem;

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
            divSiteBlockHeader.InnerText = "access defaults";

            divSiteBlockItem = new HtmlGenericControl("div");
            divSiteBlock.Controls.Add(divSiteBlockItem);
            divSiteBlockItem.Attributes["class"] = "site_block_item";

            HtmlGenericControl spanImplicitUsersCaption = new HtmlGenericControl("span");
            divSiteBlockItem.Controls.Add(spanImplicitUsersCaption);
            spanImplicitUsersCaption.Attributes["class"] = "name";
            spanImplicitUsersCaption.InnerHtml = "Implicit users&nbsp;&nbsp;";

            HtmlGenericControl spanImplicitUsersStatus = new HtmlGenericControl("span");
            divSiteBlockItem.Controls.Add(spanImplicitUsersStatus);
            if (siteData.implicitUsersAllowed)
            {
                spanImplicitUsersStatus.Attributes["class"] = "enabled_status";
                spanImplicitUsersStatus.InnerText = "allowed";
            }
            else
            {
                spanImplicitUsersStatus.Attributes["class"] = "disabled_status";
                spanImplicitUsersStatus.InnerText = "denied";
            }            

            if (siteData.guestSupported)
            {
                divSiteBlock = new HtmlGenericControl("div");
                divSiteBody.Controls.Add(divSiteBlock);
                divSiteBlock.Attributes["class"] = "site_block";

                divSiteBlockHeader = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockHeader);
                divSiteBlockHeader.Attributes["class"] = "site_block_header";
                divSiteBlockHeader.InnerText = "guest status";

                divSiteBlockItem = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockItem);
                divSiteBlockItem.Attributes["class"] = "site_block_item";

                HtmlGenericControl spanGuestCaption = new HtmlGenericControl("span");
                divSiteBlockItem.Controls.Add(spanGuestCaption);
                spanGuestCaption.Attributes["class"] = "name";
                spanGuestCaption.InnerHtml = "Guest&nbsp;&nbsp;";

                HtmlGenericControl spanGuestStatus = new HtmlGenericControl("span");
                divSiteBlockItem.Controls.Add(spanGuestStatus);
                if (siteData.guestAllowed)
                {
                    spanGuestStatus.Attributes["class"] = "enabled_status";
                    spanGuestStatus.InnerText = "allowed";
                }
                else
                {
                    spanGuestStatus.Attributes["class"] = "disabled_status";
                    spanGuestStatus.InnerText = "denied";
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
                    spanGuestPage.InnerText = string.Format("{0}/guest.aspx?site={1}", SoftnetRegistry.settings_getManagementSystemUrl(), siteData.siteKey);

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
            }

            List<UserData> explicitUsers = new List<UserData>();
            List<UserData> implicitUsers = new List<UserData>();
            foreach (UserData userData in m_dataset.users)
            {
                if (userData.kind == 2)
                {
                    if (m_dataset.siteUsers.FindIndex(x => x.siteId == siteData.siteId && x.userId == userData.userId) >= 0)
                        explicitUsers.Add(userData);
                    else if (siteData.implicitUsersAllowed && userData.dedicated == false)
                        implicitUsers.Add(userData);
                }
                else if (userData.kind == 3)
                {
                    if (m_dataset.siteUsers.FindIndex(x => x.siteId == siteData.siteId && x.userId == userData.userId) >= 0)
                        explicitUsers.Add(userData);
                    else if (siteData.implicitUsersAllowed && userData.dedicated == false)
                        implicitUsers.Add(userData);
                }
                else if (userData.kind == 1)
                {
                    if (m_dataset.siteUsers.FindIndex(x => x.siteId == siteData.siteId && x.userId == userData.userId) >= 0)
                        explicitUsers.Add(userData);
                    else if (siteData.implicitUsersAllowed)
                        implicitUsers.Add(userData);
                } // userData.kind == 4
                else if (siteData.guestSupported && siteData.guestAllowed)
                {
                    explicitUsers.Add(userData);
                }
            }

            if (explicitUsers.Count > 0)
            {
                divSiteBlock = new HtmlGenericControl("div");
                divSiteBody.Controls.Add(divSiteBlock);
                divSiteBlock.Attributes["class"] = "site_block";

                divSiteBlockHeader = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockHeader);
                divSiteBlockHeader.Attributes["class"] = "site_block_header left";
                divSiteBlockHeader.InnerText = "explicit users";

                foreach (UserData userData in explicitUsers)
                {
                    divSiteBlockItem = new HtmlGenericControl("div");
                    divSiteBlock.Controls.Add(divSiteBlockItem);
                    divSiteBlockItem.Attributes["class"] = "site_block_item";

                    if (userData.kind == 2)
                    {
                        Label labelUserName = new Label();
                        divSiteBlockItem.Controls.Add(labelUserName);
                        labelUserName.Text = userData.name;
                        labelUserName.CssClass = "user";

                        if (userData.enabled == false)
                            labelUserName.CssClass = labelUserName.CssClass + " disabled_status";

                        if (userData.dedicated)
                            labelUserName.CssClass = labelUserName.CssClass + " user_dedicated";
                    }
                    else if (userData.kind == 3)
                    {
                        Label labelUserName = new Label();
                        divSiteBlockItem.Controls.Add(labelUserName);
                        labelUserName.Text = userData.name;
                        labelUserName.CssClass = "user";

                        if (userData.dedicated)
                            labelUserName.CssClass += " user_dedicated";

                        if (userData.contactData.status >= 2)
                        {
                            if (userData.contactData.status == 2)
                            {
                                labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                                labelUserName.ToolTip = "The user should be deleted.";
                            }
                            else
                            {
                                labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                                labelUserName.ToolTip = "The user should be deleted.";
                            }
                        }
                        else if (userData.enabled == false)
                        {
                            labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                            labelUserName.ToolTip = "The user is disabled.";
                        }

                        Label labelContactName = new Label();
                        divSiteBlockItem.Controls.Add(labelContactName);
                        labelContactName.Text = "&nbsp;&nbsp;<span class='gray_text'>&#60;</span>" + ContactDisplayName.Adjust(userData.contactData.contactName) + "<span class='gray_text'>&#62;</span>";
                        labelContactName.CssClass = "contact_in_status_0";

                        if (userData.contactData.status == 1)
                        {
                            labelContactName.CssClass = "contact_in_status_1";
                            labelContactName.ToolTip = "Your partner deleted the contact.";
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
                        Label labelUserName = new Label();
                        divSiteBlockItem.Controls.Add(labelUserName);
                        labelUserName.Text = userData.name;
                        labelUserName.CssClass = "user user_owner";

                        if (userData.enabled == false)
                            labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                    }
                    else // allowedUser.userData.kind == 4
                    {
                        Label labelUserName = new Label();
                        divSiteBlockItem.Controls.Add(labelUserName);
                        labelUserName.Text = userData.name;
                        labelUserName.CssClass = "user user_guest";

                        if (userData.enabled == false)
                            labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                    }
                }
            }

            if (implicitUsers.Count > 0)
            {
                divSiteBlock = new HtmlGenericControl("div");
                divSiteBody.Controls.Add(divSiteBlock);
                divSiteBlock.Attributes["class"] = "site_block";

                divSiteBlockHeader = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockHeader);
                divSiteBlockHeader.Attributes["class"] = "site_block_header left";
                divSiteBlockHeader.InnerText = "implicit users";

                foreach (UserData userData in implicitUsers)
                {
                    divSiteBlockItem = new HtmlGenericControl("div");
                    divSiteBlock.Controls.Add(divSiteBlockItem);
                    divSiteBlockItem.Attributes["class"] = "site_block_item";

                    if (userData.kind == 2)
                    {
                        Label labelUserName = new Label();
                        divSiteBlockItem.Controls.Add(labelUserName);
                        labelUserName.Text = userData.name;
                        labelUserName.CssClass = "user";

                        if (userData.dedicated)
                            labelUserName.CssClass = labelUserName.CssClass + " user_dedicated";

                        if (userData.enabled == false)
                            labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                    }
                    else if (userData.kind == 3)
                    {
                        Label labelUserName = new Label();
                        divSiteBlockItem.Controls.Add(labelUserName);
                        labelUserName.Text = userData.name;
                        labelUserName.CssClass = "user";

                        if (userData.dedicated)
                            labelUserName.CssClass += " user_dedicated";

                        if (userData.contactData.status >= 2)
                        {
                            if (userData.contactData.status == 2)
                            {
                                labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                                labelUserName.ToolTip = "The user should be deleted.";
                            }
                            else
                            {
                                labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                                labelUserName.ToolTip = "The user should be deleted.";
                            }
                        }
                        else if (userData.enabled == false)
                        {
                            labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                            labelUserName.ToolTip = "The user is disabled.";
                        }

                        Label labelContactName = new Label();
                        divSiteBlockItem.Controls.Add(labelContactName);
                        labelContactName.Text = "&nbsp;&nbsp;<span class='gray_text'>&#60;</span>" + ContactDisplayName.Adjust(userData.contactData.contactName) + "<span class='gray_text'>&#62;</span>";
                        labelContactName.CssClass = "contact_in_status_0";

                        if (userData.contactData.status == 1)
                        {
                            labelContactName.CssClass = "contact_in_status_1";
                            labelContactName.ToolTip = "Your partner deleted the contact.";
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
                        Label labelUserName = new Label();
                        divSiteBlockItem.Controls.Add(labelUserName);
                        labelUserName.Text = userData.name;
                        labelUserName.CssClass = "user user_owner";

                        if (userData.enabled == false)
                            labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                    }
                }
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

    void drawSSite(SiteData siteData)
    {
        HtmlGenericControl divSiteFrame = new HtmlGenericControl("div");
        PH_SiteList.Controls.Add(divSiteFrame);
        divSiteFrame.Attributes["class"] = "site_frame";

        Button buttonScrollPosition = new Button();
        buttonScrollPosition.ID = string.Format("ScrollPosition_{0}", siteData.siteId);
        PH_SiteList.Controls.Add(buttonScrollPosition);
        buttonScrollPosition.Attributes["style"] = "width:0px; height:0px; border-width:0px; padding: 0px; margin:0px";

        HtmlGenericControl divSiteDescription = new HtmlGenericControl("div");
        divSiteFrame.Controls.Add(divSiteDescription);
        divSiteDescription.Attributes["style"] = "padding-top: 0px; padding-bottom: 7px; padding-left: 25px; padding-right: 30px;";

        HtmlGenericControl span = new HtmlGenericControl("span");
        divSiteDescription.Controls.Add(span);
        span.Attributes["class"] = "site_name";
        span.InnerText = siteData.description;

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
        buttonConfig.Click += new EventHandler(ConfigSSite_Click);

        if (string.IsNullOrEmpty(siteData.serviceType) == false)
        {
            td = new HtmlGenericControl("td");
            tr.Controls.Add(td);
            td.Attributes["class"] = "wide_table";
            td.Attributes["style"] = "width: auto; text-align: left; padding: 1px; padding-left: 20px;";

            HtmlGenericControl spanSiteType = new HtmlGenericControl("span");
            td.Controls.Add(spanSiteType);
            spanSiteType.Attributes["class"] = "site_type";
            spanSiteType.InnerHtml = siteData.serviceType + "<span class='gray_text'> ( </span>" + siteData.contractAuthor + "<span class='gray_text'> )</span>";
        }

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table";
        td.Attributes["style"] = "text-align:right; padding: 1px; padding-left: 4px; padding-right: 4px; white-space: nowrap;";

        HtmlGenericControl spanSiteStatus = new HtmlGenericControl("span");
        td.Controls.Add(spanSiteStatus);
        spanSiteStatus.Attributes["class"] = "object_status";
        spanSiteStatus.InnerText = "site blank";

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table";
        td.Attributes["style"] = "width:23px; padding: 1px;";

        HtmlGenericControl divDeleteSite = new HtmlGenericControl("div");
        td.Controls.Add(divDeleteSite);
        divDeleteSite.Attributes["class"] = "SubmitButtonSquareMini RedOrange";

        TButton buttonDeleteSite = new TButton();
        divDeleteSite.Controls.Add(buttonDeleteSite);
        buttonDeleteSite.Args.Add(siteData);
        buttonDeleteSite.Text = "X";
        buttonDeleteSite.ToolTip = "Delete Site";
        buttonDeleteSite.ID = string.Format("DeleteSite_{0}", siteData.siteId);
        buttonDeleteSite.Click += new EventHandler(DeleteSite_Click);

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
            divSiteBlockHeader.InnerText = "service";

            HtmlGenericControl divSiteBlockBody = new HtmlGenericControl("div");
            divSiteBlock.Controls.Add(divSiteBlockBody);
            divSiteBlockBody.Attributes["class"] = "site_block_body";

            HtmlGenericControl divSiteBlockItem = new HtmlGenericControl("div");
            divSiteBlockBody.Controls.Add(divSiteBlockItem);
            divSiteBlockItem.Attributes["class"] = "site_block_item";

            table = new HtmlGenericControl("table");
            divSiteBlockItem.Controls.Add(table);
            table.Attributes["class"] = "wide_table";
            tr = new HtmlGenericControl("tr");
            table.Controls.Add(tr);
            HtmlGenericControl tdLeft = new HtmlGenericControl("td");
            tr.Controls.Add(tdLeft);
            tdLeft.Attributes["class"] = "wide_table";
            tdLeft.Attributes["style"] = "text-align:left";
            HtmlGenericControl tdRight = new HtmlGenericControl("td");
            tr.Controls.Add(tdRight);
            tdRight.Attributes["class"] = "wide_table";
            tdRight.Attributes["style"] = "text-align:right";

            List<ServiceData> services = m_dataset.services.FindAll(x => x.siteId == siteData.siteId);
            if (services.Count != 1)
                throw new FormatException();
            ServiceData serviceData = services[0];

            HtmlGenericControl spanHostname = new HtmlGenericControl("span");
            tdLeft.Controls.Add(spanHostname);
            spanHostname.Attributes["class"] = "name";
            spanHostname.InnerHtml = serviceData.hostname + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

            if (string.IsNullOrEmpty(serviceData.serviceType) == false)
            {
                if (serviceData.serviceType.Equals(siteData.serviceType) == false || serviceData.contractAuthor.Equals(siteData.contractAuthor) == false)
                {
                    HtmlGenericControl spanServiceType = new HtmlGenericControl("span");
                    tdLeft.Controls.Add(spanServiceType);
                    spanServiceType.Attributes["class"] = "service_type";
                    spanServiceType.InnerHtml = serviceData.serviceType + "<span class='gray_text'> ( </span>" + serviceData.contractAuthor + "<span class='gray_text'> ) </span>";

                    HtmlGenericControl spanServiceStatus = new HtmlGenericControl("span");
                    tdRight.Controls.Add(spanServiceStatus);
                    spanServiceStatus.Attributes["class"] = "object_status";
                    spanServiceStatus.InnerText = "service type conflict";
                }
                else if (serviceData.ssHash.Equals(siteData.ssHash) == false)
                {
                    HtmlGenericControl spanServiceStatus = new HtmlGenericControl("span");
                    tdRight.Controls.Add(spanServiceStatus);
                    spanServiceStatus.Attributes["class"] = "object_status";
                    spanServiceStatus.InnerText = "site structure mismatch";
                }

                if (string.IsNullOrEmpty(serviceData.version) == false)
                {
                    HtmlGenericControl spanVersion = new HtmlGenericControl("span");
                    tdLeft.Controls.Add(spanVersion);
                    spanVersion.Attributes["class"] = "service_type";
                    spanVersion.InnerHtml = "<span class='legend'>ver. </span>" + serviceData.version;
                }
            }
            else
            {
                HtmlGenericControl spanServiceStatus = new HtmlGenericControl("span");
                tdRight.Controls.Add(spanServiceStatus);
                spanServiceStatus.Attributes["class"] = "object_status";
                spanServiceStatus.InnerText = "service type undefined";
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

    void drawMSite(SiteData siteData)
    {
        HtmlGenericControl divSiteFrame = new HtmlGenericControl("div");
        PH_SiteList.Controls.Add(divSiteFrame);
        divSiteFrame.Attributes["class"] = "site_frame";

        Button buttonScrollPosition = new Button();
        buttonScrollPosition.ID = string.Format("ScrollPosition_{0}", siteData.siteId);
        PH_SiteList.Controls.Add(buttonScrollPosition);
        buttonScrollPosition.Attributes["style"] = "width:0px; height:0px; border-width:0px; padding: 0px; margin:0px";

        HtmlGenericControl divSiteDescription = new HtmlGenericControl("div");
        divSiteFrame.Controls.Add(divSiteDescription);
        divSiteDescription.Attributes["style"] = "padding-top: 0px; padding-bottom: 7px; padding-left: 25px; padding-right: 30px;";

        HtmlGenericControl span = new HtmlGenericControl("span");
        divSiteDescription.Controls.Add(span);
        span.Attributes["class"] = "site_name";
        span.InnerText = siteData.description;

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
        buttonConfig.Click += new EventHandler(ConfigMSite_Click);

        if (string.IsNullOrEmpty(siteData.serviceType) == false)
        {
            td = new HtmlGenericControl("td");
            tr.Controls.Add(td);
            td.Attributes["class"] = "wide_table";
            td.Attributes["style"] = "width: auto; text-align: left; padding: 1px; padding-left: 20px;";

            HtmlGenericControl spanSiteType = new HtmlGenericControl("span");
            td.Controls.Add(spanSiteType);
            spanSiteType.Attributes["class"] = "site_type";
            spanSiteType.InnerHtml = siteData.serviceType + "<span class='gray_text'> ( </span>" + siteData.contractAuthor + "<span class='gray_text'> )</span>";
        }

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table";
        td.Attributes["style"] = "text-align:right; padding: 1px; padding-left: 4px; padding-right: 4px; white-space: nowrap;";

        HtmlGenericControl spanSiteStatus = new HtmlGenericControl("span");
        td.Controls.Add(spanSiteStatus);
        spanSiteStatus.Attributes["class"] = "object_status";
        spanSiteStatus.InnerText = "site blank";

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table";
        td.Attributes["style"] = "width:23px; padding: 1px;";

        HtmlGenericControl divDeleteSite = new HtmlGenericControl("div");
        td.Controls.Add(divDeleteSite);
        divDeleteSite.Attributes["class"] = "SubmitButtonSquareMini RedOrange";

        TButton buttonDeleteSite = new TButton();
        divDeleteSite.Controls.Add(buttonDeleteSite);
        buttonDeleteSite.Args.Add(siteData);
        buttonDeleteSite.Text = "X";
        buttonDeleteSite.ToolTip = "Delete Site";
        buttonDeleteSite.ID = string.Format("DeleteSite_{0}", siteData.siteId);
        buttonDeleteSite.Click += new EventHandler(DeleteSite_Click);

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

            HtmlGenericControl divSiteBlockBody = new HtmlGenericControl("div");
            divSiteBlock.Controls.Add(divSiteBlockBody);
            divSiteBlockBody.Attributes["class"] = "site_block_body";

            HtmlGenericControl divSiteBlockItem, tdLeft, tdRight;

            List<ServiceData> services = m_dataset.services.FindAll(x => x.siteId == siteData.siteId);
            foreach (ServiceData serviceData in services)
            {
                divSiteBlockItem = new HtmlGenericControl("div");
                divSiteBlockBody.Controls.Add(divSiteBlockItem);
                divSiteBlockItem.Attributes["class"] = "site_block_item underline";

                table = new HtmlGenericControl("table");
                divSiteBlockItem.Controls.Add(table);
                table.Attributes["class"] = "wide_table";
                tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);
                tdLeft = new HtmlGenericControl("td");
                tr.Controls.Add(tdLeft);
                tdLeft.Attributes["class"] = "wide_table";
                tdLeft.Attributes["style"] = "text-align:left";
                tdRight = new HtmlGenericControl("td");
                tr.Controls.Add(tdRight);
                tdRight.Attributes["class"] = "wide_table";
                tdRight.Attributes["style"] = "text-align:right";

                table = new HtmlGenericControl("table");
                tdLeft.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                HtmlGenericControl tdHostname = new HtmlGenericControl("td");
                tr.Controls.Add(tdHostname);
                tdHostname.Attributes["class"] = "auto_table";

                HtmlGenericControl tdServiceType = new HtmlGenericControl("td");
                tr.Controls.Add(tdServiceType);
                tdServiceType.Attributes["class"] = "auto_table";
                tdServiceType.Attributes["style"] = "padding-left: 15px;";

                HtmlGenericControl spanHostname = new HtmlGenericControl("span");
                tdHostname.Controls.Add(spanHostname);
                spanHostname.Attributes["class"] = "name";
                spanHostname.InnerHtml = serviceData.hostname;

                if (string.IsNullOrEmpty(serviceData.serviceType))
                {
                    HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                    tdRight.Controls.Add(spanStatus);
                    spanStatus.Attributes["class"] = "object_status";
                    spanStatus.InnerText = "service type undefined";
                }
                else if (serviceData.serviceType.Equals(siteData.serviceType) && serviceData.contractAuthor.Equals(siteData.contractAuthor))
                {
                    if (string.IsNullOrEmpty(serviceData.version) == false)
                    {
                        HtmlGenericControl spanServiceType = new HtmlGenericControl("span");
                        tdServiceType.Controls.Add(spanServiceType);
                        spanServiceType.Attributes["class"] = "service_type";
                        spanServiceType.InnerHtml += "<span class='legend'>ver.</span> " + serviceData.version;
                    }

                    if (serviceData.ssHash.Equals(siteData.ssHash) == false)
                    {
                        HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                        tdRight.Controls.Add(spanStatus);
                        spanStatus.Attributes["class"] = "object_status";
                        spanStatus.InnerText = "site structure mismatch";
                    }
                }
                else
                {
                    HtmlGenericControl spanServiceType = new HtmlGenericControl("span");
                    tdServiceType.Controls.Add(spanServiceType);
                    spanServiceType.Attributes["class"] = "service_type";
                    spanServiceType.InnerHtml = siteData.serviceType + "<span class='gray_text'> ( </span>" + siteData.contractAuthor + "<span class='gray_text'> )</span>";

                    if (string.IsNullOrEmpty(serviceData.version) == false)
                        spanServiceType.InnerHtml += "<span class='legend'> ver.</span> " + serviceData.version;

                    HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                    tdRight.Controls.Add(spanStatus);
                    spanStatus.Attributes["class"] = "object_status";
                    spanStatus.InnerText = "service type conflict";
                }

                if (serviceData.enabled == false)
                {
                    HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                    tdRight.Controls.Add(spanStatus);
                    spanStatus.Attributes["class"] = "object_status";
                    if (tdRight.Controls.Count == 0)
                        spanStatus.InnerText = "disabled";
                    else
                        spanStatus.InnerText = "; disabled";
                }
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

    protected void CancelDeletingDomain_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/domains/default.aspx?mode=1");
    }

    protected void DeleteDomain_Click(object sender, EventArgs e)
    {
        try
        {
            SoftnetTracker.deleteDomain(m_dataset.domainId);
            Response.Redirect("~/domains/default.aspx?mode=1");
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void NewSite_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getNextUrl(string.Format("~/domains/newsite.aspx?did={0}", m_dataset.domainId), string.Format("~/domains/domain.aspx?did={0}", m_dataset.domainId)));
    }

    protected void NewUser_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getNextUrl(string.Format("~/domains/newuser.aspx?did={0}", m_dataset.domainId), string.Format("~/domains/domain.aspx?did={0}", m_dataset.domainId)));
    }

    protected void Refresh_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/domain.aspx?did={0}", m_dataset.domainId)));
    }

    protected void ButtonDeleteUser_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        UserData userData = (UserData)button.Args[0];

        if (userData.kind == 1 || userData.kind == 4)
            return;

        try
        {
            SoftnetTracker.deleteUser(m_dataset.domainId, userData.userId);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/domain.aspx?did={0}", m_dataset.domainId)));            
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }
    
    protected void ButtonEditUser_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        UserData userData = (UserData)button.Args[0];
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/domain.aspx?did={0}&uid={1}", m_dataset.domainId, userData.userId)));
    }
    
    protected void ButtonSaveUser_Click(object sender, EventArgs e)
    {        
        TButton button = (TButton)sender;
        UserData userData = (UserData)button.Args[0];
        if (userData.kind == Constants.UserKind.Private || userData.kind == Constants.UserKind.Contact)
        {
            TextBox textboxName = (TextBox)button.Args[1];
            CheckBox checkboxDisabled = (CheckBox)button.Args[2];
            CheckBox checkboxDedicated = (CheckBox)button.Args[3];

            string userName = textboxName.Text;
            if (string.IsNullOrWhiteSpace(userName))
            {
                L_UserListError.Visible = true;
                L_UserListError.Text = "An empty username is not allowed.";
                return;
            }

            if (userName.Length > Constants.MaxLength.user_name)
            {
                L_UserListError.Visible = true;
                L_UserListError.Text = string.Format("The username must not contain more than {0} characters.", Constants.MaxLength.user_name);
                return;
            }

            if (Regex.IsMatch(userName, @"[^\x20-\x7F]", RegexOptions.None))
            {
                L_UserListError.Visible = true;
                L_UserListError.Text = "Valid symbols in the username are latin letters, numbers, spaces and the following characters: $ . * + # @ % & = ' : ^ ( ) [ ] - / !";
                return;
            }

            if (Regex.IsMatch(userName, @"^[a-zA-Z]", RegexOptions.None) == false)
            {
                L_UserListError.Visible = true;
                L_UserListError.Text = "The leading character must be a latin letter.";
                return;
            }

            if (Regex.IsMatch(userName, @"[\s]$", RegexOptions.None))
            {
                L_UserListError.Visible = true;
                L_UserListError.Text = "The trailing space is illegal.";
                return;
            }

            if (Regex.IsMatch(userName, @"[^\w\s.$*+#@%&=':\^()\[\]\-/!]", RegexOptions.None))
            {
                L_UserListError.Visible = true;
                L_UserListError.Text = "Valid symbols in the username are latin letters, numbers, spaces and the following characters: $ . * + # @ % & = ' : ^ ( ) [ ] - / !";
                return;
            }

            if (Regex.IsMatch(userName, @"[\s]{2,}", RegexOptions.None))
            {
                L_UserListError.Visible = true;
                L_UserListError.Text = "Two or more consecutive spaces are not allowed.";
                return;
            }

            if (m_dataset.users.Find(x => x.userId != userData.userId && x.name.Equals(userName, StringComparison.OrdinalIgnoreCase)) != null)
            {
                L_UserListError.Visible = true;
                L_UserListError.Text = string.Format("The user '{0}' has already exist in the domain.", textboxName.Text); 
                return;
            }

            try
            {
                SoftnetTracker.updateUser(m_dataset.domainId, userData.userId, userName, checkboxDisabled.Checked == false, checkboxDedicated.Checked);
                Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/domain.aspx?did={0}", m_dataset.domainId)));
            }
            catch (SoftnetException ex)
            {
                ExceptionHandler.exec(this, ex);
            }
        }
        else
        {
            CheckBox checkboxDisabled = (CheckBox)button.Args[1];
            try
            {
                SoftnetTracker.updateUser(m_dataset.domainId, userData.kind, userData.userId, checkboxDisabled.Checked == false);
                Response.Redirect(string.Format("~/domains/domain.aspx?did={0}", m_dataset.domainId));
            }
            catch (SoftnetException ex)
            {
                ExceptionHandler.exec(this, ex);
            }
        }
    }

    protected void ButtonViewUser_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/domain.aspx?did={0}", m_dataset.domainId)));
    }

    protected void ViewUserList_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/domain.aspx?did={0}", m_dataset.domainId)));
    }

    void LoadUsers(long selectedUserId)
    {
        int ownerIndex = m_dataset.users.FindIndex(x => x.kind == 1);
        if (ownerIndex >= 0)
        {
            UserData ownerData = m_dataset.users[ownerIndex];
            m_dataset.users.RemoveAt(ownerIndex);
            m_dataset.users.Insert(0, ownerData);
        }

        int guestIndex = m_dataset.users.FindIndex(x => x.kind == 4);
        if (guestIndex >= 0)
        {
            UserData guestData = m_dataset.users[guestIndex];
            m_dataset.users.RemoveAt(guestIndex);
            m_dataset.users.Add(guestData);
        }

        for (int i = Tb_Users.Rows.Count - 1; i > 0; i--)
            Tb_Users.Rows.RemoveAt(i);

        int lastIndex = m_dataset.users.Count - 1;
        for (int index = 0; index <= lastIndex; index++)
        {
            TableRow tableRow = new TableRow();
            Tb_Users.Rows.Add(tableRow);

            UserData userData = m_dataset.users[index];
            if (userData.kind == 2)
            {
                if (userData.userId != selectedUserId)
                {
                    TableCell td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    HtmlGenericControl divEditUser = new HtmlGenericControl("div");
                    td.Controls.Add(divEditUser);
                    divEditUser.Attributes["class"] = "SubmitButtonMini Blue";

                    TButton buttonEditUser = new TButton();
                    divEditUser.Controls.Add(buttonEditUser);
                    buttonEditUser.Args.Add(userData);
                    buttonEditUser.Text = ">>";
                    buttonEditUser.ID = "B_EditUser_" + userData.userId.ToString();
                    buttonEditUser.Click += new EventHandler(ButtonEditUser_Click);

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    Label labelUserName = new Label();
                    td.Controls.Add(labelUserName);
                    labelUserName.Text = userData.name;
                    labelUserName.CssClass = "user";

                    if (userData.dedicated)
                        labelUserName.CssClass = labelUserName.CssClass + " user_dedicated";

                    if (userData.enabled == false)
                        labelUserName.CssClass = labelUserName.CssClass + " disabled_status";

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3;";

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    if (userData.enabled == false)
                    {
                        Label labelDisabled = new Label();
                        td.Controls.Add(labelDisabled);
                        labelDisabled.Text = "Disabled";
                        labelDisabled.CssClass = "disabled_status";

                        if (userData.dedicated)
                        {
                            Label labelDedicated = new Label();
                            td.Controls.Add(labelDedicated);
                            labelDedicated.Text = "&nbsp;&nbsp;&nbsp;<span class='user_dedicated'>Dedicated</span>";
                        }
                    }
                    else if (userData.dedicated)
                    {
                        Label labelDedicated = new Label();
                        td.Controls.Add(labelDedicated);
                        labelDedicated.Text = "<span class='user_dedicated'>Dedicated</span>";
                    }

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    ClientCount clientCount = m_dataset.clientCounts.Find(x => x.userId == userData.userId);
                    if (clientCount != null)
                    {
                        Label labelClients = new Label();
                        td.Controls.Add(labelClients);
                        labelClients.Attributes["style"] = "color:#696969;";
                        labelClients.Text = clientCount.count.ToString();
                    }
                    
                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3;";
                }
                else
                {
                    TableCell td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    HtmlGenericControl divSaveUser = new HtmlGenericControl("div");
                    td.Controls.Add(divSaveUser);
                    divSaveUser.Attributes["class"] = "SubmitButtonMini Green";

                    TButton buttonSaveUser = new TButton();
                    divSaveUser.Controls.Add(buttonSaveUser);
                    buttonSaveUser.Args.Add(userData);
                    buttonSaveUser.Text = "save";
                    buttonSaveUser.Attributes["style"] = "padding-left:5px; padding-right: 5px;";
                    buttonSaveUser.ID = "B_SaveUser_" + userData.userId.ToString();
                    buttonSaveUser.Click += new EventHandler(ButtonSaveUser_Click);

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    TextBox textboxName = new TextBox();
                    td.Controls.Add(textboxName);
                    buttonSaveUser.Args.Add(textboxName);
                    textboxName.ID = "TB_Name_" + userData.userId.ToString();
                    if(this.IsPostBack == false)
                        textboxName.Text = userData.name;
                    textboxName.Attributes["style"] = "border: 1px solid #7FBA00; outline:none; width: 205px; margin: 0px; padding: 2px;"; 
                    textboxName.CssClass = "user";
                    textboxName.Attributes["autocomplete"] = "off";
                    if (userData.dedicated)
                        textboxName.CssClass = textboxName.CssClass + " user_dedicated";
                    if (userData.enabled == false)
                        textboxName.CssClass = textboxName.CssClass + " disabled_status";

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3;";

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    CheckBox checkboxDisabled = new CheckBox();
                    td.Controls.Add(checkboxDisabled);
                    buttonSaveUser.Args.Add(checkboxDisabled);
                    checkboxDisabled.Text = "Disabled";
                    if (this.IsPostBack == false)
                        checkboxDisabled.Checked = userData.enabled == false;
                    checkboxDisabled.ID = "CB_Disabled_" + userData.userId.ToString();

                    Label labelDelimeter = new Label();
                    labelDelimeter.Text = "&nbsp;&nbsp;&nbsp;";
                    td.Controls.Add(labelDelimeter);

                    CheckBox checkboxDedicated = new CheckBox();
                    td.Controls.Add(checkboxDedicated);
                    buttonSaveUser.Args.Add(checkboxDedicated);
                    checkboxDedicated.Text = "Dedicated";
                    if (this.IsPostBack == false)
                        checkboxDedicated.Checked = userData.dedicated;
                    checkboxDedicated.ID = "CB_Dedicated_" + userData.userId.ToString();

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    ClientCount clientCount = m_dataset.clientCounts.Find(x => x.userId == userData.userId);
                    if (clientCount != null)
                    {
                        Label labelClients = new Label();
                        td.Controls.Add(labelClients);
                        labelClients.Attributes["style"] = "color:#696969;";
                        labelClients.Text = clientCount.count.ToString();
                    }

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    HtmlGenericControl divDelete = new HtmlGenericControl("div");
                    td.Controls.Add(divDelete);
                    divDelete.Attributes["class"] = "SubmitButtonSquareMini RedOrange";
                    TButton buttonDelete = new TButton();
                    buttonDelete.Text = "X";
                    buttonDelete.ID = "BT_DeleteUser_" + userData.userId.ToString();
                    buttonDelete.Args.Add(userData);
                    buttonDelete.Click += new EventHandler(ButtonDeleteUser_Click);
                    divDelete.Controls.Add(buttonDelete);
                }
            }            
            else if (userData.kind == 3)
            {
                userData.contactData = m_dataset.contacts.Find(x => x.contactId == userData.contactId);
                if (userData.contactData == null)
                    userData.contactData = ContactData.nullContact;

                if (userData.contactData.status == 0 || userData.contactData.status == 1)
                {
                    if (userData.userId != selectedUserId)
                    {
                        TableCell td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                        HtmlGenericControl divEditUser = new HtmlGenericControl("div");
                        td.Controls.Add(divEditUser);
                        divEditUser.Attributes["class"] = "SubmitButtonMini Blue";

                        TButton buttonEditUser = new TButton();
                        divEditUser.Controls.Add(buttonEditUser);
                        buttonEditUser.Args.Add(userData);
                        buttonEditUser.Text = ">>";
                        buttonEditUser.ID = "B_EditUser_" + userData.userId.ToString();
                        buttonEditUser.Click += new EventHandler(ButtonEditUser_Click);

                        td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                        Label labelUserName = new Label();
                        td.Controls.Add(labelUserName);
                        labelUserName.Text = userData.name;
                        labelUserName.CssClass = "user";

                        if (userData.dedicated)
                            labelUserName.CssClass = labelUserName.CssClass + " user_dedicated";

                        if (userData.enabled == false)
                            labelUserName.CssClass = labelUserName.CssClass + " disabled_status";

                        td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                        Label labelContactName = new Label();
                        td.Controls.Add(labelContactName);
                        labelContactName.Text = userData.contactData.contactName;
                        labelContactName.CssClass = "contact_in_status_0";

                        if (userData.contactData.status == 1)
                        {
                            labelContactName.Text += " <span class='gray_text'>(</span>*<span class='gray_text'>)</span>";
                            labelContactName.CssClass = "contact_in_status_1";
                            labelContactName.ToolTip = "Your partner deleted the contact.";
                        }                        

                        td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                        if (userData.enabled == false)
                        {
                            Label labelDisabled = new Label();
                            td.Controls.Add(labelDisabled);
                            labelDisabled.Text = "Disabled";
                            labelDisabled.CssClass = "disabled_status";

                            if (userData.dedicated)
                            {
                                Label labelDedicated = new Label();
                                td.Controls.Add(labelDedicated);
                                labelDedicated.Text = "&nbsp;&nbsp;&nbsp;<span class='user_dedicated'>Dedicated</span>";
                            }
                        }
                        else if (userData.dedicated)
                        {
                            Label labelDedicated = new Label();
                            td.Controls.Add(labelDedicated);
                            labelDedicated.Text = "<span class='user_dedicated'>Dedicated</span>";
                        }

                        td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                        ClientCount clientCount = m_dataset.clientCounts.Find(x => x.userId == userData.userId);
                        if (clientCount != null)
                        {
                            Label labelClients = new Label();
                            td.Controls.Add(labelClients);
                            labelClients.Attributes["style"] = "color:#696969;";
                            labelClients.Text = clientCount.count.ToString();
                        }
                       
                        td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3;";
                    }
                    else
                    {
                        TableCell td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                        HtmlGenericControl divSaveUser = new HtmlGenericControl("div");
                        td.Controls.Add(divSaveUser);
                        divSaveUser.Attributes["class"] = "SubmitButtonMini Green";

                        TButton buttonSaveUser = new TButton();
                        divSaveUser.Controls.Add(buttonSaveUser);
                        buttonSaveUser.Args.Add(userData);
                        buttonSaveUser.Text = "save";
                        buttonSaveUser.Attributes["style"] = "padding-left:5px; padding-right: 5px;";
                        buttonSaveUser.ID = "B_SaveUser_" + userData.userId.ToString();
                        buttonSaveUser.Click += new EventHandler(ButtonSaveUser_Click);

                        td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                        TextBox textboxName = new TextBox();
                        td.Controls.Add(textboxName);
                        buttonSaveUser.Args.Add(textboxName);
                        textboxName.ID = "TB_Name_" + userData.userId.ToString();
                        textboxName.Text = userData.name;
                        textboxName.Attributes["style"] = "border: 1px solid #7FBA00; outline:none; width: 205px; margin: 0px; padding: 2px;";
                        textboxName.Attributes["autocomplete"] = "off";

                        td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                        Label labelContactName = new Label();
                        td.Controls.Add(labelContactName);
                        labelContactName.Text = userData.contactData.contactName;
                        labelContactName.CssClass = "contact_in_status_0";

                        if (userData.contactData.status == 1)
                        {
                            labelContactName.Text += " <span class='gray_text'>(</span>*<span class='gray_text'>)</span>";
                            labelContactName.CssClass = "contact_in_status_1";
                            labelContactName.ToolTip = "Your partner deleted the contact.";
                        }                        

                        td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                        CheckBox checkboxDisabled = new CheckBox();
                        td.Controls.Add(checkboxDisabled);
                        buttonSaveUser.Args.Add(checkboxDisabled);
                        checkboxDisabled.Text = "Disabled";
                        checkboxDisabled.Checked = userData.enabled == false;
                        checkboxDisabled.ID = "CB_Disabled_" + userData.userId.ToString();

                        Label labelDelimeter = new Label();
                        labelDelimeter.Text = "&nbsp;&nbsp;&nbsp;";
                        td.Controls.Add(labelDelimeter);

                        CheckBox checkboxDedicated = new CheckBox();
                        td.Controls.Add(checkboxDedicated);
                        buttonSaveUser.Args.Add(checkboxDedicated);
                        checkboxDedicated.Text = "Dedicated";
                        checkboxDedicated.Checked = userData.dedicated;
                        checkboxDedicated.ID = "CB_Dedicated_" + userData.userId.ToString();

                        td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                        ClientCount clientCount = m_dataset.clientCounts.Find(x => x.userId == userData.userId);
                        if (clientCount != null)
                        {
                            Label labelClients = new Label();
                            td.Controls.Add(labelClients);
                            labelClients.Attributes["style"] = "color:#696969;";
                            labelClients.Text = clientCount.count.ToString();
                        }

                        td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                        HtmlGenericControl divDelete = new HtmlGenericControl("div");
                        td.Controls.Add(divDelete);
                        divDelete.Attributes["class"] = "SubmitButtonSquareMini RedOrange";
                        TButton buttonDelete = new TButton();
                        buttonDelete.Text = "X";
                        buttonDelete.ID = "BT_DeleteUser_" + userData.userId.ToString();
                        buttonDelete.Args.Add(userData);
                        buttonDelete.Click += new EventHandler(ButtonDeleteUser_Click);
                        divDelete.Controls.Add(buttonDelete);
                    }
                }
                else
                {
                    if (userData.userId != selectedUserId)
                    {
                        TableCell td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                        HtmlGenericControl divEditUser = new HtmlGenericControl("div");
                        td.Controls.Add(divEditUser);
                        divEditUser.Attributes["class"] = "SubmitButtonMini Blue";

                        TButton buttonEditUser = new TButton();
                        divEditUser.Controls.Add(buttonEditUser);
                        buttonEditUser.Args.Add(userData);
                        buttonEditUser.Text = ">>";
                        buttonEditUser.ID = "B_EditUser_" + userData.userId.ToString();
                        buttonEditUser.Click += new EventHandler(ButtonEditUser_Click);

                        td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                        Label labelUserName = new Label();
                        td.Controls.Add(labelUserName);
                        labelUserName.Text = userData.name;
                        labelUserName.CssClass = "disabled_status";

                        td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                        Label labelContactName = new Label();
                        td.Controls.Add(labelContactName);

                        if (userData.contactData.status == 2)
                        {
                            labelContactName.Text = userData.contactData.contactName + " <span class='gray_text'>(</span>**<span class='gray_text'>)</span>";
                            labelContactName.CssClass = "contact_in_status_2";
                            labelContactName.ToolTip = "The contact is no longer usable as your partner has been deleted from the network.";
                            labelUserName.ToolTip = "The user should be deleted.";
                        }
                        else // userData.contactData.status == 3
                        {
                            labelContactName.Text = "Unknown <span class='gray_text'>(</span>***<span class='gray_text'>)</span>";
                            labelContactName.CssClass = "contact_in_status_3";
                            labelContactName.ToolTip = "The contact is unknown.";
                            labelUserName.ToolTip = "The user should be deleted.";
                        }

                        td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                        if (userData.enabled == false)
                        {
                            Label labelDisabled = new Label();
                            td.Controls.Add(labelDisabled);
                            labelDisabled.Text = "Disabled";
                            labelDisabled.CssClass = "disabled_status";

                            if (userData.dedicated)
                            {
                                Label labelDedicated = new Label();
                                td.Controls.Add(labelDedicated);
                                labelDedicated.Text = "&nbsp;&nbsp;&nbsp;<span class='user_dedicated'>Dedicated</span>";
                            }
                        }
                        else if (userData.dedicated)
                        {
                            Label labelDedicated = new Label();
                            td.Controls.Add(labelDedicated);
                            labelDedicated.Text = "<span class='user_dedicated'>Dedicated</span>";
                        }
                        
                        td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                        ClientCount clientCount = m_dataset.clientCounts.Find(x => x.userId == userData.userId);
                        if (clientCount != null)
                        {
                            Label labelClients = new Label();
                            td.Controls.Add(labelClients);
                            labelClients.Attributes["style"] = "color:#696969;";
                            labelClients.Text = clientCount.count.ToString();
                        }

                        td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3;";
                    }
                    else
                    {
                        TableCell td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                        HtmlGenericControl divViewUser = new HtmlGenericControl("div");
                        td.Controls.Add(divViewUser);
                        divViewUser.Attributes["class"] = "SubmitButtonMini Blue";

                        TButton buttonViewUser = new TButton();
                        divViewUser.Controls.Add(buttonViewUser);
                        buttonViewUser.Args.Add(userData);
                        buttonViewUser.Text = "<<";
                        buttonViewUser.ID = "B_ViewUser_" + userData.userId.ToString();
                        buttonViewUser.Click += new EventHandler(ButtonViewUser_Click);

                        td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                        Label labelUserName = new Label();
                        td.Controls.Add(labelUserName);
                        labelUserName.Text = userData.name;
                        labelUserName.CssClass = "disabled_status";

                        td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                        Label labelContactName = new Label();
                        td.Controls.Add(labelContactName);

                        if (userData.contactData.status == 2)
                        {
                            labelContactName.Text = userData.contactData.contactName + " <span class='gray_text'>(</span>**<span class='gray_text'>)</span>";
                            labelContactName.CssClass = "contact_in_status_2";
                            labelContactName.ToolTip = "The contact is no longer usable as your partner has been deleted from the network.";
                            labelUserName.ToolTip = "Your partner has been deleted from the network.";
                        }
                        else // userData.contactData.status == 3
                        {
                            labelContactName.Text = "Unknown <span class='gray_text'>(</span>***<span class='gray_text'>)</span>";
                            labelContactName.CssClass = "contact_in_status_3";
                            labelContactName.ToolTip = "The contact is unknown.";
                            labelUserName.ToolTip = "Your partner has been deleted from the network.";
                        }

                        td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                        if (userData.enabled == false)
                        {
                            Label labelDisabled = new Label();
                            td.Controls.Add(labelDisabled);
                            labelDisabled.Text = "Disabled";
                            labelDisabled.CssClass = "disabled_status";

                            if (userData.dedicated)
                            {
                                Label labelDedicated = new Label();
                                td.Controls.Add(labelDedicated);
                                labelDedicated.Text = "&nbsp;&nbsp;&nbsp;<span class='user_dedicated'>Dedicated</span>";
                            }
                        }
                        else if (userData.dedicated)
                        {
                            Label labelDedicated = new Label();
                            td.Controls.Add(labelDedicated);
                            labelDedicated.Text = "<span class='user_dedicated'>Dedicated</span>";
                        }                    

                        td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                        ClientCount clientCount = m_dataset.clientCounts.Find(x => x.userId == userData.userId);
                        if (clientCount != null)
                        {
                            Label labelClients = new Label();
                            td.Controls.Add(labelClients);
                            labelClients.Attributes["style"] = "color:#696969;";
                            labelClients.Text = clientCount.count.ToString();
                        }

                        td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3;";

                        HtmlGenericControl divDelete = new HtmlGenericControl("div");
                        td.Controls.Add(divDelete);
                        divDelete.Attributes["class"] = "SubmitButtonSquareMini RedOrange";
                        TButton buttonDelete = new TButton();
                        buttonDelete.Text = "X";
                        buttonDelete.ID = "BT_DeleteUser_" + userData.userId.ToString();
                        buttonDelete.Args.Add(userData);
                        buttonDelete.Click += new EventHandler(ButtonDeleteUser_Click);
                        divDelete.Controls.Add(buttonDelete);
                    }
                }
            }
            else if (userData.kind == 1)
            {
                if (userData.userId != selectedUserId)
                {
                    TableCell td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    HtmlGenericControl divEditUser = new HtmlGenericControl("div");
                    td.Controls.Add(divEditUser);
                    divEditUser.Attributes["class"] = "SubmitButtonMini Blue";

                    TButton buttonEditUser = new TButton();
                    divEditUser.Controls.Add(buttonEditUser);
                    buttonEditUser.Args.Add(userData);
                    buttonEditUser.Text = ">>";
                    buttonEditUser.ID = "B_EditUser_" + userData.userId.ToString();
                    buttonEditUser.Click += new EventHandler(ButtonEditUser_Click);

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    Label labelUserName = new Label();
                    td.Controls.Add(labelUserName);
                    labelUserName.Text = userData.name;
                    labelUserName.CssClass = "user_owner";

                    if (userData.enabled == false)
                        labelUserName.CssClass = labelUserName.CssClass + " disabled_status";

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3;";

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    if (userData.enabled == false)
                    {
                        Label labelDisabled = new Label();
                        td.Controls.Add(labelDisabled);
                        labelDisabled.Text = "Disabled";
                        labelDisabled.CssClass = "disabled_status";
                    }

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    ClientCount clientCount = m_dataset.clientCounts.Find(x => x.userId == userData.userId);
                    if (clientCount != null)
                    {
                        Label labelClients = new Label();
                        td.Controls.Add(labelClients);
                        labelClients.Attributes["style"] = "color:#696969;";
                        labelClients.Text = clientCount.count.ToString();
                    }

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3;";
                }
                else
                {
                    TableCell td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    HtmlGenericControl divSaveUser = new HtmlGenericControl("div");
                    td.Controls.Add(divSaveUser);
                    divSaveUser.Attributes["class"] = "SubmitButtonMini Green";

                    TButton buttonSaveUser = new TButton();
                    divSaveUser.Controls.Add(buttonSaveUser);
                    buttonSaveUser.Args.Add(userData);
                    buttonSaveUser.Text = "save";
                    buttonSaveUser.Attributes["style"] = "padding-left:5px; padding-right: 5px;";
                    buttonSaveUser.ID = "B_SaveUser_" + userData.userId.ToString();
                    buttonSaveUser.Click += new EventHandler(ButtonSaveUser_Click);

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    Label labelUserName = new Label();
                    td.Controls.Add(labelUserName);
                    labelUserName.Text = userData.name;
                    labelUserName.CssClass = "user_owner";

                    if (userData.enabled == false)
                        labelUserName.CssClass = labelUserName.CssClass + " disabled_status";

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3;";

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    CheckBox checkboxDisabled = new CheckBox();
                    td.Controls.Add(checkboxDisabled);
                    buttonSaveUser.Args.Add(checkboxDisabled);
                    checkboxDisabled.Text = "Disabled";
                    checkboxDisabled.Checked = userData.enabled == false;
                    checkboxDisabled.ID = "CB_Disabled_" + userData.userId.ToString();

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    ClientCount clientCount = m_dataset.clientCounts.Find(x => x.userId == userData.userId);
                    if (clientCount != null)
                    {
                        Label labelClients = new Label();
                        td.Controls.Add(labelClients);
                        labelClients.Attributes["style"] = "color:#696969;";
                        labelClients.Text = clientCount.count.ToString();
                    }

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3;";
                }
            }
            else // if (userData.kind == 4)
            {
                if (userData.userId != selectedUserId)
                {
                    TableCell td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    HtmlGenericControl divEditUser = new HtmlGenericControl("div");
                    td.Controls.Add(divEditUser);
                    divEditUser.Attributes["class"] = "SubmitButtonMini Blue";

                    TButton buttonEditUser = new TButton();
                    divEditUser.Controls.Add(buttonEditUser);
                    buttonEditUser.Args.Add(userData);
                    buttonEditUser.Text = ">>";
                    buttonEditUser.ID = "B_EditUser_" + userData.userId.ToString();
                    buttonEditUser.Click += new EventHandler(ButtonEditUser_Click);

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    Label labelUserName = new Label();
                    td.Controls.Add(labelUserName);
                    labelUserName.Text = userData.name;
                    labelUserName.CssClass = "user_guest";

                    if (userData.enabled == false)
                        labelUserName.CssClass = labelUserName.CssClass + " disabled_status";

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3;";

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    if (userData.enabled == false)
                    {
                        Label labelDisabled = new Label();
                        td.Controls.Add(labelDisabled);
                        labelDisabled.Text = "Disabled";
                        labelDisabled.CssClass = "disabled_status";
                    }

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    ClientCount clientCount = m_dataset.clientCounts.Find(x => x.userId == userData.userId);
                    if (clientCount != null)
                    {
                        Label labelClients = new Label();
                        td.Controls.Add(labelClients);
                        labelClients.Attributes["style"] = "color:#696969;";
                        labelClients.Text = clientCount.count.ToString();
                    }

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3;";
                }
                else
                {
                    TableCell td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    HtmlGenericControl divSaveUser = new HtmlGenericControl("div");
                    td.Controls.Add(divSaveUser);
                    divSaveUser.Attributes["class"] = "SubmitButtonMini Green";

                    TButton buttonSaveUser = new TButton();
                    divSaveUser.Controls.Add(buttonSaveUser);
                    buttonSaveUser.Args.Add(userData);
                    buttonSaveUser.Text = "save";
                    buttonSaveUser.Attributes["style"] = "padding-left:5px; padding-right: 5px;";
                    buttonSaveUser.ID = "B_SaveUser_" + userData.userId.ToString();
                    buttonSaveUser.Click += new EventHandler(ButtonSaveUser_Click);

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    Label labelUserName = new Label();
                    td.Controls.Add(labelUserName);
                    labelUserName.Text = userData.name;
                    labelUserName.CssClass = "user_guest";

                    if (userData.enabled == false)
                        labelUserName.CssClass = labelUserName.CssClass + " disabled_status";

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3;";

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    CheckBox checkboxDisabled = new CheckBox();
                    td.Controls.Add(checkboxDisabled);
                    buttonSaveUser.Args.Add(checkboxDisabled);
                    checkboxDisabled.Text = "Disabled";
                    checkboxDisabled.Checked = userData.enabled == false;
                    checkboxDisabled.ID = "CB_Disabled_" + userData.userId.ToString();

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    ClientCount clientCount = m_dataset.clientCounts.Find(x => x.userId == userData.userId);
                    if (clientCount != null)
                    {
                        Label labelClients = new Label();
                        td.Controls.Add(labelClients);
                        labelClients.Attributes["style"] = "color:#696969;";
                        labelClients.Text = clientCount.count.ToString();
                    }

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3;";
                }
            }
        }

        if (m_dataset.users.Find(x => x.kind == 3 && x.contactData.status == 1) != null)
        {
            P_UserListHints.Visible = true;
            Label labelDescrition = new Label();
            P_UserListHints.Controls.Add(labelDescrition);
            labelDescrition.Text = "<span class='gray_text'>(</span><span class='contact_in_status_1'>*</span><span class='gray_text'>)</span>" +
                " - Your partner deleted the contact. However it can be usable again if your partner restore it.";
            labelDescrition.Attributes["style"] = "display:block; padding-top: 5px;";
        }

        if (m_dataset.users.Find(x => x.kind == 3 && x.contactData.status == 2) != null)
        {
            P_UserListHints.Visible = true;
            Label labelDescrition = new Label();
            P_UserListHints.Controls.Add(labelDescrition);
            labelDescrition.Text = "<span class='gray_text'>(</span><span class='contact_in_status_2'>**</span><span class='gray_text'>)</span>" +
                " - The contact is no longer usable as your partner has been deleted from the network.";
            labelDescrition.Attributes["style"] = "display:block; padding-top: 5px;";
        }

        if (m_dataset.users.Find(x => x.kind == 3 && x.contactData.status == 3) != null)
        {
            P_UserListHints.Visible = true;
            Label labelDescrition = new Label();
            P_UserListHints.Controls.Add(labelDescrition);
            labelDescrition.Text = "<span class='gray_text'>(</span><span class='contact_in_status_3'>***</span><span class='gray_text'>)</span>" +
                " - The contact is unknown and the user should be deleted.";
            labelDescrition.Attributes["style"] = "display:block; padding-top: 5px;";            
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