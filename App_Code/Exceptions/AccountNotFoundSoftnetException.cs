using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for AccountNotFoundSoftnetException
/// </summary>
public class AccountNotFoundSoftnetException : UserErrorSoftnetException
{
    public AccountNotFoundSoftnetException()
        : base("Your account has not been found.") { }
    public AccountNotFoundSoftnetException(string accountName)
        : base(string.Format("The account '{0}' has not been found.", accountName)) { }
}