using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DatabaseSoftnetException
/// </summary>
public class DatabaseSoftnetException : SoftnetException
{
	public DatabaseSoftnetException(string nativeMessage)
        : base("Database error.", nativeMessage) { m_kind = 3; }
}