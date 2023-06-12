using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for NewGuestData
/// </summary>
public class GuestCreatingDataset
{
    public long domainId;
    public long ownerId;
    public string ownerName;
    public string domainName;
    public SiteData siteData = null;
    public List<ServiceData> services = null;
    public string secretKey;
    public long currentTime;
}