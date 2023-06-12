using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for InfrastructureErrorSoftnetException
/// </summary>
public class InfrastructureErrorSoftnetException : CriticalSoftnetException
{
    public InfrastructureErrorSoftnetException(string message)
        : base("Infrastructure error.", message) { }
}