using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Net;
using System.Net.Mail;

public partial class public_clients_domains : System.Web.UI.Page
{
    string m_browsingUrlParams;

    protected void Refresh_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/public/clients/domains.aspx" + this.Request.Url.Query);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        string receivedTransactionKey = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("tkey");        
        if (!string.IsNullOrWhiteSpace(receivedTransactionKey))
        {
            try
            {
                string receivedEMail = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("email");
                if (string.IsNullOrWhiteSpace(receivedEMail))
                    throw new ArgumentSoftnetException("The browsing url has an invalid format.");

                string receivedTime = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("time");
                if (string.IsNullOrWhiteSpace(receivedTime))
                    throw new ArgumentSoftnetException("The browsing url has an invalid format.");

                string ReceivedBase64Hash = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("hash");
                if (string.IsNullOrWhiteSpace(ReceivedBase64Hash))
                    throw new ArgumentSoftnetException("The browsing url has an invalid format.");

                Pair<string, long> skeyAndTime = new Pair<string, long>();
                SoftnetRegistry.settings_getSecretKeyAndTime(skeyAndTime);

                long createdTime = 0;
                long.TryParse(receivedTime, out createdTime);

                if (skeyAndTime.Second - createdTime > 60480)
                    throw new ArgumentSoftnetException("The browsing url has expired.");

                byte[] transactionKeyBytes = Encoding.UTF8.GetBytes(receivedTransactionKey);
                byte[] emailBytes = Encoding.UTF8.GetBytes(receivedEMail);
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

                if (ReceivedBase64Hash.Equals(base64Hash) == false)
                    throw new ArgumentSoftnetException("The browsing url is not valid.");

                List<OwnerData> owners = new List<OwnerData>();
                List<DomainData> domains = new List<DomainData>();
                SoftnetRegistry.public_getDomainsForEMail(receivedEMail, owners, domains);

                P_PublicDomains.Visible = true;
                L_EMail.Text = receivedEMail;
                m_browsingUrlParams = string.Format("tkey={0}&email={1}&time={2}&hash={3}", receivedTransactionKey, HttpUtility.UrlEncode(receivedEMail), receivedTime, HttpUtility.UrlEncode(ReceivedBase64Hash));

                if (owners.Count > 0)
                {                    
                    owners.Sort(delegate(OwnerData x, OwnerData y)
                    {
                        return x.fullName.CompareTo(y.fullName);
                    });                    

                    for (int i = 0; i < owners.Count; i++)
                    {
                        OwnerData owner = owners[i];

                        HtmlGenericControl divOwnerDomains = new HtmlGenericControl("div");
                        PH_Domains.Controls.Add(divOwnerDomains);
                        if (i < owners.Count - 1)
                            divOwnerDomains.Attributes["style"] = "padding-bottom: 15px;";

                        HyperLink hyperLink = new HyperLink();
                        divOwnerDomains.Controls.Add(hyperLink);
                        hyperLink.CssClass = "provider_color";
                        hyperLink.Attributes["style"] = "font-size:1.2em;";
                        hyperLink.Text = owner.fullName;
                        hyperLink.NavigateUrl = string.Format("~/public/services/domains.aspx?uid={0}", owner.ownerId);
                        hyperLink.Target = "_blank";

                        HtmlGenericControl table = new HtmlGenericControl("table");
                        divOwnerDomains.Controls.Add(table);
                        table.Attributes["class"] = "wide_table";
                        table.Attributes["style"] = "margin-top: 5px;";

                        List<DomainData> ownerDomains = domains.FindAll(x => x.ownerId == owner.ownerId);
                        ownerDomains.Sort(delegate(DomainData x, DomainData y)
                        {
                            return x.domainName.CompareTo(y.domainName);
                        });

                        foreach (DomainData domain in ownerDomains)
                        {
                            HtmlGenericControl tr = new HtmlGenericControl("tr");
                            table.Controls.Add(tr);

                            HtmlGenericControl td = new HtmlGenericControl("td");
                            tr.Controls.Add(td);
                            td.Attributes["class"] = "wide_table";
                            td.Attributes["style"] = "width: 5px; padding-left: 0px; padding-right: 10px";

                            HtmlGenericControl icon = new HtmlGenericControl("div");
                            td.Controls.Add(icon);
                            icon.Attributes["style"] = "width: 5px; height: 5px; background-color: #3C6C80";

                            td = new HtmlGenericControl("td");
                            tr.Controls.Add(td);
                            td.Attributes["class"] = "wide_table";
                            td.Attributes["style"] = "padding-top: 8px; padding-bottom: 8px;";

                            TLinkButton linkButton = new TLinkButton();
                            linkButton.Args.Add(domain);
                            linkButton.ID = string.Format("LB_Domain_{0}", domain.domainId);
                            td.Controls.Add(linkButton);
                            linkButton.Text = domain.domainName;
                            linkButton.CssClass = "domain_color";
                            linkButton.Attributes["style"] = "font-size: 1.1em; text-decoration: none; border: 1px solid #C0C0C0; border-radius: 12px; background-color: #F7F7F7; padding: 2px 10px;";
                            linkButton.Click += new EventHandler(Domain_Click);
                        }
                    }
                }
            }
            catch (SoftnetException ex)
            {
                ExceptionHandler.exec(this, ex);
            }
        }
        else
        {
            P_Blank.Visible = true;
        }
    }

    protected void SendBrowsingUrl_Click(object sender, EventArgs e)
    { 
        try
        {
            string email = TB_EMail.Text.Trim();
            if (string.IsNullOrWhiteSpace(email))
                return;

            if (EMailValidator.IsValid(email) == false)
                throw new ArgumentSoftnetException("Incorrect email format.");

            MailingData data = new MailingData();
            SoftnetRegistry.settings_getMailingData(data);

            string transactionKey = Randomizer.generateTransactionKey(Constants.Keys.transaction_key_length);
            string time = data.currentTime.ToString();

            byte[] transactionKeyBytes = Encoding.UTF8.GetBytes(transactionKey);
            byte[] emailBytes = Encoding.UTF8.GetBytes(email);
            byte[] timeBytes = Encoding.UTF8.GetBytes(time);
            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(data.secretKey);

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

            var smtpClient = new SmtpClient(data.smtpServer, data.smtpPort);
            smtpClient.Credentials = new NetworkCredential(data.emailAddress, data.emailPassword);
            smtpClient.EnableSsl = data.smtpRequiresSSL;
            smtpClient.Timeout = 20000;

            string body =
                "This message contains a url for browsing guest clients on <span style='font-weight:bold'>{0}</span>.<br/>" +
                "If you are the person who requested this url click <a href='{1}'>browse my guest clients</a>. ";

            string browsingUrl = string.Format("{0}/public/clients/domains.aspx?tkey={1}&email={2}&time={3}&hash={4}", data.siteUrl, transactionKey, HttpUtility.UrlEncode(email), time, HttpUtility.UrlEncode(base64Hash));

            var fromAddress = new MailAddress(data.emailAddress, data.siteAddress);
            var toAddress = new MailAddress(email);

            MailMessage mailMessage = new MailMessage(fromAddress, toAddress);
            mailMessage.Subject = string.Format("Confirmation mail from <{0}>", data.siteAddress);
            mailMessage.Body = string.Format(body, data.siteAddress, browsingUrl);
            mailMessage.IsBodyHtml = true;

            smtpClient.Send(mailMessage);

            TB_EMail.Text = "";

            HtmlGenericControl span = new HtmlGenericControl("span");
            PH_Messages.Controls.Add(span);
            span.Attributes["style"] = "display:block; width:500px; text-align: center; font-size: 1.2em; color: #3C6C80;";
            span.InnerHtml = "The message has been successfully sent!";

            HtmlGenericControl table = new HtmlGenericControl("table");
            PH_Messages.Controls.Add(table);
            table.Attributes["class"] = "wide_table";
            table.Attributes["style"] = "width:500px; margin-top: 10px;";
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
            divOkButton.Attributes["class"] = "SubmitButton Blue";

            Button buttonOk = new Button();
            divOkButton.Controls.Add(buttonOk);
            buttonOk.Text = "ok";
        }
        catch (ArgumentSoftnetException ex)
        {
            HtmlGenericControl span = new HtmlGenericControl("span");
            PH_Messages.Controls.Add(span);
            span.Attributes["class"] = "error_text";
            span.Attributes["style"] = "display:block; text-align:left; font-size: 1.2em;";
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

    protected void Domain_Click(object sender, EventArgs e)
    {
        TLinkButton tButton = (TLinkButton)sender;
        DomainData domain = (DomainData)tButton.Args[0];
        Response.Redirect(string.Format("~/public/clients/domain.aspx?did={0}&", domain.domainId) + m_browsingUrlParams);
    }
}