using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ManagementErrors
/// </summary>
public class ErrorCodes
{
    public const int ACCESS_DENIED = 112;

    public const int OWNER_NOT_APPROVED = 181;
    public const int CONFIG_ERROR = 233;
    public const int DATABASE_ERROR = 234;
    public const int OPERATION_FAILED = 235;
    public const int DATA_INTEGRITY_ERROR = 238;
    public const int DATA_DEFINITION_ERROR = 239;
    public const int INFRASTRUCTURE_ERROR = 240;

    public const int GENERAL_SETTINGS_ERROR = 508;
    public const int ILLEGAL_ARGUMENT = 509;
    public const int INVALID_IDENTIFIER = 510;
    public const int OBJECT_NOT_FOUND = 511;
    public const int ACCOUNT_NOT_FOUND = 512;
    public const int DOMAIN_NOT_FOUND = 513;
    public const int SITE_NOT_FOUND = 514;
    public const int USER_NOT_FOUND = 515;
    public const int CLIENT_NOT_FOUND = 516;
    public const int SERVICE_NOT_FOUND = 517;
    public const int ROLE_NOT_FOUND = 518;
}