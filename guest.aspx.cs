/*
*	Copyright 2023 Robert Koifman
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

public partial class guest : System.Web.UI.Page
{
    PublicSiteDataset m_dataset;
    long m_editedClientId;

    protected void Refresh_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Format("~/guest.aspx?site={0}", m_dataset.siteKey));
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            string siteKey = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("site");
            if (string.IsNullOrWhiteSpace(siteKey))
                throw new ArgumentSoftnetException("The site key is not provided.");

            if (this.Context.User.Identity.IsAuthenticated)
            {
                m_dataset = new PublicSiteDataset();
                SoftnetRegistry.public_getSiteDataset(this.Context.User.Identity.Name, siteKey, m_dataset);

                long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("cid"), out m_editedClientId);

                if (m_dataset.siteData.siteKind == Constants.SiteKind.SingleService)
                {
                    drawSSite(m_dataset.siteData);
                }
                else
                {
                    drawMSite(m_dataset.siteData);
                }
            }
            else
            {
                m_dataset = new PublicSiteDataset();
                SoftnetRegistry.public_getSiteDataset(siteKey, m_dataset);

                if (m_dataset.siteData.siteKind == Constants.SiteKind.SingleService)
                {
                    drawSSite2(m_dataset.siteData);
                }
                else
                {
                    drawMSite2(m_dataset.siteData);
                }
            }

            HL_Owner.NavigateUrl = string.Format("~/public/services/domains.aspx?uid={0}", m_dataset.ownerId);
            HL_Owner.Text = m_dataset.ownerName;
            
            HL_Domain.NavigateUrl = string.Format("~/public/services/domain.aspx?did={0}", m_dataset.domainId);
            HL_Domain.Text = m_dataset.domainName;
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void AddGuestClient_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        SiteData siteData = (SiteData)button.Args[0];
        UserData userData = (UserData)button.Args[1];

        try
        {
            long clientId = SoftnetRegistry.CreateGuestClient(m_dataset.creatorId, siteData.siteId, userData.userId);
            Response.Redirect(string.Format("~/guest.aspx?site={0}&cid={1}&cpr=1", siteData.siteKey, clientId));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void EditClient_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        SiteData siteData = (SiteData)button.Args[0];
        ClientData clientData = (ClientData)button.Args[1];
        Response.Redirect(string.Format("~/guest.aspx?site={0}&cid={1}&cpr=1", siteData.siteKey, clientData.clientId));
    }

    protected void ViewClient_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        SiteData siteData = (SiteData)button.Args[0];
        Response.Redirect(string.Format("~/guest.aspx?site={0}", siteData.siteKey));
    }

    protected void DeleteClient_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        SiteData siteData = (SiteData)button.Args[0];
        ClientData clientData = (ClientData)button.Args[1];

        try
        {
            SoftnetTracker.deleteClient(siteData.siteId, clientData.clientId);
            Response.Redirect(string.Format("~/guest.aspx?site={0}", siteData.siteKey));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void GenerateClientPassword_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        SiteData siteData = (SiteData)button.Args[0];
        ClientData clientData = (ClientData)button.Args[1];
        Panel panelClientPassword = (Panel)button.Args[2];

        try
        {
            int passwordLength = SoftnetRegistry.settings_getClientPasswordLength();
            string password = Randomizer.generatePassword(passwordLength);
            byte[] salt = Randomizer.generateOctetString(16);
            byte[] saltedPassword = PasswordHash.Compute(salt, password);

            SoftnetTracker.setClientPassword(siteData.siteId, clientData.clientId, Convert.ToBase64String(salt), Convert.ToBase64String(saltedPassword));

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

    protected void EditClientAccount_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        SiteData siteData = (SiteData)button.Args[0];
        ClientData clientData = (ClientData)button.Args[1];
        Response.Redirect(string.Format("~/guest.aspx?site={0}&cid={1}&cpr=1", siteData.siteKey, clientData.clientId));
    }

    protected void EditClientPing_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        SiteData siteData = (SiteData)button.Args[0];
        ClientData clientData = (ClientData)button.Args[1];
        Response.Redirect(string.Format("~/guest.aspx?site={0}&cid={1}&cpr=2", siteData.siteKey, clientData.clientId));
    }

    protected void SetPingPeriod_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        SiteData siteData = (SiteData)button.Args[0];
        ClientData clientData = (ClientData)button.Args[1];
        TextBox textboxPingPeriod = (TextBox)button.Args[2];
        try
        {
            int pingPeriod;
            if (int.TryParse(textboxPingPeriod.Text, out pingPeriod) == false)
                throw new ArgumentException("Invalid format.");

            if (pingPeriod != 0 && (pingPeriod < 10 || pingPeriod > 300))
                throw new ArgumentException("The value of the ping period must be in the range from 10 seconds to 300 seconds or 0.");

            SoftnetTracker.setClientPingPeriod(siteData.siteId, clientData.clientId, pingPeriod);
            Response.Redirect(string.Format("~/guest.aspx?site={0}&cid={1}", siteData.siteKey, clientData.clientId));
        }
        catch (ArgumentException ex)
        {
            HtmlGenericControl td = (HtmlGenericControl)button.Args[3];
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

    protected void SendMessage_Click(object sender, EventArgs e)
    {
        TButton tbutton = (TButton)sender;
        SiteData siteData = (SiteData)tbutton.Args[0];
        TextBox textboxEMail = (TextBox)tbutton.Args[1];
        HtmlGenericControl divFrame = (HtmlGenericControl)tbutton.Args[2];

        try
        {
            string email = textboxEMail.Text.Trim();
            if (string.IsNullOrWhiteSpace(email))
                return;

            if (EMailValidator.IsValid(email) == false)
                throw new ArgumentSoftnetException("Incorrect email format.");

            MailingData data = new MailingData();
            SoftnetRegistry.settings_getMailingData(data);

            string transactionKey = Randomizer.generateTransactionKey(Constants.Keys.transaction_key_length);
            string time = data.currentTime.ToString();

            byte[] siteKeyBytes = Encoding.UTF8.GetBytes(siteData.siteKey);
            byte[] transactionKeyBytes = Encoding.UTF8.GetBytes(transactionKey);
            byte[] emailBytes = Encoding.UTF8.GetBytes(email);
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

            string body = "This message contains a url for creating a guest client on <span style='font-weight:bold'>{0}</span>.<br/>" +
                "If you are the person who requested this url click <a href='{1}'>create a guest client</a>.";

            string confirmationUrl = string.Format("{0}/public/clients/guest.aspx?skey={1}&tkey={2}&email={3}&time={4}&hash={5}", data.msUrl, siteData.siteKey, transactionKey, HttpUtility.UrlEncode(email), time, HttpUtility.UrlEncode(base64Hash));

            var fromAddress = new MailAddress(data.emailAddress, data.siteAddress);
            var toAddress = new MailAddress(email);

            MailMessage mailMessage = new MailMessage(fromAddress, toAddress);
            mailMessage.Subject = string.Format("Confirmation mail from <{0}>", data.siteAddress);
            mailMessage.Body = string.Format(body, data.siteAddress, confirmationUrl);
            mailMessage.IsBodyHtml = true;

            smtpClient.Send(mailMessage);

            divFrame.Controls.Clear();

            HtmlGenericControl span = new HtmlGenericControl("span");
            divFrame.Controls.Add(span);
            span.Attributes["style"] = "display:block; width:400px; text-align: center; font-size: 1.2em; color: #3C6C80;";
            span.InnerHtml = "The message has been successfully sent!";

            HtmlGenericControl table = new HtmlGenericControl("table");
            divFrame.Controls.Add(table);
            table.Attributes["class"] = "wide_table";
            table.Attributes["style"] = "width:400px; margin-top: 10px;";
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
        catch (ArgumentSoftnetException ex)
        {
            HtmlGenericControl span = new HtmlGenericControl("span");
            divFrame.Controls.Add(span);
            span.Attributes["class"] = "error_text";
            span.Attributes["style"] = "display:block; margin-top: 15px; font-size: 1.2em;";
            span.InnerHtml = ex.Message;
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
            if (m_dataset.guestData.enabled)
            {
                if (siteData.guestAllowed)
                {
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

        if (m_dataset.siteData.guestSupported)
        {
            if (m_dataset.siteData.enabled)
            {
                if (m_dataset.guestData.enabled)
                {
                    if (siteData.guestAllowed)
                    {
                        divSiteBlockItem = new HtmlGenericControl("div");
                        divSiteBlock.Controls.Add(divSiteBlockItem);
                        divSiteBlockItem.Attributes["class"] = "site_block_item";

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
                        labelUserName.Text = m_dataset.guestData.name;
                        labelUserName.CssClass = "user_guest";

                        td = new HtmlGenericControl("td");
                        tr.Controls.Add(td);
                        td.Attributes["class"] = "auto_table";
                        td.Attributes["style"] = "padding-left: 20px";

                        HtmlGenericControl divAddClientButton = new HtmlGenericControl("div");
                        td.Controls.Add(divAddClientButton);
                        divAddClientButton.Attributes["class"] = "SubmitButtonMini Green";

                        TButton buttonAddClient = new TButton();
                        divAddClientButton.Controls.Add(buttonAddClient);
                        buttonAddClient.Args.Add(siteData);
                        buttonAddClient.Args.Add(m_dataset.guestData);
                        buttonAddClient.Text = "add client";
                        buttonAddClient.ID = string.Format("B_CreateClient_{0}_{1}", siteData.siteId, m_dataset.guestData.userId);
                        buttonAddClient.Click += new EventHandler(AddGuestClient_Click);
                    }
                    else
                    {
                        divSiteBlockItem = new HtmlGenericControl("div");
                        divSiteBlock.Controls.Add(divSiteBlockItem);
                        divSiteBlockItem.Attributes["class"] = "site_block_item underline";

                        HtmlGenericControl spanWarning = new HtmlGenericControl("span");
                        divSiteBlockItem.Controls.Add(spanWarning);
                        spanWarning.Attributes["style"] = "display: block; text-align: center; color: #E86100; font-size: 1.2em;";
                        spanWarning.InnerHtml = "Sorry, the guest is denied. Creating a new client is unavailable.";
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
                    spanWarning.InnerHtml = "Sorry, the guest is disabled. Creating a new client is unavailable.";
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
                spanWarning.InnerHtml = "Sorry, the site is disabled. Creating a new client is unavailable.";
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
            spanWarning.InnerHtml = "Sorry, the guest access is not supported. Creating a new client is unavailable.";
        }

        int cntProp = 0;
        int.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("cpr"), out cntProp);

        foreach (ClientData clientData in m_dataset.guestClients)
        {
            if (clientData.clientId != m_editedClientId)
            {
                HtmlGenericControl divClient = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divClient);
                divClient.Attributes["class"] = "site_block_item";

                HtmlGenericControl tableLayout = new HtmlGenericControl("table");
                divClient.Controls.Add(tableLayout);
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
                HtmlGenericControl divClient = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divClient);
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
                tdRight.Attributes["style"] = "width: 22px;";

                HtmlGenericControl divViewClientButton = new HtmlGenericControl("div");
                tdLeft.Controls.Add(divViewClientButton);
                divViewClientButton.Attributes["class"] = "SubmitButtonMini Selected Blue";

                TButton buttonViewClient = new TButton();
                divViewClientButton.Controls.Add(buttonViewClient);
                buttonViewClient.Args.Add(siteData);
                buttonViewClient.Text = "<<";
                buttonViewClient.ID = string.Format("B_ViewClient_{0}", clientData.clientId);
                buttonViewClient.Click += new EventHandler(ViewClient_Click);

                HtmlGenericControl divDeleteClientButton = new HtmlGenericControl("div");
                tdRight.Controls.Add(divDeleteClientButton);
                divDeleteClientButton.Attributes["class"] = "SubmitButtonSquareMini RedOrange";

                TButton buttonDeleteClient = new TButton();
                divDeleteClientButton.Controls.Add(buttonDeleteClient);
                buttonDeleteClient.Args.Add(siteData);
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
                tButton.Args.Add(siteData);
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
                tButton.Args.Add(siteData);
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
                    buttonGeneratePassword.Args.Add(siteData);
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

                    span = new HtmlGenericControl("span");
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
                    buttonSavePingPeriod.Args.Add(siteData);
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
            }
        }
    }

    void drawMSite(SiteData siteData)
    {
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

        foreach (ServiceData serviceData in m_dataset.services)
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
            if (m_dataset.guestData.enabled)
            {
                if (siteData.guestAllowed)
                {
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

        if (m_dataset.siteData.guestSupported)
        {
            if (m_dataset.siteData.enabled)
            {
                if (m_dataset.guestData.enabled)
                {
                    if (siteData.guestAllowed)
                    {
                        divSiteBlockItem = new HtmlGenericControl("div");
                        divSiteBlock.Controls.Add(divSiteBlockItem);
                        divSiteBlockItem.Attributes["class"] = "site_block_item";

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
                        labelUserName.Text = m_dataset.guestData.name;
                        labelUserName.CssClass = "user_guest";

                        td = new HtmlGenericControl("td");
                        tr.Controls.Add(td);
                        td.Attributes["class"] = "auto_table";
                        td.Attributes["style"] = "padding-left: 20px";

                        HtmlGenericControl divAddClientButton = new HtmlGenericControl("div");
                        td.Controls.Add(divAddClientButton);
                        divAddClientButton.Attributes["class"] = "SubmitButtonMini Green";

                        TButton buttonAddClient = new TButton();
                        divAddClientButton.Controls.Add(buttonAddClient);
                        buttonAddClient.Args.Add(siteData);
                        buttonAddClient.Args.Add(m_dataset.guestData);
                        buttonAddClient.Text = "add client";
                        buttonAddClient.ID = string.Format("B_CreateClient_{0}_{1}", siteData.siteId, m_dataset.guestData.userId);
                        buttonAddClient.Click += new EventHandler(AddGuestClient_Click);
                    }
                    else
                    {
                        divSiteBlockItem = new HtmlGenericControl("div");
                        divSiteBlock.Controls.Add(divSiteBlockItem);
                        divSiteBlockItem.Attributes["class"] = "site_block_item underline";

                        HtmlGenericControl spanWarning = new HtmlGenericControl("span");
                        divSiteBlockItem.Controls.Add(spanWarning);
                        spanWarning.Attributes["style"] = "display: block; text-align: center; color: #E86100; font-size: 1.2em;";
                        spanWarning.InnerHtml = "Sorry, the guest is denied. Creating a new client is unavailable.";
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
                    spanWarning.InnerHtml = "Sorry, the guest is disabled. Creating a new client is unavailable.";
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
                spanWarning.InnerHtml = "Sorry, the site is disabled. Creating a new client is unavailable.";
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
            spanWarning.InnerHtml = "Sorry, the guest access is not supported. Creating a new client is unavailable.";
        }

        int cntProp = 0;
        int.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("cpr"), out cntProp);

        foreach (ClientData clientData in m_dataset.guestClients)
        {
            if (clientData.clientId != m_editedClientId)
            {
                HtmlGenericControl divClient = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divClient);
                divClient.Attributes["class"] = "site_block_item";

                HtmlGenericControl tableLayout = new HtmlGenericControl("table");
                divClient.Controls.Add(tableLayout);
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
                HtmlGenericControl divClient = new HtmlGenericControl("div");
                divSiteBlock.Controls.Add(divClient);
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
                tdRight.Attributes["style"] = "width: 22px;";

                HtmlGenericControl divViewClientButton = new HtmlGenericControl("div");
                tdLeft.Controls.Add(divViewClientButton);
                divViewClientButton.Attributes["class"] = "SubmitButtonMini Selected Blue";

                TButton buttonViewClient = new TButton();
                divViewClientButton.Controls.Add(buttonViewClient);
                buttonViewClient.Args.Add(siteData);
                buttonViewClient.Text = "<<";
                buttonViewClient.ID = string.Format("B_ViewClient_{0}", clientData.clientId);
                buttonViewClient.Click += new EventHandler(ViewClient_Click);

                HtmlGenericControl divDeleteClientButton = new HtmlGenericControl("div");
                tdRight.Controls.Add(divDeleteClientButton);
                divDeleteClientButton.Attributes["class"] = "SubmitButtonSquareMini RedOrange";

                TButton buttonDeleteClient = new TButton();
                divDeleteClientButton.Controls.Add(buttonDeleteClient);
                buttonDeleteClient.Args.Add(siteData);
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
                tButton.Args.Add(siteData);
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
                tButton.Args.Add(siteData);
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
                    spanAccount.InnerText = string.Format("softnet-m://{0}@{1}", clientData.clientKey, SoftnetRegistry.settings_getServerAddress());

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
                    buttonGeneratePassword.Args.Add(siteData);
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

                    span = new HtmlGenericControl("span");
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
                    buttonSavePingPeriod.Args.Add(siteData);
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
            }
        }
    }

    void drawSSite2(SiteData siteData)
    {
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
        divSiteBlockHeader.InnerText = "new guest client";

        divSiteBlockItem = new HtmlGenericControl("div");
        divSiteBlock.Controls.Add(divSiteBlockItem);
        divSiteBlockItem.Attributes["class"] = "site_block_item";

        HtmlGenericControl divFrame = new HtmlGenericControl("div");
        divFrame.Attributes["style"] = "width: 450px;";

        table = new HtmlGenericControl("table");
        divSiteBlockItem.Controls.Add(table);
        table.Attributes["class"] = "wide_table";
        tr = new HtmlGenericControl("tr");
        table.Controls.Add(tr);

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table width_50";

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table";
        td.Controls.Add(divFrame);

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table width_50";

        if (siteData.guestSupported)
        {
            if (siteData.enabled)
            {
                if (m_dataset.guestEnabled)
                {
                    if (siteData.guestAllowed)
                    {
                        span = new HtmlGenericControl("span");
                        divFrame.Controls.Add(span);
                        span.Attributes["style"] = "font-size: 1.1em; color: #3C6C80;";
                        span.InnerHtml = "Type your email in the text box and click <span style='font-weight:bold;'>send confirmation mail</span>,<br/>" +
                            "then follow the url from the inbox of your email.<br/>";

                        TextBox textboxEMail = new TextBox();
                        divFrame.Controls.Add(textboxEMail);
                        textboxEMail.Attributes["style"] = "border: 1px solid #7FBA00; outline:none; width:450px; margin: 0px; margin-top: 10px; padding: 3px;";
                        textboxEMail.ID = "TB_EMail";

                        table = new HtmlGenericControl("table");
                        divFrame.Controls.Add(table);
                        table.Attributes["class"] = "wide_table";
                        table.Attributes["style"] = "width: 450px; margin-top: 10px;";
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
                        buttonSendMessage.Args.Add(textboxEMail);
                        buttonSendMessage.Args.Add(divFrame);
                        buttonSendMessage.Text = "send confirmation mail";
                        buttonSendMessage.ID = "B_SendMessage";
                        buttonSendMessage.Click += new EventHandler(SendMessage_Click);
                    }
                    else
                    {
                        span = new HtmlGenericControl("span");
                        divFrame.Controls.Add(span);
                        span.Attributes["class"] = "warning";
                        span.Attributes["style"] = "display:block; text-align: center;";
                        span.InnerHtml = "Sorry, the guest is denied. Creating a new client is unavailable.";
                    }
                }
                else
                {
                    span = new HtmlGenericControl("span");
                    divFrame.Controls.Add(span);
                    span.Attributes["class"] = "warning";
                    span.Attributes["style"] = "display:block; text-align: center;";
                    span.InnerHtml = "Sorry, the guest is disabled. Creating a new client is unavailable.";
                }
            }
            else
            {
                span = new HtmlGenericControl("span");
                divFrame.Controls.Add(span);
                span.Attributes["class"] = "warning";
                span.Attributes["style"] = "display:block; text-align: center;";
                span.InnerHtml = "Sorry, the site is disabled. Creating a new client is unavailable.";
            }
        }
        else
        {
            span = new HtmlGenericControl("span");
            divFrame.Controls.Add(span);
            span.Attributes["class"] = "warning";
            span.Attributes["style"] = "display:block; text-align: center;";
            span.InnerHtml = "Sorry, the guest is not supported. Creating a new client is unavailable.";
        }
    }

    void drawMSite2(SiteData siteData)
    {
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

        foreach (ServiceData serviceData in m_dataset.services)
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
        divSiteBlockHeader.InnerText = "new guest client";

        divSiteBlockItem = new HtmlGenericControl("div");
        divSiteBlock.Controls.Add(divSiteBlockItem);
        divSiteBlockItem.Attributes["class"] = "site_block_item";

        HtmlGenericControl divFrame = new HtmlGenericControl("div");
        divFrame.Attributes["style"] = "width: 450px;";

        table = new HtmlGenericControl("table");
        divSiteBlockItem.Controls.Add(table);
        table.Attributes["class"] = "wide_table";
        tr = new HtmlGenericControl("tr");
        table.Controls.Add(tr);

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table width_50";

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table";
        td.Controls.Add(divFrame);

        td = new HtmlGenericControl("td");
        tr.Controls.Add(td);
        td.Attributes["class"] = "wide_table width_50";

        if (siteData.guestSupported)
        {
            if (siteData.enabled)
            {
                if (m_dataset.guestEnabled)
                {
                    if (siteData.guestAllowed)
                    {
                        span = new HtmlGenericControl("span");
                        divFrame.Controls.Add(span);
                        span.Attributes["style"] = "font-size: 1.1em; color: #3C6C80;";
                        span.InnerHtml = "Type your email in the text box and click <span style='font-weight:bold;'>send confirmation mail</span>,<br/>" +
                            "then follow the url from the inbox of your email.<br/>";

                        TextBox textboxEMail = new TextBox();
                        divFrame.Controls.Add(textboxEMail);
                        textboxEMail.Attributes["style"] = "border: 1px solid #7FBA00; outline:none; width:450px; margin: 0px; margin-top: 10px; padding: 3px;";
                        textboxEMail.ID = "TB_EMail";

                        table = new HtmlGenericControl("table");
                        divFrame.Controls.Add(table);
                        table.Attributes["class"] = "wide_table";
                        table.Attributes["style"] = "width: 450px; margin-top: 10px;";
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
                        buttonSendMessage.Args.Add(textboxEMail);
                        buttonSendMessage.Args.Add(divFrame);
                        buttonSendMessage.Text = "send confirmation mail";
                        buttonSendMessage.ID = "B_SendMessage";
                        buttonSendMessage.Click += new EventHandler(SendMessage_Click);
                    }
                    else
                    {
                        span = new HtmlGenericControl("span");
                        divFrame.Controls.Add(span);
                        span.Attributes["class"] = "warning";
                        span.Attributes["style"] = "display:block; text-align: center;";
                        span.InnerHtml = "Sorry, the guest is denied. Creating a new client is unavailable.";
                    }
                }
                else
                {
                    span = new HtmlGenericControl("span");
                    divFrame.Controls.Add(span);
                    span.Attributes["class"] = "warning";
                    span.Attributes["style"] = "display:block; text-align: center;";
                    span.InnerHtml = "Sorry, the guest is disabled. Creating a new client is unavailable.";
                }
            }
            else
            {
                span = new HtmlGenericControl("span");
                divFrame.Controls.Add(span);
                span.Attributes["class"] = "warning";
                span.Attributes["style"] = "display:block; text-align: center;";
                span.InnerHtml = "Sorry, the site is disabled. Creating a new client is unavailable.";
            }
        }
        else
        {
            span = new HtmlGenericControl("span");
            divFrame.Controls.Add(span);
            span.Attributes["class"] = "warning";
            span.Attributes["style"] = "display:block; text-align: center;";
            span.InnerHtml = "Sorry, the guest is not supported. Creating a new client is unavailable.";
        }
    }
}