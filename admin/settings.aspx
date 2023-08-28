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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="settings.aspx.cs" Inherits="admin_settings" Title="Settings - Admin"  MasterPageFile="~/Site.master"  EnableViewState="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h1 style="text-align: center;">general settings</h1>
    <div style="padding: 15px 5px 10px 5px">
        <asp:Table ID="Tb_settings" CssClass="param_table" runat="server">
            <asp:TableHeaderRow style="background-color: #A2C5D3; height: 20px;" runat="server">
                <asp:TableHeaderCell CssClass="param_table lnd" style="width:320px; text-align:center; font-weight: bold; color: White;" runat="server">parameter</asp:TableHeaderCell>
                <asp:TableHeaderCell CssClass="param_table val" style="text-align:center; font-weight: bold; color: White;" runat="server">value</asp:TableHeaderCell>
                <asp:TableHeaderCell CssClass="param_table btn" style="width:65px;" runat="server"/>
            </asp:TableHeaderRow>
            <asp:TableRow runat="server">
                <asp:TableCell CssClass="param_table lnd" runat="server">Softnet MS URL (<span style='color:#BB6600'>scheme://address</span>)</asp:TableCell>
                <asp:TableCell CssClass="param_table val" ID="TD_SiteUrl" runat="server"/>                    
                <asp:TableCell CssClass="param_table btn" runat="server">
                    <asp:Panel ID="P_SiteUrlButton" runat="server" class="SubmitButtonMini Blue">
                        <asp:Button ID="B_SiteUrl" runat="server" Text="edit" onclick="SiteUrl_Click" />
                    </asp:Panel>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow runat="server">
                <asp:TableCell CssClass="param_table lnd" runat="server">Softnet server address</asp:TableCell>
                <asp:TableCell CssClass="param_table val" ID="TD_ServerAddress" runat="server"/>                    
                <asp:TableCell CssClass="param_table btn" runat="server">
                    <asp:Panel ID="P_ServerAddressButton" runat="server" class="SubmitButtonMini Blue">
                        <asp:Button ID="B_ServerAddress" runat="server" Text="edit" onclick="ServerAddress_Click" />
                    </asp:Panel>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow runat="server">
                <asp:TableCell CssClass="param_table lnd" runat="server">Secret key</asp:TableCell>
                <asp:TableCell CssClass="param_table val" ID="TD_SecretKey" runat="server"/>                    
                <asp:TableCell CssClass="param_table btn" runat="server">
                    <asp:Panel ID="P_SecretKeyButton" runat="server" class="SubmitButtonMini Blue">
                        <asp:Button ID="B_SecretKey" runat="server" Text="edit" onclick="SecretKey_Click" />
                    </asp:Panel>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow runat="server">
                <asp:TableCell CssClass="param_table lnd" runat="server">Anyone can register</asp:TableCell>
                <asp:TableCell CssClass="param_table val" ID="TD_AnyoneCanRegister" runat="server"/>
                <asp:TableCell CssClass="param_table btn" runat="server">
                    <asp:Panel ID="P_AnyoneCanRegisterButton" runat="server" class="SubmitButtonMini Blue">
                        <asp:Button ID="B_AnyoneCanRegister" runat="server" Text="edit" onclick="AnyoneCanRegister_Click" />
                    </asp:Panel>
                </asp:TableCell>
            </asp:TableRow>            
            <asp:TableRow runat="server">
                <asp:TableCell CssClass="param_table lnd" runat="server">Assign 'Provider' role to a new user</asp:TableCell>
                <asp:TableCell CssClass="param_table val" ID="TD_AutoAssignProviderRole" runat="server"/>                    
                <asp:TableCell CssClass="param_table btn" runat="server">
                    <asp:Panel ID="P_AutoAssignProviderRoleButton" runat="server" class="SubmitButtonMini Blue">
                        <asp:Button ID="B_AutoAssignProviderRole" runat="server" Text="edit" onclick="AutoAssignProviderRole_Click" />
                    </asp:Panel>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow runat="server">
                <asp:TableCell CssClass="param_table lnd" runat="server">User password min length</asp:TableCell>
                <asp:TableCell CssClass="param_table val" ID="TD_UserPasswordMinLength" runat="server"/>
                <asp:TableCell CssClass="param_table btn" runat="server">
                    <asp:Panel ID="P_UserPasswordMinLengthButton" runat="server" class="SubmitButtonMini Blue">
                        <asp:Button ID="B_UserPasswordMinLength" runat="server" Text="edit" onclick="UserPasswordMinLength_Click" />
                    </asp:Panel>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow runat="server">
                <asp:TableCell CssClass="param_table lnd" runat="server">Service password length</asp:TableCell>
                <asp:TableCell CssClass="param_table val" ID="TD_ServicePasswordLength" runat="server"/>
                <asp:TableCell CssClass="param_table btn" runat="server">
                    <asp:Panel ID="P_ServicePasswordLengthButton" runat="server" class="SubmitButtonMini Blue">
                        <asp:Button ID="B_ServicePasswordLength" runat="server" Text="edit" onclick="ServicePasswordLength_Click" />
                    </asp:Panel>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow runat="server">
                <asp:TableCell CssClass="param_table lnd" runat="server">Client password length</asp:TableCell>
                <asp:TableCell CssClass="param_table val" ID="TD_ClientPasswordLength" runat="server"/>
                <asp:TableCell CssClass="param_table btn" runat="server">
                    <asp:Panel ID="P_ClientPasswordLengthButton" runat="server" class="SubmitButtonMini Blue">
                        <asp:Button ID="B_ClientPasswordLength" runat="server" Text="edit" onclick="ClientPasswordLength_Click" />
                    </asp:Panel>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow runat="server">
                <asp:TableCell CssClass="param_table lnd" runat="server">Client key length</asp:TableCell>
                <asp:TableCell CssClass="param_table val" ID="TD_ClientKeyLength" runat="server"/>
                <asp:TableCell CssClass="param_table btn" runat="server">
                    <asp:Panel ID="P_ClientKeyLengthButton" runat="server" class="SubmitButtonMini Blue">
                        <asp:Button ID="B_ClientKeyLength" runat="server" Text="edit" onclick="ClientKeyLength_Click" />
                    </asp:Panel>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow runat="server">
                <asp:TableCell CssClass="param_table lnd" columnspan="2" runat="server">Email parameters</asp:TableCell>                                    
                <asp:TableCell CssClass="param_table btn" runat="server">
                    <asp:Panel ID="P_EMailButton" runat="server" class="SubmitButtonMini Blue">
                        <asp:Button ID="B_EMail" runat="server" Text="edit" onclick="EMail_Click" />
                    </asp:Panel>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow runat="server">
                <asp:TableCell CssClass="param_table lnd child" runat="server">Email address</asp:TableCell>
                <asp:TableCell CssClass="param_table val" ID="TD_EmailAddress" runat="server"/>   
                <asp:TableCell CssClass="param_table btn" rowspan="5" runat="server"/>                 
            </asp:TableRow>
            <asp:TableRow runat="server">
                <asp:TableCell CssClass="param_table lnd child" runat="server">Email password</asp:TableCell>
                <asp:TableCell CssClass="param_table val" ID="TD_EMailPassword" runat="server"/>     
            </asp:TableRow>
            <asp:TableRow runat="server">
                <asp:TableCell CssClass="param_table lnd child" runat="server">SMTP server</asp:TableCell>
                <asp:TableCell CssClass="param_table val" ID="TD_SmtpServer" runat="server"/>
            </asp:TableRow>
            <asp:TableRow runat="server">
                <asp:TableCell CssClass="param_table lnd child" runat="server">SMTP port</asp:TableCell>
                <asp:TableCell CssClass="param_table val" ID="TD_SmtpPort" runat="server"/>
            </asp:TableRow>
            <asp:TableRow runat="server">
                <asp:TableCell CssClass="param_table lnd child" runat="server">SMTP requires SSL</asp:TableCell>
                <asp:TableCell CssClass="param_table val" ID="TD_SmtpRequiresSsl" runat="server"/>
            </asp:TableRow>
        </asp:Table>
    </div>
</asp:Content>