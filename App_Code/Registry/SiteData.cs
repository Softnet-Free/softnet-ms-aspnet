using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SiteBriefData
/// </summary>
public class SiteData
{
    public long siteId = 0;
    public Guid siteUid = Guid.Empty;
    public int siteKind = 0;
    public string serviceType = "";
    public string contractAuthor = "";
    public string ssHash = "";
    public bool guestSupported = false;
    public bool guestAllowed = false;
    public bool statelessGuestSupported = false;
    public string siteKey = "";
    public bool rolesSupported = false;
    public long defaultRoleId = 0;
    public bool implicitUsersAllowed = true;
    public bool constructed = false;
    public bool enabled = true;
    public string description = "";
}