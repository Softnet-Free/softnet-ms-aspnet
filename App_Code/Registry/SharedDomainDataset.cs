using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SharedDomainDataset
/// </summary>
public class SharedDomainDataset
{
    public long contactId;
    public long domainId;
    public string contactName = "";
    public string domainName = "";
    public int contactStatus;
    public int partnerAuthority = 0;
    public List<SiteData> sites = null;
    public List<ServiceData> services = null;
    public List<UserData> users = null;
    public List<RoleData> roles = null;
    public List<UserInRole> usersInRoles = null;
    public List<SiteUser> siteUsers = null;
    public List<ClientData> clients = null;
    public List<ClientData> guestClients = null;
}