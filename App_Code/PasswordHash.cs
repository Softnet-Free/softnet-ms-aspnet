using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;

/// <summary>
/// Summary description for PasswordHash
/// </summary>
public class PasswordHash
{
    static SHA1CryptoServiceProvider s_Sha1CSP;

    static PasswordHash()
    {
        s_Sha1CSP = new SHA1CryptoServiceProvider();
    }

    public static byte[] Compute(byte[] salt, string password)
    {
        /*
        password hash algorithm:  Hash:= SHA1(Salt + UTF16BE(Password))  
        */
        byte[] unicode_password_bytes = Encoding.BigEndianUnicode.GetBytes(password);
        byte[] password_and_salt_bytes = new byte[unicode_password_bytes.Length + salt.Length];
        Buffer.BlockCopy(salt, 0, password_and_salt_bytes, 0, salt.Length);
        Buffer.BlockCopy(unicode_password_bytes, 0, password_and_salt_bytes, salt.Length, unicode_password_bytes.Length);
        return s_Sha1CSP.ComputeHash(password_and_salt_bytes);
    }
}