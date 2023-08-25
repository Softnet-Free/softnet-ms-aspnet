using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class contacts_default : System.Web.UI.Page
{
    ContactListData m_data;
    int m_edit;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            m_edit = 0;
            int.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("edit"), out m_edit);
            
            if (m_edit == 0)
            {
                m_data = new ContactListData();                
                SoftnetRegistry.GetContactListData(this.Context.User.Identity.Name, m_data);

                P_ContactListViewer.Visible = true;

                if (m_data.contacts.Count > 0)
                {
                    List<ContactData> contacts = m_data.contacts;
                    List<OwnerData> partners = m_data.partners;

                    contacts.Sort(delegate(ContactData x, ContactData y)
                    {
                        return x.contactName.CompareTo(y.contactName);
                    });

                    HtmlGenericControl table = new HtmlGenericControl("table");
                    PH_ContactListViewer.Controls.Add(table);
                    table.Attributes["class"] = "wide_table";
                    foreach (ContactData contactData in contacts)
                    {
                        OwnerData partner = partners.Find(x => x.ownerId == contactData.consumerId);
                        if ((contactData.status == 0 || contactData.status == 1) && partner == null)
                            continue;

                        HtmlGenericControl tr = new HtmlGenericControl("tr");
                        table.Controls.Add(tr);

                        HtmlGenericControl td = new HtmlGenericControl("td");
                        tr.Controls.Add(td);
                        td.Attributes["class"] = "wide_table";
                        td.Attributes["style"] = "width: 5px; padding-left: 10px; padding-right: 10px";

                        HtmlGenericControl icon = new HtmlGenericControl("div");
                        td.Controls.Add(icon);
                        icon.Attributes["style"] = "width: 5px; height: 5px; background-color: #3C6C80";

                        td = new HtmlGenericControl("td");
                        tr.Controls.Add(td);
                        td.Attributes["class"] = "wide_table";
                        td.Attributes["style"] = "padding-top: 8px; padding-bottom: 8px;";

                        HyperLink hyperLink = new HyperLink();
                        td.Controls.Add(hyperLink);
                        hyperLink.NavigateUrl = string.Format("~/contacts/contact.aspx?cid={0}", contactData.contactId);
                        hyperLink.Text = contactData.contactName;
                        hyperLink.Attributes["style"] = "font-size: 1.1em;";

                        if (contactData.status == 0)
                        {
                            if (partner.authority == 0)
                                hyperLink.CssClass = "consumer_color";
                            else
                                hyperLink.CssClass = "provider_color";

                            if (partner.enabled == false)
                            {
                                HtmlGenericControl span = new HtmlGenericControl("span");
                                span.InnerHtml = "&nbsp;&nbsp; <span class='gray_text'>[</span><span style='color: red'>disabled</span><span class='gray_text'>]</span>";
                                td.Controls.Add(span);
                            }
                        }
                        else if (contactData.status == 1)
                        {
                            hyperLink.CssClass = "contact_in_status_1";
                            HtmlGenericControl span = new HtmlGenericControl("span");
                            span.InnerHtml = " <span class='gray_text'>(</span><span class='contact_in_status_1'>*</span><span class='gray_text'>)</span>";
                            td.Controls.Add(span);

                            if (partner.enabled == false)
                            {
                                span = new HtmlGenericControl("span");
                                span.InnerHtml = "&nbsp;&nbsp; <span class='gray_text'>[</span><span style='color: red'>disabled</span><span class='gray_text'>]</span>";
                                td.Controls.Add(span);
                            }
                        }
                        else if (contactData.status == 2)
                        {
                            hyperLink.CssClass = "contact_in_status_2";
                            HtmlGenericControl span = new HtmlGenericControl("span");
                            span.InnerHtml = " <span class='gray_text'>(</span><span class='contact_in_status_2'>**</span><span class='gray_text'>)</span>";
                            td.Controls.Add(span);
                        }                     
                    }

                    if (m_data.contacts.Find(x => x.status == 1) != null)
                    {
                        P_ContactListHints.Visible = true;
                        Label labelDescrition = new Label();
                        P_ContactListHints.Controls.Add(labelDescrition);
                        labelDescrition.Text = "<span class='gray_text'>(</span><span class='contact_in_status_1'>*</span><span class='gray_text'>)</span>" +
                            " - Your partner deleted the contact. However it can be usable again if your partner restore it.";
                        labelDescrition.Attributes["style"] = "display:block; padding-top: 5px;";
                    }

                    if (m_data.contacts.Find(x => x.status == 2) != null)
                    {
                        P_ContactListHints.Visible = true;
                        Label labelDescrition = new Label();
                        P_ContactListHints.Controls.Add(labelDescrition);
                        labelDescrition.Text = "<span class='gray_text'>(</span><span class='contact_in_status_2'>**</span><span class='gray_text'>)</span>" +
                            " - The contact is no longer usable as your partner has been deleted from the network.";
                        labelDescrition.Attributes["style"] = "display:block; padding-top: 5px;";
                    }

                    if (m_data.partners.Find(x => x.enabled == false) != null)
                    {
                        P_ContactListHints.Visible = true;
                        Label labelDescrition = new Label();
                        P_ContactListHints.Controls.Add(labelDescrition);
                        labelDescrition.Text = "<span class='gray_text'>[</span><span style='color: red;'>disabled</span><span class='gray_text'>]</span>" +
                            " - The account of your partner has been disabled by the administrator.";
                        labelDescrition.Attributes["style"] = "display:block; padding-top: 5px;";
                    }
                }
                else
                {
                    HtmlGenericControl spanEmptyList = new HtmlGenericControl("span");
                    PH_ContactListViewer.Controls.Add(spanEmptyList);
                    spanEmptyList.Attributes["style"] = "display:block; padding: 10px; color: gray; font-size: 1.2em;";
                    spanEmptyList.InnerText = "You have no contacts";
                }

                if (m_data.invitedUsers.Count > 0)
                {
                    P_InvitationsViewer.Visible = true;

                    HtmlGenericControl table = new HtmlGenericControl("table");
                    PH_InvitationsViewer.Controls.Add(table);
                    table.Attributes["class"] = "wide_table";
                    foreach (OwnerData invitedUser in m_data.invitedUsers)
                    {
                        HtmlGenericControl tr = new HtmlGenericControl("tr");
                        table.Controls.Add(tr);

                        HtmlGenericControl td = new HtmlGenericControl("td");
                        tr.Controls.Add(td);
                        td.Attributes["class"] = "wide_table";
                        td.Attributes["style"] = "width: 5px; padding: 5px; padding-left: 10px; padding-right: 10px;";

                        HtmlGenericControl icon = new HtmlGenericControl("div");
                        td.Controls.Add(icon);
                        icon.Attributes["style"] = "width: 5px; height: 5px; background-color: #3C6C80";

                        td = new HtmlGenericControl("td");
                        tr.Controls.Add(td);
                        td.Attributes["class"] = "wide_table";
                        td.Attributes["style"] = "padding-top: 8px; padding-bottom: 8px;";

                        Label labelInvitedUser = new Label();
                        td.Controls.Add(labelInvitedUser);
                        labelInvitedUser.Text = invitedUser.fullName;
                        labelInvitedUser.Attributes["style"] = "font-size: 1.1em; color: gray";
                    }
                }

                if(m_data.contacts.Count == 0 && m_data.invitedUsers.Count == 0)
                    P_SwitchModeButton.Visible = false;
            }
            else
            {
                m_data = new ContactListData();
                SoftnetRegistry.GetContactListData2(this.Context.User.Identity.Name, m_data);
                
                if (m_data.contacts.Count > 0)
                {
                    Tb_ContactListEditor.Visible = true;

                    List<ContactData> contacts = m_data.contacts;
                    List<OwnerData> partners = m_data.partners;

                    contacts.Sort(delegate(ContactData x, ContactData y)
                    {
                        return x.contactName.CompareTo(y.contactName);
                    });

                    foreach (ContactData contactData in contacts)
                    {
                        OwnerData partner = partners.Find(x => x.ownerId == contactData.consumerId);
                        if ((contactData.status == 0 || contactData.status == 1) && partner == null)
                            continue;

                        TableRow tableRow = new TableRow();
                        Tb_ContactListEditor.Rows.Add(tableRow);

                        TableCell td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px; padding-right: 10px;";

                        HtmlGenericControl divEditContact = new HtmlGenericControl("div");
                        td.Controls.Add(divEditContact);
                        divEditContact.Attributes["class"] = "SubmitButtonMini Blue";

                        TButton buttonEditUser = new TButton();
                        divEditContact.Controls.Add(buttonEditUser);
                        buttonEditUser.Args.Add(contactData);
                        buttonEditUser.Text = ">>";
                        buttonEditUser.ID = string.Format("B_EditContact_{0}", contactData.contactId);
                        buttonEditUser.Click += new EventHandler(ButtonEditContact_Click);

                        td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";

                        HyperLink hyperLink = new HyperLink();
                        td.Controls.Add(hyperLink);
                        hyperLink.NavigateUrl = string.Format("~/contacts/contact.aspx?cid={0}", contactData.contactId);
                        hyperLink.Text = contactData.contactName;
                        hyperLink.Attributes["style"] = "font-size: 1.1em;";

                        if (contactData.status == 0)
                        {
                            if (partner.authority == 0)
                                hyperLink.CssClass = "consumer_color";
                            else
                                hyperLink.CssClass = "provider_color";

                            if (partner.enabled == false)
                            {
                                HtmlGenericControl span = new HtmlGenericControl("span");
                                span.InnerHtml = "&nbsp;&nbsp; <span class='gray_text'>[</span><span style='color: red'>disabled</span><span class='gray_text'>]</span>";
                                td.Controls.Add(span);
                            }
                        }
                        else if (contactData.status == 1)
                        {
                            hyperLink.CssClass = "contact_in_status_1";
                            HtmlGenericControl span = new HtmlGenericControl("span");
                            span.InnerHtml = " <span class='gray_text'>(</span><span class='contact_in_status_1'>*</span><span class='gray_text'>)</span>";
                            td.Controls.Add(span);

                            if (partner.enabled == false)
                            {
                                span = new HtmlGenericControl("span");
                                span.InnerHtml = "&nbsp;&nbsp; <span class='gray_text'>[</span><span style='color: red'>disabled</span><span class='gray_text'>]</span>";
                                td.Controls.Add(span);
                            }
                        }
                        else if (contactData.status == 2)
                        {
                            hyperLink.CssClass = "contact_in_status_2";
                            HtmlGenericControl span = new HtmlGenericControl("span");
                            span.InnerHtml = " <span class='gray_text'>(</span><span class='contact_in_status_2'>**</span><span class='gray_text'>)</span>";
                            td.Controls.Add(span);
                        }                        

                        td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";
                        
                        Label labelUserDefaultname = new Label();
                        td.Controls.Add(labelUserDefaultname);
                        labelUserDefaultname.Text = contactData.userDefaultName;
                        labelUserDefaultname.CssClass = "name";

                        td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 5px;";
                        
                        if (partner != null)
                        {
                            Label labelPartnerName = new Label();
                            td.Controls.Add(labelPartnerName);
                            labelPartnerName.Text = partner.fullName;
                            labelPartnerName.CssClass = "name";
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
                        buttonDelete.ID = string.Format("BT_DeleteContact_{0}", contactData.contactId);
                        buttonDelete.Args.Add(contactData);
                        buttonDelete.Click += new EventHandler(ButtonDeleteContact_Click);
                        divDelete.Controls.Add(buttonDelete);
                    }

                    if (m_data.contacts.Find(x => x.status == 1) != null)
                    {
                        P_ContactListHints.Visible = true;
                        Label labelDescrition = new Label();
                        P_ContactListHints.Controls.Add(labelDescrition);
                        labelDescrition.Text = "<span class='gray_text'>(</span><span class='contact_in_status_1'>*</span><span class='gray_text'>)</span>" +
                            " - Your partner deleted the contact.";
                        labelDescrition.Attributes["style"] = "display:block; padding-top: 5px;";
                    }

                    if (m_data.contacts.Find(x => x.status == 2) != null)
                    {
                        P_ContactListHints.Visible = true;
                        Label labelDescrition = new Label();
                        P_ContactListHints.Controls.Add(labelDescrition);
                        labelDescrition.Text = "<span class='gray_text'>(</span><span class='contact_in_status_2'>**</span><span class='gray_text'>)</span>" +
                            " - The contact is no longer usable as your partner has been deleted from the network.";
                        labelDescrition.Attributes["style"] = "display:block; padding-top: 5px;";
                    }

                    if (m_data.partners.Find(x => x.enabled == false) != null)
                    {
                        P_ContactListHints.Visible = true;
                        Label labelDescrition = new Label();
                        P_ContactListHints.Controls.Add(labelDescrition);
                        labelDescrition.Text = "<span class='gray_text'>[</span><span style='color: red;'>disabled</span><span class='gray_text'>]</span>" +
                            " - The account of your partner has been disabled by the administrator.";
                        labelDescrition.Attributes["style"] = "display:block; padding-top: 5px;";
                    }
                }
                else
                {
                    P_ContactListViewer.Visible = true;

                    HtmlGenericControl spanEmptyList = new HtmlGenericControl("span");
                    PH_ContactListViewer.Controls.Add(spanEmptyList);
                    spanEmptyList.Attributes["style"] = "display:block; padding: 10px; color: gray; font-size: 1.2em;";
                    spanEmptyList.InnerText = "You have no contacts";
                }

                if (m_data.invitedUsers.Count > 0)
                {
                    P_InvitationsEditor.Visible = true;

                    foreach (OwnerData invitedUser in m_data.invitedUsers)
                    {
                        TableRow tableRow = new TableRow();
                        Tb_InvitationsEditor.Rows.Add(tableRow);

                        TableCell td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; width: 5px; padding: 5px; padding-left: 10px; padding-right: 10px;";
                        
                        HtmlGenericControl icon = new HtmlGenericControl("div");
                        td.Controls.Add(icon);
                        icon.Attributes["style"] = "width: 5px; height: 5px; background-color: #3C6C80";

                        td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding-right: 5px; text-align: left;";

                        Label labelInvitedUser = new Label();
                        td.Controls.Add(labelInvitedUser);
                        labelInvitedUser.Text = invitedUser.fullName;
                        labelInvitedUser.Attributes["style"] = "font-size: 1.1em; color: gray";

                        td = new TableCell();
                        tableRow.Cells.Add(td);
                        td.CssClass = "wide_table";
                        td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; width: 20px; padding: 5px;";

                        HtmlGenericControl divDelete = new HtmlGenericControl("div");
                        td.Controls.Add(divDelete);
                        divDelete.Attributes["class"] = "SubmitButtonSquareMini RedOrange";
                        TButton buttonDelete = new TButton();
                        buttonDelete.Text = "X";
                        buttonDelete.ID = string.Format("BT_DeleteInvitation_{0}", invitedUser.ownerId);
                        buttonDelete.Args.Add(invitedUser);
                        buttonDelete.Click += new EventHandler(ButtonDeleteInvitation_Click);
                        divDelete.Controls.Add(buttonDelete);
                    }
                }

                if (m_data.contacts.Count == 0 && m_data.invitedUsers.Count == 0)
                {
                    P_SwitchModeButton.Visible = false;
                }
                else
                {
                    P_SwitchModeButton.CssClass = "SubmitButton Yellow";
                    B_SwitchMode.Text = "view";                
                }
            }
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void NewInvitation_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/contacts/newinv.aspx");
    }

    protected void SwitchMode_Click(object sender, EventArgs e)
    {
        if (m_edit == 0)
            Response.Redirect("~/contacts/default.aspx?edit=1");
        else
            Response.Redirect("~/contacts/default.aspx");
    }

    void ButtonEditContact_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        ContactData contactdata = (ContactData)button.Args[0];
        Response.Redirect(string.Format("~/contacts/editcont.aspx?cid={0}", contactdata.contactId));        
    }

    void ButtonDeleteContact_Click(object sender, EventArgs e)
    {
        TButton button = (TButton)sender;
        ContactData contactdata = (ContactData)button.Args[0];
        try
        {
            SoftnetTracker.deleteContact(contactdata.contactId);
            Response.Redirect(string.Format("~/contacts/default.aspx{0}", this.Request.Url.Query));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    void ButtonDeleteInvitation_Click(object sender, EventArgs e)
    {       
        TButton button = (TButton)sender;
        OwnerData invitedUser = (OwnerData)button.Args[0];
        try
        {
            SoftnetRegistry.DeleteSentInvitation(invitedUser.ownerId);
            Response.Redirect(string.Format("~/contacts/default.aspx{0}", this.Request.Url.Query));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }    
}