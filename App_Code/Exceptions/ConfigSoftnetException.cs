using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ConfigSoftnetException
/// </summary>
public class ConfigSoftnetException : SoftnetException
{
    public ConfigSoftnetException(string nativeMessage)
        : base("Configuration error.", nativeMessage) { m_kind = 4; }
}