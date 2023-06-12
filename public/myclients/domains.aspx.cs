using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class public_myclients_domains : System.Web.UI.Page
{
    protected void Refresh_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/public/myclients/domains.aspx" + this.Request.Url.Query);
    }

    protected void Domain_Click(object sender, EventArgs e)
    {
        TLinkButton tButton = (TLinkButton)sender;
        DomainData domain = (DomainData)tButton.Args[0];
        Response.Redirect(string.Format("~/public/myclients/domain.aspx?did={0}", domain.domainId));
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.Context.User.Identity.IsAuthenticated == false)
            Response.Redirect("~/default.aspx");

        try
        {
            List<OwnerData> owners = new List<OwnerData>();
            List<DomainData> domains = new List<DomainData>();
            SoftnetRegistry.public_getDomainsForAccount(this.Context.User.Identity.Name, owners, domains);

            if (owners.Count > 0)
            {
                owners.Sort(delegate(OwnerData x, OwnerData y)
                {
                    return x.fullName.CompareTo(y.fullName);
                });

                for (int i = 0; i < owners.Count; i++)
                {
                    OwnerData owner = owners[i];

                    HtmlGenericControl divOwnerDomains = new HtmlGenericControl("div");
                    PH_Domains.Controls.Add(divOwnerDomains);
                    if (i < owners.Count - 1)
                        divOwnerDomains.Attributes["style"] = "padding-bottom: 15px;";

                    HyperLink hyperLink = new HyperLink();
                    divOwnerDomains.Controls.Add(hyperLink);
                    hyperLink.CssClass = "provider_color";
                    hyperLink.Attributes["style"] = "font-size:1.2em;";
                    hyperLink.Text = owner.fullName;
                    hyperLink.NavigateUrl = string.Format("~/public/services/domains.aspx?uid={0}", owner.ownerId);

                    HtmlGenericControl table = new HtmlGenericControl("table");
                    divOwnerDomains.Controls.Add(table);
                    table.Attributes["class"] = "wide_table";
                    table.Attributes["style"] = "margin-top: 5px;";

                    List<DomainData> ownerDomains = domains.FindAll(x => x.ownerId == owner.ownerId);
                    ownerDomains.Sort(delegate(DomainData x, DomainData y)
                    {
                        return x.domainName.CompareTo(y.domainName);
                    });

                    foreach (DomainData domain in ownerDomains)
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
                        linkButton.Args.Add(domain);
                        linkButton.ID = string.Format("LB_Domain_{0}", domain.domainId);
                        td.Controls.Add(linkButton);
                        linkButton.Text = domain.domainName;
                        linkButton.CssClass = "domain_color";
                        linkButton.Attributes["style"] = "font-size: 1.1em; text-decoration: none; border: 1px solid #C0C0C0; border-radius: 12px; background-color: #F7F7F7; padding: 2px 10px;";
                        linkButton.Click += new EventHandler(Domain_Click);
                    }
                }
            }
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }
}