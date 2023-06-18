using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class ssite : System.Web.UI.Page
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
            SoftnetRegistry.GetSiteConfigDataset(this.Context.User.Identity.Name, siteId, m_siteDataset);

            string retString = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("ret");
            m_urlBuider = new UrlBuider(retString);

            if (m_siteDataset.siteData.constructed)
            {
                if (m_siteDataset.siteData.siteKind == 1)
                {
                    if (m_siteDataset.siteData.rolesSupported)
                        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/srsite.aspx?sid={0}", m_siteDataset.siteData.siteId)));
                    else
                        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/susite.aspx?sid={0}", m_siteDataset.siteData.siteId)));
                }
                else
                {
                    if (m_siteDataset.siteData.rolesSupported)
                        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/mrsite.aspx?sid={0}", m_siteDataset.siteData.siteId)));
                    else
                        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/musite.aspx?sid={0}", m_siteDataset.siteData.siteId)));
                }
                return;
            }
            else if (m_siteDataset.siteData.siteKind != 1 || m_siteDataset.services.Count != 1)
            {
                Response.Redirect(string.Format("~/domains/domain.aspx?did={0}", m_siteDataset.domainId));
                return;
            }            
            
            m_siteData = m_siteDataset.siteData;
            m_serviceData = m_siteDataset.services[0];

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
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/ssite.aspx?sid={0}", m_siteData.siteId)));
    }

    protected void DeleteSite_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getNextUrl(string.Format("~/domains/deletesite.aspx?sid={0}", m_siteData.siteId), string.Format("~/domains/ssite.aspx?sid={0}", m_siteData.siteId)));
    }

    protected void EditSiteDescription_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/ssite.aspx?sid={0}&sp=1", m_siteData.siteId)));
    }

    protected void SaveSiteDescription_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        TextBox textboxSiteDescription = (TextBox)button.Args[0];

        try
        {
            SoftnetRegistry.ChangeSiteDescription(m_siteData.siteId, textboxSiteDescription.Text);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/ssite.aspx?sid={0}", m_siteData.siteId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void EditSiteEnabledStatus_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/ssite.aspx?sid={0}&sp=2", m_siteData.siteId)));
    }

    protected void EnableSite_Click(object sender, EventArgs e)
    {
        try
        {
            SoftnetTracker.enableSite(m_siteData.siteId);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/ssite.aspx?sid={0}", m_siteData.siteId)));
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
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/ssite.aspx?sid={0}", m_siteData.siteId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void EditSiteKind_Click(object sender, EventArgs e)
    {
        try
        {
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/ssite.aspx?sid={0}&sp=3", m_siteData.siteId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void SetAsMultiservice_Click(object sender, EventArgs e)
    {
        try
        {
            SoftnetTracker.setSiteAsMultiservice(m_siteData.siteId);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/msite.aspx?sid={0}", m_siteData.siteId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void EditServiceAccount_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/ssite.aspx?sid={0}&srp=1", m_siteData.siteId)));
    }

    protected void EditServiceHostname_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/ssite.aspx?sid={0}&srp=2", m_siteData.siteId)));
    }

    protected void ShowApplyStructure_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/ssite.aspx?sid={0}&srp=3", m_siteData.siteId)));
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

        try
        {
            SoftnetTracker.changeHostname(m_siteData.siteId, m_serviceData.serviceId, textboxHostname.Text);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/domains/ssite.aspx?sid={0}", m_siteData.siteId)));
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
            {
                if(m_urlBuider.hasBackUrl())
                    Response.Redirect(string.Format("~/domains/srsite.aspx?sid={0}&ret={1}", m_siteData.siteId, m_urlBuider.getBackUrl()));
                else
                    Response.Redirect(string.Format("~/domains/srsite.aspx?sid={0}", m_siteData.siteId));
            }
            else if (siteAccessType == 0)
            {
                if (m_urlBuider.hasBackUrl())
                    Response.Redirect(string.Format("~/domains/susite.aspx?sid={0}&ret={1}", m_siteData.siteId, m_urlBuider.getBackUrl()));
                else
                    Response.Redirect(string.Format("~/domains/susite.aspx?sid={0}", m_siteData.siteId));
            }
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
            else if (siteProp == 3)
            {
                P_EditSiteKind.CssClass = "SubmitButtonSquare Selected Blue";
                P_SiteEdit.Visible = true;

                HtmlGenericControl table = new HtmlGenericControl("table");
                P_SiteEdit.Controls.Add(table);
                table.Attributes["class"] = "auto_table";
                table.Attributes["style"] = "margin-top: 2px;";
                HtmlGenericControl tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);
                HtmlGenericControl td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "auto_table";

                HtmlGenericControl divSetAsMultiservice = new HtmlGenericControl("div");
                td.Controls.Add(divSetAsMultiservice);
                divSetAsMultiservice.Attributes["class"] = "SubmitButtonMini Green";

                TButton buttonSetAsMultiservice = new TButton();
                divSetAsMultiservice.Controls.Add(buttonSetAsMultiservice);                
                buttonSetAsMultiservice.Text = "set as multi-service site";
                buttonSetAsMultiservice.ID = "B_SetAsMultiservice";
                buttonSetAsMultiservice.Click += new EventHandler(SetAsMultiservice_Click);

                HtmlGenericControl span = new HtmlGenericControl("span");
                P_SiteEdit.Controls.Add(span);
                span.Attributes["style"] = "display: block; margin-top: 10px; color: #3C6C80";
                span.InnerHtml = "<span style='color: #CF5400'>Attention!</span> All services on a multi-service site must be of the same type.";
            }
        }

        if (string.IsNullOrEmpty(m_serviceData.serviceType))
            P_ShowApplyStructureButton.Visible = false;
        else
            P_ShowApplyStructureButton.Visible = true;

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
        }

        L_Hostname.Text = m_serviceData.hostname;

        if (string.IsNullOrEmpty(m_serviceData.serviceType) == false)
        {
            L_ServiceType.Visible = true;
            L_ServiceType.Text = m_serviceData.serviceType + "<span class='gray_text'> ( </span>" + m_serviceData.contractAuthor + "<span class='gray_text'> )</span>";

            if (string.IsNullOrEmpty(m_serviceData.version) == false)
            {
                L_Version.Visible = true;
                L_Version.Text = "<span class='version_label'> ver. </span>" + m_serviceData.version;
            }
        }
        else
        {
            L_ServiceStatus.Visible = true;
            L_ServiceStatus.Text = "service type undefined";
        }
    }
}