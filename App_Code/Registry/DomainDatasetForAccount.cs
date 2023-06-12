using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DomainDatasetForAccount
/// </summary>
public class DomainDatasetForAccount
{
    public long creatorId;
    public long domainId;
    public long ownerId;
    public string ownerName;
    public string domainName;
    public List<SiteData> sites = null;
    public List<ServiceData> services = null;
    public UserData guestData = null;
    public List<ClientData> guestClients = null;
}