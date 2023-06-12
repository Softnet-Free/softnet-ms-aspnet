using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for MembershipSoftnetException
/// </summary>
public class MembershipSoftnetException : CriticalSoftnetException
{
    public MembershipSoftnetException()
        : base("The membership config error.", "The membership config error.") { }
}