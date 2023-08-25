using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ExceptionHandler
/// </summary>
public class ExceptionHandler
{
    public static void exec(System.Web.UI.Page page, SoftnetException ex)
    {
        if(ex.Kind == 1)
            page.Response.Redirect(string.Format("~/error.aspx?msg={0}", UrlMessageCodec.encode(ex.Message)));
        else
            page.Response.Redirect(string.Format("~/error.aspx?msg={0}", UrlMessageCodec.encode(ex.nativeMessage)));          
    }

    public static void exec(System.Web.UI.Page page, System.Net.Mail.SmtpException ex)
    {
        page.Response.Redirect(string.Format("~/error.aspx?msg={0}", UrlMessageCodec.encode(ex.Message)));
        //page.Response.Redirect(string.Format("~/error.aspx?msg={0}", UrlMessageCodec.encode("Message mailing error.")));
    }    
}