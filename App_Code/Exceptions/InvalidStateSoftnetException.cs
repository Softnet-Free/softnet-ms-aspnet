using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for InvalidStateSoftnetException
/// </summary>
public class InvalidStateSoftnetException : UserErrorSoftnetException
{
    public InvalidStateSoftnetException()
        : base("The state of the object is invalid.") { }
    public InvalidStateSoftnetException(string message)
        : base(message) { }
}
