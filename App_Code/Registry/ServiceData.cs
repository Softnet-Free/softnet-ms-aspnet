using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ServiceBriefData
/// </summary>
public class ServiceData
{
    public Guid serviceUid = Guid.Empty;
    public long serviceId = 0;
    public long siteId = 0;
    public string serviceType = "";
    public string contractAuthor = "";
    public string version = "";
    public string hostname = "";
    public string ssHash = "";
    public bool enabled = true;
    public int pingPeriod;
}