using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SharedDomainList
/// </summary>
public class SharedDomainList
{
    public long contactId;
    public string contactName;
    public int status;
    public int partnerAuthority;
    public bool partnerEnabled;
    public List<DomainItem> contactDomains;
    public List<DomainItem> myDomains; 
}