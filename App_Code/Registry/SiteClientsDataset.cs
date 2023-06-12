using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SiteClientsDataset
/// </summary>
public class SiteClientsDataset
{
    public long siteId;
    public long ownerId;
    public long domainId;
    public string domainName;
    public SiteData siteData = null;
    public List<ServiceData> services = null;
    public List<UserData> users = null;
    public List<ContactData> contacts = null;
    public List<RoleData> roles = null;
    public List<UserInRole> usersInRoles = null;
    public List<long> siteUsers = null;
    public List<ClientData> clients = null;
    public List<ClientData> guestClients = null;
    public int gclientCount;
}