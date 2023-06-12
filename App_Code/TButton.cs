using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for TButton
/// </summary>
public class TButton : Button
{
    public List<object> Args;
    public TButton()
    {
        Args = new List<object>();
    }
}