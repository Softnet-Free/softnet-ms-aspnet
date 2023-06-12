using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Constants
/// </summary>
public class Constants
{
    public class Keys
    {
        public const int invitation_key_length = 32;
        public const int transaction_key_length = 20;
    }

    public static class RegistryError
    {
        public const int object_not_found = -1;
        public const int account_not_found = -2;
        public const int general_settings_error = -3;
        public const int membership_object_not_found = -4;
        public const int data_integrity_error = -5;
    }

    public static class SiteKind
    {
        public const int SingleService = 1;
        public const int MultiService = 2;
        public const int Parking = 3;
    }

    public static class UserKind
    {
        public const int Owner = 1;
        public const int Private = 2;
        public const int Contact = 3;
        public const int Guest = 4;
        public const int StatelessGuest = 5;
    }
}