using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for UserDataset
/// </summary>
public class UserDataset
{
    public long domainId;
    public string domainName;
    public long userId = 0;
    public int kind = 0;
    public string name = "";
    public bool dedicated = false;
    public bool enabled = true;
    public string contactName = "";
    public bool contactExists = false;
}