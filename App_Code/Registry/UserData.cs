using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for UserData
/// </summary>
public class UserData
{
    public long userId = 0;
    public int kind = 0;
    public long contactId = 0;
    public string name = "";
    public bool dedicated = false;
    public bool enabled = true;
    public ContactData contactData = null;
}