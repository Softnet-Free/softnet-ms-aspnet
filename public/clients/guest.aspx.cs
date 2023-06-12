using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;

public partial class public_clients_guest : System.Web.UI.Page
{
    GuestCreatingDataset m_dataset;
    string m_receivedSiteKey;
    string m_receivedTransactionKey;
    string m_receivedEMail;

    protected void CreateClient_Click(object sender, EventArgs e)
    {
        try
        {
            int passwordLength = SoftnetRegistry.settings_getClientPasswordLength();
            string password = Randomizer.generatePassword(passwordLength);
            byte[] salt = Randomizer.generateOctetString(16);
            byte[] saltedPassword = PasswordHash.Compute(salt, password);

            string clientKey = SoftnetRegistry.public_createGuestClient(m_dataset.siteData.siteId, Convert.ToBase64String(saltedPassword), Convert.ToBase64String(salt), m_receivedEMail, m_receivedTransactionKey);

            TButton tButton = (TButton)sender;
            HtmlGenericControl divSiteBlockItem = (HtmlGenericControl)tButton.Args[0];
            divSiteBlockItem.Controls.Clear();

            HtmlGenericControl table = new HtmlGenericControl("table");
            divSiteBlockItem.Controls.Add(table);
            table.Attributes["class"] = "wide_table";
            table.Attributes["style"] = "margin-top: 10px;";

            HtmlGenericControl tr = new HtmlGenericControl("tr");
            table.Controls.Add(tr);

            HtmlGenericControl td = new HtmlGenericControl("td");
            tr.Controls.Add(td);
            td.Attributes["class"] = "wide_table width_25";
            td.Attributes["style"] = "text-align:right; padding-right:5px";

            HtmlGenericControl spanParamName = new HtmlGenericControl("span");
            td.Controls.Add(spanParamName);
            spanParamName.Attributes["class"] = "param_name";
            spanParamName.InnerText = "Client URI:";

            td = new HtmlGenericControl("td");
            tr.Controls.Add(td);
            td.Attributes["class"] = "wide_table width_75";
            td.Attributes["style"] = "text-align:left; padding-left:5px";

            HtmlGenericControl spanParamValue = new HtmlGenericControl("span");
            td.Controls.Add(spanParamValue);
            spanParamValue.Attributes["class"] = "param_value";
            if (m_dataset.siteData.siteKind == 1)
                spanParamValue.InnerText = string.Format("softnet-s://{0}@{1}", clientKey, SoftnetRegistry.settings_getServerAddress());
            else
                spanParamValue.InnerText = string.Format("softnet-m://{0}@{1}", clientKey, SoftnetRegistry.settings_getServerAddress());

            tr = new HtmlGenericControl("tr");
            table.Controls.Add(tr);

            td = new HtmlGenericControl("td");
            tr.Controls.Add(td);           
            td.Attributes["class"] = "wide_table width_25";
            td.Attributes["style"] = "text-align:right; padding-right:5px; padding-top: 5px";           

            spanParamName = new HtmlGenericControl("span");
            td.Controls.Add(spanParamName);
            spanParamName.Attributes["class"] = "param_name";
            spanParamName.InnerText = "Password:";

            td = new HtmlGenericControl("td");
            tr.Controls.Add(td);
            td.Attributes["class"] = "wide_table width_75";
            td.Attributes["style"] = "text-align:left; padding-left:5px; padding-top: 5px";

            spanParamValue = new HtmlGenericControl("span");
            td.Controls.Add(spanParamValue);
            spanParamValue.Attributes["class"] = "param_value";
            spanParamValue.InnerText = password;
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
            m_receivedSiteKey = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("skey");
            if (string.IsNullOrWhiteSpace(m_receivedSiteKey))
                throw new ArgumentSoftnetException("The confirmation url has an invalid format.");

            m_receivedTransactionKey = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("tkey");
            if (string.IsNullOrWhiteSpace(m_receivedTransactionKey))
                throw new ArgumentSoftnetException("The confirmation url has an invalid format.");

            m_receivedEMail = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("email");
            if (string.IsNullOrWhiteSpace(m_receivedEMail))
                throw new ArgumentSoftnetException("The confirmation url has an invalid format.");

            string receivedTime = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("time");
            if (string.IsNullOrWhiteSpace(receivedTime))
                throw new ArgumentSoftnetException("The confirmation url has an invalid format.");

            string base64ReceivedHash = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("hash");
            if (string.IsNullOrWhiteSpace(base64ReceivedHash))
                throw new ArgumentSoftnetException("The confirmation url has an invalid format.");

            m_dataset = new GuestCreatingDataset();
            SoftnetRegistry.public_getGuestCreatingDataset(m_receivedSiteKey, m_receivedTransactionKey, m_dataset);

            long createdTime = 0;
            long.TryParse(receivedTime, out createdTime);

            if (m_dataset.currentTime - createdTime > 360)
                throw new ArgumentSoftnetException("The confirmation url has expired.");

            byte[] siteKeyBytes = Encoding.UTF8.GetBytes(m_receivedSiteKey);
            byte[] transactionKeyBytes = Encoding.UTF8.GetBytes(m_receivedTransactionKey);
            byte[] emailBytes = Encoding.UTF8.GetBytes(m_receivedEMail);
            byte[] timeBytes = Encoding.UTF8.GetBytes(receivedTime);
            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(m_dataset.secretKey);

            byte[] buffer = new byte[siteKeyBytes.Length + transactionKeyBytes.Length + emailBytes.Length + timeBytes.Length + secretKeyBytes.Length];
            int offset = 0;
            System.Buffer.BlockCopy(siteKeyBytes, 0, buffer, offset, siteKeyBytes.Length);
            offset += siteKeyBytes.Length;
            System.Buffer.BlockCopy(transactionKeyBytes, 0, buffer, offset, transactionKeyBytes.Length);
            offset += transactionKeyBytes.Length;
            System.Buffer.BlockCopy(emailBytes, 0, buffer, offset, emailBytes.Length);
            offset += emailBytes.Length;
            System.Buffer.BlockCopy(timeBytes, 0, buffer, offset, timeBytes.Length);
            offset += timeBytes.Length;
            System.Buffer.BlockCopy(secretKeyBytes, 0, buffer, offset, secretKeyBytes.Length);

            byte[] hash = Sha1Hash.Compute(buffer);
            string base64Hash = Convert.ToBase64String(hash);

            if (base64ReceivedHash.Equals(base64Hash) == false)
                throw new ArgumentSoftnetException("The confirmation url is not valid.");

            HL_Owner.NavigateUrl = string.Format("~/public/services/domains.aspx?uid={0}", m_dataset.ownerId);
            HL_Owner.Text = m_dataset.ownerName;

            HL_Domain.NavigateUrl = string.Format("~/public/services/domain.aspx?did={0}", m_dataset.domainId);
            HL_Domain.Text = m_dataset.domainName;

            L_ConsumerEMail.Text = m_receivedEMail;

            if (m_dataset.siteData.siteKind == Constants.SiteKind.SingleService)
            {
                drawSSite();
            }
            else
            {
                drawMSite();
            }
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    void drawSSite()
    {
        SiteData siteData = m_dataset.siteData;

        if (m_dataset.services.Count != 1)
            throw new DataIntegritySoftnetException();
        ServiceData serviceData = m_dataset.services[0];

        HtmlGenericControl divSiteFrame = new HtmlGenericControl("div");
        PH_Site.Controls.Add(divSiteFrame);
        divSiteFrame.Attributes["class"] = "site_frame";

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
        td.Attributes["style"] = "text-align: left; padding: 2px; padding-left: 5px;";
        HtmlGenericControl spanSiteType = new HtmlGenericControl("span");
        td.Controls.Add(spanSiteType);
        spanSiteType.Attributes["class"] = "site_type";
        spanSiteType.InnerHtml = siteData.serviceType + "<span class='gray_text'> ( </span>" + siteData.contractAuthor + "<span class='gray_text'> )</span>";

        HtmlGenericControl divSiteBody = new HtmlGenericControl("div");
        divSiteFrame.Controls.Add(divSiteBody);
        divSiteBody.Attributes["class"] = "site_body";

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
        
        HtmlGenericControl tdLeft = new HtmlGenericControl("td");
        tr.Controls.Add(tdLeft);
        tdLeft.Attributes["class"] = "wide_table";
        tdLeft.Attributes["style"] = "text-align:left";
        
        HtmlGenericControl tdRight = new HtmlGenericControl("td");
        tr.Controls.Add(tdRight);
        tdRight.Attributes["class"] = "wide_table";
        tdRight.Attributes["style"] = "text-align:right";

        table = new HtmlGenericControl("table");
        tdLeft.Controls.Add(table);
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
                tdRight.Controls.Add(span);
                span.Attributes["class"] = "object_status";
                span.InnerText = "service type conflict";
            }
            else if (serviceData.ssHash.Equals(siteData.ssHash) == false)
            {
                HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                tdRight.Controls.Add(spanStatus);
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
            tdRight.Controls.Add(spanStatus);
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
        divSiteBlockHeader.InnerText = "new guest client";

        divSiteBlockItem = new HtmlGenericControl("div");
        divSiteBlock.Controls.Add(divSiteBlockItem);
        divSiteBlockItem.Attributes["class"] = "site_block_item";

        table = new HtmlGenericControl("table");
        divSiteBlockItem.Controls.Add(table);
        table.Attributes["class"] = "wide_table";
        table.Attributes["style"] = "margin-top: 5px;";
        tr = new HtmlGenericControl("tr");
        table.Controls.Add(tr);

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table width_25";

        HtmlGenericControl tdButton = new HtmlGenericControl("td");
        tr.Controls.Add(tdButton);
        tdButton.Attributes["class"] = "wide_table";
        tdButton.Attributes["style"] = "width: auto";

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table width_75";

        HtmlGenericControl divCreateClientButton = new HtmlGenericControl("div");
        tdButton.Controls.Add(divCreateClientButton);
        divCreateClientButton.Attributes["class"] = "SubmitButton Green";

        TButton buttonCreateClient = new TButton();
        divCreateClientButton.Controls.Add(buttonCreateClient);            
        buttonCreateClient.Args.Add(divSiteBlockItem);
        buttonCreateClient.Text = "create a guest client";
        buttonCreateClient.ID = string.Format("B_CreateClient_{0}", siteData.siteId);
        buttonCreateClient.Click += new EventHandler(CreateClient_Click);
    }

    void drawMSite()
    {
        SiteData siteData = m_dataset.siteData;

        HtmlGenericControl divSiteFrame = new HtmlGenericControl("div");
        PH_Site.Controls.Add(divSiteFrame);
        divSiteFrame.Attributes["class"] = "site_frame";

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
        td.Attributes["style"] = "text-align: left; padding: 2px; padding-left: 5px;";
        HtmlGenericControl spanSiteType = new HtmlGenericControl("span");
        td.Controls.Add(spanSiteType);
        spanSiteType.Attributes["class"] = "site_type";
        spanSiteType.InnerHtml = siteData.serviceType + "<span class='gray_text'> ( </span>" + siteData.contractAuthor + "<span class='gray_text'> )</span>";

        HtmlGenericControl divSiteBody = new HtmlGenericControl("div");
        divSiteFrame.Controls.Add(divSiteBody);
        divSiteBody.Attributes["class"] = "site_body";

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

        foreach (ServiceData serviceData in m_dataset.services)
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
                    tdRight.Controls.Add(span);
                    span.Attributes["class"] = "object_status";
                    span.InnerText = "service type conflict";
                }
                else if (serviceData.ssHash.Equals(siteData.ssHash) == false)
                {
                    HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                    tdRight.Controls.Add(spanStatus);
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
                tdRight.Controls.Add(spanStatus);
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
                tdRight.Controls.Add(spanStatus);
                spanStatus.Attributes["class"] = "object_status";
                if (tdRight.Controls.Count == 0)
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
        divSiteBlockHeader.InnerText = "new guest client";

        divSiteBlockItem = new HtmlGenericControl("div");
        divSiteBlock.Controls.Add(divSiteBlockItem);
        divSiteBlockItem.Attributes["class"] = "site_block_item";

        table = new HtmlGenericControl("table");
        divSiteBlockItem.Controls.Add(table);
        table.Attributes["class"] = "wide_table";
        table.Attributes["style"] = "margin-top: 5px;";
        tr = new HtmlGenericControl("tr");
        table.Controls.Add(tr);

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table width_25";

        HtmlGenericControl tdButton = new HtmlGenericControl("td");
        tr.Controls.Add(tdButton);
        tdButton.Attributes["class"] = "wide_table";
        tdButton.Attributes["style"] = "width: auto";

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table width_75";

        HtmlGenericControl divCreateClientButton = new HtmlGenericControl("div");
        tdButton.Controls.Add(divCreateClientButton);
        divCreateClientButton.Attributes["class"] = "SubmitButton Green";

        TButton buttonCreateClient = new TButton();
        divCreateClientButton.Controls.Add(buttonCreateClient);        
        buttonCreateClient.Args.Add(divSiteBlockItem);
        buttonCreateClient.Text = "create a guest client";
        buttonCreateClient.ID = string.Format("B_CreateClient_{0}", siteData.siteId);
        buttonCreateClient.Click += new EventHandler(CreateClient_Click);
    }
}