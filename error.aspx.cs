using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

public partial class error : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string encodedMessage = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("msg");
        if (string.IsNullOrEmpty(encodedMessage))
            return;

        try
        {
            L_Error.Text = UrlMessageCodec.decode(encodedMessage);
        }
        catch (FormatException)
        {
            L_Error.Text = "The format of the error message is invalid.";
        }
    }
}