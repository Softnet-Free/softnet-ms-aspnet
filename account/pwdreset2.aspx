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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="pwdreset2.aspx.cs" Inherits="account_pwdreset2" EnableViewState="false" Title="Password Reset" MasterPageFile="~/Site.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">    
    <h1 style="text-align: center;">password reset</h1>
    
    <table class="wide_table" style="margin-top: 20px;"><tr>
        <td class="wide_table" style="width: 50%"></td>
        <td class="wide_table">    
            <div style="width: 600px; text-align: center;">
                <asp:Label ID="L_Action" style="font-size: 1.2em; color: #3C6C80" runat="server" ></asp:Label>

                <div style="margin-top: 20px; text-align:center ">
                    <asp:TextBox ID="TB_Password" AutoComplete="off" runat="server" Style="font-size:1.2em; border: 1px solid #B0B0B0; outline:none; width:390px; margin: 0px; padding: 4px;"></asp:TextBox>
                </div>
                
                <table class="wide_table" style="margin-top: 20px;"><tr>
                    <td class="wide_table" style="width: 50%"></td>
                    <td class="wide_table">    
                        <asp:Panel ID="P_ApplyButton" runat="server" class="SubmitButton Blue">
                            <asp:Button ID="B_Apply" runat="server" Text="save" onclick="Save_Click" />
                        </asp:Panel>
                    </td>
                    <td class="wide_table" style="width: 50%"></td>
                </tr></table>

                <div style="padding-top: 40px; text-align: left; padding-left: 100px; padding-right: 100px;">
                    <asp:Label ID="L_ErrorMessage" Visible="false" CssClass="failureNotification" runat="server"/>
                </div>
            </div>
        </td>
        <td class="wide_table" style="width: 50%"></td>
    </tr></table>

</asp:Content>