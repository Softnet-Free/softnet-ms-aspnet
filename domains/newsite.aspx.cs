﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class newsite : System.Web.UI.Page
{
    DomainItem m_domainItem;
    UrlBuider m_urlBuider;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        try
        {
            long domainId;
            if (long.TryParse(HttpUtility.ParseQueryString(this.Request.Url.Query).Get("did"), out domainId) == false)
                throw new InvalidIdentifierSoftnetException();
        
            m_domainItem = new DomainItem();
            SoftnetRegistry.GetDomainItem(this.Context.User.Identity.Name, domainId, m_domainItem);

            string retString = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("ret");
            m_urlBuider = new UrlBuider(retString);

            this.Title = string.Format("New Site - {0}", m_domainItem.domainName);

            HL_Domain.NavigateUrl = string.Format("~/domains/domain.aspx?did={0}", m_domainItem.domainId);
            HL_Domain.Text = m_domainItem.domainName;
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void Back_Click(object sender, EventArgs e)
    {
        string retUrl = m_urlBuider.getBackUrl();
        if (retUrl != null)
            Response.Redirect(retUrl);
        else
            Response.Redirect(string.Format("~/domains/domain.aspx?did={0}", m_domainItem.domainId));
    }

    protected void CreateSite_Click(object sender, EventArgs e)
    {
        try
        {
            string siteName = TB_SiteDescription.Text.Trim();            
            long siteId = SoftnetRegistry.CreateSite(m_domainItem.domainId, siteName);
            string retUrl = m_urlBuider.getBackUrl();
            if (m_urlBuider.hasBackUrl())
                Response.Redirect(string.Format("~/domains/ssite.aspx?sid={0}&ret={1}", siteId, m_urlBuider.getBackUrl()));
            else
                Response.Redirect(string.Format("~/domains/ssite.aspx?sid={0}", siteId));
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }
}