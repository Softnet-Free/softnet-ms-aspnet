using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class newdomain : System.Web.UI.Page
{
    protected void Back_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/domains/default.aspx");
    }

    protected void CreateDomain_Click(object sender, EventArgs e)
    {
        try
        {
            string domainName = TB_DomainName.Text.Trim();

            if (domainName.Length > Constants.MaxLength.domain_name)
            {
                L_Error.Visible = true;
                L_Error.Text = string.Format("The domain name must not contain more than {0} characters.", Constants.MaxLength.domain_name);
                return;
            }

            long domainId = SoftnetRegistry.CreateDomain(this.Context.User.Identity.Name, domainName);
            Response.Redirect(string.Format("~/domains/domain.aspx?did={0}", domainId));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void Page_Load(object sender, EventArgs e) { }
}