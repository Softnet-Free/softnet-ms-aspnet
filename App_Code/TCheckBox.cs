using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for TCheckBox
/// </summary>
public class TCheckBox : CheckBox
{
    public object Token;
    public TCheckBox()
    {
        Token = null;
    }
}