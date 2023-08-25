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

<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" EnableViewState="false"
    CodeFile="default.aspx.cs" Inherits="_default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div style="min-height: 362px;">
        <table class="wide_table"><tr>
            <td class="wide_table" style="vertical-align:top;">
                <asp:Panel ID="Panel_Domains" Visible="false" Style="width:397px; margin-top: 7px; margin-right: 2px;" runat="server">
                    <table class="wide_table" style="border: 1px solid #A2C5D3;">
                        <tr>
                            <td class="wide_table" style="padding-bottom: 3px; background-color: #A2C5D3;">
                                <h4 style="color: white; text-align:center">recent domains</h4>
                            </td>
                        </tr>
                        <tr>
                            <td class="wide_table">
                                <asp:PlaceHolder ID="PH_Domains" runat="server"/>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
            <td class="wide_table" style="vertical-align:top;">
                <asp:Panel ID="Panel_Contacts" Visible="false" Style="width:397px; margin-top: 7px;" runat="server">
                    <table class="wide_table" style="border: 1px solid #A2C5D3;">
                        <tr>
                            <td class="wide_table" style="padding-bottom: 3px; background-color: #A2C5D3;">
                                <h4 style="color: white; text-align:center">recent contacts</h4>
                            </td>
                        </tr>
                        <tr>
                            <td class="wide_table">
                                <asp:PlaceHolder ID="PH_Contacts" runat="server"/>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="Panel_InvalidContacts" Visible="false" Style="width:397px; margin-top: 7px;" runat="server">
                    <table class="wide_table" style="border: 1px solid #F998A5;">
                        <tr>
                            <td class="wide_table" style="padding-bottom: 3px; background-color: #F998A5;">
                                <h4 style="color: white; text-align:center">unusable contacts</h4>
                            </td>
                        </tr>
                        <tr>
                            <td class="wide_table">
                                <asp:PlaceHolder ID="PH_InvalidContacts" runat="server"/>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr></table>
        <div style="margin-top: 7px; margin-right: 2px; font-size: 1.2em; color: #3C6C80;">
            <asp:Panel ID="Panel_AnonymFreeSigningUp" Visible="false" ViewStateMode="Disabled" runat="server">
                <ul>
                    <li>If you don't have an account, go to <a href="~/newuser.aspx" runat="server" style="text-decoration: none;">New User</a>;</li>
                    <li>On <a href="~/public/services/search.aspx" runat="server" style="text-decoration: none;">Public Services</a>, you can find public services with anonymous access;</li>
                    <li>On <a href="~/public/clients/domains.aspx" runat="server" style="text-decoration: none;">Guest Clients by EMail</a>, you can manage your stateful guest clients by email;</li>
                    <li><a href="https://softnet-free.github.io/softnet-ms/" target="_blank" style="text-decoration: none;">The User Guide to Softnet Management System</a> explains how to use Softnet MS.</li>
                </ul>
            </asp:Panel>
            <asp:Panel ID="Panel_AnonymNoFreeSigningUp" Visible="false" ViewStateMode="Disabled" runat="server">
                <ul>
                    <li>If you don't have an account, ask the administrator for invitatoin;</li>
                    <li>On <a href="~/public/services/search.aspx" runat="server" style="text-decoration: none;">Public Services</a>, you can find public services with anonymous access;</li>
                    <li>On <a href="~/public/clients/domains.aspx" runat="server" style="text-decoration: none;">Guest Clients by EMail</a>, you can manage your stateful guest clients by email;</li>
                    <li><a href="https://softnet-free.github.io/softnet-ms/" target="_blank" style="text-decoration: none;">The User Guide to Softnet Management System</a> explains how to use Softnet MS.</li>
                </ul>
            </asp:Panel>
            <asp:Panel ID="Panel_AuthorizedProviderNoItems" Visible="false" ViewStateMode="Disabled" runat="server">
                <ul>
                    <li>To deploy an IoT project, go to <a href="~/domains/default.aspx" runat="server" style="text-decoration: none;">Domains</a>;</li>
                    <li>To establish partneships with other owners, go to <a href="~/contacts/default.aspx" runat="server" style="text-decoration: none;">Contacts</a>;</li>
                    <li><a href="https://softnet-free.github.io/softnet-ms/" target="_blank" style="text-decoration: none;">The User Guide to Softnet Management System</a> explains how to use Softnet MS.</li>
                </ul>                
            </asp:Panel>
            <asp:Panel ID="Panel_AuthorizedConsumerNoItems" Visible="false" ViewStateMode="Disabled" runat="server">
                <ul>
                    <li>To establish partneships with other owners, go to <a id="A2" href="~/contacts/default.aspx" runat="server" style="text-decoration: none;">Contacts</a>;</li>
                    <li><a href="https://softnet-free.github.io/softnet-ms/" target="_blank" style="text-decoration: none;">The User Guide to Softnet Management System</a> explains how to use Softnet MS.</li>
                </ul>                
            </asp:Panel>
        </div>
    </div>            
</asp:Content>