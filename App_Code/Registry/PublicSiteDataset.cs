using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for PublicSiteDataset
/// </summary>
public class PublicSiteDataset
{
    public string siteKey;
    public long domainId;
    public string domainName;
    public long ownerId;
    public string ownerName;
    public bool guestEnabled;
    public long creatorId;
    public SiteData siteData = null;
    public List<ServiceData> services = null;
    public UserData guestData = null;
    public List<ClientData> guestClients = null;
}