using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SiteConfigData
/// </summary>
public class SiteConfigDataset
{
    public long siteId;
    public long domainId;
    public string domainName;
    public SiteData siteData = null;
    public List<ServiceData> services = null;
    public List<UserData> users = null;
    public List<ContactData> contacts = null;
    public List<RoleData> roles = null;
    public List<UserInRole> usersInRoles = null;
    public List<long> siteUsers = null;
}