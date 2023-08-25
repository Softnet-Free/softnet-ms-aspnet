using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ContactData
/// </summary>
public class ContactData
{
    public long contactId = 0;
    public long consumerId = 0;
    public string contactName = "";
    public string userDefaultName = "";
    public int status = 0;

    public static ContactData nullContact;
    static ContactData()
    {
        nullContact = new ContactData();
        nullContact.contactName = "Unknown";
        nullContact.status = 3;
    }
}