using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DataIntegritySoftnetException
/// </summary>
public class DataIntegritySoftnetException : CriticalSoftnetException
{
    public DataIntegritySoftnetException()
        : base("Data integrity error.", "Data integrity error.") { }

    public DataIntegritySoftnetException(string nativeMessage)
        : base("Data integrity error.", string.Format("Data integrity error. Description: {0}", nativeMessage)) { }
}