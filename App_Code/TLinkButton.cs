using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for TLinkButton
/// </summary>
public class TLinkButton : LinkButton
{
    public List<object> Args;
	public TLinkButton()
	{
        Args = new List<object>();
	}
}