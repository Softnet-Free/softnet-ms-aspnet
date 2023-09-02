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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="setemail.aspx.cs" Inherits="account_setemail" EnableViewState="false" Title="Email Setup - My Account Settings" MasterPageFile="~/Site.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">    
    <div style="padding-left: 10px; margin-bottom: 5px">
        <h3>my account settings</h3>
    </div>    

    <table class="wide_table" style="width:798px;"><tr>
        <td style="width:199px; vertical-align:top;">
            <table class="wide_table"><tr>
                <td style="padding-left:5px;">
                    <div class="SubmitButton Yellow">
                        <asp:Button ID="B_Back" runat="server" Text="back" onclick="Back_Click" />
                    </div>
                </td>                   
                <td style="width:100%"></td>                
            </tr></table>        
        </td>
        <td style="width: 400px; vertical-align: top;">
            <h1 style="text-align: center;">e-mail setup</h1>
        </td>
        <td style="width:199px;"></td>        
    </tr></table>        

    <table class="wide_table" style="margin-top: 15px;"><tr>
        <td class="wide_table" style="width: 50%"></td>
        <td class="wide_table">    
            <div style="width: 500px;">
                <span style="font-size: 1.2em; color: #3C6C80">
                    In order to set an email first you need to confirm it. 
                    Type it in the text box below and click <span style="font-weight: bold;">send confirmation mail</span>.                    
                </span>

                <div style="margin-top: 20px;">
                    <asp:TextBox ID="TB_EMail" AutoComplete="off" runat="server" Style="font-size:1.2em; border: 1px solid #B0B0B0; outline:none; width:490px; margin: 0px; padding: 4px;"></asp:TextBox>
                
                    <table class="wide_table" style="margin-top: 20px;"><tr>
                        <td class="wide_table" style="width: 50%"></td>
                        <td class="wide_table">  
                            <asp:Panel ID="P_SendMailButton" class="SubmitButton Blue" runat="server">
                                <asp:Button ID="B_SendMail" runat="server" Text="send confirmation mail" onclick="SendMail_Click" />                                    
                            </asp:Panel>
                        </td>
                        <td class="wide_table" style="width: 50%"></td>
                    </tr></table>
                </div>

                <asp:Panel ID="P_Success" Visible="false" runat="server">
                    <div style="margin-top: 15px; margin-bottom: 15px; text-align: center">
                        <span style=" font-size: 1.2em;">The message has been successfully sent!</span>
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
                    <div style="margin-top: 15px; margin-bottom: 15px; text-align: center">
                        <asp:Label ID="L_Error" style="font-size: 1.2em; color: #9F0000" runat="server"/>
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
