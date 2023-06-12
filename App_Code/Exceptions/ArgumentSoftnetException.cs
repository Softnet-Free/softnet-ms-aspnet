using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ArgumentSoftnetException
/// </summary>
public class ArgumentSoftnetException : UserErrorSoftnetException
{
    public ArgumentSoftnetException(string message)
        : base(message) { }
}