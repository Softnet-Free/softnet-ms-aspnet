using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class account_settings : System.Web.UI.Page
{
    AccountData m_accountData;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            m_accountData = new AccountData();
            m_accountData.accountName = this.Context.User.Identity.Name;
            SoftnetRegistry.account_getData(m_accountData);

            string parameter = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("prm");

            if (string.Equals(parameter, "name", StringComparison.InvariantCultureIgnoreCase))
            {
                TextBox textBox = new TextBox();
                TD_OwnerName.Controls.Add(textBox);
                textBox.Attributes["autocomplete"] = "off";
                textBox.Attributes["style"] = "width: 350px; margin: 0px; padding: 5px; outline:none; border-width: 0px; background-color: white;";
                if (this.IsPostBack == false)
                {
                    textBox.Text = m_accountData.ownerName;
                    textBox.Focus();
                }

                P_OwnerNameButton.CssClass = "SubmitButtonMini Green";
                B_OwnerName.Text = "save";
                
                HtmlGenericControl div = new HtmlGenericControl("div");
                PH_ViewButton.Controls.Add(div);
                div.Attributes["class"] = "SubmitButton Yellow";
                Button button = new Button();
                div.Controls.Add(button);
                button.Text = "view";
                button.Click += new EventHandler(View_Click);                 
            }
            else
            {
                if (string.IsNullOrWhiteSpace(m_accountData.ownerName) == false)
                {
                    HtmlGenericControl div = new HtmlGenericControl("div");
                    TD_OwnerName.Controls.Add(div);
                    div.Attributes["style"] = "background-color: #E9E9E9; padding: 5px;";
                    HtmlGenericControl spanValue = new HtmlGenericControl("span");
                    div.Controls.Add(spanValue);
                    spanValue.InnerText = m_accountData.ownerName;
                }
                else
                {
                    TD_OwnerName.CssClass = "param_table val not_set";
                }
            }

            if (string.Equals(parameter, "pw", StringComparison.InvariantCultureIgnoreCase))
            {
                TextBox textBox = new TextBox();
                TD_Password.Controls.Add(textBox);
                textBox.Attributes["autocomplete"] = "off";
                textBox.Attributes["style"] = "width: 350px; margin: 0px; padding: 5px; outline:none; border-width: 0px; background-color: white;";
                if (this.IsPostBack == false)
                {
                    textBox.Focus();
                }

                P_PasswordButton.CssClass = "SubmitButtonMini Green";
                B_Password.Text = "save";

                HtmlGenericControl div = new HtmlGenericControl("div");
                PH_ViewButton.Controls.Add(div);
                div.Attributes["class"] = "SubmitButton Yellow";
                Button button = new Button();
                div.Controls.Add(button);
                button.Text = "view";
                button.Click += new EventHandler(View_Click);
            }
            else
            {
                HtmlGenericControl div = new HtmlGenericControl("div");
                TD_Password.Controls.Add(div);
                div.Attributes["style"] = "background-color: #E9E9E9; padding: 5px;";
                HtmlGenericControl spanValue = new HtmlGenericControl("span");
                div.Controls.Add(spanValue);
                spanValue.InnerText = "************";
            }

            {
                HtmlGenericControl div = new HtmlGenericControl("div");
                TD_Email.Controls.Add(div);
                div.Attributes["style"] = "padding: 5px;";
                HtmlGenericControl spanValue = new HtmlGenericControl("span");
                div.Controls.Add(spanValue);
                spanValue.InnerText = m_accountData.email;
            }
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void OwnerName_Click(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        if (button.Text.Equals("edit"))
            Response.Redirect("~/account/settings.aspx?prm=name");
        else
        {
            try
            {
                TextBox textBox = (TextBox)TD_OwnerName.Controls[0];
                SoftnetRegistry.account_setOwnerName(m_accountData.ownerId, textBox.Text.Trim());
                Response.Redirect("~/account/settings.aspx");
            }
            catch (SoftnetException ex)
            {
                ExceptionHandler.exec(this, ex);
            }
        }
    }

    protected void Password_Click(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        if (button.Text.Equals("edit"))
            Response.Redirect("~/account/settings.aspx?prm=pw");

        else
        {
            try
            {
                TextBox textBox = (TextBox)TD_Password.Controls[0];

                if (textBox.Text.Length != textBox.Text.Trim().Length)
                {
                    L_ErrorMessage.Visible = true;
                    L_ErrorMessage.Text = "The password must not contain leading or trailing whitespace characters.";
                    return;
                }

                int passwordMinLength = SoftnetRegistry.settings_getUserPasswordMinLength();
                if (textBox.Text.Length < passwordMinLength)
                {
                    L_ErrorMessage.Visible = true;
                    L_ErrorMessage.Text = string.Format("The password length must be in the range [{0}, 64].", passwordMinLength);
                    return;
                }

                byte[] password = System.Text.Encoding.Unicode.GetBytes(textBox.Text);
                byte[] salt = Randomizer.generateOctetString(16);
                byte[] salt_and_password = new byte[password.Length + salt.Length];
                System.Buffer.BlockCopy(salt, 0, salt_and_password, 0, salt.Length);
                System.Buffer.BlockCopy(password, 0, salt_and_password, salt.Length, password.Length);

                var sha1CSP = new System.Security.Cryptography.SHA1CryptoServiceProvider();
                byte[] salted_password = sha1CSP.ComputeHash(salt_and_password);

                string salt_b64 = Convert.ToBase64String(salt);
                string salted_password_b64 = Convert.ToBase64String(salted_password);

                SoftnetRegistry.account_changePassword(m_accountData.ownerId, salted_password_b64, salt_b64);
                Response.Redirect("~/account/settings.aspx");
            }            
            catch (SoftnetException ex)
            {
                ExceptionHandler.exec(this, ex);
            }
        }
    }

    protected void Email_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/account/setemail.aspx");
    }

    protected void View_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/account/settings.aspx");
    }
}