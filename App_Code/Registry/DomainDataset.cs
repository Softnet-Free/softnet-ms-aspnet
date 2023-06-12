using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DomainData
/// </summary>
public class DomainDataset
{
    public long domainId;
    public string domainName;
    public List<SiteData> sites = null;
    public List<ServiceData> services = null;
    public List<UserData> users = null;
    public List<ContactData> contacts = null;
    public List<RoleData> roles = null;
    public List<UserInRole> usersInRoles = null;
    public List<SiteUser> siteUsers = null;
    public List<ClientCount> clientCounts = null;
}