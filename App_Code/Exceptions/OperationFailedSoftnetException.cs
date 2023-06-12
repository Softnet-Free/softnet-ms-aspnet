using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for OperationFailedSoftnetException
/// </summary>
public class OperationFailedSoftnetException : UserErrorSoftnetException
{
    public OperationFailedSoftnetException()
        : base("Operation failed.") { }
    public OperationFailedSoftnetException(string message)
        : base(message) { }
}