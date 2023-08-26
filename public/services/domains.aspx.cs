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

public partial class public_services_domains : System.Web.UI.Page
{
    OwnerData m_ownerData;
    List<DomainItem> m_domainItems;
    UrlBuider m_urlBuider;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            long ownerId;
            if (long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("uid"), out ownerId) == false)
                throw new InvalidIdentifierSoftnetException();

            m_urlBuider = new UrlBuider(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("ret"));
            P_BackButton.Visible = m_urlBuider.hasBackUrl();

            m_ownerData = new OwnerData();
            m_ownerData.ownerId = ownerId;
            m_domainItems = new List<DomainItem>();

            SoftnetRegistry.public_getOwnerDomains(m_ownerData, m_domainItems);

            L_OwnerName.Text = m_ownerData.fullName;
            this.Title = string.Format("{0} - Public Services", m_ownerData.fullName);

            if (m_domainItems.Count > 0)
            {
                HtmlGenericControl table = new HtmlGenericControl("table");
                PH_Domains.Controls.Add(table);
                table.Attributes["class"] = "wide_table";
                foreach (DomainItem domainItem in m_domainItems)
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
                    linkButton.ID = string.Format("NavigateDomain_{0}", domainItem.domainId);
                    td.Controls.Add(linkButton);
                    linkButton.CssClass = "domain_color";
                    linkButton.Text = domainItem.domainName;
                    linkButton.Attributes["style"] = "font-size: 1.1em; text-decoration: none; border: 1px solid #C0C0C0; border-radius: 12px; background-color: #F7F7F7; padding: 2px 10px;";
                    linkButton.Click += new EventHandler(NavigateDomain_Click);
                }
            }
            else
            {
                H_Owner.Visible = false;
                P_Body.Controls.Clear();
                HtmlGenericControl div = new HtmlGenericControl("div");
                P_Body.Controls.Add(div);
                div.Attributes["style"] = "text-align: center; padding-top: 20px;";
                HtmlGenericControl spanWarning = new HtmlGenericControl("span");
                div.Controls.Add(spanWarning);
                spanWarning.Attributes["style"] = "font-size: 1.6em";
                spanWarning.InnerHtml = string.Format("<span class='orange_text'>{0}</span> has no public services.", m_ownerData.fullName);
            }
        }
        catch (InvalidStateSoftnetException)
        {
            if (m_urlBuider.hasBackUrl())
                Response.Redirect(m_urlBuider.getBackUrl());
            else
                Response.Redirect("~/public/services/search.aspx");
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
    }

    protected void Refresh_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/public/services/domains.aspx?uid={0}", m_ownerData.ownerId)));
    }

    protected void NavigateDomain_Click(object sender, EventArgs e)
    {
        TLinkButton linkButton = (TLinkButton)sender;
        DomainItem domainItem = (DomainItem)linkButton.Args[0];
        Response.Redirect(m_urlBuider.getNextUrl(string.Format("~/public/services/domain.aspx?did={0}", domainItem.domainId), string.Format("~/public/services/domains.aspx?uid={0}", m_ownerData.ownerId)));
    }
}