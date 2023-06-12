using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Single
/// </summary>
public class Atomic<T>
{
	public Atomic() { }

    public Atomic(T value)
    {
        this.Value = value;
    }

    public T Value { get; set; }
}