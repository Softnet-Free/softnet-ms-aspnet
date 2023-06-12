using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

public partial class signup : System.Web.UI.Page
{
    string m_email;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            string email = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("email");
            if (string.IsNullOrEmpty(email))
                throw new ArgumentSoftnetException("The confirmation url is not valid.");

            string strCreatedTime = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("time");
            if (string.IsNullOrEmpty(strCreatedTime))
                throw new ArgumentSoftnetException("The confirmation url is not valid.");

            string base64ReceivedHash = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("hash");
            if (string.IsNullOrEmpty(base64ReceivedHash))
                throw new ArgumentSoftnetException("The confirmation url is not valid.");

            TB_EMail.Text = email;

            string secretKey = SoftnetRegistry.settings_getSecretKey();
            
            byte[] emailBytes = Encoding.UTF8.GetBytes(email);
            byte[] timeBytes = Encoding.UTF8.GetBytes(strCreatedTime);
            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

            byte[] buffer = new byte[emailBytes.Length + timeBytes.Length + secretKeyBytes.Length];
            int offset = 0;
            System.Buffer.BlockCopy(emailBytes, 0, buffer, offset, emailBytes.Length);
            offset += emailBytes.Length;
            System.Buffer.BlockCopy(timeBytes, 0, buffer, offset, timeBytes.Length);
            offset += timeBytes.Length;
            System.Buffer.BlockCopy(secretKeyBytes, 0, buffer, offset, secretKeyBytes.Length);

            byte[] hash = Sha1Hash.Compute(buffer);
            string base64Hash = Convert.ToBase64String(hash);

            if(base64ReceivedHash.Equals(base64Hash) == false)
                throw new ArgumentSoftnetException("The confirmation url is not valid.");

            long createdTime = 0;
            try
            {
                createdTime = int.Parse(strCreatedTime);
            }
            catch (Exception)
            {
                throw new ArgumentSoftnetException("The confirmation url is not valid.");
            }

            long currentTime = SoftnetRegistry.getTimeTicks();
            if(createdTime > currentTime)
                throw new ArgumentSoftnetException("The confirmation url is not valid.");

            if((createdTime + 60480) < currentTime)
                throw new ArgumentSoftnetException("Sorry, the confirmation url has expired.");

            m_email = email;
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void CreateAccount_Click(object sender, EventArgs e)
    {
        try
        {            
            if (TB_UserName.Text.Length != TB_UserName.Text.Trim().Length)
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

            SoftnetRegistry.account_signupUser(TB_UserName.Text, TB_AccountName.Text, m_email, salted_password_b64, salt_b64);
            Response.Redirect("~/account/login.aspx");
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }
}