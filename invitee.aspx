<%--
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
--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="invitee.aspx.cs" Inherits="invitee" Title="Sign Up By Invitation" MasterPageFile="~/Site.master"  EnableViewState="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h1 style="text-align: center; margin-bottom: 25px;">sign up by invitation</h1>

    <table class="wide_table"><tr>
        <td class="wide_table" style="width: 50%"></td>
        <td class="wide_table">            
            <div style="width: 500px; padding: 20px; padding-top: 10px; margin-bottom: 5px; background-color: #A2C5D3">
                <table class="wide_table">
                <tr>
                    <td class="wide_table">
                        <span class="white_caption">Your name</span>
                        <asp:TextBox ID="TB_InviteeName" runat="server" Style="border-width: 0px; outline:none; width:494px; margin: 0px; padding: 3px;"></asp:TextBox>                        
                    </td>
                    <td class="wide_table" style="vertical-align: bottom">
                        <asp:RequiredFieldValidator ID="FullNameRequired" runat="server" ControlToValidate="TB_InviteeName" 
                                     CssClass="failureNotification" ErrorMessage="Your name is required." 
                                     ValidationGroup="SignUpUserValidationGroup">&nbsp;*&nbsp;</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="wide_table" style="padding-top: 15px">
                        <span class="white_caption">Account name</span>
                        <asp:TextBox ID="TB_AccountName" runat="server" Style="border-width: 0px; outline:none; width:494px; margin: 0px; padding: 3px;"></asp:TextBox>
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
                        <asp:TextBox ID="TB_Password" runat="server" 
                            Style="background-color: White; border-width: 0px; outline:none; width:494px; margin: 0px; padding: 3px;"></asp:TextBox>
                    </td> 
                    <td class="wide_table" style="vertical-align: bottom">                                              
                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="TB_Password" 
                                     CssClass="failureNotification" ErrorMessage="Password is required." 
                                     ValidationGroup="SignUpUserValidationGroup">&nbsp;*&nbsp;</asp:RequiredFieldValidator>                        
                    </td>                                        
                </tr>
                <tr>
                    <td class="wide_table" style="padding-top: 15px" ID="TD_EMail" runat="server">
                        <span class="white_caption">Email</span>
                        <asp:TextBox ID="TB_EMail" runat="server"
                            Style="background-color: White; border-width: 0px; outline:none; width:494px; margin: 0px; padding: 3px;"></asp:TextBox>
                    </td> 
                    <td class="wide_table" style="vertical-align: bottom">                                              
                        <asp:RequiredFieldValidator ID="EMailRequired" runat="server" ControlToValidate="TB_EMail" 
                                     CssClass="failureNotification" ErrorMessage="Email is required." 
                                     ValidationGroup="SignUpUserValidationGroup">&nbsp;*&nbsp;</asp:RequiredFieldValidator>                        
                    </td> 
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
                <asp:ValidationSummary ID="SignUpUserValidationSummary" runat="server" CssClass="failureNotification" 
                    ValidationGroup="SignUpUserValidationGroup"/>
                <asp:Label ID="L_ErrorMessage" Visible="false" CssClass="failureNotification" runat="server"/>
            </div>
        </td>
        <td class="wide_table" style="width: 50%"></td>
    </tr></table>
</asp:Content>