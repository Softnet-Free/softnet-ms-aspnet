using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class admin_mgt_user : System.Web.UI.Page
{
    MemberData m_memberData;
    UrlBuider m_urlBuider;

    protected void Back_Click(object sender, EventArgs e)
    {
        string retUrl = m_urlBuider.getBackUrl();
        if (retUrl != null)
            Response.Redirect(retUrl);
        else
            Response.Redirect("~/admin/mgt/search.aspx");
    }

    protected void Refresh_Click(object sender, EventArgs e)
    {
        Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/admin/mgt/user.aspx?uid={0}", m_memberData.ownerId)));
    }

    protected void AssignRoleProvider_Click(object sender, EventArgs e)
    {
        TButton tButton = (TButton)sender;
        MemberData memberData = (MemberData)tButton.Args[0];
        try
        {
            SoftnetTracker.assignRoleProvider(memberData.ownerId);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/admin/mgt/user.aspx?uid={0}", memberData.ownerId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void RemoveRoleProvider_Click(object sender, EventArgs e)
    {
        TButton tButton = (TButton)sender;
        MemberData memberData = (MemberData)tButton.Args[0];
        try
        {
            SoftnetTracker.removeRoleProvider(memberData.ownerId);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/admin/mgt/user.aspx?uid={0}", memberData.ownerId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void EnableUser_Click(object sender, EventArgs e)
    {
        TButton tButton = (TButton)sender;
        MemberData memberData = (MemberData)tButton.Args[0];
        try
        {
            SoftnetTracker.enableOwner(memberData.ownerId);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/admin/mgt/user.aspx?uid={0}", memberData.ownerId)));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void DisableUser_Click(object sender, EventArgs e)
    {
        TButton tButton = (TButton)sender;
        MemberData memberData = (MemberData)tButton.Args[0];
        try
        {
            SoftnetTracker.disableOwner(memberData.ownerId);
            Response.Redirect(m_urlBuider.getLoopUrl(string.Format("~/admin/mgt/user.aspx?uid={0}", memberData.ownerId)));
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
            long ownerId;
            if (long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("uid"), out ownerId) == false)
                throw new InvalidIdentifierSoftnetException();

            m_memberData = new MemberData();
            m_memberData.ownerId = ownerId;
            SoftnetRegistry.usermgt_getUserData(m_memberData);

            string retString = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("ret");
            m_urlBuider = new UrlBuider(retString);

            Title = string.Format("{0} - User Management - Admin", m_memberData.fullName);

            L_UserName.Text = m_memberData.fullName;
            L_AccountName.Text = m_memberData.accountName;
            L_EMail.Text = m_memberData.email;

            if (m_memberData.authority == 1)
            {
                L_Authority.Text = "Role: &nbsp;<span style='color: #1F66FF; font-weight: bold'>Provider</span>";
                
                HtmlGenericControl divRemoveRoleButton = new HtmlGenericControl("div");
                PH_AuthButton.Controls.Add(divRemoveRoleButton);
                divRemoveRoleButton.Attributes["class"] = "SubmitButtonMini Gray";

                TButton buttonRemoveRole = new TButton();
                divRemoveRoleButton.Controls.Add(buttonRemoveRole);
                buttonRemoveRole.Args.Add(m_memberData);
                buttonRemoveRole.Text = "remove the role Provider";
                buttonRemoveRole.ID = "RemoveRole";
                buttonRemoveRole.Click += new EventHandler(RemoveRoleProvider_Click);
            }
            else if (m_memberData.authority == 0)
            {
                L_Authority.Text = "Role: &nbsp;<span style='color: #000000; font-weight: bold'>Consumer</span>";
                
                HtmlGenericControl divAssignRoleButton = new HtmlGenericControl("div");
                PH_AuthButton.Controls.Add(divAssignRoleButton);
                divAssignRoleButton.Attributes["class"] = "SubmitButtonMini Blue";

                TButton buttonAssignRole = new TButton();
                divAssignRoleButton.Controls.Add(buttonAssignRole);
                buttonAssignRole.Args.Add(m_memberData);
                buttonAssignRole.Text = "assign the role Provider";
                buttonAssignRole.ID = "AssignRole";
                buttonAssignRole.Click += new EventHandler(AssignRoleProvider_Click);
            }
            else
            {
                L_Authority.Text = "Role: &nbsp;<span style='color: #009F00; font-weight: bold'>Administrator</span>";
            }

            if (m_memberData.isEnabled)
            {
                L_Status.Text = "User &nbsp;<span style='color: #009F00; font-weight: bold'>enabled</span>";

                if (m_memberData.authority <= 1)
                {
                    HtmlGenericControl divDisableButton = new HtmlGenericControl("div");
                    PH_StatusButton.Controls.Add(divDisableButton);
                    divDisableButton.Attributes["class"] = "SubmitButtonMini RedOrange";

                    TButton buttonDisable = new TButton();
                    divDisableButton.Controls.Add(buttonDisable);
                    buttonDisable.Args.Add(m_memberData);
                    buttonDisable.Text = "disable user";
                    buttonDisable.ID = "Disable";
                    buttonDisable.Click += new EventHandler(DisableUser_Click);
                }
            }
            else
            {
                L_Status.Text = "User &nbsp;<span style='color: #9F0000; font-weight: bold'>disabled</span>";

                if (m_memberData.authority <= 1)
                {
                    HtmlGenericControl divEnableButton = new HtmlGenericControl("div");
                    PH_StatusButton.Controls.Add(divEnableButton);
                    divEnableButton.Attributes["class"] = "SubmitButtonMini Green";

                    TButton buttonEnable = new TButton();
                    divEnableButton.Controls.Add(buttonEnable);
                    buttonEnable.Args.Add(m_memberData);
                    buttonEnable.Text = "enable user";
                    buttonEnable.ID = "Enable";
                    buttonEnable.Click += new EventHandler(EnableUser_Click);
                }
            }
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }
}