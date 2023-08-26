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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="invsrecv.aspx.cs" Inherits="contacts_invs_recv" MasterPageFile="~/Site.master" Title="Invitations Received" EnableViewState="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h1 style="text-align: center;">invitations received</h1>

    <table class="wide_table"><tr>
        <td class="wide_table" style="width: 50%"></td>
        <td class="wide_table">
            <div style="width: 600px; padding-top: 20px">
                <div style="height: 8px; background-color: #A2C5D3;"></div>
                <asp:Table ID="Tb_Invitations" runat="server" CssClass="wide_table" style="border: 1px solid #A2C5D3;">
                </asp:Table>
            </div>
        </td>
        <td class="wide_table" style="width: 50%"></td>
    </tr></table>
</asp:Content>
