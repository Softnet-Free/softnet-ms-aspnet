using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for InvalidIdentifierSoftnetException
/// </summary>
public class InvalidIdentifierSoftnetException : UserErrorSoftnetException
{
    public InvalidIdentifierSoftnetException()
        : base("The object identifier has not been provided or has an invalid format.") { }
}