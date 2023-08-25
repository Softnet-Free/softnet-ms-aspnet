using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Security;

public partial class domains_default : System.Web.UI.Page
{
    List<DomainItem> m_domains;
    int m_edit;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            m_domains = new List<DomainItem>();
            SoftnetRegistry.GetDomainList(this.Context.User.Identity.Name, m_domains);

            if (m_domains.Count > 0)
            {
                m_edit = 0;
                int.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("edit"), out m_edit);

                if (m_edit == 0)
                {
                    HtmlGenericControl table = new HtmlGenericControl("table");
                    PH_DomainList.Controls.Add(table);
                    table.Attributes["class"] = "wide_table";                    
                    foreach (DomainItem domainItem in m_domains)
                    {
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
                        hyperLink.NavigateUrl = string.Format("~/domains/domain.aspx?did={0}", domainItem.domainId);
                        hyperLink.Text = domainItem.domainName;
                        hyperLink.Attributes["class"] = "domain_color";
                        hyperLink.Attributes["style"] = "font-size: 1.1em;";
                    }
                }
                else
                {
                    P_SwitchModeButton.CssClass = "SubmitButton Yellow";
                    B_SwitchMode.Text = "view";

                    long editedDomainId = 0;
                    long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("did"), out editedDomainId);

                    if (m_domains.Count > 0)
                    {
                        HtmlGenericControl table = new HtmlGenericControl("table");
                        PH_DomainList.Controls.Add(table);
                        table.Attributes["class"] = "wide_table";                        
                        foreach (DomainItem domainItem in m_domains)
                        {
                            if (domainItem.domainId != editedDomainId)
                            {
                                HtmlGenericControl tr = new HtmlGenericControl("tr");
                                table.Controls.Add(tr);
                                tr.Attributes["style"] = "border-top: 1px solid #A2C5D3;";

                                HtmlGenericControl td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "wide_table";
                                td.Attributes["style"] = "padding-left: 5px; padding-right: 10px;";

                                HtmlGenericControl divShowNameEditorButton = new HtmlGenericControl("div");
                                td.Controls.Add(divShowNameEditorButton);
                                divShowNameEditorButton.Attributes["class"] = "SubmitButtonMini Blue";

                                TButton buttonShowNameEditor = new TButton();
                                divShowNameEditorButton.Controls.Add(buttonShowNameEditor);
                                buttonShowNameEditor.Args.Add(domainItem);
                                buttonShowNameEditor.Text = ">>";
                                buttonShowNameEditor.ID = string.Format("B_ShowNameEditor_{0}", domainItem.domainId);
                                buttonShowNameEditor.Click += new EventHandler(ShowNameEditor_Click);

                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "wide_table";
                                td.Attributes["style"] = "width: 100%; padding-top: 8px; padding-bottom: 7px;";

                                HyperLink hyperLink = new HyperLink();
                                td.Controls.Add(hyperLink);
                                hyperLink.NavigateUrl = string.Format("~/domains/domain.aspx?did={0}", domainItem.domainId);
                                hyperLink.Text = domainItem.domainName;
                                hyperLink.Attributes["class"] = "domain_color";
                                hyperLink.Attributes["style"] = "font-size: 1.1em;";

                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "wide_table";
                                td.Attributes["style"] = "padding: 5px;";

                                HtmlGenericControl divDeleteButton = new HtmlGenericControl("div");
                                td.Controls.Add(divDeleteButton);
                                divDeleteButton.Attributes["class"] = "SubmitButtonSquareMini RedOrange";
                                TButton buttonDelete = new TButton();
                                divDeleteButton.Controls.Add(buttonDelete);
                                buttonDelete.Text = "X";
                                buttonDelete.ID = string.Format("BT_DeleteDomain_{0}", domainItem.domainId);
                                buttonDelete.Args.Add(domainItem);
                                buttonDelete.Click += new EventHandler(DeleteDomain_Click);
                            }
                            else
                            {
                                HtmlGenericControl tr = new HtmlGenericControl("tr");
                                table.Controls.Add(tr);
                                tr.Attributes["style"] = "border-top: 1px solid #A2C5D3;";

                                HtmlGenericControl td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "wide_table";
                                td.Attributes["style"] = "padding-left: 5px; padding-right: 10px;";

                                HtmlGenericControl divSaveNameButton = new HtmlGenericControl("div");
                                td.Controls.Add(divSaveNameButton);
                                divSaveNameButton.Attributes["class"] = "SubmitButtonMini Green";

                                TButton buttonSaveName = new TButton();
                                divSaveNameButton.Controls.Add(buttonSaveName);
                                buttonSaveName.Args.Add(domainItem);
                                buttonSaveName.Text = "save";
                                buttonSaveName.Attributes["style"] = "padding-left:5px; padding-right: 5px;";
                                buttonSaveName.ID = string.Format("B_SaveName_{0}", domainItem.domainId);
                                buttonSaveName.Click += new EventHandler(SaveDomainName_Click);

                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "wide_table";
                                td.Attributes["style"] = "width: 100%; padding-top: 5px; padding-bottom: 4px;";

                                TextBox textboxName = new TextBox();
                                td.Controls.Add(textboxName);
                                buttonSaveName.Args.Add(textboxName);
                                textboxName.ID = string.Format("TB_Name_{0}", domainItem.domainId);
                                textboxName.Text = domainItem.domainName;
                                textboxName.Attributes["style"] = "border: 1px solid #7FBA00; outline:none; font-size: 1.1em; width: 462px; margin: 0px; padding: 2px;";
                                textboxName.CssClass = "user";
                                textboxName.Attributes["autocomplete"] = "off";

                                td = new HtmlGenericControl("td");
                                tr.Controls.Add(td);
                                td.Attributes["class"] = "wide_table";
                                td.Attributes["style"] = "padding: 5px;";

                                HtmlGenericControl divDeleteButton = new HtmlGenericControl("div");
                                td.Controls.Add(divDeleteButton);
                                divDeleteButton.Attributes["class"] = "SubmitButtonSquareMini RedOrange";
                                TButton buttonDelete = new TButton();
                                divDeleteButton.Controls.Add(buttonDelete);
                                buttonDelete.Text = "X";
                                buttonDelete.ID = string.Format("BT_DeleteDomain_{0}", domainItem.domainId);
                                buttonDelete.Args.Add(domainItem);
                                buttonDelete.Click += new EventHandler(DeleteDomain_Click);
                            }
                        }

                        ((HtmlGenericControl)table.Controls[0]).Attributes["style"] = "border-top-width: 0px;";
                    }
                }
            }
            else
            {
                HtmlGenericControl spanEmptyList = new HtmlGenericControl("span");
                PH_DomainList.Controls.Add(spanEmptyList);
                spanEmptyList.Attributes["style"] = "display:block; padding: 10px; color: gray; font-size: 1.2em;";
                spanEmptyList.InnerText = "You have no domains";

                P_SwitchModeButton.Visible = false;
            }
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void NewDomain_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/domains/newdomain.aspx");
    }

    protected void SwitchMode_Click(object sender, EventArgs e)
    {
        if (m_edit == 0)
            Response.Redirect("~/domains/default.aspx?edit=1");
        else
            Response.Redirect("~/domains/default.aspx");
    }

    protected void ShowNameEditor_Click(object sender, EventArgs e)
    {
        TButton tButton = (TButton)sender;
        DomainItem domainItem = (DomainItem)tButton.Args[0];
        Response.Redirect(string.Format("~/domains/default.aspx?edit=1&did={0}", domainItem.domainId));
    }

    protected void SaveDomainName_Click(object sender, EventArgs e)
    {
        TButton tButton = (TButton)sender;
        DomainItem domainItem = (DomainItem)tButton.Args[0];
        TextBox textboxDomainName = (TextBox)tButton.Args[1];
        
        string domainName = textboxDomainName.Text.Trim();
        if (string.IsNullOrEmpty(domainName))
        {
            L_Error.Visible = true;
            L_Error.Text = "The domain name must not be empty.";
            return;
        }

        if (domainName.Length > Constants.MaxLength.domain_name)
        {
            L_Error.Visible = true;
            L_Error.Text = string.Format("The length of a domain name must not be more than {0} characters.", Constants.MaxLength.domain_name);
            return;
        }

        try
        {
            SoftnetRegistry.ChangeDomainName(domainItem.domainId, domainName);
            Response.Redirect("~/domains/default.aspx?edit=1");
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void DeleteDomain_Click(object sender, EventArgs e)
    {
        TButton tButton = (TButton)sender;
        DomainItem domainItem = (DomainItem)tButton.Args[0];
        Response.Redirect(string.Format("~/domains/domain.aspx?did={0}&delete=1", domainItem.domainId));
    }
}