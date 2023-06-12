using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for GeneralSettingsSoftnetException
/// </summary>
public class GeneralSettingsSoftnetException : CriticalSoftnetException
{
	public GeneralSettingsSoftnetException(string message)
        : base("A required parameter in the general settings has not been specified or has an illegal value.", message) { }
}