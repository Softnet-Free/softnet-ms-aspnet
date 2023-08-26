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

<%@ Page Title="Log In" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="login.aspx.cs" Inherits="account_login" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h1 style="text-align: center;margin-bottom: 20px;">log in</h1>    
    <table class="wide_table"><tr>
    <td class="wide_table" style="width: 50%"></td>
    <td class="wide_table">
        <div style="padding-top: 4px">
            <asp:Login ID="LoginUser" OnLoginError="LoginUser_LoginError" runat="server" EnableViewState="false" RenderOuterTable="false">
                <LayoutTemplate>                    
                    <div style="width: 400px; padding: 20px; margin-bottom: 20px; background-color: #A2C5D3">
                        <table class="wide_table">
                        <tr>
                            <td class="wide_table">   
                                <span class="white_caption">Account name</span>                                                         
                                <asp:TextBox ID="UserName" AutoComplete="off" runat="server" Style="border-width: 0px; outline:none; width:394px; margin: 0px; padding: 3px;"></asp:TextBox>
                            </td>
                            <td class="wide_table" style="vertical-align: bottom">
                                <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" 
                                        CssClass="failureNotification" ErrorMessage="Account name is required." ToolTip="Account name is required." 
                                        ValidationGroup="LoginUserValidationGroup">&nbsp;*&nbsp;</asp:RequiredFieldValidator>
                             </td>
                        </tr>
                        <tr>
                            <td class="wide_table" style="padding-top: 15px">
                                <span class="white_caption">Password</span>
                                <asp:TextBox ID="Password" runat="server" TextMode="Password" Style="border-width: 0px; outline:none; width:394px; margin: 0px; padding: 3px;"></asp:TextBox>
                            </td>
                            <td class="wide_table" style="vertical-align: bottom">    
                                <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" 
                                        CssClass="failureNotification" ErrorMessage="Password is required." ToolTip="Password is required." 
                                        ValidationGroup="LoginUserValidationGroup">&nbsp;*&nbsp;</asp:RequiredFieldValidator>
                            </td>   
                        </tr>
                        <tr>
                            <td class="wide_table" style="padding-top: 15px">
                                <asp:CheckBox ID="RememberMe" runat="server"/>
                                <asp:Label ID="RememberMeLabel" runat="server" Style=" font-size: 1.1em;" AssociatedControlID="RememberMe" CssClass="inline">Keep me logged in</asp:Label>
                            </td>
                            <td class="wide_table"></td>
                        </tr>
                        </table>
                        
                        <table class="wide_table" style="margin-top: 15px"><tr>
                            <td class="wide_table" style="width:50%"></td>
                            <td class="wide_table">
                                <div class="SubmitButtonEmbodied LightBlue">
                                    <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Log In" ValidationGroup="LoginUserValidationGroup"/>
                                </div>
                            </td>
                            <td class="wide_table" style="width:50%"></td>
                        </tr></table>         
                        
                        <div style="margin-top: 10px; font-size: 1.1em;">
                        <a href="~/account/pwdreset.aspx" runat="server">Forgot your password?</a>          
                        </div>  
                    </div>
                        <span class="failureNotification">
                            <asp:Literal ID="FailureText" runat="server"></asp:Literal>
                        </span>
                        <asp:ValidationSummary ID="LoginUserValidationSummary" runat="server" CssClass="failureNotification" 
                             ValidationGroup="LoginUserValidationGroup"/>
                </LayoutTemplate>
            </asp:Login>
        </div>
    </td>
    <td class="wide_table" style="width: 50%"></td>
    </tr></table>
</asp:Content>