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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="unlock2.aspx.cs" Inherits="account_unlock2" EnableViewState="false" Title="Account Unlocking" MasterPageFile="~/Site.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent"> 
    <h1 style="text-align: center;">account unlocking</h1>

    <table class="wide_table" style="margin-top: 20px;"><tr>
        <td class="wide_table" style="width: 50%"></td>
        <td class="wide_table">    
            <div style="width: 500px; text-align: center;">
                <asp:Label ID="L_Action" style="font-size: 1.2em; color: #3C6C80" runat="server" ></asp:Label>
            </div>
        </td>
        <td class="wide_table" style="width: 50%"></td>
    </tr></table>
</asp:Content>