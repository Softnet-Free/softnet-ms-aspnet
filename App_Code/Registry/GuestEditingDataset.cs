using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for GuestAccountDataset
/// </summary>
public class GuestEditingDataset
{
    public long domainId;
    public long ownerId;
    public string ownerName;
    public string domainName;
    public bool guestEnabled;
    public SiteData siteData = null;
    public List<ServiceData> services = null;
    public ClientData clientData = null;
    public string email = null;
    public string secretKey;
}