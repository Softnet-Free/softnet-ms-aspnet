using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for TrivialSoftnetException
/// </summary>
public class UserErrorSoftnetException: SoftnetException
{
    public UserErrorSoftnetException(string message)
        : base(message) { m_kind = 1; }  
}
