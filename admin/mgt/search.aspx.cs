using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class admin_mgt_search : System.Web.UI.Page
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
                    SoftnetRegistry.usermgt_findNextUsers(filter, boundaryId, foundUsers);
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
                    SoftnetRegistry.usermgt_findPrevUsers(filter, boundaryId, foundUsers);
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
                    SoftnetRegistry.usermgt_findUsers(filter, foundUsers);
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
                    hyperlink.Attributes["Style"] = "font-size: 1.1em;";
                    if (foundUser.authority == 0)
                        hyperlink.CssClass = "consumer_color";
                    else if (foundUser.authority == 1)
                        hyperlink.CssClass = "provider_color";
                    else if (foundUser.authority == 2)
                    {
                        hyperlink.CssClass = "admin_color";
                        hyperlink.Attributes["Style"] = "font-size: 1.1em; font-weight: bold;";
                    }
                    //hyperlink.Target = "_blank";
                    hyperlink.NavigateUrl = string.Format("~/admin/mgt/user.aspx?uid={0}&ret={1}", foundUser.ownerId, HttpUtility.UrlEncode("~/admin/mgt/search.aspx" + this.Request.Url.Query));

                    if (foundUser.enabled == false)
                    {
                        HtmlGenericControl span = new HtmlGenericControl("span");
                        span.InnerHtml = "&nbsp;&nbsp; <span class='gray_text'>[</span><span style='color: red'>disabled</span><span class='gray_text'>]</span>";
                        td.Controls.Add(span);
                    }
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
            Response.Redirect(string.Format("~/admin/mgt/search.aspx?filter={0}", HttpUtility.UrlEncode(filter)));
        else
            Response.Redirect(string.Format("~/admin/mgt/search.aspx?filter={0}", HttpUtility.UrlEncode(" ")));
    }

    protected void Prev_Click(object sender, EventArgs e)
    {
        string filter = TB_SearchFilter.Text.Trim();
        if (filter.Length > 0)
            Response.Redirect(string.Format("~/admin/mgt/search.aspx?filter={0}&prev={1}", HttpUtility.UrlEncode(filter), m_firstId));
        else
            Response.Redirect(string.Format("~/admin/mgt/search.aspx?filter={0}&prev={1}", HttpUtility.UrlEncode(" "), m_firstId));
    }

    protected void Next_Click(object sender, EventArgs e)
    {
        string filter = TB_SearchFilter.Text.Trim();
        if (filter.Length > 0)
            Response.Redirect(string.Format("~/admin/mgt/search.aspx?filter={0}&next={1}", HttpUtility.UrlEncode(filter), m_lastId));
        else
            Response.Redirect(string.Format("~/admin/mgt/search.aspx?filter={0}&next={1}", HttpUtility.UrlEncode(" "), m_lastId));
    }
}