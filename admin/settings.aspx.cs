/*
*   Copyright 2023 Robert Koifman
*   
*   Licensed under the Apache License, Version 2.0 (the "License");
*   you may not use this file except in compliance with the License.
*   You may obtain a copy of the License at
*
*   http://www.apache.org/licenses/LICENSE-2.0
*
*   Unless required by applicable law or agreed to in writing, software
*   distributed under the License is distributed on an "AS IS" BASIS,
*   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*   See the License for the specific language governing permissions and
*   limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class admin_settings : System.Web.UI.Page
{
    SettingsData m_settingsData;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            m_settingsData = new SettingsData();
            SoftnetRegistry.admin_getGeneralSettings(m_settingsData);

            string parameter = HttpUtility.ParseQueryString(this.Request.Url.Query).Get("prm");

            if (string.Equals(parameter, "su", StringComparison.InvariantCultureIgnoreCase))
            {
                TD_SiteUrl.CssClass = "param_table val edit";

                TextBox textBox = new TextBox();
                TD_SiteUrl.Controls.Add(textBox);
                textBox.Attributes["autocomplete"] = "off";
                textBox.Width = Unit.Pixel(370);
                if (this.IsPostBack == false)
                {
                    textBox.Text = m_settingsData.siteUrl;
                    textBox.Focus();
                }

                P_SiteUrlButton.CssClass = "SubmitButtonMini Green";
                B_SiteUrl.Text = "save";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(m_settingsData.siteUrl) == false)
                {
                    HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                    TD_SiteUrl.Controls.Add(spanStatus);
                    spanStatus.InnerText = m_settingsData.siteUrl;
                }
                else
                {
                    TD_SiteUrl.CssClass = "param_table val not_set";
                }
            }

            if (string.Equals(parameter, "srv", StringComparison.InvariantCultureIgnoreCase))
            {
                TD_ServerAddress.CssClass = "param_table val edit";

                TextBox textBox = new TextBox();
                TD_ServerAddress.Controls.Add(textBox);
                textBox.Attributes["autocomplete"] = "off";
                textBox.Width = Unit.Pixel(370);
                if (this.IsPostBack == false)
                {
                    textBox.Text = m_settingsData.serverAddress;
                    textBox.Focus();
                }

                P_ServerAddressButton.CssClass = "SubmitButtonMini Green";
                B_ServerAddress.Text = "save";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(m_settingsData.serverAddress) == false)
                {
                    HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                    TD_ServerAddress.Controls.Add(spanStatus);
                    spanStatus.InnerText = m_settingsData.serverAddress;
                }
                else
                {
                    TD_ServerAddress.CssClass = "param_table val not_set";
                }
            }

            if (string.Equals(parameter, "skey", StringComparison.InvariantCultureIgnoreCase))
            {
                TD_SecretKey.CssClass = "param_table val edit";

                TextBox textBox = new TextBox();
                TD_SecretKey.Controls.Add(textBox);
                textBox.Attributes["autocomplete"] = "off";
                textBox.Width = Unit.Pixel(370);
                if (this.IsPostBack == false)
                {
                    textBox.Text = m_settingsData.secretKey;
                    textBox.Focus();
                }

                P_SecretKeyButton.CssClass = "SubmitButtonMini Green";
                B_SecretKey.Text = "save";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(m_settingsData.secretKey) == false)
                {
                    HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                    TD_SecretKey.Controls.Add(spanStatus);
                    spanStatus.InnerText = "************";
                }
                else
                {
                    TD_SecretKey.CssClass = "param_table val not_set";
                }
            }

            if (string.Equals(parameter, "acr", StringComparison.InvariantCultureIgnoreCase))
            {
                TD_AnyoneCanRegister.CssClass = "param_table val edit";

                P_AnyoneCanRegisterButton.CssClass = "SubmitButtonMini Green";
                B_AnyoneCanRegister.Text = "save";

                RadioButton rbYes = new RadioButton();
                rbYes.GroupName = "acr";
                rbYes.ID = "AcrYes";
                rbYes.Text = "YES";
                RadioButton rbNo = new RadioButton();
                rbNo.GroupName = "acr";
                rbNo.ID = "AcrNo";
                rbNo.Text = "NO";
                rbNo.Attributes["style"] = "margin-left: 10px;";

                TD_AnyoneCanRegister.Controls.Add(rbYes);
                TD_AnyoneCanRegister.Controls.Add(rbNo);

                if (this.IsPostBack == false)
                {
                    if (m_settingsData.anyoneCanRegister == "1")
                    {
                        rbYes.Checked = true;
                    }
                    else if (m_settingsData.anyoneCanRegister == "0")
                    {
                        rbNo.Checked = true;
                    }
                }
            }
            else
            {
                if (m_settingsData.anyoneCanRegister == "1")
                {
                    HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                    TD_AnyoneCanRegister.Controls.Add(spanStatus);
                    spanStatus.InnerText = "YES";
                }
                else if (m_settingsData.anyoneCanRegister == "0")
                {
                    HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                    TD_AnyoneCanRegister.Controls.Add(spanStatus);
                    spanStatus.InnerText = "NO";
                }
                else
                {
                    TD_AnyoneCanRegister.CssClass = "param_table val not_set";
                }
            }

            if (string.Equals(parameter, "apr", StringComparison.InvariantCultureIgnoreCase))
            {
                TD_AutoAssignProviderRole.CssClass = "param_table val edit";

                P_AutoAssignProviderRoleButton.CssClass = "SubmitButtonMini Green";
                B_AutoAssignProviderRole.Text = "save";

                RadioButton rbYes = new RadioButton();
                rbYes.GroupName = "apr";
                rbYes.ID = "AprYes";
                rbYes.Text = "YES";
                RadioButton rbNo = new RadioButton();
                rbNo.GroupName = "apr";
                rbNo.ID = "AprNo";
                rbNo.Text = "NO";
                rbNo.Attributes["style"] = "margin-left: 10px;";

                TD_AutoAssignProviderRole.Controls.Add(rbYes);
                TD_AutoAssignProviderRole.Controls.Add(rbNo);

                if (this.IsPostBack == false)
                {
                    if (m_settingsData.assignProviderRole == "1")
                    {
                        rbYes.Checked = true;
                    }
                    else if (m_settingsData.assignProviderRole == "0")
                    {
                        rbNo.Checked = true;
                    }
                }
            }
            else
            {
                if (m_settingsData.assignProviderRole == "1")
                {
                    HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                    TD_AutoAssignProviderRole.Controls.Add(spanStatus);
                    spanStatus.InnerText = "YES";
                }
                else if (m_settingsData.assignProviderRole == "0")
                {
                    HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                    TD_AutoAssignProviderRole.Controls.Add(spanStatus);
                    spanStatus.InnerText = "NO";
                }
                else
                {
                    TD_AutoAssignProviderRole.CssClass = "param_table val not_set";
                }
            }

            if (string.Equals(parameter, "upl", StringComparison.InvariantCultureIgnoreCase))
            {
                TD_UserPasswordMinLength.CssClass = "param_table val edit";

                TextBox textBox = new TextBox();
                TD_UserPasswordMinLength.Controls.Add(textBox);
                textBox.Attributes["autocomplete"] = "off";
                textBox.Width = Unit.Pixel(370);
                if (this.IsPostBack == false)
                {
                    textBox.Text = m_settingsData.userPasswordMinLength;
                    textBox.Focus();
                }

                P_UserPasswordMinLengthButton.CssClass = "SubmitButtonMini Green";
                B_UserPasswordMinLength.Text = "save";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(m_settingsData.userPasswordMinLength) == false)
                {
                    HtmlGenericControl spanValue = new HtmlGenericControl("span");
                    TD_UserPasswordMinLength.Controls.Add(spanValue);
                    spanValue.InnerText = m_settingsData.userPasswordMinLength;
                }
                else
                {
                    TD_UserPasswordMinLength.CssClass = "param_table val not_set";
                }
            }

            if (string.Equals(parameter, "spl", StringComparison.InvariantCultureIgnoreCase))
            {
                TD_ServicePasswordLength.CssClass = "param_table val edit";

                TextBox textBox = new TextBox();
                TD_ServicePasswordLength.Controls.Add(textBox);
                textBox.Attributes["autocomplete"] = "off";
                textBox.Width = Unit.Pixel(370);
                if (this.IsPostBack == false)
                {
                    textBox.Text = m_settingsData.servicePasswordLength;
                    textBox.Focus();
                }

                P_ServicePasswordLengthButton.CssClass = "SubmitButtonMini Green";
                B_ServicePasswordLength.Text = "save";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(m_settingsData.servicePasswordLength) == false)
                {
                    HtmlGenericControl spanValue = new HtmlGenericControl("span");
                    TD_ServicePasswordLength.Controls.Add(spanValue);
                    spanValue.InnerText = m_settingsData.servicePasswordLength;
                }
                else
                {
                    TD_ServicePasswordLength.CssClass = "param_table val not_set";
                }
            }

            if (string.Equals(parameter, "cpl", StringComparison.InvariantCultureIgnoreCase))
            {
                TD_ClientPasswordLength.CssClass = "param_table val edit";

                TextBox textBox = new TextBox();
                TD_ClientPasswordLength.Controls.Add(textBox);
                textBox.Attributes["autocomplete"] = "off";
                textBox.Width = Unit.Pixel(370);
                if (this.IsPostBack == false)
                {
                    textBox.Text = m_settingsData.clientPasswordLength;
                    textBox.Focus();
                }

                P_ClientPasswordLengthButton.CssClass = "SubmitButtonMini Green";
                B_ClientPasswordLength.Text = "save";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(m_settingsData.clientPasswordLength) == false)
                {
                    HtmlGenericControl spanValue = new HtmlGenericControl("span");
                    TD_ClientPasswordLength.Controls.Add(spanValue);
                    spanValue.InnerText = m_settingsData.clientPasswordLength;
                }
                else
                {
                    TD_ClientPasswordLength.CssClass = "param_table val not_set";
                }
            }

            if (string.Equals(parameter, "ckl", StringComparison.InvariantCultureIgnoreCase))
            {
                TD_ClientKeyLength.CssClass = "param_table val edit";

                TextBox textBox = new TextBox();
                TD_ClientKeyLength.Controls.Add(textBox);
                textBox.Attributes["autocomplete"] = "off";
                textBox.Width = Unit.Pixel(370);
                if (this.IsPostBack == false)
                {
                    textBox.Text = m_settingsData.clientKeyLength;
                    textBox.Focus();
                }

                P_ClientKeyLengthButton.CssClass = "SubmitButtonMini Green";
                B_ClientKeyLength.Text = "save";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(m_settingsData.clientKeyLength) == false)
                {
                    HtmlGenericControl spanStatus = new HtmlGenericControl("span");
                    TD_ClientKeyLength.Controls.Add(spanStatus);
                    spanStatus.InnerText = m_settingsData.clientKeyLength;
                }
                else
                {
                    TD_ClientKeyLength.CssClass = "param_table val not_set";
                }
            }

            if (string.Equals(parameter, "email", StringComparison.InvariantCultureIgnoreCase))
            {
                TD_EmailAddress.CssClass = "param_table val edit";
                TD_EMailPassword.CssClass = "param_table val edit";
                TD_SmtpServer.CssClass = "param_table val edit";
                TD_SmtpPort.CssClass = "param_table val edit";
                TD_SmtpRequiresSsl.CssClass = "param_table val edit";

                TextBox textBox = new TextBox();
                textBox.ID = "EmailAddress";
                TD_EmailAddress.Controls.Add(textBox);
                textBox.Attributes["autocomplete"] = "off";
                textBox.Width = Unit.Pixel(370);
                if (this.IsPostBack == false)
                    textBox.Text = m_settingsData.emailAddress;

                textBox = new TextBox();
                textBox.ID = "EmailPassword";
                TD_EMailPassword.Controls.Add(textBox);
                textBox.Attributes["autocomplete"] = "off";
                textBox.Width = Unit.Pixel(370);
                if (this.IsPostBack == false)
                    textBox.Text = m_settingsData.emailPassword;

                textBox = new TextBox();
                TD_SmtpServer.Controls.Add(textBox);
                textBox.Attributes["autocomplete"] = "off";
                textBox.Width = Unit.Pixel(370);
                if (this.IsPostBack == false)
                    textBox.Text = m_settingsData.smtpServer;

                textBox = new TextBox();
                TD_SmtpPort.Controls.Add(textBox);
                textBox.Attributes["autocomplete"] = "off";
                textBox.Width = Unit.Pixel(370);
                if (this.IsPostBack == false)
                    textBox.Text = m_settingsData.smtpPort;

                RadioButton rbYes = new RadioButton();
                rbYes.GroupName = "sslRequired";
                rbYes.ID = "SslRequiredYes";
                rbYes.Text = "YES";
                RadioButton rbNo = new RadioButton();
                rbNo.GroupName = "sslRequired";
                rbNo.ID = "SslRequiredNo";
                rbNo.Text = "NO";
                rbNo.Attributes["style"] = "margin-left: 10px;";

                TD_SmtpRequiresSsl.Controls.Add(rbYes);
                TD_SmtpRequiresSsl.Controls.Add(rbNo);

                if (this.IsPostBack == false)
                {
                    if (m_settingsData.smtpRequiresSSL == "1")
                    {
                        rbYes.Checked = true;
                    }
                    else if (m_settingsData.smtpRequiresSSL == "0")
                    {
                        rbNo.Checked = true;
                    }
                }

                P_EMailButton.CssClass = "SubmitButtonMini Green";
                B_EMail.Text = "save";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(m_settingsData.emailAddress) == false)
                {
                    HtmlGenericControl spanValue = new HtmlGenericControl("span");
                    TD_EmailAddress.Controls.Add(spanValue);
                    spanValue.InnerText = m_settingsData.emailAddress;
                }
                else
                {
                    TD_EmailAddress.CssClass = "param_table val not_set";
                }

                if (string.IsNullOrWhiteSpace(m_settingsData.emailPassword) == false)
                {
                    HtmlGenericControl spanValue = new HtmlGenericControl("span");
                    TD_EMailPassword.Controls.Add(spanValue);
                    spanValue.InnerText = "******"; //m_settingsData.emailPassword;
                }
                else
                {
                    TD_EMailPassword.CssClass = "param_table val not_set";
                }

                if (string.IsNullOrWhiteSpace(m_settingsData.smtpServer) == false)
                {
                    HtmlGenericControl spanValue = new HtmlGenericControl("span");
                    TD_SmtpServer.Controls.Add(spanValue);
                    spanValue.InnerText = m_settingsData.smtpServer;
                }
                else
                {
                    TD_SmtpServer.CssClass = "param_table val not_set";
                }

                if (string.IsNullOrWhiteSpace(m_settingsData.smtpPort) == false)
                {
                    HtmlGenericControl spanValue = new HtmlGenericControl("span");
                    TD_SmtpPort.Controls.Add(spanValue);
                    spanValue.InnerText = m_settingsData.smtpPort;
                }
                else
                {
                    TD_SmtpPort.CssClass = "param_table val not_set";
                }

                if (string.IsNullOrWhiteSpace(m_settingsData.smtpRequiresSSL) == false)
                {
                    if (m_settingsData.smtpRequiresSSL == "1")
                    {
                        HtmlGenericControl spanValue = new HtmlGenericControl("span");
                        TD_SmtpRequiresSsl.Controls.Add(spanValue);
                        spanValue.InnerText = "YES";
                    }
                    else if (m_settingsData.smtpRequiresSSL == "0")
                    {
                        HtmlGenericControl spanValue = new HtmlGenericControl("span");
                        TD_SmtpRequiresSsl.Controls.Add(spanValue);
                        spanValue.InnerText = "NO";
                    }
                    else
                    {
                        TD_SmtpRequiresSsl.CssClass = "param_table val not_set";
                    }
                }
                else
                {
                    TD_SmtpRequiresSsl.CssClass = "param_table val not_set";
                }
            }
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void SiteUrl_Click(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        if (button.Text.Equals("edit"))
            Response.Redirect("~/admin/settings.aspx?prm=su");
        else
        {
            try
            {
                TextBox textBox = (TextBox)TD_SiteUrl.Controls[0];
                SoftnetRegistry.admin_setSiteUrl(textBox.Text.Trim());
                Response.Redirect("~/admin/settings.aspx");
            }
            catch (SoftnetException ex)
            {
                ExceptionHandler.exec(this, ex);
            }
        }
    }

    protected void ServerAddress_Click(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        if (button.Text.Equals("edit"))
            Response.Redirect("~/admin/settings.aspx?prm=srv");
        else
        {
            try
            {
                TextBox textBox = (TextBox)TD_ServerAddress.Controls[0];
                SoftnetRegistry.admin_SetServerAddress(textBox.Text.Trim());
                Response.Redirect("~/admin/settings.aspx");
            }
            catch (SoftnetException ex)
            {
                ExceptionHandler.exec(this, ex);
            }
        }
    }

    protected void SecretKey_Click(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        if (button.Text.Equals("edit"))
            Response.Redirect("~/admin/settings.aspx?prm=skey");
        else
        {
            try
            {
                TextBox textBox = (TextBox)TD_SecretKey.Controls[0];
                SoftnetRegistry.admin_SetSecretKey(textBox.Text.Trim());
                Response.Redirect("~/admin/settings.aspx");
            }
            catch (SoftnetException ex)
            {
                ExceptionHandler.exec(this, ex);
            }
        }
    }

    protected void AnyoneCanRegister_Click(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        if (button.Text.Equals("edit"))
            Response.Redirect("~/admin/settings.aspx?prm=acr");
        else
        {
            try
            {
                RadioButton rbYes = (RadioButton)TD_AnyoneCanRegister.Controls[0];
                RadioButton rbNo = (RadioButton)TD_AnyoneCanRegister.Controls[1];
                if (rbYes.Checked)
                    SoftnetRegistry.admin_SetAnyoneCanRegister("1");
                else if (rbNo.Checked)
                    SoftnetRegistry.admin_SetAnyoneCanRegister("0");
                Response.Redirect("~/admin/settings.aspx");
            }
            catch (SoftnetException ex)
            {
                ExceptionHandler.exec(this, ex);
            }
        }
    }
    
    protected void AutoAssignProviderRole_Click(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        if (button.Text.Equals("edit"))
            Response.Redirect("~/admin/settings.aspx?prm=apr");
        else
        {
            try
            {
                RadioButton rbYes = (RadioButton)TD_AutoAssignProviderRole.Controls[0];
                RadioButton rbNo = (RadioButton)TD_AutoAssignProviderRole.Controls[1];
                if (rbYes.Checked)
                    SoftnetRegistry.admin_SetAutoAssignProviderRole("1");
                else if (rbNo.Checked)
                    SoftnetRegistry.admin_SetAutoAssignProviderRole("0");
                Response.Redirect("~/admin/settings.aspx");
            }
            catch (SoftnetException ex)
            {
                ExceptionHandler.exec(this, ex);
            }
        }
    }

    protected void UserPasswordMinLength_Click(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        if (button.Text.Equals("edit"))
            Response.Redirect("~/admin/settings.aspx?prm=upl");
        else
        {
            try
            {
                TextBox textBox = (TextBox)TD_UserPasswordMinLength.Controls[0];
                SoftnetRegistry.admin_SetUserPasswordMinLength(textBox.Text.Trim());
                Response.Redirect("~/admin/settings.aspx");
            }
            catch (SoftnetException ex)
            {
                ExceptionHandler.exec(this, ex);
            }
        }
    }

    protected void ServicePasswordLength_Click(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        if (button.Text.Equals("edit"))
            Response.Redirect("~/admin/settings.aspx?prm=spl");
        else
        {
            try
            {
                TextBox textBox = (TextBox)TD_ServicePasswordLength.Controls[0];
                SoftnetRegistry.admin_SetServicePasswordLength(textBox.Text.Trim());
                Response.Redirect("~/admin/settings.aspx");
            }
            catch (SoftnetException ex)
            {
                ExceptionHandler.exec(this, ex);
            }
        }
    }

    protected void ClientPasswordLength_Click(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        if (button.Text.Equals("edit"))
            Response.Redirect("~/admin/settings.aspx?prm=cpl");
        else
        {
            try
            {
                TextBox textBox = (TextBox)TD_ClientPasswordLength.Controls[0];
                SoftnetRegistry.admin_SetClientPasswordLength(textBox.Text.Trim());
                Response.Redirect("~/admin/settings.aspx");
            }
            catch (SoftnetException ex)
            {
                ExceptionHandler.exec(this, ex);
            }
        }
    }

    protected void ClientKeyLength_Click(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        if (button.Text.Equals("edit"))
            Response.Redirect("~/admin/settings.aspx?prm=ckl");
        else
        {
            try
            {
                TextBox textBox = (TextBox)TD_ClientKeyLength.Controls[0];
                SoftnetRegistry.admin_SetClientKeyLength(textBox.Text.Trim());
                Response.Redirect("~/admin/settings.aspx");
            }
            catch (SoftnetException ex)
            {
                ExceptionHandler.exec(this, ex);
            }
        }    
    }

    protected void EMail_Click(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        if (button.Text.Equals("edit"))
            Response.Redirect("~/admin/settings.aspx?prm=email");
        else
        {
            try
            {
                TextBox textboxEmailAddress = (TextBox)TD_EmailAddress.Controls[0];
                TextBox textboxEMailPassword = (TextBox)TD_EMailPassword.Controls[0];
                TextBox textboxSmtpServer = (TextBox)TD_SmtpServer.Controls[0];
                TextBox textboxSmtpPort = (TextBox)TD_SmtpPort.Controls[0];
                RadioButton rbYes = (RadioButton)TD_SmtpRequiresSsl.Controls[0];
                RadioButton rbNo = (RadioButton)TD_SmtpRequiresSsl.Controls[1];
                string smtpRequiresSsl = "";
                if (rbYes.Checked)
                    smtpRequiresSsl = "1";
                else if (rbNo.Checked)
                    smtpRequiresSsl = "0";
                SoftnetRegistry.admin_SetEMail(textboxEmailAddress.Text, textboxEMailPassword.Text, textboxSmtpServer.Text, textboxSmtpPort.Text, smtpRequiresSsl);
                Response.Redirect("~/admin/settings.aspx");
            }
            catch (SoftnetException ex)
            {
                ExceptionHandler.exec(this, ex);
            }
        }
    }            
}