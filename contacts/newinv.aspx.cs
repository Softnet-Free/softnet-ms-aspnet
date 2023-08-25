using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class contacts_newinv : System.Web.UI.Page
{
    long m_ownerId;
    string m_filter;
    long m_firstId;
    long m_lastId;
    int m_function;
    long m_boundaryId;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            string originalFilter = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("filter");
            if (string.IsNullOrEmpty(originalFilter) == false)
            {
                m_filter = originalFilter;

                if(this.IsPostBack == false)
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

                long boundaryId;
                List<OwnerData> foundUsers = new List<OwnerData>();
                if (long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("next"), out boundaryId))
                {
                    m_ownerId = SoftnetRegistry.Contacts_FindNextUsers(this.Context.User.Identity.Name, filter, boundaryId, foundUsers);                    
                    if (foundUsers.Count > 0)
                    {
                        m_function = 1;
                        m_boundaryId = boundaryId;
                        m_firstId = foundUsers[0].ownerId;
                        m_lastId = foundUsers[foundUsers.Count - 1].ownerId;

                        P_PrevButton.Visible = true;
                        if(foundUsers.Count == 15)
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
                    m_ownerId = SoftnetRegistry.Contacts_FindPrevUsers(this.Context.User.Identity.Name, filter, boundaryId, foundUsers);
                    if (foundUsers.Count > 0)
                    {
                        m_function = -1;
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
                    m_ownerId = SoftnetRegistry.Contacts_FindUsers(this.Context.User.Identity.Name, filter, foundUsers);
                    if (foundUsers.Count > 0)
                    {
                        m_function = 0;
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

                    TLinkButton linkButton = new TLinkButton();
                    linkButton.Args.Add(foundUser);
                    linkButton.ID = string.Format("SendInvitation_{0}", foundUser.ownerId);
                    td.Controls.Add(linkButton);
                    linkButton.Text = foundUser.fullName;
                    linkButton.Click += new EventHandler(SendInvitation_Click);

                    if (foundUser.authority >= 1)
                    {
                        linkButton.CssClass = "provider_color";
                        linkButton.Attributes["style"] = "font-size: 1.1em; text-decoration: none; border: 1px solid #C0C0C0; border-radius: 12px; background-color: #F7F7F7; padding: 2px 10px;";
                    }
                    else if (foundUser.authority == 0)
                    {
                        linkButton.CssClass = "consumer_color";
                        linkButton.Attributes["style"] = "font-size: 1.1em; text-decoration: none; border: 1px solid #C0C0C0; border-radius: 12px; background-color: #F7F7F7; padding: 2px 10px;";
                    }
                }
            }

            string evt = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("evt");
            string userName = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("user");
            if (string.IsNullOrEmpty(evt) == false && string.IsNullOrEmpty(userName) == false)
            {
                if (evt.Equals("1"))
                    L_Message.Text = string.Format("An invitation is sent to <span style='color:black'>{0}</span>.", userName);
                else if (evt.Equals("2"))
                    L_Message.Text = string.Format("The contact <span style='color:black'>{0}</span> has been created.", userName);
                else if (evt.Equals("3"))
                    L_Message.Text = string.Format("The person/organization <span style='color:black'>{0}</span> has not been found.", userName);
            }
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void Back_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/contacts/default.aspx");
    }

    protected void Search_Click(object sender, EventArgs e)
    {
        string filter = TB_SearchFilter.Text.Trim();
        if(filter.Length > 0)
            Response.Redirect(string.Format("~/contacts/newinv.aspx?filter={0}", HttpUtility.UrlEncode(filter)));
        else
            Response.Redirect(string.Format("~/contacts/newinv.aspx?filter={0}", HttpUtility.UrlEncode(" ")));
    }

    protected void Prev_Click(object sender, EventArgs e)
    {
        string filter = TB_SearchFilter.Text.Trim();
        if (filter.Length > 0)
            Response.Redirect(string.Format("~/contacts/newinv.aspx?filter={0}&prev={1}", HttpUtility.UrlEncode(filter), m_firstId));
        else
            Response.Redirect(string.Format("~/contacts/newinv.aspx?filter={0}&prev={1}", HttpUtility.UrlEncode(" "), m_firstId));
    }

    protected void Next_Click(object sender, EventArgs e)
    {
        string filter = TB_SearchFilter.Text.Trim();
        if (filter.Length > 0)
            Response.Redirect(string.Format("~/contacts/newinv.aspx?filter={0}&next={1}", HttpUtility.UrlEncode(filter), m_lastId));
        else
            Response.Redirect(string.Format("~/contacts/newinv.aspx?filter={0}&next={1}", HttpUtility.UrlEncode(" "), m_lastId));
    }

    protected void SendInvitation_Click(object sender, EventArgs e)
    {
        TLinkButton linkButton = (TLinkButton)sender;
        OwnerData selectedUser = (OwnerData)linkButton.Args[0];

        try
        {
            bool peerContactExists = SoftnetRegistry.WhetherPeerContactExists(m_ownerId, selectedUser.ownerId);
            if (peerContactExists == false)
            {
                int result = SoftnetRegistry.SendInvitation(m_ownerId, selectedUser.ownerId);
                string encodedUserName = HttpUtility.UrlEncode(selectedUser.fullName);
                if (result == 0)
                {
                    if (m_function == 0)
                        Response.Redirect(string.Format("~/contacts/newinv.aspx?filter={0}&evt={1}&user={2}", HttpUtility.UrlEncode(m_filter), 1, encodedUserName));
                    else if (m_function == 1)
                        Response.Redirect(string.Format("~/contacts/newinv.aspx?filter={0}&next={1}&evt={2}&user={3}", HttpUtility.UrlEncode(m_filter), m_boundaryId, 1, encodedUserName));
                    else // m_function == -1
                        Response.Redirect(string.Format("~/contacts/newinv.aspx?filter={0}&prev={1}&evt={2}&user={3}", HttpUtility.UrlEncode(m_filter), m_boundaryId, 1, encodedUserName));
                }
                else if (result == 1)
                {
                    if (m_function == 0)
                        Response.Redirect(string.Format("~/contacts/newinv.aspx?filter={0}&evt={1}&user={2}", HttpUtility.UrlEncode(m_filter), 2, encodedUserName));
                    else if (m_function == 1)
                        Response.Redirect(string.Format("~/contacts/newinv.aspx?filter={0}&next={1}&evt={2}&user={3}", HttpUtility.UrlEncode(m_filter), m_boundaryId, 2, encodedUserName));
                    else // m_function == -1
                        Response.Redirect(string.Format("~/contacts/newinv.aspx?filter={0}&prev={1}&evt={2}&user={3}", HttpUtility.UrlEncode(m_filter), m_boundaryId, 2, encodedUserName));
                }
                else // result == -1
                {
                    if (m_function == 0)
                        Response.Redirect(string.Format("~/contacts/newinv.aspx?filter={0}&evt={1}&user={2}", HttpUtility.UrlEncode(m_filter), 3, encodedUserName));
                    else if (m_function == 1)
                        Response.Redirect(string.Format("~/contacts/newinv.aspx?filter={0}&next={1}&evt={2}&user={3}", HttpUtility.UrlEncode(m_filter), m_boundaryId, 3, encodedUserName));
                    else // m_function == -1
                        Response.Redirect(string.Format("~/contacts/newinv.aspx?filter={0}&prev={1}&evt={2}&user={3}", HttpUtility.UrlEncode(m_filter), m_boundaryId, 3, encodedUserName));
                }
            }
            else
            {
                SoftnetTracker.restoreContact(m_ownerId, selectedUser.ownerId);
                string encodedUserName = HttpUtility.UrlEncode(selectedUser.fullName);
                if (m_function == 0)
                    Response.Redirect(string.Format("~/contacts/newinv.aspx?filter={0}&evt={1}&user={2}", HttpUtility.UrlEncode(m_filter), 2, encodedUserName));
                else if (m_function == 1)
                    Response.Redirect(string.Format("~/contacts/newinv.aspx?filter={0}&next={1}&evt={2}&user={3}", HttpUtility.UrlEncode(m_filter), m_boundaryId, 2, encodedUserName));
                else // m_function == -1
                    Response.Redirect(string.Format("~/contacts/newinv.aspx?filter={0}&prev={1}&evt={2}&user={3}", HttpUtility.UrlEncode(m_filter), m_boundaryId, 2, encodedUserName));
            }
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }
}