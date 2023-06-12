using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DataDefinitionSoftnetException
/// </summary>
public class DataDefinitionSoftnetException : CriticalSoftnetException
{
    public DataDefinitionSoftnetException()
        : base("Data definition error.")
    {
        //base.m_nativeMessage = string.Format("Data definition error. Stack trace: {0}", Environment.StackTrace);
    }

    public DataDefinitionSoftnetException(string nativeMessage)
        : base("Data definition error.", nativeMessage) { }
}

