using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class msite : System.Web.UI.Page
{
    SiteConfigDataset m_siteDataset;
    SiteData m_siteData;
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
            SoftnetRegistry.GetSiteConfigDataset(this.Context.User.Identity.Name, siteId, m_siteDataset);

            string retString = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("ret");
            m_urlBuider = new UrlBuider(retString);

            if (m_siteDataset.siteData.constructed)
            {
                if (m_siteDataset.siteData.siteKind == 2)
                {
                    if (m_siteDataset.siteData.rolesSupported)
                        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/mrsite.aspx?sid={0}", m_siteDataset.siteData.siteId)));
                    else
                        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/musite.aspx?sid={0}", m_siteDataset.siteData.siteId)));
                }
                else
                {
                    if (m_siteDataset.siteData.rolesSupported)
                        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/srsite.aspx?sid={0}", m_siteDataset.siteData.siteId)));
                    else
                        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/susite.aspx?sid={0}", m_siteDataset.siteData.siteId)));
                }
                return;
            }
            else if (m_siteDataset.siteData.siteKind != 2)
            {
                Response.Redirect(string.Format("~/domains/domain.aspx?did={0}", m_siteDataset.domainId));
                return;
            }

            m_siteData = m_siteDataset.siteData;
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
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/msite.aspx?sid={0}", m_siteData.siteId)));
    }

    protected void DeleteSite_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getNextUrl(string.Format("~/domains/deletesite.aspx?sid={0}", m_siteData.siteId), string.Format("~/domains/msite.aspx?sid={0}", m_siteData.siteId)));
    }

    protected void EditSiteDescription_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/msite.aspx?sid={0}&sp=1", m_siteData.siteId)));
    }

    protected void SaveSiteDescription_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        TextBox textboxSiteDescription = (TextBox)button.Args[0];

        try
        {
            SoftnetRegistry.ChangeSiteDescription(m_siteData.siteId, textboxSiteDescription.Text);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/msite.aspx?sid={0}", m_siteData.siteId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void EditSiteEnabledStatus_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/msite.aspx?sid={0}&sp=2", m_siteData.siteId)));
    }

    protected void EnableSite_Click(object sender, EventArgs e)
    {
        try
        {
            SoftnetTracker.enableSite(m_siteData.siteId);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/msite.aspx?sid={0}", m_siteData.siteId)));
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
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/msite.aspx?sid={0}", m_siteData.siteId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void NewService_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/msite.aspx?sid={0}&sp=3", m_siteData.siteId)));
    }

    protected void AddService_Click(object sender, EventArgs e)
    {
        try
        {
            SoftnetTracker.addService(m_siteData.siteId, TB_NewServiceHostname.Text.Trim());
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/msite.aspx?sid={0}", m_siteData.siteId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void EditService_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        ServiceData serviceData = (ServiceData)button.Args[0];
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/msite.aspx?sid={0}&srid={1}", m_siteData.siteId, serviceData.serviceId)));
    }

    protected void ViewService_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/msite.aspx?sid={0}", m_siteData.siteId)));
    }

    protected void DeleteService_Click(object sender, EventArgs e)
    {
        TButton tButton = (TButton)sender;
        ServiceData serviceData = (ServiceData)tButton.Args[0];
        try
        {
            SoftnetTracker.deleteService(m_siteData.siteId, serviceData.serviceId);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/msite.aspx?sid={0}", m_siteData.siteId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void EditServiceAccount_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        ServiceData serviceData = (ServiceData)button.Args[0];
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/msite.aspx?sid={0}&srid={1}&srp=1", m_siteData.siteId, serviceData.serviceId)));
    }

    protected void EditHostname_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        ServiceData serviceData = (ServiceData)button.Args[0];
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/msite.aspx?sid={0}&srid={1}&srp=2", m_siteData.siteId, serviceData.serviceId)));
    }

    protected void EditServiceEnabledStatus_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        ServiceData serviceData = (ServiceData)button.Args[0];
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/msite.aspx?sid={0}&srid={1}&srp=3", m_siteData.siteId, serviceData.serviceId)));
    }

    protected void ShowApplyStructure_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        ServiceData serviceData = (ServiceData)button.Args[0];
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/msite.aspx?sid={0}&srid={1}&srp=4", m_siteData.siteId, serviceData.serviceId)));
    }

    protected void GeneratePassword_Click(object sender, EventArgs e)
    {
        TButton tButton = (TButton)sender;
        ServiceData serviceData = (ServiceData)tButton.Args[0];
        Panel panelWorkArea = (Panel)tButton.Args[1];
        HtmlGenericControl tableGeneratePassword = (HtmlGenericControl)tButton.Args[2];
        tableGeneratePassword.Visible = false;

        try
        {
            int passwordLength = SoftnetRegistry.settings_getServicePasswordLength();
            string password = Randomizer.generatePassword(passwordLength);
            byte[] salt = Randomizer.generateOctetString(16);
            byte[] saltedPassword = PasswordHash.Compute(salt, password);

            SoftnetTracker.setServicePassword(m_siteData.siteId, serviceData.serviceId, Convert.ToBase64String(salt), Convert.ToBase64String(saltedPassword));

            HtmlGenericControl tablePassword = new HtmlGenericControl("table");
            panelWorkArea.Controls.Add(tablePassword);
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
            panelWorkArea.Controls.Add(divOkButton);
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
        TButton tButton = (TButton)sender;
        ServiceData serviceData = (ServiceData)tButton.Args[0];
        TextBox textboxHostname = (TextBox)tButton.Args[1];

        try
        {
            SoftnetTracker.changeHostname(m_siteData.siteId, serviceData.serviceId, textboxHostname.Text);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/msite.aspx?sid={0}&srid={1}", m_siteData.siteId, serviceData.serviceId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void EnableService_Click(object sender, EventArgs e)
    {
        TButton tButton = (TButton)sender;
        ServiceData serviceData = (ServiceData)tButton.Args[0];

        try
        {
            SoftnetTracker.enableService(m_siteData.siteId, serviceData.serviceId);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/msite.aspx?sid={0}&srid={1}", m_siteData.siteId, serviceData.serviceId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void DisableService_Click(object sender, EventArgs e)
    {
        TButton tButton = (TButton)sender;
        ServiceData serviceData = (ServiceData)tButton.Args[0];

        try
        {
            SoftnetTracker.disableService(m_siteData.siteId, serviceData.serviceId);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/msite.aspx?sid={0}&srid={1}", m_siteData.siteId, serviceData.serviceId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void ApplyStructure_Click(object sender, EventArgs e)
    {
        TButton tButton = (TButton)sender;
        ServiceData serviceData = (ServiceData)tButton.Args[0];
        try
        {
            SoftnetTracker.applyStructure(m_siteData.siteId, serviceData.serviceId);
            int siteAccessType = SoftnetRegistry.GetSiteAccessType(m_siteData.siteId);
            if (siteAccessType == 1)
                Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/mrsite.aspx?sid={0}", m_siteData.siteId)));
            else if (siteAccessType == 0)
                Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/musite.aspx?sid={0}", m_siteData.siteId)));
            else
                Response.Redirect(string.Format("~/domains/domain.aspx?did={0}", m_siteDataset.domainId));
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

        if (string.IsNullOrEmpty(m_siteData.serviceType) == false)
            L_SiteType.Text = m_siteData.serviceType + "<span class='gray_text'> ( </span>" + m_siteData.contractAuthor + "<span class='gray_text'> )</span>";

        if (m_siteData.enabled == false)
            L_SiteStatus.Text += "; disabled";

        int siteProp = 0;
        int.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("sp"), out siteProp);

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
        else if (siteProp == 3)
        {
            P_NewServiceButton.CssClass = "SubmitButtonSquare Selected Blue";
            P_NewService.Visible = true;
        }

        long editedServiceId = 0;
        long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("srid"), out editedServiceId);

        int srvProp = 0;
        int.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("srp"), out srvProp);

        PH_Services.Controls.Clear();
        foreach (ServiceData serviceData in m_siteDataset.services)
        {
            Panel panelService = new Panel();
            PH_Services.Controls.Add(panelService);
            panelService.CssClass = "site_block_item underline";
            panelService.EnableViewState = false;

            if (PH_Services.Controls.Count == 1)
                panelService.CssClass += " upperline";

            if (serviceData.serviceId != editedServiceId)
            {
                HtmlGenericControl tableLayout = new HtmlGenericControl("table");
                panelService.Controls.Add(tableLayout);
                tableLayout.Attributes["class"] = "wide_table";
                HtmlGenericControl tr = new HtmlGenericControl("tr");
                tableLayout.Controls.Add(tr);

                HtmlGenericControl tdLeft = new HtmlGenericControl("td");
                tr.Controls.Add(tdLeft);
                tdLeft.Attributes["class"] = "wide_table";
                tdLeft.Attributes["style"] = "width: 47px; padding-right: 13px;";

                HtmlGenericControl tdRight = new HtmlGenericControl("td");
                tr.Controls.Add(tdRight);
                tdRight.Attributes["class"] = "wide_table";

                HtmlGenericControl divEditServiceButton = new HtmlGenericControl("div");
                tdLeft.Controls.Add(divEditServiceButton);
                divEditServiceButton.Attributes["class"] = "SubmitButtonMini Blue";

                TButton buttonEditService = new TButton();
                divEditServiceButton.Controls.Add(buttonEditService);
                buttonEditService.Args.Add(serviceData);
                buttonEditService.Text = ">>";
                buttonEditService.ID = string.Format("B_EditService_{0}", serviceData.serviceId);
                buttonEditService.Click += new EventHandler(EditService_Click);

                Panel panelWorkArea = new Panel();
                tdRight.Controls.Add(panelWorkArea);

                HtmlGenericControl table = new HtmlGenericControl("table");
                panelWorkArea.Controls.Add(table);
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

                HtmlGenericControl td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";

                HtmlGenericControl spanHostname = new HtmlGenericControl("span");
                td.Controls.Add(spanHostname);
                spanHostname.InnerText = serviceData.hostname;
                spanHostname.Attributes["class"] = "name";

                if (string.IsNullOrEmpty(serviceData.serviceType) == false)
                {
                    td = new HtmlGenericControl("td");
                    tr.Controls.Add(td);
                    td.Attributes["class"] = "auto_table";
                    td.Attributes["style"] = "padding-left: 10px";

                    HtmlGenericControl spanServiceType = new HtmlGenericControl("span");
                    td.Controls.Add(spanServiceType);
                    spanServiceType.Attributes["class"] = "service_type";

                    if (serviceData.serviceType.Equals(m_siteData.serviceType) && serviceData.contractAuthor.Equals(m_siteData.contractAuthor))
                    {
                        if (string.IsNullOrEmpty(serviceData.version) == false)
                            spanServiceType.InnerHtml = "<span class='version_label'>ver.</span> " + serviceData.version;

                        if (serviceData.ssHash.Equals(m_siteData.ssHash) == false)
                        {
                            HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                            tdStatus.Controls.Add(spanStatus);
                            spanStatus.Attributes["class"] = "object_status";
                            spanStatus.InnerText = "site structure mismatch";
                        }
                    }
                    else
                    {
                        spanServiceType.InnerHtml = serviceData.serviceType + "<span class='gray_text'> ( </span>" + serviceData.contractAuthor + "<span class='gray_text'> )</span>";

                        if (string.IsNullOrEmpty(serviceData.version) == false)
                            spanServiceType.InnerHtml += "<span class='version_label'> ver.</span> " + serviceData.version;

                        HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                        tdStatus.Controls.Add(spanStatus);
                        spanStatus.Attributes["class"] = "object_status";
                        spanStatus.InnerText = "service type conflict";
                    }
                }
                else
                {
                    HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                    tdStatus.Controls.Add(spanStatus);
                    spanStatus.Attributes["class"] = "object_status";
                    spanStatus.InnerText = "service type undefined";
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
            else
            {
                HtmlGenericControl tableLayout = new HtmlGenericControl("table");
                panelService.Controls.Add(tableLayout);
                tableLayout.Attributes["class"] = "wide_table";
                HtmlGenericControl tr = new HtmlGenericControl("tr");
                tableLayout.Controls.Add(tr);

                HtmlGenericControl tdLeft = new HtmlGenericControl("td");
                tr.Controls.Add(tdLeft);
                tdLeft.Attributes["class"] = "wide_table";
                tdLeft.Attributes["style"] = "width: 47px; padding-right: 13px; vertical-align: top;";

                HtmlGenericControl tdMiddle = new HtmlGenericControl("td");
                tr.Controls.Add(tdMiddle);
                tdMiddle.Attributes["class"] = "wide_table";
                tdMiddle.Attributes["style"] = "padding-right: 10px; padding-top: 3px; vertical-align: top;";

                HtmlGenericControl tdRight = new HtmlGenericControl("td");
                tr.Controls.Add(tdRight);
                tdRight.Attributes["class"] = "wide_table";
                tdRight.Attributes["style"] = "width: 22px; vertical-align: top;";

                HtmlGenericControl divViewServiceButton = new HtmlGenericControl("div");
                tdLeft.Controls.Add(divViewServiceButton);
                divViewServiceButton.Attributes["class"] = "SubmitButtonMini Selected Blue";

                TButton buttonViewService = new TButton();
                divViewServiceButton.Controls.Add(buttonViewService);
                buttonViewService.Args.Add(serviceData);
                buttonViewService.Text = "<<";
                buttonViewService.ID = string.Format("B_ViewService_{0}", serviceData.serviceId);
                buttonViewService.Click += new EventHandler(ViewService_Click);

                HtmlGenericControl divDeleteServiceButton = new HtmlGenericControl("div");
                tdRight.Controls.Add(divDeleteServiceButton);
                divDeleteServiceButton.Attributes["class"] = "SubmitButtonSquareMini RedOrange";

                TButton buttonDeleteService = new TButton();
                divDeleteServiceButton.Controls.Add(buttonDeleteService);
                buttonDeleteService.Args.Add(serviceData);
                buttonDeleteService.Text = "X";
                buttonDeleteService.ToolTip = "Delete Service";
                buttonDeleteService.ID = string.Format("B_DeleteService_{0}", serviceData.serviceId);
                buttonDeleteService.Click += new EventHandler(DeleteService_Click);

                Panel panelWorkArea = new Panel();
                tdMiddle.Controls.Add(panelWorkArea);
                panelWorkArea.Attributes["style"] = "padding-bottom: 5px;";

                HtmlGenericControl table = new HtmlGenericControl("table");
                panelWorkArea.Controls.Add(table);
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

                HtmlGenericControl td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";

                HtmlGenericControl spanHostname = new HtmlGenericControl("span");
                td.Controls.Add(spanHostname);
                spanHostname.InnerText = serviceData.hostname;
                spanHostname.Attributes["class"] = "name";

                if (string.IsNullOrEmpty(serviceData.serviceType) == false)
                {
                    td = new HtmlGenericControl("td");
                    tr.Controls.Add(td);
                    td.Attributes["class"] = "auto_table";
                    td.Attributes["style"] = "padding-left: 10px";

                    HtmlGenericControl spanServiceType = new HtmlGenericControl("span");
                    td.Controls.Add(spanServiceType);
                    spanServiceType.Attributes["class"] = "service_type";

                    if (serviceData.serviceType.Equals(m_siteData.serviceType) && serviceData.contractAuthor.Equals(m_siteData.contractAuthor))
                    {
                        if (string.IsNullOrEmpty(serviceData.version) == false)
                            spanServiceType.InnerHtml = "<span class='version_label'>ver.</span> " + serviceData.version;

                        if (serviceData.ssHash.Equals(m_siteData.ssHash) == false)
                        {
                            HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                            tdStatus.Controls.Add(spanStatus);
                            spanStatus.Attributes["class"] = "object_status";
                            spanStatus.InnerText = "site structure mismatch";
                        }
                    }
                    else
                    {
                        spanServiceType.InnerHtml = serviceData.serviceType + "<span class='gray_text'> ( </span>" + serviceData.contractAuthor + "<span class='gray_text'> )</span>";

                        if (string.IsNullOrEmpty(serviceData.version) == false)
                            spanServiceType.InnerHtml += "<span class='version_label'> ver.</span> " + serviceData.version;

                        HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                        tdStatus.Controls.Add(spanStatus);
                        spanStatus.Attributes["class"] = "object_status";
                        spanStatus.InnerText = "service type conflict";
                    }
                }
                else
                {
                    HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                    tdStatus.Controls.Add(spanStatus);
                    spanStatus.Attributes["class"] = "object_status";
                    spanStatus.InnerText = "service type undefined";
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

                table = new HtmlGenericControl("table");
                panelWorkArea.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                table.Attributes["style"] = "margin-top: 8px;";
                tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";

                HtmlGenericControl divButton = new HtmlGenericControl("div");
                td.Controls.Add(divButton);
                if (srvProp == 1)
                    divButton.Attributes["class"] = "SubmitButtonSquare Selected Blue";
                else
                    divButton.Attributes["class"] = "SubmitButtonSquare Blue";

                TButton tButton = new TButton();
                tButton.Args.Add(serviceData);
                divButton.Controls.Add(tButton);
                tButton.Text = "account";
                tButton.ID = string.Format("B_EditServiceAccount_{0}", serviceData.serviceId);
                tButton.Click += new EventHandler(EditServiceAccount_Click);

                td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";
                td.Attributes["style"] = "padding-left: 15px;";

                divButton = new HtmlGenericControl("div");
                td.Controls.Add(divButton);
                if (srvProp == 2)
                    divButton.Attributes["class"] = "SubmitButtonSquare Selected Blue";
                else
                    divButton.Attributes["class"] = "SubmitButtonSquare Blue";

                tButton = new TButton();
                tButton.Args.Add(serviceData);
                divButton.Controls.Add(tButton);
                tButton.Text = "hostname";                
                tButton.ID = string.Format("B_EditHostname_{0}", serviceData.serviceId);
                tButton.Click += new EventHandler(EditHostname_Click);

                td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";
                td.Attributes["style"] = "padding-left: 15px;";

                divButton = new HtmlGenericControl("div");
                td.Controls.Add(divButton);
                if (srvProp == 3)
                    divButton.Attributes["class"] = "SubmitButtonSquare Selected Blue";
                else
                    divButton.Attributes["class"] = "SubmitButtonSquare Blue";

                tButton = new TButton();
                tButton.Args.Add(serviceData);
                divButton.Controls.Add(tButton);
                tButton.Text = "enable / disable";
                tButton.ID = string.Format("B_EditServiceStatus_{0}", serviceData.serviceId);
                tButton.Click += new EventHandler(EditServiceEnabledStatus_Click);

                if (string.IsNullOrEmpty(serviceData.serviceType) == false)
                {
                    td = new HtmlGenericControl("td");
                    tr.Controls.Add(td);
                    td.Attributes["class"] = "auto_table";
                    td.Attributes["style"] = "padding-left: 15px;";

                    divButton = new HtmlGenericControl("div");
                    td.Controls.Add(divButton);
                    if (srvProp == 4)
                        divButton.Attributes["class"] = "SubmitButtonSquare Selected Blue";
                    else
                        divButton.Attributes["class"] = "SubmitButtonSquare Blue";

                    tButton = new TButton();
                    tButton.Args.Add(serviceData);
                    divButton.Controls.Add(tButton);
                    tButton.Text = "structure";
                    tButton.ID = string.Format("B_ShowApplyStructure_{0}", serviceData.serviceId);
                    tButton.Click += new EventHandler(ShowApplyStructure_Click);
                }

                if (srvProp == 1)
                {
                    HtmlGenericControl spanAccount = new HtmlGenericControl("span");
                    panelWorkArea.Controls.Add(spanAccount);
                    spanAccount.Attributes["class"] = "client_uri";
                    spanAccount.Attributes["style"] = "display:block; margin-top: 15px;";
                    spanAccount.InnerText = string.Format("softnet-srv://{0}@{1}", serviceData.serviceUid.ToString(), SoftnetRegistry.settings_getServerAddress());

                    HtmlGenericControl tableGeneratePassword = new HtmlGenericControl("table");
                    panelWorkArea.Controls.Add(tableGeneratePassword);
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
                    buttonGeneratePassword.Args.Add(serviceData);
                    buttonGeneratePassword.Args.Add(panelWorkArea);
                    buttonGeneratePassword.Args.Add(tableGeneratePassword);
                    buttonGeneratePassword.Text = "generate passowrd";
                    buttonGeneratePassword.ID = string.Format("B_GeneratePassword_{0}", serviceData.serviceId);
                    buttonGeneratePassword.Click += new EventHandler(GeneratePassword_Click);
                }
                else if (srvProp == 2)
                {
                    table = new HtmlGenericControl("table");
                    panelWorkArea.Controls.Add(table);
                    table.Attributes["class"] = "auto_table";
                    table.Attributes["style"] = "margin-top: 15px;";
                    tr = new HtmlGenericControl("tr");
                    table.Controls.Add(tr);

                    td = new HtmlGenericControl("td");
                    tr.Controls.Add(td);
                    td.Attributes["class"] = "auto_table";

                    TextBox textboxHostname = new TextBox();
                    td.Controls.Add(textboxHostname);
                    textboxHostname.Attributes["style"] = "border: 1px solid #7FBA00; outline:none; width:300px; margin: 0px; padding: 3px;";
                    textboxHostname.Text = serviceData.hostname;
                    textboxHostname.ID = string.Format("TB_Hostname_{0}", serviceData.serviceId);
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
                    buttonSaveHostname.Args.Add(serviceData);
                    buttonSaveHostname.Args.Add(textboxHostname);
                    buttonSaveHostname.Text = "save";
                    buttonSaveHostname.ID = "B_SaveHostname";
                    buttonSaveHostname.Click += new EventHandler(SaveHostname_Click);
                }
                else if (srvProp == 3)
                {
                    table = new HtmlGenericControl("table");
                    panelWorkArea.Controls.Add(table);
                    table.Attributes["class"] = "auto_table";
                    table.Attributes["style"] = "margin-top: 15px;";
                    tr = new HtmlGenericControl("tr");
                    table.Controls.Add(tr);

                    HtmlGenericControl td1 = new HtmlGenericControl("td");
                    tr.Controls.Add(td1);
                    td1.Attributes["class"] = "auto_table";

                    HtmlGenericControl spanStatusCaption = new HtmlGenericControl("span");
                    td1.Controls.Add(spanStatusCaption);
                    spanStatusCaption.Attributes["class"] = "caption";
                    spanStatusCaption.InnerText = "Service";

                    HtmlGenericControl td2 = new HtmlGenericControl("td");
                    tr.Controls.Add(td2);
                    td2.Attributes["class"] = "auto_table";
                    td2.Attributes["style"] = "padding-left: 4px";

                    HtmlGenericControl td3 = new HtmlGenericControl("td");
                    tr.Controls.Add(td3);
                    td3.Attributes["class"] = "auto_table";
                    td3.Attributes["style"] = "padding-left: 15px";

                    if (serviceData.enabled)
                    {
                        HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                        td2.Controls.Add(spanStatus);
                        spanStatus.Attributes["class"] = "enabled_status";
                        spanStatus.InnerText = "enabled";

                        HtmlGenericControl divDisableService = new HtmlGenericControl("div");
                        td3.Controls.Add(divDisableService);
                        divDisableService.Attributes["class"] = "SubmitButtonMini RedOrange";

                        TButton buttonDisableService = new TButton();
                        divDisableService.Controls.Add(buttonDisableService);
                        buttonDisableService.Args.Add(serviceData);
                        buttonDisableService.ID = string.Format("B_DisableService_{0}", serviceData.serviceId);
                        buttonDisableService.Text = "disable";
                        buttonDisableService.Click += new EventHandler(DisableService_Click);
                    }
                    else
                    {
                        HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                        td2.Controls.Add(spanStatus);
                        spanStatus.Attributes["class"] = "disabled_status";
                        spanStatus.InnerText = "disabled";

                        HtmlGenericControl divDisableService = new HtmlGenericControl("div");
                        td3.Controls.Add(divDisableService);
                        divDisableService.Attributes["class"] = "SubmitButtonMini Green";

                        TButton buttonDisableService = new TButton();
                        divDisableService.Controls.Add(buttonDisableService);
                        buttonDisableService.Args.Add(serviceData);
                        buttonDisableService.ID = string.Format("B_EnableService_{0}", serviceData.serviceId);
                        buttonDisableService.Text = "enable";
                        buttonDisableService.Click += new EventHandler(EnableService_Click);
                    }
                }
                else if (srvProp == 4 && string.IsNullOrEmpty(serviceData.serviceType) == false)
                {
                    table = new HtmlGenericControl("table");
                    panelWorkArea.Controls.Add(table);
                    table.Attributes["class"] = "auto_table";
                    table.Attributes["style"] = "margin-top: 15px;";
                    tr = new HtmlGenericControl("tr");
                    table.Controls.Add(tr);

                    td = new HtmlGenericControl("td");
                    tr.Controls.Add(td);
                    td.Attributes["class"] = "auto_table";

                    HtmlGenericControl divApplyStructure = new HtmlGenericControl("div");
                    td.Controls.Add(divApplyStructure);
                    divApplyStructure.Attributes["class"] = "SubmitButtonMini Green";

                    TButton buttonApplyStructure = new TButton();
                    divApplyStructure.Controls.Add(buttonApplyStructure);
                    buttonApplyStructure.Args.Add(serviceData);
                    buttonApplyStructure.Text = "apply structure to the site";
                    buttonApplyStructure.ID = string.Format("B_ApplyStructure_{0}", serviceData.serviceId);
                    buttonApplyStructure.Click += new EventHandler(ApplyStructure_Click);
                }
            }
        }
    }
}