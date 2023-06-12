using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for CriticalSoftnetException
/// </summary>
public class CriticalSoftnetException : SoftnetException
{
    public CriticalSoftnetException(string message)
        : base(message) { m_kind = 2; }
    public CriticalSoftnetException(string message, string nativeMessage)
        : base(message, nativeMessage) { m_kind = 2; }    
}