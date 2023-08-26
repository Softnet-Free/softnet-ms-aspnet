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

public partial class public_services_search : System.Web.UI.Page
{
    string m_filter;
    long m_firstId;
    long m_lastId;    
    long m_boundaryId;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            string originalFilter = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("filter");
            if (string.IsNullOrEmpty(originalFilter) == false)
            {
                m_filter = originalFilter;

                if (this.IsPostBack == false)
                    TB_SearchFilter.Text = originalFilter.Trim();

                string filter = originalFilter.Trim();

                if (filter.Length == 0)
                    filter = "%";
                else
                {
                    while (filter.Contains("  "))
                        filter = filter.Replace("  ", " ");

                    string[] filterParts = filter.Split(' ');
                    filter = "%";
                    for (int i = 0; i < filterParts.Length; i++)
                        filter = filter + filterParts[i] + "%";
                }

                List<OwnerData> foundUsers = new List<OwnerData>();
                long boundaryId;
                if (long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("next"), out boundaryId))
                {
                    SoftnetRegistry.public_findNextUsers(filter, boundaryId, foundUsers);
                    if (foundUsers.Count > 0)
                    {
                        m_boundaryId = boundaryId;
                        m_firstId = foundUsers[0].ownerId;
                        m_lastId = foundUsers[foundUsers.Count - 1].ownerId;

                        P_PrevButton.Visible = true;
                        if (foundUsers.Count == 15)
                            P_NextButton.Visible = true;
                    }
                    else
                    {
                        m_lastId = boundaryId;
                        m_firstId = m_lastId + 1;

                        P_PrevButton.Visible = true;
                    }
                }
                else if (long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("prev"), out boundaryId))
                {
                    SoftnetRegistry.public_findPrevUsers(filter, boundaryId, foundUsers);
                    if (foundUsers.Count > 0)
                    {
                        m_boundaryId = boundaryId;
                        foundUsers.Sort(delegate(OwnerData x, OwnerData y)
                        {
                            return x.ownerId.CompareTo(y.ownerId);
                        });
                        m_firstId = foundUsers[0].ownerId;
                        m_lastId = foundUsers[foundUsers.Count - 1].ownerId;

                        P_NextButton.Visible = true;
                        if (foundUsers.Count == 15)
                            P_PrevButton.Visible = true;
                    }
                    else
                    {
                        m_firstId = boundaryId;
                        m_lastId = m_firstId - 1;

                        P_NextButton.Visible = true;
                    }
                }
                else
                {
                    SoftnetRegistry.public_findUsers(filter, foundUsers);
                    if (foundUsers.Count > 0)
                    {
                        m_firstId = foundUsers[0].ownerId;
                        m_lastId = foundUsers[foundUsers.Count - 1].ownerId;

                        if (foundUsers.Count == 15)
                            P_NextButton.Visible = true;
                    }
                }

                foundUsers.Sort(delegate(OwnerData x, OwnerData y)
                {
                    return x.fullName.CompareTo(y.fullName);
                });

                HtmlGenericControl table = new HtmlGenericControl("table");
                PH_FoundUsers.Controls.Add(table);
                table.Attributes["class"] = "wide_table";
                foreach (OwnerData foundUser in foundUsers)
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
                    td.Attributes["style"] = "padding-top: 6px; padding-bottom: 6px;";

                    HyperLink hyperlink = new HyperLink();
                    td.Controls.Add(hyperlink);
                    hyperlink.Text = foundUser.fullName;
                    hyperlink.CssClass = "provider_color";
                    hyperlink.Attributes["style"] = "font-size: 1.1em;";
                    //hyperlink.Target = "_blank";
                    hyperlink.NavigateUrl = string.Format("~/public/services/domains.aspx?uid={0}&ret={1}", foundUser.ownerId, HttpUtility.UrlEncode("~/public/services/search.aspx" + this.Request.Url.Query));
                }
            }
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void Search_Click(object sender, EventArgs e)
    {
        string filter = TB_SearchFilter.Text.Trim();
        if (filter.Length > 0)
            Response.Redirect(string.Format("~/public/services/search.aspx?filter={0}", HttpUtility.UrlEncode(filter)));
        else
            Response.Redirect(string.Format("~/public/services/search.aspx?filter={0}", HttpUtility.UrlEncode(" ")));
    }

    protected void Prev_Click(object sender, EventArgs e)
    {
        string filter = TB_SearchFilter.Text.Trim();
        if (filter.Length > 0)
            Response.Redirect(string.Format("~/public/services/search.aspx?filter={0}&prev={1}", HttpUtility.UrlEncode(filter), m_firstId));
        else
            Response.Redirect(string.Format("~/public/services/search.aspx?filter={0}&prev={1}", HttpUtility.UrlEncode(" "), m_firstId));
    }

    protected void Next_Click(object sender, EventArgs e)
    {
        string filter = TB_SearchFilter.Text.Trim();
        if (filter.Length > 0)
            Response.Redirect(string.Format("~/public/services/search.aspx?filter={0}&next={1}", HttpUtility.UrlEncode(filter), m_lastId));
        else
            Response.Redirect(string.Format("~/public/services/search.aspx?filter={0}&next={1}", HttpUtility.UrlEncode(" "), m_lastId));
    }
}