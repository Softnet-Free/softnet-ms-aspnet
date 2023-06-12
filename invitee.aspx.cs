using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Text;

public partial class invitee : System.Web.UI.Page
{
    protected void CreateAccount_Click(object sender, EventArgs e)
    {
        try
        {
            string ikey = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("key");
            if (string.IsNullOrEmpty(ikey))
                return;

            if (TB_InviteeName.Text.Length != TB_InviteeName.Text.Trim().Length)
            {
                L_ErrorMessage.Text = "Your name must not contain leading or trailing whitespace characters.";
                return;
            }

            if (TB_AccountName.Text.Length != TB_AccountName.Text.Trim().Length)
            {
                L_ErrorMessage.Text = "The account name must not contain leading or trailing whitespace characters.";
                return;
            }            

            if (TB_Password.Text.Length != TB_Password.Text.Trim().Length)
            {
                L_ErrorMessage.Text = "The password must not contain leading or trailing whitespace characters.";
                return;
            }

            int passwordMinLength = SoftnetRegistry.settings_getUserPasswordMinLength();
            if (TB_Password.Text.Length < passwordMinLength)
            {
                L_ErrorMessage.Text = string.Format("The password length must be in range [{0}, 64].", passwordMinLength);
                return;            
            }

            byte[] password = Encoding.Unicode.GetBytes(TB_Password.Text);
            byte[] salt = Randomizer.generateOctetString(16);
            byte[] salt_and_password = new byte[password.Length + salt.Length];
            System.Buffer.BlockCopy(salt, 0, salt_and_password, 0, salt.Length);
            System.Buffer.BlockCopy(password, 0, salt_and_password, salt.Length, password.Length);

            var sha1CSP = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            byte[] salted_password = sha1CSP.ComputeHash(salt_and_password);

            string salt_b64 = Convert.ToBase64String(salt);
            string salted_password_b64 = Convert.ToBase64String(salted_password);

            SoftnetRegistry.account_signupByInvitation(ikey, TB_InviteeName.Text, TB_AccountName.Text, salted_password_b64, salt_b64);
            Response.Redirect("~/account/login.aspx");
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            string ikey = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("key");
            if (string.IsNullOrEmpty(ikey))
                throw new ArgumentSoftnetException("The invitation key has not been specified.");

            InvitationData data = new InvitationData();
            data.ikey = ikey;
            SoftnetRegistry.getInvitationData(data);
            if (data.status == 1)
                throw new ArgumentSoftnetException("The invitation has already been applied.");
            if (data.status == 2)
                throw new ArgumentSoftnetException("Sorry, the invitation has expired.");

            if (string.IsNullOrEmpty(data.email) == false)
            {
                TD_EMail.Visible = true;
                TB_EMail.Text = data.email;
            }
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }
}