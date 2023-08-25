using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for LimitReachedSoftnetException
/// </summary>
public class LimitReachedSoftnetException : UserErrorSoftnetException
{
    public LimitReachedSoftnetException(string message)
        : base(message)
	{ }
}