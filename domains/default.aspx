﻿<%--
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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="domains_default" MasterPageFile="~/Site.master"  
    MaintainScrollPositionOnPostback="true" EnableViewState="false" Title="My Domains" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h1 style="text-align:center">my domains</h1>            

    <table class="wide_table" style="margin-bottom: 10px"><tr>
        <td class="wide_table" style="padding-left: 8px">
            <table class="wide_table"><tr>
                <td class="wide_table">
                    <div class="SubmitButton Blue">
                        <asp:Button ID="B_NewDomain" runat="server" Text="new domain" onclick="NewDomain_Click" />
                    </div>
                </td>                 
                <td class="wide_table" style="width:100%"></td>                
            </tr></table>        
        </td>                
        <td class="wide_table" style="width:100%"></td>
        <td class="wide_table" style="padding-right: 8px">
            <table class="wide_table"><tr>
                <td class="wide_table" style="width:100%"></td>
                <td class="wide_table">
                    <asp:Panel ID="P_SwitchModeButton" CssClass="SubmitButton Gray" runat="server">
                        <asp:Button ID="B_SwitchMode" runat="server" Text="edit" style="padding-left:15px; padding-right:15px" onclick="SwitchMode_Click" />
                    </asp:Panel>
                </td>
            </tr></table>
        </td>
        <td class="wide_table"></td>
    </tr></table>    
    
    <div style="width: 794px; margin-top: 5px; border: 1px solid #A2C5D3;">
        <div style="height: 8px; background-color: #A2C5D3;"></div>
        <asp:PlaceHolder ID="PH_DomainList" runat="server"/>   
    </div>    
    <asp:Label ID="L_Error" Visible="false" runat="server" Text="" Style="display:block; margin-left:55px; margin-right:55px; margin-top:30px; color:#9F0000"></asp:Label>                 
</asp:Content>