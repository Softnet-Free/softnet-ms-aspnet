using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

/// <summary>
/// Summary description for Randomizer
/// </summary>
public class Randomizer
{
    static Random Rnd;

    static Randomizer()
    {
        Rnd = new Random();
    }

    public static int integer()
    {
        return Rnd.Next();
    }

    public static int getInteger(int minValue, int maxValue)
    {
        return Rnd.Next(minValue, maxValue);
    }

    static public byte[] generateOctetString(int length)
    {
        byte[] buffer = new byte[length];
        Rnd.NextBytes(buffer);
        return buffer;
    }

    //  0..9, a...z, A..Z   excluded: I(73), O(79), l(108), o(111)
    static byte[] comfortableCharset =
    {
       48,  49,  50,  51,  52,  53,  54,  55,  56,  57,  
       65,  66,  67,  68,  69,  70,  71,  72,  74,  75,  76,  77,  78, 80,  81,  82,  83,  84,  85,  86,  87,  88,  89,  90,  
       97,  98,  99,  100, 101, 102, 103, 104, 105, 106, 107, 109, 110, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122  
    };

    static public string generatePassword(int length)
    {
        byte[] buffer = new byte[length];
        int upperIndex = comfortableCharset.Length - 1;
        for (int i = 0; i < length; i++)
            buffer[i] = (byte)comfortableCharset[Rnd.Next(0, upperIndex)];
        return Encoding.ASCII.GetString(buffer);
    }

    static public string generateTransactionKey(int length)
    {
        byte[] buffer = new byte[length];
        int upperIndex = comfortableCharset.Length - 1;
        buffer[0] = (byte)comfortableCharset[Rnd.Next(10, upperIndex)];
        for (int i = 1; i < length; i++)
            buffer[i] = (byte)comfortableCharset[Rnd.Next(0, upperIndex)];
        return Encoding.ASCII.GetString(buffer);
    }

    //  0..9, a...z   excluded: l(108), o(111)
    static byte[] clientKeyCharset =
    {
        48,  49,  50,  51,  52,  53,  54,  55,  56,  57,  
        97,  98,  99, 100, 101, 102, 103, 104, 105, 106, 107, 109, 110, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122
    };

    static public string generateClientKey(int length)
    {
        byte[] buffer = new byte[length];
        int upperIndex = clientKeyCharset.Length - 1;
        buffer[0] = (byte)clientKeyCharset[Rnd.Next(10, upperIndex)];
        for (int i = 1; i < length; i++)
            buffer[i] = (byte)clientKeyCharset[Rnd.Next(0, upperIndex)];    
        return Encoding.ASCII.GetString(buffer);
    }

    static public string generateInvitationKey(int length)
    {
        byte[] buffer = new byte[length];
        int upperIndex = clientKeyCharset.Length - 1;
        buffer[0] = (byte)clientKeyCharset[Rnd.Next(10, upperIndex)];
        for (int i = 1; i < length; i++)
            buffer[i] = (byte)clientKeyCharset[Rnd.Next(0, upperIndex)];
        return Encoding.ASCII.GetString(buffer);
    }    
}