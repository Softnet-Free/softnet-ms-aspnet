using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for InvitationData
/// </summary>
public class InvitationData
{
    public long invitationId = 0;
    public string ikey = "";
    public string email = "";
    public string description = "";
    public bool assignProviderRole;
    public int status = 0;
    public long newUserId = 0;
    public string newUserName = "";
    public int newUserAuthority = 0;
}