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
using System.Text;
using System.Web.UI.HtmlControls;
using System.Net;
using System.Net.Mail;

public partial class public_clients_domain : System.Web.UI.Page
{
    DomainDatasetForEMail m_dataset;
    string m_mgtSystemUrl;
    string m_browsingUrlParams;
    string m_receivedEMail;
    long m_selectedClientId;
    long m_selectedSiteId;

    protected void Back_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/public/clients/domains.aspx?" + m_browsingUrlParams);
    }

    protected void Refresh_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Format("~/public/clients/domain.aspx?did={0}&", m_dataset.domainId) + m_browsingUrlParams);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            long domainId = 0;
            if (long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("did"), out domainId) == false)
                throw new InvalidIdentifierSoftnetException();

            string receivedTransactionKey = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("tkey");
            if (string.IsNullOrWhiteSpace(receivedTransactionKey))
                throw new ArgumentSoftnetException("The browsing url has an invalid format.");

            m_receivedEMail = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("email");
            if (string.IsNullOrWhiteSpace(m_receivedEMail))
                throw new ArgumentSoftnetException("The browsing url has an invalid format.");

            string receivedTime = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("time");
            if (string.IsNullOrWhiteSpace(receivedTime))
                throw new ArgumentSoftnetException("The browsing url has an invalid format.");

            string receivedBase64Hash = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("hash");
            if (string.IsNullOrWhiteSpace(receivedBase64Hash))
                throw new ArgumentSoftnetException("The browsing url has an invalid format.");

            Pair<string, long> skeyAndTime = new Pair<string, long>();
            SoftnetRegistry.settings_getSecretKeyAndTime(skeyAndTime);

            long createdTime = 0;
            long.TryParse(receivedTime, out createdTime);

            if (skeyAndTime.Second - createdTime > 60480)
                throw new ArgumentSoftnetException("The browsing url has expired.");

            byte[] transactionKeyBytes = Encoding.UTF8.GetBytes(receivedTransactionKey);
            byte[] emailBytes = Encoding.UTF8.GetBytes(m_receivedEMail);
            byte[] timeBytes = Encoding.UTF8.GetBytes(receivedTime);
            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(skeyAndTime.First);

            byte[] buffer = new byte[transactionKeyBytes.Length + emailBytes.Length + timeBytes.Length + secretKeyBytes.Length];
            int offset = 0;
            System.Buffer.BlockCopy(transactionKeyBytes, 0, buffer, offset, transactionKeyBytes.Length);
            offset += transactionKeyBytes.Length;
            System.Buffer.BlockCopy(emailBytes, 0, buffer, offset, emailBytes.Length);
            offset += emailBytes.Length;
            System.Buffer.BlockCopy(timeBytes, 0, buffer, offset, timeBytes.Length);
            offset += timeBytes.Length;
            System.Buffer.BlockCopy(secretKeyBytes, 0, buffer, offset, secretKeyBytes.Length);

            byte[] hash = Sha1Hash.Compute(buffer);
            string base64Hash = Convert.ToBase64String(hash);

            if (receivedBase64Hash.Equals(base64Hash) == false)
                throw new ArgumentSoftnetException("The browsing url is not valid.");

            m_browsingUrlParams = string.Format("tkey={0}&email={1}&time={2}&hash={3}", receivedTransactionKey, HttpUtility.UrlEncode(m_receivedEMail), receivedTime, HttpUtility.UrlEncode(receivedBase64Hash));

            m_dataset = new DomainDatasetForEMail();
            m_dataset.domainId = domainId;
            SoftnetRegistry.public_getDomainDatasetForEMail(m_receivedEMail, m_dataset);

            m_mgtSystemUrl = SoftnetRegistry.settings_getManagementSystemUrl();

            this.Title = string.Format("{0} - Guest Clients by Email", m_dataset.domainName);
            L_EMail.Text = m_receivedEMail;
            HL_Owner.Text = m_dataset.ownerName;
            HL_Owner.NavigateUrl = string.Format("~/public/services/domains.aspx?uid={0}", m_dataset.ownerId);
            HL_Domain.Text = m_dataset.domainName;
            HL_Domain.NavigateUrl = string.Format("~/public/services/domain.aspx?did={0}", m_dataset.domainId);

            long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("cid"), out m_selectedClientId);
            long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("sid"), out m_selectedSiteId);

            foreach (SiteData siteData in m_dataset.sites)
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

            if (this.IsPostBack == false)
            {
                long scrollId;
                if (long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("sp"), out scrollId))
                {
                    Control spTextbox = PH_Sites.FindControl(string.Format("ScrollPosition_{0}", scrollId));
                    if (spTextbox != null)
                        spTextbox.Focus();
                }
                else if (long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("sid"), out scrollId))
                {
                    Control spTextbox = PH_Sites.FindControl(string.Format("ScrollPosition_{0}", scrollId));
                    if (spTextbox != null)
                        spTextbox.Focus();
                }
            }
        }
        catch (InvalidStateSoftnetException)
        {
            Response.Redirect("~/public/clients/domains.aspx?" + m_browsingUrlParams);
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void SelectClient_Click(object sender, EventArgs e)
    {
        TButton tButton = (TButton)sender;
        SiteData siteData = (SiteData)tButton.Args[0];
        ClientData clientData = (ClientData)tButton.Args[1];
        Response.Redirect(string.Format("~/public/clients/domain.aspx?did={0}&cid={1}&sp={2}&", m_dataset.domainId, clientData.clientId, siteData.siteId) + m_browsingUrlParams);
    }

    protected void UnselectClient_Click(object sender, EventArgs e)
    {
        TButton tButton = (TButton)sender;
        SiteData siteData = (SiteData)tButton.Args[0];
        Response.Redirect(string.Format("~/public/clients/domain.aspx?did={0}&sp={1}&", m_dataset.domainId, siteData.siteId) + m_browsingUrlParams);
    }

    protected void NewClient_Click(object sender, EventArgs e)
    {
        TButton tButton = (TButton)sender;
        SiteData siteData = (SiteData)tButton.Args[0];
        Response.Redirect(string.Format("~/public/clients/domain.aspx?did={0}&sid={1}&", m_dataset.domainId, siteData.siteId) + m_browsingUrlParams);
    }

    protected void CancelNewClient_Click(object sender, EventArgs e)
    {
        TButton tButton = (TButton)sender;
        SiteData siteData = (SiteData)tButton.Args[0];
        Response.Redirect(string.Format("~/public/clients/domain.aspx?did={0}&sp={1}&", m_dataset.domainId, siteData.siteId) + m_browsingUrlParams);
    }

    protected void SendAccountAccessUrl_Click(object sender, EventArgs e)
    {        
        TButton tButton = (TButton)sender;
        SiteData siteData = (SiteData)tButton.Args[0];
        ClientData clientData = (ClientData)tButton.Args[1];
        HtmlGenericControl tdMiddle = (HtmlGenericControl)tButton.Args[2];

        try
        {
            string accessKey = Randomizer.generateTransactionKey(Constants.Keys.transaction_key_length);
            accessKey = SoftnetRegistry.public_SaveAccountAccessKey(clientData.clientId, accessKey);

            MailingData data = new MailingData();
            SoftnetRegistry.settings_getMailingData(data);

            byte[] clientKeyBytes = Encoding.UTF8.GetBytes(clientData.clientKey);
            byte[] accessKeyBytes = Encoding.UTF8.GetBytes(accessKey);
            byte[] emailBytes = Encoding.UTF8.GetBytes(m_receivedEMail);            
            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(data.secretKey);

            byte[] buffer = new byte[clientKeyBytes.Length + accessKeyBytes.Length + emailBytes.Length + secretKeyBytes.Length];
            int offset = 0;
            System.Buffer.BlockCopy(clientKeyBytes, 0, buffer, offset, clientKeyBytes.Length);
            offset += clientKeyBytes.Length;
            System.Buffer.BlockCopy(accessKeyBytes, 0, buffer, offset, accessKeyBytes.Length);
            offset += accessKeyBytes.Length;
            System.Buffer.BlockCopy(emailBytes, 0, buffer, offset, emailBytes.Length);
            offset += emailBytes.Length;
            System.Buffer.BlockCopy(secretKeyBytes, 0, buffer, offset, secretKeyBytes.Length);

            byte[] hash = Sha1Hash.Compute(buffer);
            string base64Hash = Convert.ToBase64String(hash);

            var smtpClient = new SmtpClient(data.smtpServer, data.smtpPort);
            smtpClient.Credentials = new NetworkCredential(data.emailAddress, data.emailPassword);
            smtpClient.EnableSsl = data.smtpRequiresSSL;
            smtpClient.Timeout = 20000;

            string body =
                "This message contains a url that gives access to the account of the guest client <span style='font-weight:bold'>{0}</span>.<br/>" +
                "If you are the person who requested this url click <a href='{1}'>access the client's account</a>.";

            string confirmationUrl = string.Format("{0}/public/clients/account.aspx?ckey={1}&akey={2}&hash={3}", data.msUrl, clientData.clientKey, accessKey, HttpUtility.UrlEncode(base64Hash));

            var fromAddress = new MailAddress(data.emailAddress, data.siteAddress);
            var toAddress = new MailAddress(m_receivedEMail);

            MailMessage mailMessage = new MailMessage(fromAddress, toAddress);
            mailMessage.Subject = string.Format("Confirmation mail from <{0}>", data.siteAddress);
            if(siteData.siteKind == 1)
                mailMessage.Body = string.Format(body, clientData.clientKey + "-s", confirmationUrl);
            else
                mailMessage.Body = string.Format(body, clientData.clientKey + "-m", confirmationUrl);
            mailMessage.IsBodyHtml = true;

            smtpClient.Send(mailMessage);

            tdMiddle.Controls.Clear();

            HtmlGenericControl span = new HtmlGenericControl("span");
            tdMiddle.Controls.Add(span);
            span.Attributes["style"] = "display:block; margin-top: 10px; text-align: center; font-size: 1.2em; color: #3C6C80;";
            span.InnerHtml = "The message has been successfully sent!";

            HtmlGenericControl table = new HtmlGenericControl("table");
            tdMiddle.Controls.Add(table);
            table.Attributes["class"] = "wide_table";
            table.Attributes["style"] = "margin-top: 10px; margin-bottom: 10px;";
            HtmlGenericControl tr = new HtmlGenericControl("tr");
            table.Controls.Add(tr);

            HtmlGenericControl td = new HtmlGenericControl("td");
            tr.Controls.Add(td);
            td.Attributes["class"] = "wide_table width_50";

            HtmlGenericControl tdButton = new HtmlGenericControl("td");
            tr.Controls.Add(tdButton);
            tdButton.Attributes["class"] = "wide_table";

            td = new HtmlGenericControl("td");
            tr.Controls.Add(td);
            td.Attributes["class"] = "wide_table width_50";

            HtmlGenericControl divOkButton = new HtmlGenericControl("div");
            tdButton.Controls.Add(divOkButton);
            divOkButton.Attributes["class"] = "SubmitButton Gray";

            Button buttonOk = new Button();
            divOkButton.Controls.Add(buttonOk);
            buttonOk.Text = "ok";
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void SendGuestCreatingUrl_Click(object sender, EventArgs e)
    {
        TButton tbutton = (TButton)sender;
        SiteData siteData = (SiteData)tbutton.Args[0];
        HtmlGenericControl controlContainer = (HtmlGenericControl)tbutton.Args[1];

        try
        {
            MailingData data = new MailingData();
            SoftnetRegistry.settings_getMailingData(data);

            string transactionKey = Randomizer.generateTransactionKey(Constants.Keys.transaction_key_length);
            string time = data.currentTime.ToString();

            byte[] siteKeyBytes = Encoding.UTF8.GetBytes(siteData.siteKey);
            byte[] transactionKeyBytes = Encoding.UTF8.GetBytes(transactionKey);
            byte[] emailBytes = Encoding.UTF8.GetBytes(m_receivedEMail);
            byte[] timeBytes = Encoding.UTF8.GetBytes(time);
            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(data.secretKey);

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

            var smtpClient = new SmtpClient(data.smtpServer, data.smtpPort);
            smtpClient.Credentials = new NetworkCredential(data.emailAddress, data.emailPassword);
            smtpClient.EnableSsl = data.smtpRequiresSSL;
            smtpClient.Timeout = 20000;

            string body =
                "This message contains a url for creating a guest client on <span style='font-weight:bold'>{0}</span>.<br/>" +
                "If you are the person who requested this url click <a href='{1}'>create a guest client</a>.";

            string confirmationUrl = string.Format("{0}/public/clients/guest.aspx?skey={1}&tkey={2}&email={3}&time={4}&hash={5}", data.msUrl, siteData.siteKey, transactionKey, HttpUtility.UrlEncode(m_receivedEMail), time, HttpUtility.UrlEncode(base64Hash));

            var fromAddress = new MailAddress(data.emailAddress, data.siteAddress);
            var toAddress = new MailAddress(m_receivedEMail);

            MailMessage mailMessage = new MailMessage(fromAddress, toAddress);
            mailMessage.Subject = string.Format("Confirmation mail from <{0}>", data.siteAddress);
            mailMessage.Body = string.Format(body, data.siteAddress, confirmationUrl);
            mailMessage.IsBodyHtml = true;

            smtpClient.Send(mailMessage);

            controlContainer.Controls.Clear();

            HtmlGenericControl span = new HtmlGenericControl("span");
            controlContainer.Controls.Add(span);
            span.Attributes["style"] = "display:block; text-align: center; font-size: 1.1em; color: #3C6C80;";
            span.InnerHtml = "The message has been successfully sent!";

            HtmlGenericControl table = new HtmlGenericControl("table");
            controlContainer.Controls.Add(table);
            table.Attributes["class"] = "wide_table";
            table.Attributes["style"] = "margin-top: 10px;";
            HtmlGenericControl tr = new HtmlGenericControl("tr");
            table.Controls.Add(tr);

            HtmlGenericControl td = new HtmlGenericControl("td");
            tr.Controls.Add(td);
            td.Attributes["class"] = "wide_table width_50";

            HtmlGenericControl tdMiddle = new HtmlGenericControl("td");
            tr.Controls.Add(tdMiddle);
            tdMiddle.Attributes["class"] = "wide_table";

            td = new HtmlGenericControl("td");
            tr.Controls.Add(td);
            td.Attributes["class"] = "wide_table width_50";

            HtmlGenericControl divOkButton = new HtmlGenericControl("div");
            tdMiddle.Controls.Add(divOkButton);
            divOkButton.Attributes["class"] = "SubmitButton Gray";

            Button buttonOk = new Button();
            divOkButton.Controls.Add(buttonOk);
            buttonOk.Text = "ok";
        }        
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
        catch (SmtpException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    void drawSSite(SiteData siteData)
    { 
        List<ClientData> clients = m_dataset.clients.FindAll(x => x.siteId == siteData.siteId);
        if (clients.Count == 0)
            return;

        List<ServiceData> services = m_dataset.services.FindAll(x => x.siteId == siteData.siteId);
        if (services.Count != 1)
            throw new DataIntegritySoftnetException();
        ServiceData serviceData = services[0];

        HtmlGenericControl divSiteFrame = new HtmlGenericControl("div");
        PH_Sites.Controls.Add(divSiteFrame);
        divSiteFrame.Attributes["class"] = "site_frame";

        TextBox textBoxScrollPosition = new TextBox();
        textBoxScrollPosition.ID = string.Format("ScrollPosition_{0}", siteData.siteId);
        PH_Sites.Controls.Add(textBoxScrollPosition);
        textBoxScrollPosition.Attributes["style"] = "display:block; width:1px; height:1px; border-width:0px; padding: 0px; margin:0px";

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

        if (siteData.enabled == false)
        {
            td = new HtmlGenericControl("td");
            tr.Controls.Add(td);
            td.Attributes["class"] = "wide_table";
            td.Attributes["style"] = "text-align: right; padding: 2px; padding-right: 5px;";
            HtmlGenericControl spanStatus = new HtmlGenericControl("span");
            td.Controls.Add(spanStatus);
            spanStatus.Attributes["class"] = "object_status";
            spanStatus.InnerText = "site disabled";
        }

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

        if (siteData.guestSupported)
        {
            if (m_dataset.guestEnabled)
            {
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
                    spanGuestPage.InnerText = string.Format("{0}/guest.aspx?site={1}", m_mgtSystemUrl, siteData.siteKey);

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
                else
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

                    HtmlGenericControl spanGuestStatus = new HtmlGenericControl("span");
                    divSiteBlockItem.Controls.Add(spanGuestStatus);
                    spanGuestStatus.InnerHtml = "<span class='name'>Guest</span>&nbsp;&nbsp;<span class='disabled_status'>denied</span>";
                }
            }
            else
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

                HtmlGenericControl spanGuestStatus = new HtmlGenericControl("span");
                divSiteBlockItem.Controls.Add(spanGuestStatus);
                spanGuestStatus.InnerHtml = "<span class='name'>Guest</span>&nbsp;&nbsp;<span class='disabled_status'>disabled</span>";
            }
        }
        else
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

            HtmlGenericControl spanGuestStatus = new HtmlGenericControl("span");
            divSiteBlockItem.Controls.Add(spanGuestStatus);
            spanGuestStatus.InnerHtml = "<span class='name'>Guest</span>&nbsp;&nbsp;<span class='disabled_status'>not supported</span>";
        }
            
        divSiteBlock = new HtmlGenericControl("div");
        divSiteBody.Controls.Add(divSiteBlock);
        divSiteBlock.Attributes["class"] = "site_block";

        divSiteBlockHeader = new HtmlGenericControl("div");
        divSiteBlock.Controls.Add(divSiteBlockHeader);
        divSiteBlockHeader.Attributes["class"] = "site_block_header left";
        divSiteBlockHeader.Attributes["style"] = "margin-bottom: 5px;"; 
        divSiteBlockHeader.InnerText = "guest clients";   
        
        if (siteData.guestSupported)
        {
            if (siteData.enabled)
            {
                if (m_dataset.guestEnabled)
                {
                    if (siteData.guestAllowed)
                    {
                        divSiteBlockItem = new HtmlGenericControl("div");
                        divSiteBlock.Controls.Add(divSiteBlockItem);
                        divSiteBlockItem.Attributes["class"] = "site_block_item";

                        if (siteData.siteId != m_selectedSiteId)
                        {
                            table = new HtmlGenericControl("table");
                            divSiteBlockItem.Controls.Add(table);
                            table.Attributes["class"] = "auto_table";

                            tr = new HtmlGenericControl("tr");
                            table.Controls.Add(tr);

                            td = new HtmlGenericControl("td");
                            tr.Controls.Add(td);
                            td.Attributes["class"] = "auto_table";

                            HtmlGenericControl divNewClientButton = new HtmlGenericControl("div");
                            td.Controls.Add(divNewClientButton);
                            divNewClientButton.Attributes["class"] = "SubmitButtonSquare Blue";

                            TButton buttonNewClient = new TButton();
                            divNewClientButton.Controls.Add(buttonNewClient);
                            buttonNewClient.Args.Add(siteData);
                            buttonNewClient.Text = "new guest client";
                            buttonNewClient.ID = string.Format("B_NewClient_{0}", siteData.siteId);
                            buttonNewClient.Click += new EventHandler(NewClient_Click);
                        }
                        else
                        {
                            HtmlGenericControl divDashedFrame = new HtmlGenericControl("div");
                            divSiteBlockItem.Controls.Add(divDashedFrame);
                            divDashedFrame.Attributes["style"] = "border: 1px dashed gray; padding: 1px; padding-bottom: 10px;";

                            table = new HtmlGenericControl("table");
                            divDashedFrame.Controls.Add(table);
                            table.Attributes["class"] = "wide_table";

                            tr = new HtmlGenericControl("tr");
                            table.Controls.Add(tr);

                            tdLeft = new HtmlGenericControl("td");
                            tr.Controls.Add(tdLeft);
                            tdLeft.Attributes["class"] = "wide_table_vtop";
                            tdLeft.Attributes["style"] = "width: 125px;";

                            tdRight = new HtmlGenericControl("td");
                            tr.Controls.Add(tdRight);
                            tdRight.Attributes["class"] = "wide_table";

                            table = new HtmlGenericControl("table");
                            tdLeft.Controls.Add(table);
                            table.Attributes["class"] = "auto_table";
                            tr = new HtmlGenericControl("tr");
                            table.Controls.Add(tr);
                            td = new HtmlGenericControl("td");
                            tr.Controls.Add(td);
                            td.Attributes["class"] = "auto_table";

                            HtmlGenericControl divNewClientButton = new HtmlGenericControl("div");
                            td.Controls.Add(divNewClientButton);
                            divNewClientButton.Attributes["class"] = "SubmitButtonSquare Selected Blue";

                            TButton buttonNewClient = new TButton();
                            divNewClientButton.Controls.Add(buttonNewClient);
                            buttonNewClient.Args.Add(siteData);
                            buttonNewClient.Text = "new guest client";
                            buttonNewClient.ID = string.Format("B_CancelNewClient_{0}", siteData.siteId);
                            buttonNewClient.Click += new EventHandler(CancelNewClient_Click);

                            span = new HtmlGenericControl("span");
                            tdRight.Controls.Add(span);
                            span.Attributes["style"] = "font-size: 1.1em; color: #3C6C80;";
                            span.InnerHtml = "In order to create a new client, you need first to confirm the email <span style='color: #CF5400'>" + m_receivedEMail + "</span>. " +
                                "Click the button <span style='font-weight:bold;'>send confirmation mail</span> then follow the url from the inbox of the email.";

                            table = new HtmlGenericControl("table");
                            tdRight.Controls.Add(table);
                            table.Attributes["class"] = "wide_table";
                            table.Attributes["style"] = "margin-top: 10px;";
                            tr = new HtmlGenericControl("tr");
                            table.Controls.Add(tr);

                            td = new HtmlGenericControl("td");
                            tr.Controls.Add(td);
                            td.Attributes["class"] = "wide_table width_50";

                            HtmlGenericControl tdButton = new HtmlGenericControl("td");
                            tr.Controls.Add(tdButton);
                            tdButton.Attributes["class"] = "wide_table";

                            td = new HtmlGenericControl("td");
                            tr.Controls.Add(td);
                            td.Attributes["class"] = "wide_table width_50";

                            HtmlGenericControl divSendMessageButton = new HtmlGenericControl("div");
                            tdButton.Controls.Add(divSendMessageButton);
                            divSendMessageButton.Attributes["class"] = "SubmitButtonMini Green";

                            TButton buttonSendMessage = new TButton();
                            divSendMessageButton.Controls.Add(buttonSendMessage);
                            buttonSendMessage.Args.Add(siteData);
                            buttonSendMessage.Args.Add(tdRight);
                            buttonSendMessage.Text = "send confirmation mail";
                            buttonSendMessage.ID = string.Format("B_SendGuestCreatingUrl_{0}", siteData.siteId);
                            buttonSendMessage.Click += new EventHandler(SendGuestCreatingUrl_Click);                           
                        }
                    }
                    else
                    {
                        divSiteBlockItem = new HtmlGenericControl("div");
                        divSiteBlock.Controls.Add(divSiteBlockItem);
                        divSiteBlockItem.Attributes["class"] = "site_block_item underline";

                        HtmlGenericControl spanWarning = new HtmlGenericControl("span");
                        divSiteBlockItem.Controls.Add(spanWarning);
                        spanWarning.Attributes["style"] = "display: block; text-align: center; color: #E86100; font-size: 1.2em;";
                        spanWarning.InnerHtml = "The guest is denied. Creating a new client is unavailable.";
                    }
                }
                else
                {
                    divSiteBlockItem = new HtmlGenericControl("div");
                    divSiteBlock.Controls.Add(divSiteBlockItem);
                    divSiteBlockItem.Attributes["class"] = "site_block_item underline";

                    HtmlGenericControl spanWarning = new HtmlGenericControl("span");
                    divSiteBlockItem.Controls.Add(spanWarning);
                    spanWarning.Attributes["style"] = "display: block; text-align: center; color: #E86100; font-size: 1.2em;";
                    spanWarning.InnerHtml = "The guest is disabled. Creating a new client is unavailable.";
                }
            }
            else
            {
                divSiteBlockItem = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockItem);
                divSiteBlockItem.Attributes["class"] = "site_block_item underline";

                HtmlGenericControl spanWarning = new HtmlGenericControl("span");
                divSiteBlockItem.Controls.Add(spanWarning);
                spanWarning.Attributes["style"] = "display: block; text-align: center; color: #E86100; font-size: 1.2em;";
                spanWarning.InnerHtml = "The site is disabled. Creating a new client is unavailable.";
            }
        }
        else
        {
            divSiteBlockItem = new HtmlGenericControl("div");
            divSiteBlock.Controls.Add(divSiteBlockItem);
            divSiteBlockItem.Attributes["class"] = "site_block_item underline";

            HtmlGenericControl spanWarning = new HtmlGenericControl("span");
            divSiteBlockItem.Controls.Add(spanWarning);
            spanWarning.Attributes["style"] = "display: block; text-align: center; color: #E86100; font-size: 1.2em;";
            spanWarning.InnerHtml = "The guest access is not supported. Creating a new client is unavailable.";
        }

        for (int i = 0; i < clients.Count; i++)
        {
            ClientData clientData = clients[i];
                
            divSiteBlockItem = new HtmlGenericControl("div");
            divSiteBlock.Controls.Add(divSiteBlockItem);
            divSiteBlockItem.Attributes["class"] = "site_block_item";

            if (clientData.clientId != m_selectedClientId)
            {
                HtmlGenericControl tableLayout = new HtmlGenericControl("table");
                divSiteBlockItem.Controls.Add(tableLayout);
                tableLayout.Attributes["class"] = "wide_table";
                tr = new HtmlGenericControl("tr");
                tableLayout.Controls.Add(tr);

                tdLeft = new HtmlGenericControl("td");
                tr.Controls.Add(tdLeft);
                tdLeft.Attributes["class"] = "wide_table";
                tdLeft.Attributes["style"] = "width: 47px; padding-right: 10px;";

                tdRight = new HtmlGenericControl("td");
                tr.Controls.Add(tdRight);
                tdRight.Attributes["class"] = "wide_table";

                HtmlGenericControl divEditClientButton = new HtmlGenericControl("div");
                tdLeft.Controls.Add(divEditClientButton);
                divEditClientButton.Attributes["class"] = "SubmitButtonMini Blue";

                TButton buttonEditClient = new TButton();
                divEditClientButton.Controls.Add(buttonEditClient);
                buttonEditClient.Args.Add(siteData);
                buttonEditClient.Args.Add(clientData);
                buttonEditClient.Text = ">>";
                buttonEditClient.ID = string.Format("B_SelectClient_{0}", clientData.clientId);
                buttonEditClient.Click += new EventHandler(SelectClient_Click);

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
            else
            {
                HtmlGenericControl tableLayout = new HtmlGenericControl("table");
                divSiteBlockItem.Controls.Add(tableLayout);
                tableLayout.Attributes["class"] = "wide_table";
                tableLayout.Attributes["style"] = "border: 1px dashed #696969;";
                tr = new HtmlGenericControl("tr");
                tableLayout.Controls.Add(tr);
                tr.Attributes["style"] = "background-color: #E0E0E0;";

                tdLeft = new HtmlGenericControl("td");
                tr.Controls.Add(tdLeft);
                tdLeft.Attributes["class"] = "wide_table";
                tdLeft.Attributes["style"] = "width: 47px; padding-right: 10px;";

                HtmlGenericControl tdMiddle = new HtmlGenericControl("td");
                tr.Controls.Add(tdMiddle);
                tdMiddle.Attributes["class"] = "wide_table";
                tdMiddle.Attributes["style"] = "padding-right: 3px;";

                tdRight = new HtmlGenericControl("td");
                tr.Controls.Add(tdRight);
                tdRight.Attributes["class"] = "wide_table";
                tdRight.Attributes["style"] = "width: 22px; padding-bottom: 1px;";

                HtmlGenericControl divViewClientButton = new HtmlGenericControl("div");
                tdLeft.Controls.Add(divViewClientButton);
                divViewClientButton.Attributes["class"] = "SubmitButtonMini Selected Blue";

                TButton buttonViewClient = new TButton();
                divViewClientButton.Controls.Add(buttonViewClient);
                buttonViewClient.Args.Add(siteData);
                buttonViewClient.Text = "<<";
                buttonViewClient.ID = string.Format("B_UnselectClient_{0}", clientData.clientId);
                buttonViewClient.Click += new EventHandler(UnselectClient_Click);                    

                Panel panelClientBody = new Panel();
                tdMiddle.Controls.Add(panelClientBody);

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

                tr = new HtmlGenericControl("tr");
                tableLayout.Controls.Add(tr);

                tdLeft = new HtmlGenericControl("td");
                tr.Controls.Add(tdLeft);
                tdLeft.Attributes["class"] = "wide_table";

                tdMiddle = new HtmlGenericControl("td");
                tr.Controls.Add(tdMiddle);
                tdMiddle.Attributes["class"] = "wide_table";                    

                tdRight = new HtmlGenericControl("td");
                tr.Controls.Add(tdRight);
                tdRight.Attributes["class"] = "wide_table";

                span = new HtmlGenericControl("span");
                tdMiddle.Controls.Add(span);
                span.Attributes["style"] = "display:block; margin-top: 10px; font-size: 1.1em; color: #3C6C80;";
                span.InnerHtml =
                    "In order to get an access to the client's account, click the button <span style='font-weight:bold;white-space:nowrap;'>send confirmation mail</span> "+
                    "then follow the url from the inbox of the email <span style='color: #CF5400'>" + m_receivedEMail + "</span>.";                    

                table = new HtmlGenericControl("table");
                tdMiddle.Controls.Add(table);
                table.Attributes["class"] = "wide_table";
                table.Attributes["style"] = "margin-top: 10px; margin-bottom: 10px;";
                tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "wide_table width_50";

                HtmlGenericControl tdButton = new HtmlGenericControl("td");
                tr.Controls.Add(tdButton);
                tdButton.Attributes["class"] = "wide_table";

                td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "wide_table width_50";

                HtmlGenericControl divSendMessageButton = new HtmlGenericControl("div");
                tdButton.Controls.Add(divSendMessageButton);
                divSendMessageButton.Attributes["class"] = "SubmitButtonMini Green";

                TButton buttonSendMessage = new TButton();
                divSendMessageButton.Controls.Add(buttonSendMessage);
                buttonSendMessage.Args.Add(siteData);
                buttonSendMessage.Args.Add(clientData);
                buttonSendMessage.Args.Add(tdMiddle);                    
                buttonSendMessage.Text = "send confirmation mail";
                buttonSendMessage.ID = string.Format("B_SendAccessUrl_{0}", siteData.siteId);
                buttonSendMessage.Click += new EventHandler(SendAccountAccessUrl_Click);
            }                
        }
    }

    void drawMSite(SiteData siteData)
    {
        List<ClientData> clients = m_dataset.clients.FindAll(x => x.siteId == siteData.siteId);
        if (clients.Count == 0)
            return;

        HtmlGenericControl divSiteFrame = new HtmlGenericControl("div");
        PH_Sites.Controls.Add(divSiteFrame);
        divSiteFrame.Attributes["class"] = "site_frame";

        TextBox textBoxScrollPosition = new TextBox();
        textBoxScrollPosition.ID = string.Format("ScrollPosition_{0}", siteData.siteId);
        PH_Sites.Controls.Add(textBoxScrollPosition);
        textBoxScrollPosition.Attributes["style"] = "display:block; width:1px; height:1px; border-width:0px; padding: 0px; margin:0px";

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

        if (siteData.enabled == false)
        {
            td = new HtmlGenericControl("td");
            tr.Controls.Add(td);
            td.Attributes["class"] = "wide_table";
            td.Attributes["style"] = "text-align: right; padding: 2px; padding-right: 5px;";
            HtmlGenericControl spanStatus = new HtmlGenericControl("span");
            td.Controls.Add(spanStatus);
            spanStatus.Attributes["class"] = "object_status";
            spanStatus.InnerText = "site disabled";
        }

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

        if (siteData.guestSupported)
        {
            if (m_dataset.guestEnabled)
            {
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
                    spanGuestPage.InnerText = string.Format("{0}/guest.aspx?site={1}", m_mgtSystemUrl, siteData.siteKey);

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
                else
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

                    HtmlGenericControl spanGuestStatus = new HtmlGenericControl("span");
                    divSiteBlockItem.Controls.Add(spanGuestStatus);
                    spanGuestStatus.InnerHtml = "<span class='name'>Guest</span>&nbsp;&nbsp;<span class='disabled_status'>denied</span>";
                }
            }
            else
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

                HtmlGenericControl spanGuestStatus = new HtmlGenericControl("span");
                divSiteBlockItem.Controls.Add(spanGuestStatus);
                spanGuestStatus.InnerHtml = "<span class='name'>Guest</span>&nbsp;&nbsp;<span class='disabled_status'>disabled</span>";
            }
        }
        else
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

            HtmlGenericControl spanGuestStatus = new HtmlGenericControl("span");
            divSiteBlockItem.Controls.Add(spanGuestStatus);
            spanGuestStatus.InnerHtml = "<span class='name'>Guest</span>&nbsp;&nbsp;<span class='disabled_status'>not supported</span>";
        }

        divSiteBlock = new HtmlGenericControl("div");
        divSiteBody.Controls.Add(divSiteBlock);
        divSiteBlock.Attributes["class"] = "site_block";

        divSiteBlockHeader = new HtmlGenericControl("div");
        divSiteBlock.Controls.Add(divSiteBlockHeader);
        divSiteBlockHeader.Attributes["class"] = "site_block_header left";
        divSiteBlockHeader.Attributes["style"] = "margin-bottom: 5px;";
        divSiteBlockHeader.InnerText = "guest clients";

        if (siteData.guestSupported)
        {
            if (siteData.enabled)
            {
                if (m_dataset.guestEnabled)
                {
                    if (siteData.guestAllowed)
                    {
                        divSiteBlockItem = new HtmlGenericControl("div");
                        divSiteBlock.Controls.Add(divSiteBlockItem);
                        divSiteBlockItem.Attributes["class"] = "site_block_item";

                        if (siteData.siteId != m_selectedSiteId)
                        {
                            table = new HtmlGenericControl("table");
                            divSiteBlockItem.Controls.Add(table);
                            table.Attributes["class"] = "auto_table";

                            tr = new HtmlGenericControl("tr");
                            table.Controls.Add(tr);

                            td = new HtmlGenericControl("td");
                            tr.Controls.Add(td);
                            td.Attributes["class"] = "auto_table";

                            HtmlGenericControl divNewClientButton = new HtmlGenericControl("div");
                            td.Controls.Add(divNewClientButton);
                            divNewClientButton.Attributes["class"] = "SubmitButtonSquare Blue";

                            TButton buttonNewClient = new TButton();
                            divNewClientButton.Controls.Add(buttonNewClient);
                            buttonNewClient.Args.Add(siteData);
                            buttonNewClient.Text = "new guest client";
                            buttonNewClient.ID = string.Format("B_NewClient_{0}", siteData.siteId);
                            buttonNewClient.Click += new EventHandler(NewClient_Click);
                        }
                        else
                        {
                            HtmlGenericControl divDashedFrame = new HtmlGenericControl("div");
                            divSiteBlockItem.Controls.Add(divDashedFrame);
                            divDashedFrame.Attributes["style"] = "border: 1px dashed gray; padding: 1px; padding-bottom: 10px;";

                            table = new HtmlGenericControl("table");
                            divDashedFrame.Controls.Add(table);
                            table.Attributes["class"] = "wide_table";

                            tr = new HtmlGenericControl("tr");
                            table.Controls.Add(tr);

                            tdLeft = new HtmlGenericControl("td");
                            tr.Controls.Add(tdLeft);
                            tdLeft.Attributes["class"] = "wide_table_vtop";
                            tdLeft.Attributes["style"] = "width: 125px;";

                            tdRight = new HtmlGenericControl("td");
                            tr.Controls.Add(tdRight);
                            tdRight.Attributes["class"] = "wide_table";

                            table = new HtmlGenericControl("table");
                            tdLeft.Controls.Add(table);
                            table.Attributes["class"] = "auto_table";
                            tr = new HtmlGenericControl("tr");
                            table.Controls.Add(tr);
                            td = new HtmlGenericControl("td");
                            tr.Controls.Add(td);
                            td.Attributes["class"] = "auto_table";

                            HtmlGenericControl divNewClientButton = new HtmlGenericControl("div");
                            td.Controls.Add(divNewClientButton);
                            divNewClientButton.Attributes["class"] = "SubmitButtonSquare Selected Blue";

                            TButton buttonNewClient = new TButton();
                            divNewClientButton.Controls.Add(buttonNewClient);
                            buttonNewClient.Args.Add(siteData);
                            buttonNewClient.Text = "new guest client";
                            buttonNewClient.ID = string.Format("B_CancelNewClient_{0}", siteData.siteId);
                            buttonNewClient.Click += new EventHandler(CancelNewClient_Click);

                            span = new HtmlGenericControl("span");
                            tdRight.Controls.Add(span);
                            span.Attributes["style"] = "font-size: 1.1em; color: #3C6C80;";
                            span.InnerHtml = "In order to create a new client, you need first to confirm the email <span style='color: #CF5400'>" + m_receivedEMail + "</span>. " +
                                "Click the button <span style='font-weight:bold;'>send confirmation mail</span> then follow the url from the inbox of the email.";

                            table = new HtmlGenericControl("table");
                            tdRight.Controls.Add(table);
                            table.Attributes["class"] = "wide_table";
                            table.Attributes["style"] = "margin-top: 10px;";
                            tr = new HtmlGenericControl("tr");
                            table.Controls.Add(tr);

                            td = new HtmlGenericControl("td");
                            tr.Controls.Add(td);
                            td.Attributes["class"] = "wide_table width_50";

                            HtmlGenericControl tdButton = new HtmlGenericControl("td");
                            tr.Controls.Add(tdButton);
                            tdButton.Attributes["class"] = "wide_table";

                            td = new HtmlGenericControl("td");
                            tr.Controls.Add(td);
                            td.Attributes["class"] = "wide_table width_50";

                            HtmlGenericControl divSendMessageButton = new HtmlGenericControl("div");
                            tdButton.Controls.Add(divSendMessageButton);
                            divSendMessageButton.Attributes["class"] = "SubmitButtonMini Green";

                            TButton buttonSendMessage = new TButton();
                            divSendMessageButton.Controls.Add(buttonSendMessage);
                            buttonSendMessage.Args.Add(siteData);
                            buttonSendMessage.Args.Add(tdRight);
                            buttonSendMessage.Text = "send confirmation mail";
                            buttonSendMessage.ID = string.Format("B_SendGuestCreatingUrl_{0}", siteData.siteId);
                            buttonSendMessage.Click += new EventHandler(SendGuestCreatingUrl_Click);
                        }
                    }
                    else
                    {
                        divSiteBlockItem = new HtmlGenericControl("div");
                        divSiteBlock.Controls.Add(divSiteBlockItem);
                        divSiteBlockItem.Attributes["class"] = "site_block_item underline";

                        HtmlGenericControl spanWarning = new HtmlGenericControl("span");
                        divSiteBlockItem.Controls.Add(spanWarning);
                        spanWarning.Attributes["style"] = "display: block; text-align: center; color: #E86100; font-size: 1.2em;";
                        spanWarning.InnerHtml = "The guest is denied. Creating a new client is unavailable.";
                    }
                }
                else
                {
                    divSiteBlockItem = new HtmlGenericControl("div");
                    divSiteBlock.Controls.Add(divSiteBlockItem);
                    divSiteBlockItem.Attributes["class"] = "site_block_item underline";

                    HtmlGenericControl spanWarning = new HtmlGenericControl("span");
                    divSiteBlockItem.Controls.Add(spanWarning);
                    spanWarning.Attributes["style"] = "display: block; text-align: center; color: #E86100; font-size: 1.2em;";
                    spanWarning.InnerHtml = "The guest is disabled. Creating a new client is unavailable.";
                }
            }
            else
            {
                divSiteBlockItem = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divSiteBlockItem);
                divSiteBlockItem.Attributes["class"] = "site_block_item underline";

                HtmlGenericControl spanWarning = new HtmlGenericControl("span");
                divSiteBlockItem.Controls.Add(spanWarning);
                spanWarning.Attributes["style"] = "display: block; text-align: center; color: #E86100; font-size: 1.2em;";
                spanWarning.InnerHtml = "The site is disabled. Creating a new client is unavailable.";
            }
        }
        else
        {
            divSiteBlockItem = new HtmlGenericControl("div");
            divSiteBlock.Controls.Add(divSiteBlockItem);
            divSiteBlockItem.Attributes["class"] = "site_block_item underline";

            HtmlGenericControl spanWarning = new HtmlGenericControl("span");
            divSiteBlockItem.Controls.Add(spanWarning);
            spanWarning.Attributes["style"] = "display: block; text-align: center; color: #E86100; font-size: 1.2em;";
            spanWarning.InnerHtml = "The guest access is not supported. Creating a new client is unavailable.";
        }

        for (int i = 0; i < clients.Count; i++)
        {
            ClientData clientData = clients[i];

            divSiteBlockItem = new HtmlGenericControl("div");
            divSiteBlock.Controls.Add(divSiteBlockItem);
            divSiteBlockItem.Attributes["class"] = "site_block_item";

            if (clientData.clientId != m_selectedClientId)
            {
                HtmlGenericControl tableLayout = new HtmlGenericControl("table");
                divSiteBlockItem.Controls.Add(tableLayout);
                tableLayout.Attributes["class"] = "wide_table";
                tr = new HtmlGenericControl("tr");
                tableLayout.Controls.Add(tr);

                tdLeft = new HtmlGenericControl("td");
                tr.Controls.Add(tdLeft);
                tdLeft.Attributes["class"] = "wide_table";
                tdLeft.Attributes["style"] = "width: 47px; padding-right: 10px;";

                tdRight = new HtmlGenericControl("td");
                tr.Controls.Add(tdRight);
                tdRight.Attributes["class"] = "wide_table";

                HtmlGenericControl divEditClientButton = new HtmlGenericControl("div");
                tdLeft.Controls.Add(divEditClientButton);
                divEditClientButton.Attributes["class"] = "SubmitButtonMini Blue";

                TButton buttonEditClient = new TButton();
                divEditClientButton.Controls.Add(buttonEditClient);
                buttonEditClient.Args.Add(siteData);
                buttonEditClient.Args.Add(clientData);
                buttonEditClient.Text = ">>";
                buttonEditClient.ID = string.Format("B_SelectClient_{0}", clientData.clientId);
                buttonEditClient.Click += new EventHandler(SelectClient_Click);

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
            else
            {
                HtmlGenericControl tableLayout = new HtmlGenericControl("table");
                divSiteBlockItem.Controls.Add(tableLayout);
                tableLayout.Attributes["class"] = "wide_table";
                tableLayout.Attributes["style"] = "border: 1px dashed #696969;";
                tr = new HtmlGenericControl("tr");
                tableLayout.Controls.Add(tr);
                tr.Attributes["style"] = "background-color: #E0E0E0;";

                tdLeft = new HtmlGenericControl("td");
                tr.Controls.Add(tdLeft);
                tdLeft.Attributes["class"] = "wide_table";
                tdLeft.Attributes["style"] = "width: 47px; padding-right: 10px;";

                HtmlGenericControl tdMiddle = new HtmlGenericControl("td");
                tr.Controls.Add(tdMiddle);
                tdMiddle.Attributes["class"] = "wide_table";
                tdMiddle.Attributes["style"] = "padding-right: 3px;";

                tdRight = new HtmlGenericControl("td");
                tr.Controls.Add(tdRight);
                tdRight.Attributes["class"] = "wide_table";
                tdRight.Attributes["style"] = "width: 22px; padding-bottom: 1px;";

                HtmlGenericControl divViewClientButton = new HtmlGenericControl("div");
                tdLeft.Controls.Add(divViewClientButton);
                divViewClientButton.Attributes["class"] = "SubmitButtonMini Selected Blue";

                TButton buttonViewClient = new TButton();
                divViewClientButton.Controls.Add(buttonViewClient);
                buttonViewClient.Args.Add(siteData);
                buttonViewClient.Text = "<<";
                buttonViewClient.ID = string.Format("B_UnselectClient_{0}", clientData.clientId);
                buttonViewClient.Click += new EventHandler(UnselectClient_Click);

                Panel panelClientBody = new Panel();
                tdMiddle.Controls.Add(panelClientBody);

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

                tr = new HtmlGenericControl("tr");
                tableLayout.Controls.Add(tr);

                tdLeft = new HtmlGenericControl("td");
                tr.Controls.Add(tdLeft);
                tdLeft.Attributes["class"] = "wide_table";

                tdMiddle = new HtmlGenericControl("td");
                tr.Controls.Add(tdMiddle);
                tdMiddle.Attributes["class"] = "wide_table";

                tdRight = new HtmlGenericControl("td");
                tr.Controls.Add(tdRight);
                tdRight.Attributes["class"] = "wide_table";

                span = new HtmlGenericControl("span");
                tdMiddle.Controls.Add(span);
                span.Attributes["style"] = "display:block; margin-top: 10px; font-size: 1.1em; color: #3C6C80;";
                span.InnerHtml =
                    "In order to get an access to the client's account, click the button <span style='font-weight:bold;white-space:nowrap;'>send confirmation mail</span> " +
                    "then follow the url from the inbox of the email <span style='color: #CF5400'>" + m_receivedEMail + "</span>.";                    

                table = new HtmlGenericControl("table");
                tdMiddle.Controls.Add(table);
                table.Attributes["class"] = "wide_table";
                table.Attributes["style"] = "margin-top: 10px; margin-bottom: 10px;";
                tr = new HtmlGenericControl("tr");
                table.Controls.Add(tr);

                td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "wide_table width_50";

                HtmlGenericControl tdButton = new HtmlGenericControl("td");
                tr.Controls.Add(tdButton);
                tdButton.Attributes["class"] = "wide_table";

                td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                td.Attributes["class"] = "wide_table width_50";

                HtmlGenericControl divSendMessageButton = new HtmlGenericControl("div");
                tdButton.Controls.Add(divSendMessageButton);
                divSendMessageButton.Attributes["class"] = "SubmitButtonMini Green";

                TButton buttonSendMessage = new TButton();
                divSendMessageButton.Controls.Add(buttonSendMessage);
                buttonSendMessage.Args.Add(siteData);
                buttonSendMessage.Args.Add(clientData);
                buttonSendMessage.Args.Add(tdMiddle);
                buttonSendMessage.Text = "send confirmation mail";
                buttonSendMessage.ID = string.Format("B_SendAccessUrl_{0}", siteData.siteId);
                buttonSendMessage.Click += new EventHandler(SendAccountAccessUrl_Click);
            }
        }
    }
}