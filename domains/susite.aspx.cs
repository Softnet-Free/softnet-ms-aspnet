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

public partial class susite : System.Web.UI.Page
{
    SiteConfigDataset m_siteDataset;
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

            m_siteDataset = new SiteConfigDataset();
            SoftnetRegistry.GetUSiteConfigDataset(this.Context.User.Identity.Name, siteId, m_siteDataset);

            if (m_siteDataset.siteData.siteKind != 1 || m_siteDataset.services.Count != 1 || m_siteDataset.siteData.structured == false || m_siteDataset.siteData.rolesSupported)
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

    protected void NewUser_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getNextUrl(string.Format("~/domains/newuser.aspx?did={0}", m_siteDataset.domainId), string.Format("~/domains/susite.aspx?sid={0}", m_siteData.siteId)));
    }

    protected void Refresh_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/susite.aspx?sid={0}", m_siteData.siteId)));
    }

    protected void Clients_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/suclients.aspx?sid={0}", m_siteData.siteId)));
    }

    protected void DeleteSite_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getNextUrl(string.Format("~/domains/deletesite.aspx?sid={0}", m_siteData.siteId), string.Format("~/domains/susite.aspx?sid={0}", m_siteData.siteId)));
    }

    protected void EditSiteDescription_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/susite.aspx?sid={0}&sp=1", m_siteData.siteId)));
    }

    protected void SaveSiteDescription_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        TextBox textboxSiteDescription = (TextBox)button.Args[0];

        string siteDescription = textboxSiteDescription.Text;
        if (siteDescription.Length > Constants.MaxLength.site_description)
        {
            HtmlGenericControl span = new HtmlGenericControl("span");
            P_SiteEdit.Controls.Add(span);
            span.Attributes["style"] = "display: block; margin-top: 10px; color: #FF0000";
            span.InnerText = string.Format("The site description must not contain more than {0} characters.", Constants.MaxLength.site_description);
            return;
        }

        try
        {
            SoftnetRegistry.ChangeSiteDescription(m_siteData.siteId, siteDescription);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/susite.aspx?sid={0}", m_siteData.siteId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void EditSiteEnabledStatus_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/susite.aspx?sid={0}&sp=2", m_siteData.siteId)));
    }

    protected void EnableSite_Click(object sender, EventArgs e)
    {
        try
        {
            SoftnetTracker.enableSite(m_siteData.siteId);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/susite.aspx?sid={0}", m_siteData.siteId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void DisableSite_Click(object sender, EventArgs e)
    {
        try
        {
            SoftnetTracker.disableSite(m_siteData.siteId);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/susite.aspx?sid={0}", m_siteData.siteId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void EditServiceAccount_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/susite.aspx?sid={0}&srp=1", m_siteData.siteId)));
    }

    protected void EditServiceHostname_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/susite.aspx?sid={0}&srp=2", m_siteData.siteId)));
    }

    protected void ShowApplyStructure_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/susite.aspx?sid={0}&srp=3", m_siteData.siteId)));
    }

    protected void ShowPingSettings_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/susite.aspx?sid={0}&srp=4", m_siteData.siteId)));
    }   

    protected void GeneratePassword_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        Panel panelServiceEdit = (Panel)button.Args[0];
        HtmlGenericControl tableGeneratePassword = (HtmlGenericControl)button.Args[1];
        tableGeneratePassword.Visible = false;

        try
        {
            int passwordLength = SoftnetRegistry.settings_getServicePasswordLength();
            string password = Randomizer.generatePassword(passwordLength);
            byte[] salt = Randomizer.generateOctetString(16);
            byte[] saltedPassword = PasswordHash.Compute(salt, password);

            SoftnetTracker.setServicePassword(m_siteData.siteId, m_serviceData.serviceId, Convert.ToBase64String(salt), Convert.ToBase64String(saltedPassword));

            HtmlGenericControl tablePassword = new HtmlGenericControl("table");
            panelServiceEdit.Controls.Add(tablePassword);
            tablePassword.Attributes["style"] = "margin-top: 8px;";
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
            panelServiceEdit.Controls.Add(divOkButton);
            divOkButton.Attributes["class"] = "SubmitButtonMini Blue";
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

    protected void SaveHostname_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        TextBox textboxHostname = (TextBox)button.Args[0];
        Panel panelWorkArea = P_ServiceEdit;

        string hostName = textboxHostname.Text.Trim();
        if (hostName.Length != textboxHostname.Text.Length)
        {
            HtmlGenericControl span = new HtmlGenericControl("span");
            panelWorkArea.Controls.Add(span);
            span.Attributes["style"] = "display: block; margin-top: 10px; color: #FF0000";
            span.InnerText = "The hostname must not contain leading or trailing whitespace characters.";
            return;
        }

        if (hostName.Length == 0)
        {
            HtmlGenericControl span = new HtmlGenericControl("span");
            panelWorkArea.Controls.Add(span);
            span.Attributes["style"] = "display: block; margin-top: 10px; color: #FF0000";
            span.InnerText = "The hostname must not be empty.";
            return;
        }

        if (hostName.Length > Constants.MaxLength.host_name)
        {
            HtmlGenericControl span = new HtmlGenericControl("span");
            panelWorkArea.Controls.Add(span);
            span.Attributes["style"] = "display: block; margin-top: 10px; color: #FF0000";
            span.InnerText = string.Format("The hostname must not contain more than {0} characters.", Constants.MaxLength.host_name);
            return;
        }

        if (Regex.IsMatch(hostName, @"[^\x20-\x7F]", RegexOptions.None))
        {
            HtmlGenericControl span = new HtmlGenericControl("span");
            panelWorkArea.Controls.Add(span);
            span.Attributes["style"] = "display: block; margin-top: 10px; color: #FF0000";
            span.InnerText = "Valid symbols in the hostname are latin letters, numbers, spaces and the following characters: $ . * + # @ % & = ' : ^ ( ) [ ] - / !";
            return;
        }

        if (Regex.IsMatch(hostName, @"[^\w\s.$*+#@%&=':\^()\[\]\-/!]", RegexOptions.None))
        {
            HtmlGenericControl span = new HtmlGenericControl("span");
            panelWorkArea.Controls.Add(span);
            span.Attributes["style"] = "display: block; margin-top: 10px; color: #FF0000";
            span.InnerText = "Valid symbols in the hostname are latin letters, numbers, spaces and the following characters: $ . * + # @ % & = ' : ^ ( ) [ ] - / !";
            return;
        }

        if (Regex.IsMatch(hostName, @"[\s]{2,}", RegexOptions.None))
        {
            HtmlGenericControl span = new HtmlGenericControl("span");
            panelWorkArea.Controls.Add(span);
            span.Attributes["style"] = "display: block; margin-top: 10px; color: #FF0000";
            span.InnerText = "Two or more consecutive spaces are not allowed";
            return;
        }

        try
        {
            SoftnetTracker.changeHostname(m_siteData.siteId, m_serviceData.serviceId, hostName);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/susite.aspx?sid={0}", m_siteData.siteId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void ApplyStructure_Click(object sender, EventArgs e)
    {
        try
        {
            SoftnetTracker.applyStructure(m_siteData.siteId, m_serviceData.serviceId);
            int siteAccessType = SoftnetRegistry.GetSiteAccessType(m_siteData.siteId);
            if (siteAccessType == 1)
                Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/srsite.aspx?sid={0}", m_siteData.siteId)));
            else if (siteAccessType == 0)
                Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/susite.aspx?sid={0}", m_siteData.siteId)));
            else
                Response.Redirect(string.Format("~/domains/domain.aspx?did={0}", m_siteDataset.domainId));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void SetPingPeriod_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        Panel panelServiceEdit = (Panel)button.Args[0];
        TextBox textboxPingPeriod = (TextBox)button.Args[1];

        try
        {
            int pingPeriod;
            if (int.TryParse(textboxPingPeriod.Text, out pingPeriod) == false)
                throw new ArgumentException("Invalid format.");

            if (pingPeriod != 0 && (pingPeriod < 10 || pingPeriod > 300))
                throw new ArgumentException("The value of ping period must be in the range from 10 seconds to 300 seconds or 0.");

            SoftnetTracker.setServicePingPeriod(m_siteData.siteId, m_serviceData.serviceId, pingPeriod);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/susite.aspx?sid={0}", m_siteData.siteId)));
        }
        catch (ArgumentException ex)
        {
            HtmlGenericControl spanMessage = new HtmlGenericControl("span");
            panelServiceEdit.Controls.Add(spanMessage);
            spanMessage.Attributes["class"] = "error_message";
            spanMessage.Attributes["style"] = "display: block; margin-top: 10px;";
            spanMessage.InnerText = "Error: " + ex.Message;
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }    

    protected void AllowGuest_Click(object sender, EventArgs e)
    {
        try
        {
            SoftnetTracker.allowGuest(m_siteData.siteId);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/susite.aspx?sid={0}", m_siteData.siteId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void DenyGuest_Click(object sender, EventArgs e)
    {
        try
        {
            SoftnetTracker.denyGuest(m_siteData.siteId);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/susite.aspx?sid={0}", m_siteData.siteId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void AllowImplicitUsers_Click(object sender, EventArgs e)
    {
        try
        {
            SoftnetTracker.allowImplicitUsers(m_siteData.siteId);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/susite.aspx?sid={0}", m_siteData.siteId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }    
    }

    protected void DenyImplicitUsers_Click(object sender, EventArgs e)
    {
        try
        {
            SoftnetTracker.denyImplicitUsers(m_siteData.siteId);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/susite.aspx?sid={0}", m_siteData.siteId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }        
    }    

    protected void AddUser_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        UserData userData = (UserData)button.Args[0];
        try
        {
            SoftnetTracker.addSiteUser(m_siteData.siteId, userData.userId);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/susite.aspx?sid={0}", m_siteData.siteId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void RemoveUser_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        UserData userData = (UserData)button.Args[0];
        try
        {
            SoftnetTracker.removeSiteUser(m_siteData.siteId, userData.userId);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/susite.aspx?sid={0}", m_siteData.siteId)));
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

        P_SiteEdit.Visible = false;

        int siteProp = 0;
        if (int.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("sp"), out siteProp))
        {
            if (siteProp == 1)
            {
                P_EditSiteDescription.CssClass = "SubmitButtonSquare Selected Blue";
                P_SiteEdit.Visible = true;

                HtmlGenericControl table = new HtmlGenericControl("table");
                P_SiteEdit.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                HtmlGenericControl tableRaw = new HtmlGenericControl("tr");
                table.Controls.Add(tableRaw);

                HtmlGenericControl td1 = new HtmlGenericControl("td");
                tableRaw.Controls.Add(td1);
                td1.Attributes["class"] = "auto_table";

                TextBox textboxSiteDescription = new TextBox();
                td1.Controls.Add(textboxSiteDescription);
                textboxSiteDescription.Attributes["style"] = "border: 1px solid #7FBA00; outline:none; width:300px; margin: 0px; padding: 3px;";
                textboxSiteDescription.Text = m_siteData.description;
                textboxSiteDescription.ID = "TB_SiteDescription";
                textboxSiteDescription.Attributes["autocomplete"] = "off";

                HtmlGenericControl td2 = new HtmlGenericControl("td");
                tableRaw.Controls.Add(td2);
                td2.Attributes["class"] = "auto_table";
                td2.Attributes["style"] = "padding-left: 15px;";

                HtmlGenericControl divSaveSiteDescription = new HtmlGenericControl("div");
                td2.Controls.Add(divSaveSiteDescription);
                divSaveSiteDescription.Attributes["class"] = "SubmitButtonMini Green";

                TButton buttonSaveSiteDescription = new TButton();
                divSaveSiteDescription.Controls.Add(buttonSaveSiteDescription);
                buttonSaveSiteDescription.Args.Add(textboxSiteDescription);
                buttonSaveSiteDescription.Text = "save";
                buttonSaveSiteDescription.ID = "B_SaveSiteDescription";
                buttonSaveSiteDescription.Click += new EventHandler(SaveSiteDescription_Click);
            }
            else if (siteProp == 2)
            {
                P_EditSiteEnabledStatus.CssClass = "SubmitButtonSquare Selected Blue";
                P_SiteEdit.Visible = true;

                HtmlGenericControl tableSiteStatusEdit = new HtmlGenericControl("table");
                P_SiteEdit.Controls.Add(tableSiteStatusEdit);
                tableSiteStatusEdit.Attributes["class"] = "auto_table";
                HtmlGenericControl tableRow = new HtmlGenericControl("tr");
                tableSiteStatusEdit.Controls.Add(tableRow);

                HtmlGenericControl td1 = new HtmlGenericControl("td");
                tableRow.Controls.Add(td1);
                td1.Attributes["class"] = "auto_table";

                HtmlGenericControl spanStatusCaption = new HtmlGenericControl("span");
                td1.Controls.Add(spanStatusCaption);
                spanStatusCaption.Attributes["class"] = "caption";
                spanStatusCaption.InnerText = "Site";

                HtmlGenericControl td2 = new HtmlGenericControl("td");
                tableRow.Controls.Add(td2);
                td2.Attributes["class"] = "auto_table";
                td2.Attributes["style"] = "padding-left: 4px";

                HtmlGenericControl td3 = new HtmlGenericControl("td");
                tableRow.Controls.Add(td3);
                td3.Attributes["class"] = "auto_table";
                td3.Attributes["style"] = "padding-left: 15px";

                if (m_siteData.enabled)
                {
                    HtmlGenericControl spanSiteStatus = new HtmlGenericControl("span");
                    td2.Controls.Add(spanSiteStatus);
                    spanSiteStatus.Attributes["class"] = "enabled_status";
                    spanSiteStatus.InnerText = "enabled";

                    HtmlGenericControl divDisableSite = new HtmlGenericControl("div");
                    td3.Controls.Add(divDisableSite);
                    divDisableSite.Attributes["class"] = "SubmitButtonMini RedOrange";

                    Button buttonDisableSite = new Button();
                    divDisableSite.Controls.Add(buttonDisableSite);
                    buttonDisableSite.ID = "B_DisableSite";
                    buttonDisableSite.Text = "disable";
                    buttonDisableSite.Click += new EventHandler(DisableSite_Click);
                }
                else
                {
                    HtmlGenericControl spanSiteStatus = new HtmlGenericControl("span");
                    td2.Controls.Add(spanSiteStatus);
                    spanSiteStatus.Attributes["class"] = "disabled_status";
                    spanSiteStatus.InnerText = "disabled";

                    HtmlGenericControl divEnableSite = new HtmlGenericControl("div");
                    td3.Controls.Add(divEnableSite);
                    divEnableSite.Attributes["class"] = "SubmitButtonMini Green";

                    Button buttonEnableSite = new Button();
                    divEnableSite.Controls.Add(buttonEnableSite);
                    buttonEnableSite.ID = "B_EnableSite";
                    buttonEnableSite.Text = "enable";
                    buttonEnableSite.Click += new EventHandler(EnableSite_Click);
                }
            }
        }

        if (string.IsNullOrEmpty(m_serviceData.serviceType) == false)
            P_ShowApplyStructureButton.Visible = true;
        else
            P_ShowApplyStructureButton.Visible = false;

        P_ServiceEdit.Visible = false;

        int srvProp = 0;
        if (int.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("srp"), out srvProp))
        {
            if (srvProp == 1)
            {
                P_EditServiceAccount.CssClass = "SubmitButtonSquare Selected Blue";
                P_ServiceEdit.Visible = true;

                HtmlGenericControl spanAccount = new HtmlGenericControl("span");
                P_ServiceEdit.Controls.Add(spanAccount);
                spanAccount.Attributes["class"] = "client_uri";
                spanAccount.Attributes["style"] = "display:block;";
                spanAccount.InnerText = string.Format("softnet-srv://{0}@{1}", m_serviceData.serviceUid.ToString(), SoftnetRegistry.settings_getServerAddress());

                HtmlGenericControl tableGeneratePassword = new HtmlGenericControl("table");
                P_ServiceEdit.Controls.Add(tableGeneratePassword);
                tableGeneratePassword.Attributes["class"] = "auto_table";
                tableGeneratePassword.Attributes["style"] = "margin-top: 12px;";
                HtmlGenericControl trGeneratePassword = new HtmlGenericControl("tr");
                tableGeneratePassword.Controls.Add(trGeneratePassword);
                HtmlGenericControl tdGeneratePassword = new HtmlGenericControl("td");
                trGeneratePassword.Controls.Add(tdGeneratePassword);
                tdGeneratePassword.Attributes["class"] = "auto_table";

                HtmlGenericControl divGeneratePassword = new HtmlGenericControl("div");
                tdGeneratePassword.Controls.Add(divGeneratePassword);
                divGeneratePassword.Attributes["class"] = "SubmitButtonMini Green";

                TButton buttonGeneratePassword = new TButton();
                divGeneratePassword.Controls.Add(buttonGeneratePassword);
                buttonGeneratePassword.Args.Add(P_ServiceEdit);
                buttonGeneratePassword.Args.Add(tableGeneratePassword);
                buttonGeneratePassword.Text = "generate passowrd";
                buttonGeneratePassword.ID = "B_GeneratePassword";
                buttonGeneratePassword.Click += new EventHandler(GeneratePassword_Click);
            }
            else if (srvProp == 2)
            {
                P_EditServiceHostname.CssClass = "SubmitButtonSquare Selected Blue";
                P_ServiceEdit.Visible = true;

                HtmlGenericControl table = new HtmlGenericControl("table");
                P_ServiceEdit.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                HtmlGenericControl tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                HtmlGenericControl td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";

                TextBox textboxHostname = new TextBox();
                td.Controls.Add(textboxHostname);
                textboxHostname.Attributes["style"] = "border: 1px solid #7FBA00; outline:none; width:300px; margin: 0px; padding: 3px;";
                textboxHostname.Text = m_serviceData.hostname;
                textboxHostname.ID = "TB_Hostname";
                textboxHostname.Attributes["autocomplete"] = "off";

                td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";
                td.Attributes["style"] = "padding-left: 15px;";

                HtmlGenericControl divSaveHostname = new HtmlGenericControl("div");
                td.Controls.Add(divSaveHostname);
                divSaveHostname.Attributes["class"] = "SubmitButtonMini Green";

                TButton buttonSaveHostname = new TButton();
                divSaveHostname.Controls.Add(buttonSaveHostname);
                buttonSaveHostname.Args.Add(textboxHostname);
                buttonSaveHostname.Text = "save";
                buttonSaveHostname.ID = "B_SaveHostname";
                buttonSaveHostname.Click += new EventHandler(SaveHostname_Click);
            }
            else if (srvProp == 3 && string.IsNullOrEmpty(m_serviceData.serviceType) == false)
            {
                P_ShowApplyStructureButton.CssClass = "SubmitButtonSquare Selected Blue";
                P_ServiceEdit.Visible = true;

                HtmlGenericControl table = new HtmlGenericControl("table");
                P_ServiceEdit.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                HtmlGenericControl tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                HtmlGenericControl td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";

                HtmlGenericControl divApplyStructure = new HtmlGenericControl("div");
                td.Controls.Add(divApplyStructure);
                divApplyStructure.Attributes["class"] = "SubmitButtonMini Green";

                TButton buttonApplyStructure = new TButton();
                divApplyStructure.Controls.Add(buttonApplyStructure);
                buttonApplyStructure.Text = "apply structure to the site";
                buttonApplyStructure.ID = "B_ApplyStructure";
                buttonApplyStructure.Click += new EventHandler(ApplyStructure_Click);
            }
            else if (srvProp == 4)
            {
                P_ShowPingPeriodButton.CssClass = "SubmitButtonSquare Selected Blue";
                P_ServiceEdit.Visible = true;

                HtmlGenericControl table = new HtmlGenericControl("table");
                P_ServiceEdit.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                HtmlGenericControl tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                HtmlGenericControl td = new HtmlGenericControl("td");
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
                textboxPingPeriod.Text = m_serviceData.pingPeriod.ToString();
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
                buttonSavePingPeriod.Args.Add(P_ServiceEdit);
                buttonSavePingPeriod.Args.Add(textboxPingPeriod);
                buttonSavePingPeriod.Text = "save";
                buttonSavePingPeriod.ID = "B_SavePingPeriod";
                buttonSavePingPeriod.Click += new EventHandler(SetPingPeriod_Click);

                span = new HtmlGenericControl("span");
                P_ServiceEdit.Controls.Add(span);
                span.Attributes["style"] = "display: block; margin-top: 10px; color: #3C6C80";
                span.InnerHtml =
                    "The minimum value is 10 seconds and the maximum is 300 seconds.<br/>" +
                    "The default value is 0 which sets the ping period to the endpoint's local value.";
            }
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

        if (m_siteData.implicitUsersAllowed)
        {
            L_ImplicitUserStatus.CssClass = "enabled_status";
            L_ImplicitUserStatus.Text = "allowed";

            Button buttonDenyImplicitUsers = new Button();
            buttonDenyImplicitUsers.Text = "deny";
            buttonDenyImplicitUsers.ID = "B_DenyImplicitUsers";
            buttonDenyImplicitUsers.Click += DenyImplicitUsers_Click;
            P_ChangeImplicitUserStatusButton.CssClass = "SubmitButtonMini RedOrange";
            P_ChangeImplicitUserStatusButton.Controls.Add(buttonDenyImplicitUsers);
        }
        else
        {
            L_ImplicitUserStatus.CssClass = "disabled_status";
            L_ImplicitUserStatus.Text = "denied";

            Button buttonAllowImplicitUsers = new Button();
            buttonAllowImplicitUsers.Text = "allow";
            buttonAllowImplicitUsers.ID = "B_AllowImplicitUsers";
            buttonAllowImplicitUsers.Click += AllowImplicitUsers_Click;
            P_ChangeImplicitUserStatusButton.CssClass = "SubmitButtonMini Green";
            P_ChangeImplicitUserStatusButton.Controls.Add(buttonAllowImplicitUsers);
        }

        if (m_siteData.guestSupported)
        {
            P_GuestStatus.Visible = true;
            if (m_siteData.guestAllowed)
            {
                L_GuestStatus.CssClass = "enabled_status";
                L_GuestStatus.Text = "allowed";

                P_ChangeGuestStatusButton.CssClass = "SubmitButtonMini RedOrange";
                Button buttonDenyGuest = new Button();
                buttonDenyGuest.Text = "deny";
                buttonDenyGuest.ID = "B_DenyGuest";
                buttonDenyGuest.Click += DenyGuest_Click;
                P_ChangeGuestStatusButton.Controls.Add(buttonDenyGuest);
            }
            else
            {
                L_GuestStatus.CssClass = "disabled_status";
                L_GuestStatus.Text = "denied";

                P_ChangeGuestStatusButton.CssClass = "SubmitButtonMini Green";
                Button buttonAllowGuest = new Button();
                buttonAllowGuest.Text = "allow";
                buttonAllowGuest.ID = "B_AllowGuest";
                buttonAllowGuest.Click += AllowGuest_Click;
                P_ChangeGuestStatusButton.Controls.Add(buttonAllowGuest);
            }

            if (m_siteData.guestAllowed)
            {
                P_GuestPage.Visible = true;
                L_GuestPage.Text = string.Format("{0}/guest.aspx?site={1}", SoftnetRegistry.settings_getManagementSystemUrl(), m_siteData.siteKey);

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

        List<UserData> explicitUsers = new List<UserData>();
        List<UserData> implicitUsers = new List<UserData>();
        foreach (UserData userData in m_siteDataset.users)
        {
            if (userData.kind == 2)
            {
                if (m_siteDataset.siteUsers.FindIndex(x => x == userData.userId) >= 0)
                    explicitUsers.Add(userData);
                else if (m_siteData.implicitUsersAllowed && userData.dedicated == false)
                    implicitUsers.Add(userData);
            }
            else if (userData.kind == 3)
            {
                userData.contactData = m_siteDataset.contacts.Find(x => x.contactId == userData.contactId);
                if (userData.contactData == null)
                    userData.contactData = ContactData.nullContact;

                if (m_siteDataset.siteUsers.FindIndex(x => x == userData.userId) >= 0)
                    explicitUsers.Add(userData);
                else if (m_siteData.implicitUsersAllowed && userData.dedicated == false)
                    implicitUsers.Add(userData);
            }
            else if (userData.kind == 1)
            {
                if (m_siteDataset.siteUsers.FindIndex(x => x == userData.userId) >= 0)
                    explicitUsers.Add(userData);
                else if (m_siteData.implicitUsersAllowed)
                    implicitUsers.Add(userData);
            } // userData.kind == 4
            else if (m_siteData.guestSupported && m_siteData.guestAllowed)
            {
                explicitUsers.Add(userData);
            }
        }        

        PH_ExplicitUsers.Controls.Clear();
        foreach (UserData userData in explicitUsers)
        {
            Panel panelUser = new Panel();
            PH_ExplicitUsers.Controls.Add(panelUser);
            panelUser.CssClass = "site_block_item";

            if (userData.kind == 2)
            {
                HtmlGenericControl table = new HtmlGenericControl("table");
                panelUser.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                HtmlGenericControl tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                HtmlGenericControl td1 = new HtmlGenericControl("td");
                tr.Controls.Add(td1);
                td1.Attributes["class"] = "auto_table";
                td1.Attributes["style"] = "width:50px;";

                HtmlGenericControl divRemoveUser = new HtmlGenericControl("div");
                td1.Controls.Add(divRemoveUser);
                divRemoveUser.Attributes["class"] = "SubmitButtonMini Blue";

                TButton buttonRemoveUser = new TButton();
                divRemoveUser.Controls.Add(buttonRemoveUser);
                buttonRemoveUser.Args.Add(userData);
                buttonRemoveUser.Text = "rem";
                buttonRemoveUser.ID = "B_RemoveUser_" + userData.userId.ToString();
                buttonRemoveUser.Click += new EventHandler(RemoveUser_Click);

                HtmlGenericControl td2 = new HtmlGenericControl("td");
                tr.Controls.Add(td2);
                td2.Attributes["class"] = "auto_table";
                td2.Attributes["style"] = "width: 10px;";

                HtmlGenericControl td3 = new HtmlGenericControl("td");
                tr.Controls.Add(td3);
                td3.Attributes["class"] = "auto_table";

                Label labelUserName = new Label();
                td3.Controls.Add(labelUserName);
                labelUserName.Text = userData.name;
                labelUserName.CssClass = "user";

                if (userData.dedicated)
                    labelUserName.CssClass = labelUserName.CssClass + " user_dedicated";

                if (userData.enabled == false)
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";                    
            }
            else if (userData.kind == 3)
            {
                HtmlGenericControl table = new HtmlGenericControl("table");
                panelUser.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                HtmlGenericControl tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                HtmlGenericControl td1 = new HtmlGenericControl("td");
                tr.Controls.Add(td1);
                td1.Attributes["class"] = "auto_table";
                td1.Attributes["style"] = "width:50px;";

                HtmlGenericControl divRemoveUser = new HtmlGenericControl("div");
                td1.Controls.Add(divRemoveUser);
                divRemoveUser.Attributes["class"] = "SubmitButtonMini Blue";

                TButton buttonRemoveUser = new TButton();
                divRemoveUser.Controls.Add(buttonRemoveUser);
                buttonRemoveUser.Args.Add(userData);
                buttonRemoveUser.Text = "rem";
                buttonRemoveUser.ID = "B_RemoveUser_" + userData.userId.ToString();
                buttonRemoveUser.Click += new EventHandler(RemoveUser_Click);

                HtmlGenericControl td2 = new HtmlGenericControl("td");
                tr.Controls.Add(td2);
                td2.Attributes["class"] = "auto_table";
                td2.Attributes["style"] = "width: 10px;";

                HtmlGenericControl td3 = new HtmlGenericControl("td");
                tr.Controls.Add(td3);
                td3.Attributes["class"] = "auto_table";

                Label labelUserName = new Label();
                td3.Controls.Add(labelUserName);
                labelUserName.Text = userData.name;
                labelUserName.CssClass = "user";

                if (userData.dedicated)
                    labelUserName.CssClass = labelUserName.CssClass + " user_dedicated";

                if (userData.contactData.status == 2)
                {
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                    labelUserName.ToolTip = "Your partner has been deleted from the network.";
                }
                else if (userData.contactData.status == 3)
                {
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                    labelUserName.ToolTip = "Your partner has been deleted from the network.";
                }
                else if (userData.enabled == false)
                {
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                    labelUserName.ToolTip = "The user is disabled.";
                }

                Label labelContactName = new Label();
                td3.Controls.Add(labelContactName);
                labelContactName.Text = "&nbsp;&nbsp;<span class='gray_text'>&#60;</span>" + ContactDisplayName.Adjust(userData.contactData.contactName) + "<span class='gray_text'>&#62;</span>";
                labelContactName.CssClass = "contact_in_status_0";

                if (userData.contactData.status == 1)
                {
                    labelContactName.CssClass = "contact_in_status_1";
                    labelContactName.ToolTip = "Your partner deleted the contact.";
                }
                else if(userData.contactData.status == 2)
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
                HtmlGenericControl table = new HtmlGenericControl("table");
                panelUser.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                HtmlGenericControl tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                HtmlGenericControl td1 = new HtmlGenericControl("td");
                tr.Controls.Add(td1);
                td1.Attributes["class"] = "auto_table";
                td1.Attributes["style"] = "width:50px;";

                HtmlGenericControl divRemoveUser = new HtmlGenericControl("div");
                td1.Controls.Add(divRemoveUser);
                divRemoveUser.Attributes["class"] = "SubmitButtonMini Blue";

                TButton buttonRemoveUser = new TButton();
                divRemoveUser.Controls.Add(buttonRemoveUser);
                buttonRemoveUser.Args.Add(userData);
                buttonRemoveUser.Text = "rem";
                buttonRemoveUser.ID = "B_RemoveUser_" + userData.userId.ToString();
                buttonRemoveUser.Click += new EventHandler(RemoveUser_Click);

                HtmlGenericControl td2 = new HtmlGenericControl("td");
                tr.Controls.Add(td2);
                td2.Attributes["class"] = "auto_table";
                td2.Attributes["style"] = "width: 10px;";

                HtmlGenericControl td3 = new HtmlGenericControl("td");
                tr.Controls.Add(td3);
                td3.Attributes["class"] = "auto_table";

                Label labelUserName = new Label();
                td3.Controls.Add(labelUserName);
                labelUserName.Text = userData.name;
                labelUserName.CssClass = "user user_owner";

                if (userData.enabled == false)
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
            }
            else // allowedUser.userData.kind == 4
            {
                HtmlGenericControl table = new HtmlGenericControl("table");
                panelUser.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                HtmlGenericControl tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                HtmlGenericControl td1 = new HtmlGenericControl("td");
                tr.Controls.Add(td1);
                td1.Attributes["class"] = "auto_table";
                td1.Attributes["style"] = "width: 50px; padding-top: 4px;";

                HtmlGenericControl td2 = new HtmlGenericControl("td");
                tr.Controls.Add(td2);
                td2.Attributes["class"] = "auto_table";
                td2.Attributes["style"] = "width: 10px;";

                HtmlGenericControl td3 = new HtmlGenericControl("td");
                tr.Controls.Add(td3);
                td3.Attributes["class"] = "auto_table";
                td3.Attributes["style"] = "padding-top: 4px;";

                Label labelUserName = new Label();
                td3.Controls.Add(labelUserName);
                labelUserName.Text = userData.name;
                labelUserName.CssClass = "user user_guest";

                if (userData.enabled == false)
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
            }
        }

        if (PH_ExplicitUsers.Controls.Count == 0)
            P_ExplicitUsers.Visible = false;

        PH_ImplicitUsers.Controls.Clear();
        foreach (UserData userData in implicitUsers)
        {
            Panel panelUser = new Panel();
            PH_ImplicitUsers.Controls.Add(panelUser);
            panelUser.CssClass = "site_block_item";

            if (userData.kind == 2)
            {
                HtmlGenericControl table = new HtmlGenericControl("table");
                panelUser.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                HtmlGenericControl tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                HtmlGenericControl td1 = new HtmlGenericControl("td");
                tr.Controls.Add(td1);
                td1.Attributes["class"] = "auto_table";
                td1.Attributes["style"] = "width:50px;";

                HtmlGenericControl divAddUser = new HtmlGenericControl("div");
                td1.Controls.Add(divAddUser);
                divAddUser.Attributes["class"] = "SubmitButtonMini Blue";

                TButton buttonAddUser = new TButton();
                divAddUser.Controls.Add(buttonAddUser);
                buttonAddUser.Args.Add(userData);
                buttonAddUser.Text = "exp";
                buttonAddUser.ID = "B_AddUser_" + userData.userId.ToString();
                buttonAddUser.Click += new EventHandler(AddUser_Click);

                HtmlGenericControl td2 = new HtmlGenericControl("td");
                tr.Controls.Add(td2);
                td2.Attributes["class"] = "auto_table";
                td2.Attributes["style"] = "width: 10px;";

                HtmlGenericControl td3 = new HtmlGenericControl("td");
                tr.Controls.Add(td3);
                td3.Attributes["class"] = "auto_table";

                Label labelUserName = new Label();
                td3.Controls.Add(labelUserName);
                labelUserName.Text = userData.name;
                labelUserName.CssClass = "user";

                if (userData.dedicated)
                    labelUserName.CssClass = labelUserName.CssClass + " user_dedicated";

                if (userData.enabled == false)
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
            }
            else if (userData.kind == 3)
            {
                HtmlGenericControl table = new HtmlGenericControl("table");
                panelUser.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                HtmlGenericControl tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                HtmlGenericControl td1 = new HtmlGenericControl("td");
                tr.Controls.Add(td1);
                td1.Attributes["class"] = "auto_table";
                td1.Attributes["style"] = "width:50px;";

                HtmlGenericControl divAddUser = new HtmlGenericControl("div");
                td1.Controls.Add(divAddUser);
                divAddUser.Attributes["class"] = "SubmitButtonMini Blue";

                TButton buttonAddUser = new TButton();
                divAddUser.Controls.Add(buttonAddUser);
                buttonAddUser.Args.Add(userData);
                buttonAddUser.Text = "exp";
                buttonAddUser.ID = "B_AddUser_" + userData.userId.ToString();
                buttonAddUser.Click += new EventHandler(AddUser_Click);

                HtmlGenericControl td2 = new HtmlGenericControl("td");
                tr.Controls.Add(td2);
                td2.Attributes["class"] = "auto_table";
                td2.Attributes["style"] = "width: 10px;";

                HtmlGenericControl td3 = new HtmlGenericControl("td");
                tr.Controls.Add(td3);
                td3.Attributes["class"] = "auto_table";

                Label labelUserName = new Label();
                td3.Controls.Add(labelUserName);
                labelUserName.Text = userData.name;
                labelUserName.CssClass = "user";
               
                if (userData.dedicated)
                    labelUserName.CssClass = labelUserName.CssClass + " user_dedicated";

                if (userData.contactData.status == 2)
                {
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                    labelUserName.ToolTip = "Your partner has been deleted from the network.";
                    divAddUser.Visible = false;
                }
                else if (userData.contactData.status == 3)
                {
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                    labelUserName.ToolTip = "Your partner has been deleted from the network.";
                    divAddUser.Visible = false;
                }
                else if (userData.enabled == false)
                {
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                    labelUserName.ToolTip = "The user is disabled.";
                }

                Label labelContactName = new Label();
                td3.Controls.Add(labelContactName);
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
                HtmlGenericControl table = new HtmlGenericControl("table");
                panelUser.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                HtmlGenericControl tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                HtmlGenericControl td1 = new HtmlGenericControl("td");
                tr.Controls.Add(td1);
                td1.Attributes["class"] = "auto_table";
                td1.Attributes["style"] = "width:50px;";

                HtmlGenericControl divAddUser = new HtmlGenericControl("div");
                td1.Controls.Add(divAddUser);
                divAddUser.Attributes["class"] = "SubmitButtonMini Blue";

                TButton buttonAddUser = new TButton();
                divAddUser.Controls.Add(buttonAddUser);
                buttonAddUser.Args.Add(userData);
                buttonAddUser.Text = "exp";
                buttonAddUser.ID = "B_AddUser_" + userData.userId.ToString();
                buttonAddUser.Click += new EventHandler(AddUser_Click);

                HtmlGenericControl td2 = new HtmlGenericControl("td");
                tr.Controls.Add(td2);
                td2.Attributes["class"] = "auto_table";
                td2.Attributes["style"] = "width: 10px;";

                HtmlGenericControl td3 = new HtmlGenericControl("td");
                tr.Controls.Add(td3);
                td3.Attributes["class"] = "auto_table";

                Label labelUserName = new Label();
                td3.Controls.Add(labelUserName);
                labelUserName.Text = userData.name;
                labelUserName.CssClass = "user user_owner";

                if (userData.enabled == false)
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
            }
        }

        if (PH_ImplicitUsers.Controls.Count == 0)
            P_ImplicitUsers.Visible = false;

        PH_DeniedUsers.Controls.Clear();
        foreach (UserData userData in m_siteDataset.users)
        {
            if (explicitUsers.Find(x => x.userId == userData.userId) != null)
                continue;

            if (implicitUsers.Find(x => x.userId == userData.userId) != null)
                continue;

            if (userData.kind == 2)
            {
                Panel panelUser = new Panel();
                PH_DeniedUsers.Controls.Add(panelUser);
                panelUser.CssClass = "site_block_item";

                HtmlGenericControl table = new HtmlGenericControl("table");
                panelUser.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                HtmlGenericControl tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                HtmlGenericControl td1 = new HtmlGenericControl("td");
                tr.Controls.Add(td1);
                td1.Attributes["class"] = "auto_table";
                td1.Attributes["style"] = "width:50px;";

                HtmlGenericControl divAddUser = new HtmlGenericControl("div");
                td1.Controls.Add(divAddUser);
                divAddUser.Attributes["class"] = "SubmitButtonMini Blue";

                TButton buttonAddUser = new TButton();
                divAddUser.Controls.Add(buttonAddUser);
                buttonAddUser.Args.Add(userData);
                buttonAddUser.Text = "add";
                buttonAddUser.ID = "B_AddUser_" + userData.userId.ToString();
                buttonAddUser.Click += new EventHandler(AddUser_Click);

                HtmlGenericControl td2 = new HtmlGenericControl("td");
                tr.Controls.Add(td2);
                td2.Attributes["class"] = "auto_table";
                td2.Attributes["style"] = "width: 10px;";

                HtmlGenericControl td3 = new HtmlGenericControl("td");
                tr.Controls.Add(td3);
                td3.Attributes["class"] = "auto_table";

                Label labelUserName = new Label();
                td3.Controls.Add(labelUserName);
                labelUserName.Text = userData.name;
                labelUserName.CssClass = "user";

                if (userData.dedicated)
                    labelUserName.CssClass = labelUserName.CssClass + " user_dedicated";

                if (userData.enabled == false)
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
            }
            else if (userData.kind == 3)
            {
                Panel panelUser = new Panel();
                PH_DeniedUsers.Controls.Add(panelUser);
                panelUser.CssClass = "site_block_item";

                HtmlGenericControl table = new HtmlGenericControl("table");
                panelUser.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                HtmlGenericControl tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                HtmlGenericControl td1 = new HtmlGenericControl("td");
                tr.Controls.Add(td1);
                td1.Attributes["class"] = "auto_table";
                td1.Attributes["style"] = "width:50px;";

                HtmlGenericControl divAddUser = new HtmlGenericControl("div");
                td1.Controls.Add(divAddUser);
                divAddUser.Attributes["class"] = "SubmitButtonMini Blue";

                TButton buttonAddUser = new TButton();
                divAddUser.Controls.Add(buttonAddUser);
                buttonAddUser.Args.Add(userData);
                buttonAddUser.Text = "add";
                buttonAddUser.ID = "B_AddUser_" + userData.userId.ToString();
                buttonAddUser.Click += new EventHandler(AddUser_Click);

                HtmlGenericControl td2 = new HtmlGenericControl("td");
                tr.Controls.Add(td2);
                td2.Attributes["class"] = "auto_table";
                td2.Attributes["style"] = "width: 10px;";

                HtmlGenericControl td3 = new HtmlGenericControl("td");
                tr.Controls.Add(td3);
                td3.Attributes["class"] = "auto_table";

                Label labelUserName = new Label();
                td3.Controls.Add(labelUserName);
                labelUserName.Text = userData.name;
                labelUserName.CssClass = "user";
                
                if (userData.dedicated)
                    labelUserName.CssClass = labelUserName.CssClass + " user_dedicated";

                if (userData.contactData.status == 2)
                {
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                    labelUserName.ToolTip = "Your partner has been deleted from the network.";
                    divAddUser.Visible = false;
                }
                else if (userData.contactData.status == 3)
                {
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                    labelUserName.ToolTip = "Your partner has been deleted from the network.";
                    divAddUser.Visible = false;
                }
                else if (userData.enabled == false)
                {
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
                    labelUserName.ToolTip = "The user is disabled.";
                }

                Label labelContactName = new Label();
                td3.Controls.Add(labelContactName);
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
                Panel panelUser = new Panel();
                PH_DeniedUsers.Controls.Add(panelUser);
                panelUser.CssClass = "site_block_item";

                HtmlGenericControl table = new HtmlGenericControl("table");
                panelUser.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                HtmlGenericControl tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                HtmlGenericControl td1 = new HtmlGenericControl("td");
                tr.Controls.Add(td1);
                td1.Attributes["class"] = "auto_table";
                td1.Attributes["style"] = "width:50px;";

                HtmlGenericControl divAddUser = new HtmlGenericControl("div");
                td1.Controls.Add(divAddUser);
                divAddUser.Attributes["class"] = "SubmitButtonMini Blue";

                TButton buttonAddUser = new TButton();
                divAddUser.Controls.Add(buttonAddUser);
                buttonAddUser.Args.Add(userData);
                buttonAddUser.Text = "add";
                buttonAddUser.ID = "B_AddUser_" + userData.userId.ToString();
                buttonAddUser.Click += new EventHandler(AddUser_Click);

                HtmlGenericControl td2 = new HtmlGenericControl("td");
                tr.Controls.Add(td2);
                td2.Attributes["class"] = "auto_table";
                td2.Attributes["style"] = "width: 10px;";

                HtmlGenericControl td3 = new HtmlGenericControl("td");
                tr.Controls.Add(td3);
                td3.Attributes["class"] = "auto_table";

                Label labelUserName = new Label();
                td3.Controls.Add(labelUserName);
                labelUserName.Text = userData.name;
                labelUserName.CssClass = "user user_owner";

                if (userData.enabled == false)
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
            }
            else // userData.kind == 4
            {
                Panel panelUser = new Panel();
                PH_DeniedUsers.Controls.Add(panelUser);
                panelUser.CssClass = "site_block_item";

                HtmlGenericControl table = new HtmlGenericControl("table");
                panelUser.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                HtmlGenericControl tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                HtmlGenericControl td1 = new HtmlGenericControl("td");
                tr.Controls.Add(td1);
                td1.Attributes["class"] = "auto_table";
                td1.Attributes["style"] = "width: 50px; padding-top: 4px;";

                HtmlGenericControl td2 = new HtmlGenericControl("td");
                tr.Controls.Add(td2);
                td2.Attributes["class"] = "auto_table";
                td2.Attributes["style"] = "width: 10px;";

                HtmlGenericControl td3 = new HtmlGenericControl("td");
                tr.Controls.Add(td3);
                td3.Attributes["class"] = "auto_table";
                td3.Attributes["style"] = "padding-top: 4px;";

                Label labelUserName = new Label();
                td3.Controls.Add(labelUserName);
                labelUserName.Text = userData.name;
                labelUserName.CssClass = "user user_guest";

                if (userData.enabled == false)
                    labelUserName.CssClass = labelUserName.CssClass + " disabled_status";
            }
        }

        if (PH_DeniedUsers.Controls.Count == 0)
            P_DeniedUsers.Visible = false;
    }
}