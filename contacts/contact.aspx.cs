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

public partial class contacts_contact : System.Web.UI.Page
{
    SharedDomainList m_dataset;
    UrlBuider m_urlBuider;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            long contactId;
            if (long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("cid"), out contactId) == false)
                throw new InvalidIdentifierSoftnetException();

            bool meProvider = false;
            if (this.Context.User.IsInRole("Provider") || this.Context.User.IsInRole("Administrator"))
                meProvider = true;

            m_dataset = new SharedDomainList();
            SoftnetRegistry.GetSharedDomainList(this.Context.User.Identity.Name, contactId, meProvider, m_dataset);

            string retString = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("ret");
            m_urlBuider = new UrlBuider(retString);

            L_ContactName.Text = m_dataset.contactName;
            if (m_dataset.partnerAuthority == 0)
                L_ContactName.CssClass += " consumer_color";
            else
                L_ContactName.CssClass += " provider_color";

            this.Title = string.Format("{0} - My Contacts", m_dataset.contactName);

            if (meProvider)
            {
                P_MyDomains.Visible = true;
                if (m_dataset.myDomains.Count > 0)
                {
                    List<DomainItem> myDomains = m_dataset.myDomains;
                    myDomains.Sort(delegate(DomainItem x, DomainItem y)
                    {
                        return x.domainName.CompareTo(y.domainName);
                    });

                    HtmlGenericControl table = new HtmlGenericControl("table");
                    PH_MyDomains.Controls.Add(table);
                    table.Attributes["class"] = "wide_table";
                    foreach (DomainItem domainItem in myDomains)
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
                        linkButton.Args.Add(domainItem);
                        linkButton.ID = string.Format("NavigateMyDomain_{0}", domainItem.domainId);
                        td.Controls.Add(linkButton);
                        linkButton.Text = domainItem.domainName;
                        linkButton.CssClass = "domain_color";
                        linkButton.Attributes["style"] = "font-size: 1.1em; text-decoration: none; border: 1px solid #C0C0C0; border-radius: 12px; background-color: #F7F7F7; padding: 2px 10px;";
                        linkButton.Click += new EventHandler(NavigateMyDomain_Click);
                    }
                }                
            }

            if (m_dataset.status > 0)
            {
                if (m_dataset.status == 1)
                {
                    L_ContactName.CssClass = "h2_name contact_in_status_1";
                    HtmlGenericControl spanWarning = new HtmlGenericControl("span");
                    PH_Warning.Controls.Add(spanWarning);
                    spanWarning.Attributes["class"] = "warning";
                    spanWarning.Attributes["style"] = "display:block; margin-top:20px; margin-left:10px; margin-right:10px";
                    spanWarning.InnerHtml = "<span class='contact_in_status_1'>Warning!</span> Your partner deleted the contact. However it can be usable again if your partner restore it.";
                }
                else // m_dataset.status == 2
                {
                    L_ContactName.CssClass = "h2_name contact_in_status_2";
                    HtmlGenericControl spanWarning = new HtmlGenericControl("span");
                    PH_Warning.Controls.Add(spanWarning);
                    spanWarning.Attributes["class"] = "warning";
                    spanWarning.Attributes["style"] = "display:block; margin-top:20px; margin-left:10px; margin-right:10px";
                    spanWarning.InnerHtml = "<span class='contact_in_status_2'>Warning!</span> The contact is no longer usable as your partner has been deleted from the network.";
                }
            }
            else if (m_dataset.partnerEnabled == false)
            {                
                HtmlGenericControl spanWarning = new HtmlGenericControl("span");
                PH_Warning.Controls.Add(spanWarning);
                spanWarning.Attributes["class"] = "warning";
                spanWarning.Attributes["style"] = "display:block; margin-top:20px; margin-left:10px; margin-right:10px";
                spanWarning.InnerHtml = "<span class='red_text'>Warning!</span> The account of your partner has been disabled by the administrator.";            
            }
            else if (m_dataset.partnerAuthority > 0 && m_dataset.contactDomains.Count > 0)
            {
                List<DomainItem> contactDomains = m_dataset.contactDomains;
                contactDomains.Sort(delegate(DomainItem x, DomainItem y)
                {
                    return x.domainName.CompareTo(y.domainName);
                });

                HtmlGenericControl table = new HtmlGenericControl("table");
                PH_ContactDomains.Controls.Add(table);
                table.Attributes["class"] = "wide_table";
                foreach (DomainItem domainItem in contactDomains)
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
                    linkButton.Args.Add(domainItem);
                    linkButton.ID = string.Format("NavigateMyDomain_{0}", domainItem.domainId);
                    td.Controls.Add(linkButton);
                    linkButton.Text = domainItem.domainName;
                    linkButton.CssClass = "domain_color";
                    linkButton.Attributes["style"] = "font-size: 1.1em; text-decoration: none; border: 1px solid #C0C0C0; border-radius: 12px; background-color: #F7F7F7; padding: 2px 10px;";
                    linkButton.Click += new EventHandler(NavigateContactDomain_Click);
                }
            }            
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void Back_Click(object sender, EventArgs e)
    {
        string retUrl = m_urlBuider.getBackUrl();
        if (retUrl != null)
            Response.Redirect(retUrl);
        else
            Response.Redirect("~/contacts/default.aspx");

    }

    protected void Refresh_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/contacts/contact.aspx?cid={0}", m_dataset.contactId)));
    }

    protected void NavigateContactDomain_Click(object sender, EventArgs e)
    {
        TLinkButton linkButton = (TLinkButton)sender;
        DomainItem domainItem = (DomainItem)linkButton.Args[0];
        Response.Redirect(m_urlBuider.getNextUrl(string.Format("~/contacts/cdomain.aspx?cid={0}&did={1}", m_dataset.contactId, domainItem.domainId), string.Format("~/contacts/contact.aspx?cid={0}", m_dataset.contactId)));
    }

    protected void NavigateMyDomain_Click(object sender, EventArgs e)
    {
        TLinkButton linkButton = (TLinkButton)sender;
        DomainItem domainItem = (DomainItem)linkButton.Args[0];
        Response.Redirect(m_urlBuider.getNextUrl(string.Format("~/contacts/mdomain.aspx?cid={0}&did={1}", m_dataset.contactId, domainItem.domainId), string.Format("~/contacts/contact.aspx?cid={0}", m_dataset.contactId)));
    }
}