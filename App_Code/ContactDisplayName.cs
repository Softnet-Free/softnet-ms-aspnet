using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ContactDisplayName
/// </summary>
public class ContactDisplayName
{
    public static string Adjust(string name)
    {
        if (name.Length > Constants.MaxLength.contact_display_name)
            return name.Substring(0, Constants.MaxLength.contact_display_name) + "...";
        return name;
    }
}