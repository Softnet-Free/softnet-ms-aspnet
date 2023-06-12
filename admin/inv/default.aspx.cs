using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class admin_inv_default : System.Web.UI.Page
{
    List<InvitationData> m_invitationList;

    protected void NewInvitation_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/admin/inv/new.aspx");
    }

    protected void Refresh_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/admin/inv/default.aspx");
    }

    protected void ButtonView_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        InvitationData inv = (InvitationData)button.Args[0];
        Response.Redirect(string.Format("~/admin/inv/view.aspx?inv={0}", inv.invitationId));
    }

    protected void ButtonDelete_Click(object sender, EventArgs e)
    {
        try
        {
            TButton button = (TButton)sender;
            InvitationData inv = (InvitationData)button.Args[0];
            SoftnetRegistry.admin_deleteInvitation(this.Context.User.Identity.Name, inv.invitationId);
            Response.Redirect("~/admin/inv/default.aspx");
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void ClearExpiredInvitationList_Click(object sender, EventArgs e)
    {
        try
        {
            SoftnetRegistry.admin_clearExpiredInvitationList(this.Context.User.Identity.Name);
            Response.Redirect("~/admin/inv/default.aspx");
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void ClearAcceptedInvitationList_Click(object sender, EventArgs e)
    {
        try
        {
            SoftnetRegistry.admin_clearAcceptedInvitationList(this.Context.User.Identity.Name);
            Response.Redirect("~/admin/inv/default.aspx");
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }
    
    protected void NewUser_Click(object sender, EventArgs e)
    {
        TLinkButton button = (TLinkButton)sender;
        InvitationData inv = (InvitationData)button.Args[0];
        UrlBuider urlBuider = new UrlBuider();
        Response.Redirect(urlBuider.getNextUrl(string.Format("~/admin/mgt/user.aspx?uid={0}", inv.newUserId), "~/admin/inv/default.aspx"));
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            m_invitationList = new List<InvitationData>();
            SoftnetRegistry.admin_getInvitationList(this.Context.User.Identity.Name, m_invitationList);

            if (m_invitationList.Count == 0)
            {
                P_Invitations.Visible = true;
                P_Invitations.Controls.Clear();
                
                HtmlGenericControl div = new HtmlGenericControl("div");
                P_Invitations.Controls.Add(div);
                div.Attributes["style"] = "height: 8px; background-color: #A2C5D3;";
                
                div = new HtmlGenericControl("div");
                P_Invitations.Controls.Add(div);
                div.Attributes["style"] = "border: 1px solid #A2C5D3; padding: 7px;";

                HtmlGenericControl span = new HtmlGenericControl("span");
                div.Controls.Add(span);
                span.Attributes["style"] = "color: gray; font-size: 1.2em;";
                span.InnerText = "You have no invitations";

                return;
            }

            List<InvitationData> validInvitations = m_invitationList.FindAll(x => x.status == 0);
            List<InvitationData> acceptedInvitations = m_invitationList.FindAll(x => x.status == 1);
            List<InvitationData> expiredInvitations = m_invitationList.FindAll(x => x.status == 2);

            if (validInvitations.Count > 0)
            {
                P_Invitations.Visible = true;

                for (int i = 0; i < validInvitations.Count; i++)
                {
                    InvitationData inv = validInvitations[i];
                    TableRow tableRow = new TableRow();
                    T_Invitations.Rows.Add(tableRow);

                    TableCell td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    HtmlGenericControl divViewButton = new HtmlGenericControl("div");
                    td.Controls.Add(divViewButton);
                    divViewButton.Attributes["class"] = "SubmitButtonMini Blue";

                    TButton buttonView = new TButton();
                    divViewButton.Controls.Add(buttonView);
                    buttonView.Args.Add(inv);
                    buttonView.Text = ">>";
                    buttonView.ID = string.Format("B_View_{0}", inv.invitationId);
                    buttonView.Click += new EventHandler(ButtonView_Click);
                    
                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "font-size: 1.1em; border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px; padding-left: 10px; text-align: left;";

                    if (!string.IsNullOrEmpty(inv.email))
                    {
                        Label labelEMail = new Label();
                        td.Controls.Add(labelEMail);
                        labelEMail.Text = inv.email;
                        labelEMail.Attributes["style"] = "color: black;";

                        if (!string.IsNullOrEmpty(inv.description))
                        {
                            Label labelDescription = new Label();
                            td.Controls.Add(labelDescription);
                            labelDescription.Text = "&nbsp;&nbsp;&nbsp;" + inv.description;
                            labelDescription.Attributes["style"] = "color: #4F8DA6;";
                        }
                    }
                    else if (!string.IsNullOrEmpty(inv.description))
                    {
                        Label labelDescription = new Label();
                        td.Controls.Add(labelDescription);
                        labelDescription.Text = inv.description;
                        labelDescription.Attributes["style"] = "color: #4F8DA6;";
                    }

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "font-size: 1.1em; color: #0000FF; border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px; padding-left: 25px; text-align: left;";

                    if (inv.assignProviderRole)
                    {
                        Label labelP = new Label();
                        td.Controls.Add(labelP);
                        labelP.Text = "YES";
                    }

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    HtmlGenericControl divDelete = new HtmlGenericControl("div");
                    td.Controls.Add(divDelete);
                    divDelete.Attributes["class"] = "SubmitButtonSquareMini RedOrange";
                    TButton buttonDelete = new TButton();
                    buttonDelete.Text = "X";
                    buttonDelete.ID = string.Format("BT_Delete_{0}", inv.invitationId);
                    buttonDelete.Args.Add(inv);
                    buttonDelete.Click += new EventHandler(ButtonDelete_Click);
                    divDelete.Controls.Add(buttonDelete);
                }
            }

            if (acceptedInvitations.Count > 0)
            {
                P_AcceptedInvitations.Visible = true;
                for (int i = 0; i < acceptedInvitations.Count; i++)
                {
                    InvitationData inv = acceptedInvitations[i];
                    TableRow tableRow = new TableRow();
                    T_AcceptedInvitations.Rows.Add(tableRow);

                    TableCell td = new TableCell();
                    tableRow.Controls.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; width: 5px; padding-left: 10px; padding-right: 10px";

                    HtmlGenericControl icon = new HtmlGenericControl("div");
                    td.Controls.Add(icon);
                    icon.Attributes["style"] = "width: 5px; height: 5px; background-color: #3C6C80";

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "font-size: 1.1em; border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px; text-align: left;";

                    if (!string.IsNullOrEmpty(inv.email))
                    {
                        Label labelEMail = new Label();
                        td.Controls.Add(labelEMail);
                        labelEMail.Text = inv.email;
                        labelEMail.Attributes["style"] = "color: black;";

                        if (!string.IsNullOrEmpty(inv.description))
                        {
                            Label labelDescription = new Label();
                            td.Controls.Add(labelDescription);
                            labelDescription.Text = "&nbsp;&nbsp;&nbsp;" + inv.description;
                            labelDescription.Attributes["style"] = "color: #4F8DA6;";
                        }
                    }
                    else if (!string.IsNullOrEmpty(inv.description))
                    {
                        Label labelDescription = new Label();
                        td.Controls.Add(labelDescription);
                        labelDescription.Text = inv.description;
                        labelDescription.Attributes["style"] = "color: #4F8DA6;";
                    }

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px; padding-left: 0px;";
                    
                    TLinkButton linkButton = new TLinkButton();
                    td.Controls.Add(linkButton);
                    linkButton.Args.Add(inv);
                    linkButton.Text = inv.newUserName;
                    if (inv.newUserAuthority == 1)
                    {
                        linkButton.CssClass = "provider_color";
                        linkButton.Attributes["style"] = "font-size: 1.1em; text-decoration: none; border: 1px solid #C0C0C0; border-radius: 12px; background-color: #FFFFFF; padding: 2px 10px;";
                    }
                    else if (inv.newUserAuthority == 0)
                    {
                        linkButton.CssClass = "consumer_color";
                        linkButton.Attributes["style"] = "font-size: 1.1em; text-decoration: none; border: 1px solid #C0C0C0; border-radius: 12px; background-color: #FFFFFF; padding: 2px 10px;";
                    }
                    else if (inv.newUserAuthority == 2)
                    {
                        linkButton.CssClass = "admin_color";
                        linkButton.Attributes["style"] = "font-size: 1.1em; text-decoration: none; font-weight: bold; border: 1px solid #C0C0C0; border-radius: 12px; background-color: #FFFFFF; padding: 2px 10px;";
                    }
                    linkButton.Click += NewUser_Click;

                    if (inv.newUserName.Length <= 32)
                        linkButton.Text = inv.newUserName;
                    else
                        linkButton.Text = inv.newUserName.Substring(0, 32) + "...";

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    HtmlGenericControl divDelete = new HtmlGenericControl("div");
                    td.Controls.Add(divDelete);
                    divDelete.Attributes["class"] = "SubmitButtonSquareMini RedOrange";
                    TButton buttonDelete = new TButton();
                    buttonDelete.Text = "X";
                    buttonDelete.ID = string.Format("BT_Delete_{0}", inv.invitationId);
                    buttonDelete.Args.Add(inv);
                    buttonDelete.Click += new EventHandler(ButtonDelete_Click);
                    divDelete.Controls.Add(buttonDelete);
                }
            }

            if (expiredInvitations.Count > 0)
            {
                P_ExpiredInvitations.Visible = true;
                for (int i = 0; i < expiredInvitations.Count; i++)
                {
                    InvitationData inv = expiredInvitations[i];
                    TableRow tableRow = new TableRow();
                    T_ExpiredInvitations.Rows.Add(tableRow);

                    TableCell td = new TableCell();
                    tableRow.Controls.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; width: 5px; padding-left: 10px; padding-right: 10px";

                    HtmlGenericControl icon = new HtmlGenericControl("div");
                    td.Controls.Add(icon);
                    icon.Attributes["style"] = "width: 5px; height: 5px; background-color: #3C6C80";                    

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "font-size: 1.1em; border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px; text-align: left;";

                    if (!string.IsNullOrEmpty(inv.email))
                    {
                        Label labelEMail = new Label();
                        td.Controls.Add(labelEMail);
                        labelEMail.Text = inv.email;
                        labelEMail.Attributes["style"] = "color: black;";

                        if (!string.IsNullOrEmpty(inv.description))
                        {
                            Label labelDescription = new Label();
                            td.Controls.Add(labelDescription);
                            labelDescription.Text = "&nbsp;&nbsp;&nbsp;" + inv.description;
                            labelDescription.Attributes["style"] = "color: #4F8DA6;";
                        }
                    }
                    else if (!string.IsNullOrEmpty(inv.description))
                    {
                        Label labelDescription = new Label();
                        td.Controls.Add(labelDescription);
                        labelDescription.Text = inv.description;
                        labelDescription.Attributes["style"] = "color: #4F8DA6;";
                    }

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                    HtmlGenericControl divDelete = new HtmlGenericControl("div");
                    td.Controls.Add(divDelete);
                    divDelete.Attributes["class"] = "SubmitButtonSquareMini RedOrange";
                    TButton buttonDelete = new TButton();
                    buttonDelete.Text = "X";
                    buttonDelete.ID = string.Format("BT_Delete_{0}", inv.invitationId);
                    buttonDelete.Args.Add(inv);
                    buttonDelete.Click += new EventHandler(ButtonDelete_Click);
                    divDelete.Controls.Add(buttonDelete);
                }
            }
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }
}