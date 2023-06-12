using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for AutoNameSuffix
/// </summary>
public class AutoNaming
{
    public static string addSuffix(string name)
    {
        DateTime dateTime = DateTime.Now;
        string month = "";
        switch (dateTime.Month)
        {
            case 1:
                month = "JAN";
                break;
            case 2:
                month = "FEB";
                break;
            case 3:
                month = "MAR";
                break;
            case 4:
                month = "APR";
                break;
            case 5:
                month = "MAY";
                break;
            case 6:
                month = "JUN";
                break;
            case 7:
                month = "JUL";
                break;
            case 8:
                month = "AUG";
                break;
            case 9:
                month = "SEP";
                break;
            case 10:
                month = "OCT";
                break;
            case 11:
                month = "NOV";
                break;
            case 12:
                month = "DEC";
                break;
        }

        return name + string.Format("_{0:D2}", dateTime.Day) + "." + month + "." + string.Format("{0:D2}", dateTime.Year % 100) + "_" +
            string.Format("{0:D2}", dateTime.Hour) + ":" + string.Format("{0:D2}", dateTime.Minute) + ":" + string.Format("{0:D2}", dateTime.Second);
    }
}