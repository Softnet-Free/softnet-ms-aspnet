using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

/// <summary>
/// Summary description for UrlMessageCodec
/// </summary>
public class UrlMessageCodec
{
    public static string encode(string message)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        string messageBase64 = Convert.ToBase64String(messageBytes);
        return HttpUtility.UrlEncode(messageBase64);
    }

    public static string decode(string message)
    {
        byte[] messageBytes = Convert.FromBase64String(message);
        return Encoding.UTF8.GetString(messageBytes);
    }
}