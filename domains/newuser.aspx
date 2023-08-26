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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="newuser.aspx.cs" Inherits="newuser" MasterPageFile="~/Site.master" Title="New User" EnableViewState="true" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div style="padding-left: 10px; padding-bottom: 10px">
        <h3>
            <span>domain </span>
            <asp:HyperLink CssClass="h3_name domain_color" ID="HL_Domain" runat="server"/>
        </h3>
    </div>    

    <table class="auto_table"><tr>
        <td style="padding-left:5px;">
            <div class="SubmitButton Gray">
                <asp:Button ID="B_Back" runat="server" Text="back" onclick="Back_Click" />
            </div>
        </td>
    </tr></table>

    <h1 style="text-align: center;">new user</h1>

    <div style="padding: 0px 10px 10px 10px;">
        <table class="wide_table"><tr>
        <td class="wide_table" style="width: 50%"></td>
        <td class="wide_table">
            <div style="padding:5px 0px 4px 5px;">
                <h2 style="color: #508397">private user</h2>
            </div>
            <div style="width: 560px; padding: 5px; background-color: #A2C5D3">
                <table style="border-collapse: collapse; border-width: 0px; margin: 0px; padding: 0px"><tr>
                    <td style="width:250px">
                        <span style="display: block; color: white; padding-left: 6px; padding-bottom: 2px">user name</span>
                        <asp:TextBox ID="TB_PrivateUserName" AutoComplete="off" runat="server" Style="border-width: 0px; outline:none; width:242px; margin: 0px; padding: 3px;"></asp:TextBox>
                    </td>
                    <td style="width:200px;vertical-align:bottom">
                        <div style="width:200px; text-align:left;">
                            &nbsp;&nbsp;<asp:CheckBox ID="CB_PrivateUserDedicated" Checked="false" Text="Dedicated" runat="server" />
                        </div>
                    </td>            
                    <td style="width:100px; text-align:right; vertical-align:bottom">
                        <table style="border-collapse: collapse; border-width: 0px; margin: 0px; padding: 0px;"><tr>
                            <td style="width: 100%"></td>
                            <td>
                                <div class="SubmitButtonEmbodied LightBlue">
                                    <asp:Button ID="B_CreatePrivateUser" runat="server" Text="Create" onclick="CreatePrivateUser_Click" />
                                </div>
                            </td>
                        </tr></table>
                    </td>
                </tr></table>
                <asp:Label ID="L_PrivateUserError" Visible="false" runat="server" Text="" Style="display:block; margin-top:10px; color:#9F0000"></asp:Label>
            </div>
    
            <div style="padding:15px 0px 4px 5px">
                <h2 style="color: #508397">contact user</h2>
            </div>
            <div style="width: 560px; padding: 5px; background-color: #A2C5D3">
                <span style="display: block; color: white; padding-left: 6px; padding-bottom: 2px">contact</span>
                <asp:DropDownList ID="DDL_ContactList" AutoPostBack="true" OnSelectedIndexChanged="DDL_ContactSelected" runat="server" 
                style="border-width: 0px; outline:none; width:420px; margin: 0px; padding: 3px;"/>
                
                <table class="auto_table" style="margin-top: 5px"><tr>
                    <td style="width:250px; vertical-align:middle">
                        <span style="display: block; color: white; padding-left: 6px; padding-bottom: 2px">user name</span>
                        <asp:TextBox ID="TB_ContactUserName" AutoComplete="off" runat="server" Style="border-width: 0px; outline:none; width:242px; margin: 0px; padding: 3px;"></asp:TextBox>
                    </td>
                    <td style="width:200px; vertical-align:bottom">
                        <div style="width:200px; text-align:left;">
                            &nbsp;&nbsp;<asp:CheckBox ID="CB_ContactUserDedicated" Checked="true" Text="Dedicated" runat="server" />
                            &nbsp;&nbsp;<asp:CheckBox ID="CB_ContactUserEnabled" Checked="true" Text="Enabled" runat="server" />
                        </div>
                    </td>
                    <td style="width:100px; text-align:right; vertical-align:bottom">
                        <table class="wide_table"><tr>
                            <td style="width: 100%"></td>
                            <td>
                                <div class="SubmitButtonEmbodied LightBlue">
                                    <asp:Button ID="B_CreateContactUser" runat="server" Text="Create" onclick="CreateContactUser_Click" />
                                </div>
                            </td>
                        </tr></table>
                    </td>
                </tr></table>
                <asp:Label ID="L_ContactUserError" Visible="false" runat="server" Text="" Style="display:block; margin-top:10px; color:#9F0000"></asp:Label>
            </div>
            <div style="padding:15px 0px 4px 5px">
                <h2 style="color: #508397">existing users</h2>
            </div>
            <div style="width: 570px; border: 1px solid #A2C5D3;">
                <asp:Table ID="T_ExistingUsers" runat="server" Style="width: 100%; border-collapse:collapse; border-width: 0px">
                    <asp:TableHeaderRow style="background-color: #A2C5D3; height: 22px">
                        <asp:TableHeaderCell Style="width:50%; text-align: center; color: White; border-bottom: 1px solid #A2C5D3; border-right: 1px solid #A2C5D3;">&nbsp;&nbsp;user name</asp:TableHeaderCell>
                        <asp:TableHeaderCell Style="width:50%; text-align: center; color: White; border-bottom: 1px solid #A2C5D3">&nbsp;&nbsp;contact</asp:TableHeaderCell>            
                    </asp:TableHeaderRow>
                </asp:Table>
            </div>
        </td>
        <td class="wide_table" style="width: 50%"></td>
        </tr></table>
    </div>
</asp:Content>
