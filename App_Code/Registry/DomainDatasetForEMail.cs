using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DomainDatasetForEMail
/// </summary>
public class DomainDatasetForEMail
{
    public long domainId;
    public long ownerId;
    public string ownerName;
    public string domainName;
    public bool guestEnabled;
    public List<SiteData> sites = null;
    public List<ServiceData> services = null;
    public List<ClientData> clients = null;
}