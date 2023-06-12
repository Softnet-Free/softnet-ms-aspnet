using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SiteDataset
/// </summary>
public class SiteResolutionDataset
{    
    public string ownerName;
    public string domainName;
    public SiteData siteData = null;
    public List<ServiceData> services = null;
}