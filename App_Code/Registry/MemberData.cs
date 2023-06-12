using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for UserProps
/// </summary>
public class MemberData
{
    public long ownerId;
    public string accountName;
    public string fullName;
    public string email;
    public int authority;
    public bool isEnabled;
    public bool isLocked;
    public bool authMismatch;
}