<%@ Page Language="C#" AutoEventWireup="true" CodeFile="signup.aspx.cs" Inherits="signup" Title="Sign up" MasterPageFile="~/Site.master"  EnableViewState="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h1 style="text-align: center; margin-bottom: 25px;">sign up</h1>

    <table class="wide_table"><tr>
        <td class="wide_table" style="width: 50%"></td>
        <td class="wide_table">  
            <div style="width: 500px; padding: 20px; padding-top: 10px; margin-bottom: 5px; background-color: #A2C5D3">
                <table class="wide_table">
                    <tr>
                        <td class="wide_table">
                            <span class="white_caption">Your name</span>
                            <asp:TextBox ID="TB_UserName" AutoComplete="off" runat="server" Style="border-width: 0px; outline:none; width:494px; margin: 0px; padding: 3px;"></asp:TextBox>                        
                        </td>
                        <td class="wide_table" style="vertical-align: bottom">
                            <asp:RequiredFieldValidator ID="FullNameRequired" runat="server" ControlToValidate="TB_UserName" 
                                         CssClass="failureNotification" ErrorMessage="Your name is required." 
                                         ValidationGroup="SignUpUserValidationGroup">&nbsp;*&nbsp;</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="wide_table" style="padding-top: 15px">
                            <span class="white_caption">Account name</span>
                            <asp:TextBox ID="TB_AccountName" AutoComplete="off" runat="server" Style="border-width: 0px; outline:none; width:494px; margin: 0px; padding: 3px;"></asp:TextBox>
                        </td>
                        <td class="wide_table" style="vertical-align: bottom">
                            <asp:RequiredFieldValidator ID="AccountNameRequired" runat="server" ControlToValidate="TB_AccountName" 
                                         CssClass="failureNotification" ErrorMessage="Account name is required." 
                                         ValidationGroup="SignUpUserValidationGroup">&nbsp;*&nbsp;</asp:RequiredFieldValidator>
                        </td>                                      
                    </tr>
                    <tr>
                        <td class="wide_table" style="padding-top: 15px">
                            <span class="white_caption">Password</span>
                            <asp:TextBox ID="TB_Password" AutoComplete="off" runat="server" 
                                Style="background-color: White; border-width: 0px; outline:none; width:494px; margin: 0px; padding: 3px;"></asp:TextBox>
                        </td> 
                        <td class="wide_table" style="vertical-align: bottom">                                              
                            <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="TB_Password" 
                                         CssClass="failureNotification" ErrorMessage="Password is required." 
                                         ValidationGroup="SignUpUserValidationGroup">&nbsp;*&nbsp;</asp:RequiredFieldValidator>                        
                        </td>                                        
                    </tr>
                    <tr>
                        <td class="wide_table" style="padding-top: 15px">
                            <span class="white_caption">Email</span>
                            <asp:TextBox ID="TB_EMail" runat="server" ReadOnly="true" 
                                Style="background-color: White; border-width: 0px; background-color: #E1E1E1; outline:none; width:494px; margin: 0px; padding: 3px;"></asp:TextBox>
                        </td> 
                        <td class="wide_table"></td>                                        
                    </tr>
                </table>

                <table class="wide_table" style="margin-top: 25px"><tr>
                    <td class="wide_table" style="width:50%"></td>
                    <td class="wide_table">
                        <div class="SubmitButtonEmbodied LightBlue">
                            <asp:Button ID="B_CreateAccount" runat="server" ValidationGroup="SignUpUserValidationGroup" Text="create account" onclick="CreateAccount_Click" />                                    
                        </div>
                    </td>
                    <td class="wide_table" style="width:50%"></td>
                </tr></table>
            </div>   

            <div style="padding-top: 5px">
                <asp:ValidationSummary ID="SignUpUserValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="SignUpUserValidationGroup"/>
                <asp:Label ID="L_ErrorMessage" Visible="false" CssClass="failureNotification" runat="server"/>
            </div>
        </td>
        <td class="wide_table" style="width: 50%"></td>
    </tr></table>
</asp:Content>