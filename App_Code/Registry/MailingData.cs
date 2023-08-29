using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ConfirmationMailData
/// </summary>
public class MailingData
{
    public string msUrl;
    public string siteAddress;
    public string secretKey;
    public long currentTime;
    public string emailAddress;
    public string emailPassword;
    public string smtpServer;
    public int smtpPort;
    public bool smtpRequiresSSL;
}