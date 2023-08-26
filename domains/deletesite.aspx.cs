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

public partial class deletesite : System.Web.UI.Page
{
    SiteDeletingData m_data;
    UrlBuider m_urlBuider;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            long siteId;
            if (long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("sid"), out siteId) == false)
                throw new InvalidIdentifierSoftnetException();

            m_data = new SiteDeletingData();
            SoftnetRegistry.GetSiteDeletingData(this.Context.User.Identity.Name, siteId, m_data);
            
            string retString = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("ret");
            m_urlBuider = new UrlBuider(retString);

            HL_Domain.NavigateUrl = string.Format("~/domains/domain.aspx?did={0}", m_data.domainId);
            HL_Domain.Text = m_data.domainName;

            L_Description.Text = m_data.description;

            if (m_data.serviceCount > 0)
            {
                L_Warning.Text = "Attention! The site contains " + (m_data.serviceCount == 1 ? "1 service" : m_data.serviceCount.ToString() + " services");
                if (m_data.clientCount > 0)
                    L_Warning.Text += " and " + (m_data.clientCount == 1 ? "1 client" : m_data.clientCount.ToString() + " clients");
                L_Warning.Text += ".";
                L_Warning.Text += (m_data.serviceCount + m_data.clientCount > 1 ? " These objects will also be deleted." : " This object will also be deleted.");                    
            }
            else if (m_data.clientCount > 0)
            {
                L_Warning.Text = "Attention! The site contains " + (m_data.clientCount == 1 ? "1 client." : m_data.clientCount.ToString() + " clients.");
                L_Warning.Text += (m_data.clientCount > 1 ? " These objects will also be deleted." : " This object will also be deleted.");
            }
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void Cancel_Click(object sender, EventArgs e)
    {
        if (m_urlBuider.hasBackUrl())
            Response.Redirect(m_urlBuider.getBackUrl());
        else
            Response.Redirect(string.Format("~/domains/domain.aspx?did={0}", m_data.domainId));

    }

    protected void Delete_Click(object sender, EventArgs e)
    {
        try
        {
            SoftnetTracker.deleteSite(m_data.siteId);
            Response.Redirect(string.Format("~/domains/domain.aspx?did={0}", m_data.domainId));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }
}