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