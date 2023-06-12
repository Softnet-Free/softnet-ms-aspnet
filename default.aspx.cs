using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Security;

public partial class _default : System.Web.UI.Page
{
    List<ContactData> m_contacts;
    List<ContactData> m_invalidContacts;
    List<OwnerData> m_partners;
    List<DomainItem> m_domains;

    bool m_anyoneCanRegister = false;

    protected override void OnInit(EventArgs e)
    {
        try
        {
            m_anyoneCanRegister = SoftnetRegistry.settings_getAnyoneCanRegister();
            SiteMaster siteMaster = (SiteMaster)this.Master;
            siteMaster.m_anyoneCanRegister = m_anyoneCanRegister;
        }
        catch (SoftnetException) { }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.Context.User.Identity.IsAuthenticated)
        {
            bool isProviderOrAdministrator = this.Context.User.IsInRole("Provider") || this.Context.User.IsInRole("Administrator");
            try
            {
                m_contacts = new List<ContactData>();
                m_invalidContacts = new List<ContactData>();
                m_partners = new List<OwnerData>();
                SoftnetRegistry.GetRecentlyUsedContacts(this.Context.User.Identity.Name, m_contacts, m_invalidContacts, m_partners);

                if (m_contacts.Count > 0)
                {
                    Panel_Contacts.Visible = true;

                    HtmlGenericControl table = new HtmlGenericControl("table");
                    PH_Contacts.Controls.Add(table);
                    table.Attributes["class"] = "wide_table";
                    foreach (ContactData contactData in m_contacts)
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
                        hyperLink.NavigateUrl = string.Format("~/contacts/contact.aspx?cid={0}&ret={1}", contactData.contactId, HttpUtility.UrlEncode("~/default.aspx"));
                        hyperLink.Text = contactData.contactName;
                        hyperLink.Attributes["style"] = "font-size: 1.1em;";

                        OwnerData partner = m_partners.Find(x => x.ownerId == contactData.consumerId);
                        if (partner != null && partner.authority > 0)
                            hyperLink.CssClass = "provider_color";
                        else
                            hyperLink.CssClass = "consumer_color";
                    }
                }

                if (m_invalidContacts.Count > 0)
                {
                    Panel_InvalidContacts.Visible = true;

                    HtmlGenericControl table = new HtmlGenericControl("table");
                    PH_InvalidContacts.Controls.Add(table);
                    table.Attributes["class"] = "wide_table";
                    foreach (ContactData contactData in m_invalidContacts)
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
                        hyperLink.NavigateUrl = string.Format("~/contacts/contact.aspx?cid={0}&ret={1}", contactData.contactId, HttpUtility.UrlEncode("~/default.aspx"));
                        hyperLink.Text = contactData.contactName;
                        hyperLink.Attributes["style"] = "font-size: 1.1em;";

                        if (contactData.status == 1)
                        {
                            hyperLink.CssClass = "contact_in_status_1";
                            hyperLink.ToolTip = "Your partner has deleted the contact.";
                        }
                        else
                        {
                            hyperLink.CssClass = "contact_in_status_2";
                            hyperLink.ToolTip = "The contact is no longer usable as your partner has been deleted from the network.";
                        }
                    }
                }

                if (isProviderOrAdministrator)
                {
                    m_domains = new List<DomainItem>();
                    SoftnetRegistry.GetRecentlyUsedDomains(this.Context.User.Identity.Name, m_domains);

                    if (m_domains.Count > 0)
                    {
                        Panel_Domains.Visible = true;

                        HtmlGenericControl table = new HtmlGenericControl("table");
                        PH_Domains.Controls.Add(table);
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
                            hyperLink.NavigateUrl = string.Format("~/domains/domain.aspx?did={0}&ret={1}", domainItem.domainId, HttpUtility.UrlEncode("~/default.aspx"));
                            hyperLink.Text = domainItem.domainName;
                            hyperLink.CssClass = "domain_color";
                            hyperLink.Attributes["style"] = "font-size: 1.1em;";
                        }
                    }
                }

                if (isProviderOrAdministrator)
                {
                    if (!(Panel_Domains.Visible || Panel_Contacts.Visible || Panel_InvalidContacts.Visible))
                        Panel_AuthorizedProviderNoItems.Visible = true;
                }
                else
                {
                    if (!(Panel_Contacts.Visible || Panel_InvalidContacts.Visible))
                        Panel_AuthorizedConsumerNoItems.Visible = true;
                }
            }
            catch (SoftnetException ex)
            {
                ExceptionHandler.exec(this, ex);
            }
        }
        else
        {
            if (m_anyoneCanRegister)
                Panel_AnonymFreeSigningUp.Visible = true;
            else
                Panel_AnonymNoFreeSigningUp.Visible = true;
        }
    }
}