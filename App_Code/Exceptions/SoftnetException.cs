using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ManagementException
/// </summary>
public class SoftnetException: Exception
{
    protected int m_kind;
    public int Kind
    {
        get { return m_kind; }
    }

    protected string m_nativeMessage;
    public string nativeMessage
    {
        get { return m_nativeMessage; }
    }

    public SoftnetException(string message)
        : base(message)
    {
        this.m_nativeMessage = string.Empty;
    }

    public SoftnetException(string message, string nativeMessage)
        : base(message)
    {
        this.m_nativeMessage = nativeMessage;
    }

    public SoftnetException(int code)
        : base("")
    {
        this.m_nativeMessage = string.Empty;
    }
}