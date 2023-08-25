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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="newuser.aspx.cs" Inherits="newuser" Title="New User" MasterPageFile="~/Site.master"  EnableViewState="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h1 style="text-align: center; margin-bottom: 25px;">new user</h1>
    <table class="wide_table"><tr>
        <td class="wide_table" style="width: 50%"></td>
        <td class="wide_table">    
            <div style="width: 500px;">
                <span style="font-size: 1.2em; color: #3C6C80">
                    Thank you for your decision to join us. In order to create an account you need first to confirm your email address. 
                    Type it in the text box and click <span style="font-weight: bold;">send confirmation mail</span>.
                    Then follow the url from the inbox of your email.
                </span>

                <div style="width: 480px; height: 100px; background-color: #A2C5D3; padding: 10px; padding-top: 5px;  margin-top: 20px;">
                    <span class="white_caption">email</span>
                    <asp:TextBox ID="TB_EMail" AutoComplete="off" runat="server" Style="font-size:1.2em; border-width: 0px; outline:none; width:474px; margin: 0px; padding: 3px;"></asp:TextBox>
                
                    <table class="wide_table" style="margin-top: 20px;"><tr>
                        <td class="wide_table" style="width: 50%"></td>
                        <td class="wide_table">  
                            <asp:Panel ID="P_SendMailButton" class="SubmitButtonEmbodied LightBlue" runat="server">
                                <asp:Button ID="B_SendMail" runat="server" Text="send confirmation mail" onclick="SendMail_Click" />                                    
                            </asp:Panel>
                        </td>
                        <td class="wide_table" style="width: 50%"></td>
                    </tr></table>
                </div>

                <asp:Panel ID="P_Success" Visible="false" runat="server">
                    <div style="margin-top: 20px; margin-bottom: 15px; text-align: center">
                        <span style=" font-size: 1.1em;">The message has been successfully sent!</span>
                    </div>
                    <table class="wide_table"><tr>
                        <td class="wide_table" style="width:50%"></td>
                        <td class="wide_table">
                            <div class="SubmitButton Blue">
                                <asp:Button ID="B_OK" runat="server" Text="ok"/>
                            </div>
                        </td>
                        <td class="wide_table" style="width:50%"></td>
                    </tr></table>
                </asp:Panel>

                <asp:Panel ID="P_Error" Visible="false" runat="server">
                    <div style="margin-top: 20px; margin-bottom: 15px; text-align: center">
                        <asp:Label ID="L_Error" style="font-size: 1.1em; color: #9F0000" runat="server"/>
                    </div>
                    <table class="wide_table"><tr>
                        <td class="wide_table" style="width:50%"></td>
                        <td class="wide_table">
                            <div class="SubmitButton Gray">
                                <asp:Button ID="B_Error" runat="server" Text="ok"/>
                            </div>
                        </td>
                        <td class="wide_table" style="width:50%"></td>
                    </tr></table>
                </asp:Panel>
            </div>
        </td>
        <td class="wide_table" style="width: 50%"></td>
    </tr></table>
</asp:Content>