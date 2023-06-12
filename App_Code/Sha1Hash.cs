using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;

/// <summary>
/// Summary description for Sha1Hash
/// </summary>
public class Sha1Hash
{
	static SHA1CryptoServiceProvider s_Sha1CSP;

    static Sha1Hash()
    {
        s_Sha1CSP = new SHA1CryptoServiceProvider();
    }

    public static byte[] Compute(byte[] buffer)
    {
        return s_Sha1CSP.ComputeHash(buffer);
    }
}